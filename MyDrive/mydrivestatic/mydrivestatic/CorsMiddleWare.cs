using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MyDriveStatic
{
    public class CorsMiddleWare
    {
        private readonly RequestDelegate _next;

        public CorsMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            //if (httpContext.Request.Method == "OPTIONS")
            //{
            //    string[] origins = new string[] { "https://localhost:44332/", "https://localhost:44331/" };
            //    httpContext.Response.Headers["Access-Control-Allow-Origin"] = origins;
            //    httpContext.Response.Headers["Access-Control-Allow-Credentials"] = "true";
            //    httpContext.Response.Headers["Access-Control-Allow-Headers"] = "*";
            //    httpContext.Response.Headers["Access-Control-Allow-Methods"] = "GET,PUT,POST,DELETE,HEAD,OPTIONS,PATCH,PROPFIND,PROPPATCH,MKCOL,COPY,MOVE,LOCK";
            //    httpContext.Response.Headers["Access-Control-Expose-Headers"] = "*";

            //    httpContext.Response.StatusCode = 200;
            //    await httpContext.Response.WriteAsync("OK");
            //    return;
            //}

            if (context.Request.Method == "OPTIONS")
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                //context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, X-Requested-With, Content-Type, Accept" });
                //context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });
                //context.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });
                context.Response.StatusCode = 200;
                return context.Response.WriteAsync("OK");
            }

            return _next.Invoke(context);
        }
    }

    public static class OptionsMiddlewareExtensions
    {
        public static IApplicationBuilder UseOptions(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsMiddleWare>();
        }
    }
}
