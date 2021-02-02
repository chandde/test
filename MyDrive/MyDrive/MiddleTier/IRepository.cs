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
        public User GetUser(string username, string userid);

        public File CreateFolder(string userid, string parentfolderid, string foldername);
        public void DeleteFolder(string folderid);
        public File GetFolder(string folderid);
        public List<File> ListFolder(string folderid);

        public Task<List<File>> CreateFileAsync(string userid, string folderid, HttpRequest request);
        public void DeleteFile(string fileid);
        public File DonwloadFile();
    }
}
