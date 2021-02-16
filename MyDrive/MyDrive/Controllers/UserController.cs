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
    }
}
