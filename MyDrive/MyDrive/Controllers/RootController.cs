using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainService.Controllers
{
    public class RootController
    {
        [HttpGet]
        [Route("/")]
        // GET: UserController
        public ActionResult GetRoot([FromQuery] string userId)
        {
            return new RedirectResult("/index.html");
        }

        [HttpGet]
        [Route("/login")]
        // GET: UserController
        public ActionResult Login([FromQuery] string userId)
        {
            return new RedirectResult("/index.html");
        }
    }
}
