using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainService.MiddleTier
{
    public interface IRepository
    {
        public User CreateUser(string username);
        public User UpdateUser(string username);
        public void RemoveUser(string username);
        public User GetUser(string username, string userId);

        public File CreateFolder(string userId, string parentFolderId, string folderName);
        public void DeleteFolder(string folderId);
        public File GetFolder(string folderId);
        public List<File> ListFolder(string folderId);

        public Task<List<File>> CreateFileAsync(string userId, string folderId, HttpRequest request);
        public void DeleteFile(string fileId);
        public File DonwloadFile();
    }
}
