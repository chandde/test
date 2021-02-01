using MainService.MiddleTier;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace MainService.Controllers
{
    public class UserController : Controller
    {
        ICache cache;
        MySqlContext mySqlContext;
        IRepository repo;

        public UserController(MySqlContext mySqlContext, IRepository repository)
        {
            // this.cache = cache;
            this.mySqlContext = mySqlContext;
            this.repo = repository;
        }

        [HttpGet]
        [Route("user")]
        // GET: UserController
        public ActionResult<User> GetUserById([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new BadRequestResult();
            }

            var result = repo.GetUser(null, userId);

            return result;
        }

        [HttpGet]
        [Route("user/all")]
        // GET: UserController
        public ActionResult<List<User>> GetAllUsers()
        {
            try
            {
                var users = mySqlContext.User.ToList();
                return users;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return new EmptyResult();
            }
        }

        [HttpGet]
        [Route("user/create")]
        // GET: UserController
        public ActionResult<User> CreateUser([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return new BadRequestResult();
            }

            var user = repo.CreateUser(username);
            return user;
        }

        [HttpGet]
        [Route("user/{userid?}/folder/{folderid?}/createfolder")]
        // GET: UserController
        public ActionResult<File> CreateFolder(
            [FromRoute] string userid,
            [FromRoute] string parentfolderid,
            [FromQuery] string newFolder
        )
        {
            if (string.IsNullOrWhiteSpace(userid)
                || string.IsNullOrWhiteSpace(parentfolderid)
                || string.IsNullOrWhiteSpace(newFolder)
            )
            {
                return new BadRequestResult();
            }

            var folder = repo.GetFolder(parentfolderid);
            var user = repo.GetUser(null, userid);
            if (folder == null || user == null)
            {
                return new BadRequestResult();
            }

            var file = repo.CreateFolder(userid, parentfolderid, newFolder);

            return file;
        }

        [HttpGet]
        [Route("user/{userid?}/file/{fileid?}/delete")]
        public ActionResult DeleteFile([FromRoute] string fileid, [FromRoute] string userid)
        {
            if(string.IsNullOrWhiteSpace(fileid) || string.IsNullOrWhiteSpace(userid))
            {
                return new BadRequestResult();
            }

            repo.DeleteFile(fileid);

            return new OkResult();
        }

        [HttpGet]
        [Route("user/{userid?}/folder/{folderid?}/delete")]
        public ActionResult DeleteFolder([FromRoute] string folderid, [FromRoute] string userid)
        {
            if (string.IsNullOrWhiteSpace(folderid) || string.IsNullOrWhiteSpace(userid))
            {
                return new BadRequestResult();
            }

            repo.DeleteFolder(folderid);

            return new OkResult();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string userId)
        {
            return new OkResult();
        }
    }
}
