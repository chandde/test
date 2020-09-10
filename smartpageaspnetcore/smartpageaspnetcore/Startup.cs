using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SmartPageServer
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            configuration = new AppConfiguration();
            config.GetSection("ServerConfig").Bind(configuration);
            subdomainRootHandler = new SubdomainRootHandler(configuration);
        }

        public AppConfiguration configuration { get; }
        public SubdomainRootHandler subdomainRootHandler { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var host = context.Request.Host.Host.ToLower();
                    logger.LogInformation(string.Format("handling request for {0}", host));

                    if (configuration.AppServiceDomains.Contains(host))
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync("hello world!");
                    }
                    else if (host != configuration.WwwDomain && host.IndexOf(configuration.ShortDomain) > 1)
                    {
                        await this.subdomainRootHandler.HandleRequest(context, logger);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("Page not found!");
                    }
                });

                endpoints.MapPost("/subscription", async context =>
                {
                    // Placeholder: call mail client to send subscription email
                });
            });
        }
    }
}
