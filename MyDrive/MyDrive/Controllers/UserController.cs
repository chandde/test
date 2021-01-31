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

        public UserController(MySqlContext mySqlContext)
        {
            // this.cache = cache;
            this.mySqlContext = mySqlContext;
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

            var user = mySqlContext.User.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return new NotFoundResult();
            }

            return user;
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
            try
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
            catch(Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("user/{userid?}/folder/{folderid?}/createfolder")]
        // GET: UserController
        public ActionResult CreateFolder(
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

            var folder = mySqlContext.File.FirstOrDefault(f => f.FileId == folderid);
            var user = mySqlContext.User.FirstOrDefault(u => u.UserId == userid);
            if (folder == null || user == null)
            {
                return new BadRequestResult();
            }

            // create a new folder needs to
            // 1. add an entry in file table
            // 2. add an entry in folder table, a new child in parent folder

            using (var transaction = mySqlContext.Database.BeginTransaction())
            {
                var newFolderId = Guid.NewGuid().ToString();

                mySqlContext.File.Add(new File {
                    FileId = newFolderId,
                    FileType = "Folder",
                    FileName = newFolder,
                    CreatedAt = DateTime.Now,
                    ParentFolderId = folderid
                });

                //mySqlContext.Folder.Add(new Folder
                //{
                //    FolderId = folderid,
                //    Child = newFolderId,
                //});

                mySqlContext.SaveChanges();

                transaction.Commit();

                return new OkResult();
            }
        }

        //// GET: UserController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: UserController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string userId)
        {
            return new OkResult();
        }

        //// GET: UserController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: UserController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: UserController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: UserController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
