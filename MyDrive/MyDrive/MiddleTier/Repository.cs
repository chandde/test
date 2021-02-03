using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MainService.MiddleTier
{
    public class Repository
    {
        // TO DO: split folder and user into two repos
        // TO DO: ACL implementation

        MySqlContext mySqlContext;
        Authentication auth;

        public Repository(MySqlContext mySqlContext, Authentication auth)
        {
            this.mySqlContext = mySqlContext;
            this.auth = auth;
        }

        public File CreateFolder(string userid, string parentfolderid, string foldername)
        {
            // create a new folder needs to
            // 1. add an entry in file table
            // 2. add an entry in folder table, a new child in parent folder

            using (var transaction = mySqlContext.Database.BeginTransaction())
            {
                var newFolderId = Guid.NewGuid().ToString();

                var folder = new File
                {
                    FileId = newFolderId,
                    FileType = "Folder",
                    FileName = foldername,
                    CreatedAt = DateTime.Now,
                    ParentFolderId = parentfolderid
                };

                mySqlContext.File.Add(folder);
                mySqlContext.SaveChanges();
                transaction.Commit();

                return folder;
            }
        }

        public User CreateUser(string username, string password)
        {
            // use transaction to create user and its root folder altogether
            using (var transaction = mySqlContext.Database.BeginTransaction())
            {
                var rootFolderId = Guid.NewGuid().ToString();


                var passwordsha = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
                var passwordshaStr = string.Join("", passwordsha.Select(b => b.ToString("X2")));

                mySqlContext.User.Add(new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = username,
                    RootFolderId = rootFolderId,
                    PasswordSHA256 = passwordshaStr
                });

                mySqlContext.File.Add(new File
                {
                    FileId = rootFolderId,
                    FileType = "Folder",
                    CreatedAt = DateTime.Now,
                });

                mySqlContext.SaveChanges();

                transaction.Commit();
            }

            return mySqlContext.User.First(u => u.UserName == username);
        }

        public string Authenticate(string username, string password)
        {
            // validate username and password
            // generate token and return

            // generate SHA256 on password
            var sha = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(password));
            var shaStr = string.Join("", sha.Select(b => b.ToString("X2")));

            var user = mySqlContext.User.SingleOrDefault(u => u.UserName == username && u.PasswordSHA256 == shaStr);

            if (user == null)
            {
                return "";
            }

            return auth.GenerateJwtTokenForUser(user);
        }

        public void DeleteFile(string fileid)
        {
            // 1. remove entry from file table
            // 2. search for sha256 in file table, if no more references, remove its entry from hash table
            // use transaction to create user and its root folder altogether
            using (var transaction = mySqlContext.Database.BeginTransaction())
            {
                var file = mySqlContext.File.FirstOrDefault(f => f.FileId == fileid);
                if (file != null)
                {
                    var sha256 = file.SHA256;
                    mySqlContext.File.Remove(file);
                    mySqlContext.SaveChanges();

                    // TODO: reimplement below logic as we no longer use Hash table
                    // if the same hash is no longer used by any file, delete it from Azure blob
                    //var filesWithSameSHA = mySqlContext.File.Select(f => f.SHA256 == file.SHA256);
                    //if (filesWithSameSHA.Count() == 0)
                    //{
                    //    // no more file using the SHA
                    //    mySqlContext.Hash.Remove(mySqlContext.Hash.First(h => h.SHA256 == sha256));
                    //    mySqlContext.SaveChanges();
                    //}
                }

                transaction.Commit();
            }
        }

        public void DeleteFolder(string folderid)
        {
            // DFS or BFS calling into DeleteFolder or DeleteFile on all children, child could be file or folder


        }

        public File DonwloadFile()
        {
            throw new NotImplementedException();
        }
        public File GetFolder(string folderid)
        {
            return mySqlContext.File.FirstOrDefault(f => f.FileId == folderid && f.FileType == "Folder");
        }

        public List<File> ListFolder(string folderid)
        {
            if (string.IsNullOrWhiteSpace(folderid))
            {
                throw new Exception("missing folderid");
            }

            var files = mySqlContext.File.Where(f => f.ParentFolderId == folderid);

            // it's possible to have an empty folder without any content
            return files.ToList();
        }

        public User GetUser(string username, string userid)
        {
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(userid))
            {
                throw new Exception("username and userid both are empty");
            }

            User user = null;

            if (string.IsNullOrWhiteSpace(username))
            {
                user = mySqlContext.User.FirstOrDefault(u => u.UserId == userid);
            }
            else
            {
                user = mySqlContext.User.FirstOrDefault(u => u.UserName == username);
            }

            return user;
        }

        public void RemoveUser(string username)
        {
            throw new NotImplementedException();
        }

        public User UpdateUser(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<List<File>> CreateFileAsync(string userid, string parent, HttpRequest Request)
        {
            var boundary = Request.GetMultipartBoundary();

            if (boundary == null)
            {
                throw new ArgumentException("failed to get multi part boundary from request");
            }

            var parentFolder = GetFolder(parent);
            var user = GetUser(null, userid);

            if (parentFolder == null)
            {
                throw new ArgumentNullException($"no folder was found with id {parent}");
            }

            if (user == null)
            {
                throw new ArgumentNullException($"no user was found with id {userid}");
            }

            var ret = new List<File>();

            var reader = new MultipartReader(boundary, Request.Body);
            var section = await reader.ReadNextSectionAsync();
            while (section != null && section.GetContentDispositionHeader() != null)
            {
                var fileSection = section.AsFileSection();
                var fileName = fileSection.FileName;
                var ms = new MemoryStream();
                await fileSection.FileStream.CopyToAsync(ms);
                var sha = SHA256.Create().ComputeHash(ms.ToArray());
                var shaStr = string.Join("", sha.Select(b => b.ToString("X2")));

                var files = ListFolder(parent);
                if (files.Find(f => f.FileName == fileName) != null)
                {
                    throw new ArgumentException($"file name {fileName} already exists in the folder {parent}");
                }

                // check SHA DB if we already have the file
                var exsiting = mySqlContext.File.FirstOrDefault(h => h.SHA256 == shaStr);

                if (exsiting == null)
                {
                    new AzureWorker().UploadFile(shaStr, ms);
                    // no need to use a separate hash table, given the hash as name, fixed storage and fixed container
                    // we can build the url for the file to download!
                }

                var fileId = Guid.NewGuid().ToString();

                mySqlContext.File.Add(new File
                {
                    FileId = fileId,
                    FileName = fileName,
                    ParentFolderId = parentFolder.FileId,
                    CreatedAt = DateTime.Now,
                    FileType = "File",
                    SHA256 = shaStr
                });

                mySqlContext.SaveChanges();

                ret.Add(mySqlContext.File.First(f => f.FileId == fileId));
                section = await reader.ReadNextSectionAsync();
            }

            return ret;
        }
    }
}
