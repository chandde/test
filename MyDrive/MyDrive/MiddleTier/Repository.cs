using MainService.Types;
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

        public File CreateFolder(ClientContext context)
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
                    FileName = context.FileName, // file id and file name in context is for new folder 
                    CreatedAt = DateTime.Now,
                    ParentFolderId = context.FolderId // parent folder id
                };

                mySqlContext.File.Add(folder);
                mySqlContext.SaveChanges();
                transaction.Commit();

                return folder;
            }
        }

        public User CreateUser(ClientContext clientContext)
        {
            // use transaction to create user and its root folder altogether
            using (var transaction = mySqlContext.Database.BeginTransaction())
            {
                var rootFolderId = Guid.NewGuid().ToString();


                var passwordsha = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(clientContext.Password));
                var passwordshaStr = string.Join("", passwordsha.Select(b => b.ToString("X2")));

                mySqlContext.User.Add(new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = clientContext.UserName,
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

            return mySqlContext.User.First(u => u.UserName == clientContext.UserName);
        }

        public void Authenticate(ClientContext clientContext, out User user, out string token)
        {
            // validate username and password
            // generate token and return

            // generate SHA256 on password
            var sha = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(clientContext.Password));
            var shaStr = string.Join("", sha.Select(b => b.ToString("X2")));

            user = mySqlContext.User.SingleOrDefault(u => u.UserName == clientContext.UserName && u.PasswordSHA256 == shaStr);
            token = auth.GenerateJwtTokenForUser(user);
        }



        // TO DO make this middleware
        bool ValidateToken(string token, string userId)
        {
            var userIdfromtoken = auth.ValidateAndExtractToken(token);
            if (string.IsNullOrWhiteSpace(userIdfromtoken) || userIdfromtoken != userId)
            {
                return false;
            }

            return true;
        }

        public void DeleteFile(ClientContext context)
        {
            // 1. remove entry from file table
            // 2. search for sha256 in file table, if no more references, remove its entry from hash table
            // use transaction to create user and its root folder altogether

            // TO DO: verify user has access to the file

            using (var transaction = mySqlContext.Database.BeginTransaction())
            {
                var file = mySqlContext.File.FirstOrDefault(f => f.FileId == context.FileId);
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

        public void DeleteFolder(string folderId)
        {
            // DFS or BFS calling into DeleteFolder or DeleteFile on all children, child could be file or folder


        }

        public async Task<Byte[]> DownloadFile(ClientContext context, string fileId)
        {
            // verify user has access to the file
            var file = mySqlContext.File.SingleOrDefault(f => f.FileId == fileId);
            if (file == null)
            {
                throw new Exception($"file {fileId} does not exist");
            }

            var folder = mySqlContext.File.SingleOrDefault(f => f.FileId == file.ParentFolderId);
            if (folder == null)
            {
                throw new Exception($"could not find container folder {file.ParentFolderId}");
            }

            // recursively look for parent folder until root, whose ParentFolderId is null
            while (folder.ParentFolderId != null)
            {
                folder = mySqlContext.File.SingleOrDefault(f => f.FileId == folder.ParentFolderId);
            }

            // now we have the root folder, check if user owns the root folder
            var user = mySqlContext.User.SingleOrDefault(u => u.UserId == context.UserId);
            if (user == null)
            {
                throw new Exception($"could not find user {context.UserId}");
            }

            if (user.RootFolderId != folder.FileId)
            {
                throw new Exception($"user {user.UserId} does not own root folder {folder.FileId} for file {file.FileId}");
            }

            // call azure worker to download file
            var content = await new AzureWorker().DownloadFile(file.SHA256);

            return content;
        }

        public File GetFolder(ClientContext context)
        {
            return mySqlContext.File.FirstOrDefault(f => f.FileId == context.FolderId && f.FileType == "Folder");
        }

        public List<File> ListFolder(ClientContext context)
        {
            if (string.IsNullOrWhiteSpace(context.FolderId))
            {
                throw new Exception("missing folderId");
            }

            var files = mySqlContext.File.Where(f => f.ParentFolderId == context.FolderId);

            // it's possible to have an empty folder without any content
            return new List<File>(files);
        }

        public User GetUser(ClientContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UserId))
            {
                throw new Exception("username and userId both are empty");
            }

            User user = null;
            user = mySqlContext.User.FirstOrDefault(u => u.UserId == context.UserId);

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

        public string GetParent(ClientContext context)
        {
            var folder = mySqlContext.File.SingleOrDefault(f => f.FileId == context.FolderId);
            return folder?.ParentFolderId;
        }

        public async Task<List<File>> CreateFileAsync(HttpContext httpContext)
        {
            var boundary = httpContext.Request.GetMultipartBoundary();

            if (boundary == null)
            {
                throw new ArgumentException("failed to get multi part boundary from request");
            }

            var clientContext = httpContext.Items["ClientContext"] as ClientContext;

            var parentFolder = GetFolder(clientContext);
            var user = GetUser(clientContext);

            if (parentFolder == null)
            {
                throw new ArgumentNullException($"no folder was found with id {clientContext.FolderId}");
            }

            if (user == null)
            {
                throw new ArgumentNullException($"no user was found with id {clientContext.UserId}");
            }

            var ret = new List<File>();

            var reader = new MultipartReader(boundary, httpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();
            while (section != null && section.GetContentDispositionHeader() != null)
            {
                var fileSection = section.AsFileSection();
                var fileName = fileSection.FileName;
                var ms = new MemoryStream();
                await fileSection.FileStream.CopyToAsync(ms);
                var sha = SHA256.Create().ComputeHash(ms.ToArray());
                var shaStr = string.Join("", sha.Select(b => b.ToString("X2")));

                var files = ListFolder(clientContext);
                if (files.Find(f => f.FileName == fileName) != null)
                {
                    throw new ArgumentException($"file name {fileName} already exists in the folder {clientContext.FolderId}");
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
