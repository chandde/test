using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainService.MiddleTier
{
    public class Repository : IRepository
    {
        // TO DO: split folder and user into two repos
        // TO DO: ACL implementation

        MySqlContext mySqlContext;

        public Repository(MySqlContext mySqlContext)
        {
            this.mySqlContext = mySqlContext;
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

        public User CreateUser(string username)
        {
            // use transaction to create user and its root folder altogether
            using (var transaction = mySqlContext.Database.BeginTransaction())
            {
                var rootFolderId = Guid.NewGuid().ToString();

                mySqlContext.User.Add(new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = username,
                    RootFolderId = rootFolderId
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

                    var filesWithSameSHA = mySqlContext.File.Select(f => f.SHA256 == file.SHA256);
                    if (filesWithSameSHA.Count() == 0)
                    {
                        // no more file using the SHA
                        mySqlContext.Hash.Remove(mySqlContext.Hash.First(h => h.SHA256 == sha256));
                        mySqlContext.SaveChanges();
                    }
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

        public File CreateFile()
        {
            throw new NotImplementedException();
        }
    }
}
