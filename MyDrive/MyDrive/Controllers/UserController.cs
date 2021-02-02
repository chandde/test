﻿using MainService.MiddleTier;
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
using System.Threading.Tasks;
using System.Transactions;

namespace MainService.Controllers
{
    public class UserController : Controller
    {
        ICache cache;
        MySqlContext mySqlContext;
        Repository repo;

        public UserController(MySqlContext mySqlContext)
        {
            // this.cache = cache;
            this.mySqlContext = mySqlContext;
            this.repo = new Repository(mySqlContext);
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
            [FromRoute] string folderid,
            [FromQuery] string newFolder
        )
        {
            if (string.IsNullOrWhiteSpace(userid)
                || string.IsNullOrWhiteSpace(folderid)
                || string.IsNullOrWhiteSpace(newFolder)
            )
            {
                return new BadRequestResult();
            }

            var folder = repo.GetFolder(folderid);
            var user = repo.GetUser(null, userid);
            if (folder == null || user == null)
            {
                return new BadRequestResult();
            }

            var file = repo.CreateFolder(userid, folderid, newFolder);

            return file;
        }

        [HttpGet]
        [Route("user/{userid?}/file/{fileid?}/delete")]
        public ActionResult DeleteFile([FromRoute] string fileid, [FromRoute] string userid)
        {
            if (string.IsNullOrWhiteSpace(fileid) || string.IsNullOrWhiteSpace(userid))
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

        [HttpPost]
        [Route("user/{userid?}/folder/{folderid?}/uploadfile")]
        public async Task<ActionResult> UploadFile([FromRoute] string userid, [FromRoute] string folderid)
        {
            if (string.IsNullOrWhiteSpace(folderid)
                || string.IsNullOrWhiteSpace(userid)
            )
            {
                return new BadRequestResult();
            }

            var file = await repo.CreateFileAsync(userid, folderid, Request);

            return new OkResult();
        }
    }
}