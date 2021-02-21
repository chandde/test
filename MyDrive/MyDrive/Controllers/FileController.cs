using MainService.MiddleTier;
using MainService.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainService.Controllers
{
    public class FileController : Controller
    {
        ICache cache;
        MySqlContext mySqlContext;
        Repository repo;
        Authentication auth;

        public FileController(MySqlContext mySqlContext, IConfiguration configuration, Repository repo, Authentication auth)
        {
            // this.cache = cache;
            this.mySqlContext = mySqlContext;
            this.repo = repo;
            this.auth = auth;
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
        [Route("/deletefile")]
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
        public async Task<ActionResult> DownloadFile([FromQuery] string fileId)
        {
            var clientContext = HttpContext.Items["ClientContext"] as ClientContext;

            if (clientContext == null
                || string.IsNullOrWhiteSpace(clientContext.UserId)
                || string.IsNullOrWhiteSpace(fileId)
            )
            {
                return new BadRequestResult();
            }

            // TO DO: token validation
            var contentBytes = await repo.DownloadFile(clientContext, fileId);

            var file = new FileContentResult(contentBytes, "application/octet-stream");

            return file;
        }
    }
}
