using MainService.MiddleTier;
using MainService.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MainService
{
    public class ContextMiddleware
    {
        private readonly RequestDelegate _next;

        public ContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, Authentication auth, MySqlContext db)
        {
            await handler(httpContext, auth);
            await _next(httpContext);
        }

        private async Task handler(HttpContext httpContext, Authentication auth)
        {
            ClientContext context = null;

            // special handling for uploadfile
            // 1. userif and folderid is from url querystring, because the body is the actual file being uploaded
            // 2. do not consume the body at all, otherwise later in multipart reader will have trouble accessing the stream
            if (httpContext.Request.Path.Value == "/uploadfile")
            {
                if (httpContext.Request.QueryString.HasValue)
                {
                    var qs = QueryHelpers.ParseQuery(httpContext.Request.QueryString.Value);
                    if (!string.IsNullOrWhiteSpace(qs["userid"]) || !string.IsNullOrWhiteSpace(qs["folderid"]))
                    {
                        context = new ClientContext
                        {
                            UserId = qs["userid"],
                            FolderId = qs["folderid"],
                        };
                    }
                }
            }
            else
            {
                using (StreamReader stream = new StreamReader(httpContext.Request.Body))
                {
                    var body = await stream.ReadToEndAsync();
                    if (string.IsNullOrWhiteSpace(body))
                    {
                        return;
                    }
                    context = JsonSerializer.Deserialize<ClientContext>(body);
                }
            }

            var tokens = httpContext.Request.Headers[HeaderNames.Authorization];
            string jwtToken = "";
            foreach (var token in tokens)
            {
                if (token.StartsWith("Bearer "))
                {
                    jwtToken = token.Substring("Bearer ".Length);
                    break;
                }
            }

            if (context == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(jwtToken))
            {
                if (httpContext.Request.Path.HasValue && (httpContext.Request.Path.Value == "/authenticate" || httpContext.Request.Path.Value == "/createuser"))
                {
                    // if there is no token in header, still valid for create user and login scenarios
                    httpContext.Items.Add("ClientContext", context);
                }
                return;
            }

            var userid = auth.ValidateAndExtractToken(jwtToken);

            if (string.IsNullOrWhiteSpace(userid))
            {
                return;
            }

            httpContext.Items.Add("ClientContext", context);
        }
    }
}
