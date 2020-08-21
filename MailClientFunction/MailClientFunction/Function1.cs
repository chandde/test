using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Mailjet;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace MailClientFunction
{
    public static class Function1
    {
        [FunctionName("SmartPageMailClient")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            MailjetClient client = new MailjetClient("public key", "private key")
            {
                Version = ApiVersion.V3_1
            };

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource
            }.Property(Send.Messages, new JArray {
                new JObject {
                 {"From", new JObject {
                  {"Email", "hello@smartpagemailclienttest.com"},
                  {"Name", "Microsoft Ads Smart Page Notification"}
                  }},
                 {"To", new JArray {
                  new JObject {
                   {"Email", "my@email.com"},
                   {"Name", "You"}
                   }
                  }},
                 {"Subject", "My first Mailjet Email!"},
                 {"TextPart", "Mailjet test"},
                 {"HTMLPart", "Please keep this in your inbox"}
                 }
                });
            MailjetResponse response = await client.PostAsync(request);

            return new OkObjectResult("hello world!");
        }
    }
}

//if (response.IsSuccessStatusCode)
//{
//    Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
//    Console.WriteLine(response.GetData());
//}
//else
//{
//    Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
//    Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
//    Console.WriteLine(response.GetData());
//    Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
//}