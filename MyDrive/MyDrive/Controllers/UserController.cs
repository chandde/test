using MainService.MiddleTier;
using MainService.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Transactions;

namespace MainService.Controllers
{
    public class UserController : Controller
    {
        ICache cache;
        MySqlContext mySqlContext;
        Repository repo;
        Authentication auth;

        public UserController(MySqlContext mySqlContext, IConfiguration configuration, Repository repo, Authentication auth)
        {
            // this.cache = cache;
            this.mySqlContext = mySqlContext;
            this.repo = repo;
            this.auth = auth;
        }

        [HttpPost]
        [Route("user")]
        // GET: UserController
        public ActionResult<User> GetUserById()
        {
            var clientContext = HttpContext.Items["ClientContext"] as ClientContext;

            if (string.IsNullOrWhiteSpace(clientContext.UserId))
            {
                return new BadRequestResult();
            }

            var result = repo.GetUser(clientContext);

            return result;
        }

        [HttpPost]
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

        [HttpPost]
        [Route("/createuser")]
        // GET: UserController
        public ActionResult<User> CreateUser()
        {
            var clientContext = HttpContext.Items["ClientContext"] as ClientContext;

            if (clientContext == null || string.IsNullOrWhiteSpace(clientContext.UserName) || string.IsNullOrWhiteSpace(clientContext.Password))
            {
                return new BadRequestResult();
            }

            var user = repo.CreateUser(clientContext);
            return user;
        }

        [HttpPost]
        [Route("/authenticate")]
        // GET: UserController
        public ActionResult<User> Authenticate()
        {
            var clientContext = HttpContext.Items["ClientContext"] as ClientContext;

            if (string.IsNullOrWhiteSpace(clientContext.UserName) || string.IsNullOrWhiteSpace(clientContext.Password))
            {
                return new BadRequestResult();
            }

            repo.Authenticate(clientContext, out var user, out var token);

            if (user != null && !string.IsNullOrWhiteSpace(token))
            {
                HttpContext.Response.Cookies.Append("jwttokencookie", token);
                HttpContext.Response.Cookies.Append("userid", user.UserId);
                HttpContext.Response.Cookies.Append("folderid", user.RootFolderId);
                HttpContext.Response.Cookies.Append("username", user.UserName);
                return new OkObjectResult(token);
            }

            return new BadRequestResult();
        }


        [HttpPost]
        [Route("/createfolder")]
        // GET: UserController
        public ActionResult<File> CreateFolder()
        {
            var clientContext = HttpContext.Items["ClientContext"] as ClientContext;

            if (string.IsNullOrWhiteSpace(clientContext.UserId)
                || string.IsNullOrWhiteSpace(clientContext.FolderId)
                || string.IsNullOrWhiteSpace(clientContext.FileName)
            )
            {
                return new BadRequestResult();
            }

            var file = repo.CreateFolder(clientContext);

            return file;
        }

        [HttpPost]
        [Route("user/deletefile")]
        public ActionResult DeleteFile()
        {
            var clientContext = HttpContext.Items["ClientContext"] as ClientContext;

            if (string.IsNullOrWhiteSpace(clientContext.FileId)
                || string.IsNullOrWhiteSpace(clientContext.UserId)
            )
            {
                return new BadRequestResult();
            }

            repo.DeleteFile(clientContext);

            return new OkResult();
        }

        //[HttpGet]
        //[Route("user/{userid?}/folder/{folderid?}/delete")]
        //public ActionResult DeleteFolder([FromRoute] string folderid, [FromRoute] string userid)
        //{
        //    if (string.IsNullOrWhiteSpace(folderid) || string.IsNullOrWhiteSpace(userid))
        //    {
        //        return new BadRequestResult();
        //    }

        //    repo.DeleteFolder(folderid);

        //    return new OkResult();
        //}

        // POST: UserController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(string userId)
        //{
        //    return new OkResult();
        //}

        [HttpPost]
        [Route("/getparent")]
        public ActionResult GetParent()
        {
            var clientContext = HttpContext.Items["ClientContext"] as ClientContext;
            if (string.IsNullOrWhiteSpace(clientContext.FolderId)
                || string.IsNullOrWhiteSpace(clientContext.UserId)
            )
            {
                return new BadRequestResult();
            }

            return Ok(repo.GetParent(clientContext));
        }

        [HttpPost]
        [Route("/uploadfile")]
        public async Task<ActionResult> UploadFile()
        {
            var clientContext = HttpContext.Items["ClientContext"] as ClientContext;
            if (string.IsNullOrWhiteSpace(clientContext.FolderId)
                || string.IsNullOrWhiteSpace(clientContext.UserId)
            )
            {
                return new BadRequestResult();
            }

            var file = await repo.CreateFileAsync(HttpContext);

            return new OkResult();
        }

        [HttpPost]
        [Route("/listfolder")]
        public ActionResult<List<File>> ListFolder()
        {
            var clientContext = HttpContext.Items["ClientContext"] as ClientContext;

            if (clientContext == null
                || string.IsNullOrWhiteSpace(clientContext.FolderId)
                || string.IsNullOrWhiteSpace(clientContext.UserId)
            )
            {
                return new BadRequestResult();
            }

            // TO DO: token validation
            var files = repo.ListFolder(clientContext);

            return new OkObjectResult(files);
        }

        [HttpPost]
        [Route("/downloadfile")]
        public async Task<ActionResult> DownloadFile([FromQuery] string fileid)
        {
            var clientContext = HttpContext.Items["ClientContext"] as ClientContext;

            if (clientContext == null
                || string.IsNullOrWhiteSpace(clientContext.UserId)
                || string.IsNullOrWhiteSpace(fileid)
            )
            {
                return new BadRequestResult();
            }

            // TO DO: token validation
            var contentBytes = await repo.DownloadFile(clientContext, fileid);

            var file = new FileContentResult(contentBytes, "application/octet-stream");

            return file;
        }
    }
}
