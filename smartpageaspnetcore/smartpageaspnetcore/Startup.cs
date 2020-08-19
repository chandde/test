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
using System.Text;

// TO DO
// 1. fix/beautify 404
// 2. logging, hook up with Kusto SDK

namespace SmartPageServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Cdn = Configuration["Cdn"];
            Blob = Configuration["Blob"];
            WwwDomain = Configuration["WwwDomain"];
            ShortDomain = Configuration["ShortDomain"];
            TrafficManager = Configuration["TrafficManager"];
            AppServiceDomain = Configuration["AppServiceDomain"];

        }

        public IConfiguration Configuration { get; }

        public string Cdn { get; }

        public string Blob { get; }

        public string WwwDomain { get; }

        public string ShortDomain { get; }

        public string TrafficManager { get; }

        public string AppServiceDomain { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                    var host = context.Request.Host.Host;
                    if(host == TrafficManager || host == AppServiceDomain)
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync("hello world!");
                    }
                    else if (host != WwwDomain && host.IndexOf(ShortDomain) > 1)
                    {
                        // we're looking at a subdomain root level access
                        // e.g. cars.smartpage.com
                        var subDomain = host.Substring(0, host.IndexOf(ShortDomain) - 1);
                        var html = await PopulateIndexHtml(subDomain);

                        if (!String.IsNullOrEmpty(html))
                        {
                            context.Response.StatusCode = 200;
                            await context.Response.WriteAsync(html);
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            await context.Response.WriteAsync("Page not found!");
                        }
                    } 
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("Page not found!");
                    }
                });
            });
        }

        public async Task<string> PopulateIndexHtml(string page)
        {
            var indexHtml = "";
            var commonVersion = await GetCommonVersion();
            if (!String.IsNullOrEmpty(commonVersion))
            {
                var bundlePath = string.Format("{0}common/{1}/static", Cdn, commonVersion);

                var pageVersion = await GetPageVersion(page);

                if (!String.IsNullOrEmpty(pageVersion))
                {
                    // only when we can get valid common version and page version, shall we proceed
                    var pageConfigPath = string.Format("{0}pages/{1}/{2}/config.js", Cdn, page, pageVersion);

                    var indexHtmlPath = string.Format("{0}common/{1}/index.html", Blob, commonVersion);
                    indexHtml = await GetTextContent(indexHtmlPath);
                    indexHtml = indexHtml.Replace("%%config.js%%", pageConfigPath).Replace("./static", bundlePath);
                }
            }

            return indexHtml;
        }

        public async Task<string> GetCommonVersion()
        {
            var url = String.Format("{0}common/version.txt", Blob);
            return await GetTextContent(url);
        }

        public async Task<string> GetPageVersion(string page)
        {
            var url = string.Format("{0}pages/{1}/version.txt", Blob, page);
            return await GetTextContent(url);
        }

        public async Task<string> GetTextContent(string url)
        {
            var ret = "";
            var response = await new HttpClient().GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ret = await response.Content.ReadAsStringAsync();
            }
            return ret;
        }
    }
}
