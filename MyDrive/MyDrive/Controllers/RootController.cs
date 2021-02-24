using MainService.MiddleTier;
using MainService.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainService.Controllers
{
    public class RootController : Controller
    {
        ICache cache;
        MySqlContext mySqlContext;
        Repository repo;
        Authentication auth;

        public RootController(MySqlContext mySqlContext, IConfiguration configuration, Repository repo, Authentication auth)
        {
            // this.cache = cache;
            this.mySqlContext = mySqlContext;
            this.repo = repo;
            this.auth = auth;
        }

        //[HttpGet]
        //[Route("/")]
        //// GET: UserController
        //public ActionResult GetRoot([FromQuery] string userId)
        //{
        //    return new RedirectResult("/index.html");
        //}

        //[HttpGet]
        //[Route("/login")]
        //// GET: UserController
        //public ActionResult Login([FromQuery] string userId)
        //{
        //    return new RedirectResult("/index.html");
        //}

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
                HttpContext.Response.Cookies.Append("userId", user.UserId);
                HttpContext.Response.Cookies.Append("folderId", user.RootFolderId);
                HttpContext.Response.Cookies.Append("username", user.UserName);
                return new OkObjectResult(token);
            }

            return new BadRequestResult();
        }
    }
}
