using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyDriveStatic
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy", policyBuilder => policyBuilder
            //            .SetIsOriginAllowedToAllowWildcardSubdomains()
            //            .AllowAnyMethod()
            //            .AllowAnyHeader()
            //            .AllowAnyOrigin());
            //});
            // services.AddCors();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ICorsService corsService, ICorsPolicyProvider corsPolicyProvider)
        {
            // app.UseOptions();

            // app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    ServeUnknownFileTypes = true,
            //    OnPrepareResponse = (ctx) =>
            //    {
            //        var policy = corsPolicyProvider.GetPolicyAsync(ctx.Context, "CorsPolicy")
            //            .ConfigureAwait(false)
            //            .GetAwaiter().GetResult();

            //        var corsResult = corsService.EvaluatePolicy(ctx.Context, policy);

            //        corsService.ApplyResult(corsResult, ctx.Context.Response);
            //    }
            //});

            app.UseStaticFiles();

            //app.UseCors((builder) =>
            //{
            //    builder.AllowAnyOrigin();
            //    builder.AllowAnyMethod();
            //    builder.AllowAnyHeader();
            //});

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    OnPrepareResponse = context =>
            //    {
            //        // context.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            //        // string[] origins = new string[] { "https://localhost:44332/", "https://localhost:44331/" };
            //        context.Context.Response.Headers["Access-Control-Allow-Origin"] = "https://localhost:44332/";
            //        context.Context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
            //        context.Context.Response.Headers["Access-Control-Allow-Headers"] = "Origin, X-Requested-With, Content-Type, Accept";
            //        context.Context.Response.Headers["Access-Control-Allow-Methods"] = "GET,PUT,POST,DELETE,HEAD,OPTIONS,PATCH,PROPFIND,PROPPATCH,MKCOL,COPY,MOVE,LOCK";
            //        context.Context.Response.Headers["Access-Control-Expose-Headers"] = "*";
            //    }
            //});

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // app.UseMiddleware<CorsMiddleWare>();
        }
    }
}
