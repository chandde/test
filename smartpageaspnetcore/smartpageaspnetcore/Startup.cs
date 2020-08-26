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
using System.Reflection.PortableExecutable;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;

// TO DO
// 1. fix/beautify 404
// 2. logging

namespace SmartPageServer
{
    public class Cache
    {
        public string CommonVersion { get; set; }

        public string IndexHtml { get; set; }

        public Dictionary<string /*page*/, string /*version*/> PageVersion { get; set; }
    }

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
            AppServiceDomainLinux = Configuration["AppServiceDomainLinux"];
            AppServiceDomainWin = Configuration["AppServiceDomainWin"];

            Cache = new Cache()
            {
                PageVersion = new Dictionary<string, string>(),
            };
        }

        public Cache Cache { get; set; }

        public IConfiguration Configuration { get; }

        public string Cdn { get; }

        public string Blob { get; }

        public string WwwDomain { get; }

        public string ShortDomain { get; }

        public string TrafficManager { get; }

        public string AppServiceDomain { get; }

        public string AppServiceDomainLinux { get; }

        public string AppServiceDomainWin { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> Logger)
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
                    Logger.LogInformation(string.Format("handling request for {0}", host));

                    if (host == TrafficManager || host == AppServiceDomain || host == AppServiceDomainLinux || host == AppServiceDomainWin)
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync("hello world!");
                    }
                    else if (host != WwwDomain && host.IndexOf(ShortDomain) > 1)
                    {
                        // we're looking at a subdomain root level access
                        // e.g. cars.smartpage.com
                        var subDomain = host.Substring(0, host.IndexOf(ShortDomain) - 1);

                        bool commonVersionCacheUpdated = false;
                        bool indexHtmlCacheUpdated = false;
                        bool pageVersionCacheUpdated = false;

                        Func<Task> ensureCommon = async () =>
                        {
                            if (string.IsNullOrEmpty(Cache.CommonVersion))
                            {
                                commonVersionCacheUpdated = true;

                                Logger.LogInformation("Cache missed: common version");

                                Cache.CommonVersion = await GetCommonVersion();
                            }
                            if (string.IsNullOrEmpty(Cache.IndexHtml))
                            {
                                indexHtmlCacheUpdated = true;

                                Logger.LogInformation("Cache missed: index.html");

                                var indexHtmlPath = string.Format("{0}common/{1}/index.html", Blob, Cache.CommonVersion);
                                Cache.IndexHtml = await GetTextContent(indexHtmlPath);
                            }
                        };

                        Func<Task> ensurePage = async () =>
                        {
                            if (!Cache.PageVersion.ContainsKey(subDomain))
                            {
                                pageVersionCacheUpdated = true;

                                Logger.LogInformation("Cache missed: page version for " + subDomain);

                                Cache.PageVersion[subDomain] = await GetPageVersion(subDomain);
                            }
                        };

                        // TO DO concurrent access blob and wait all
                        await Task.WhenAll(ensureCommon(), ensurePage());

                        var html = PopulateIndexHtml(subDomain);

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

                        Task.Run(async () =>
                        {
                            // TO DO potential threading conflict if task is updating cache and a new request is accessing the cache
                            // TO DO concurrent access blob
                            if (!commonVersionCacheUpdated)
                            {
                                Logger.LogInformation("Cache hit: common version, lazy updating cache");
                                Cache.CommonVersion = await GetCommonVersion();
                            }
                            if (!indexHtmlCacheUpdated)
                            {
                                Logger.LogInformation("Cache hit: index html, lazy updating cache");
                                var indexHtmlPath = string.Format("{0}common/{1}/index.html", Blob, Cache.CommonVersion);
                                Cache.IndexHtml = await GetTextContent(indexHtmlPath);
                            }
                            if (!pageVersionCacheUpdated)
                            {
                                Logger.LogInformation("Cache hit: page version, lazy updating cache " + subDomain);

                                Cache.PageVersion[subDomain] = await GetPageVersion(subDomain);
                            }
                        }).ConfigureAwait(false);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("Page not found!");
                    }
                });

                endpoints.MapPost("/subscription", async context =>
                {
                    // TO DO
                });

                endpoints.MapGet("/manifest.json", async context =>
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("");
                });
            });
        }

        public string PopulateIndexHtml(string page)
        {
            var indexHtml = "";
            var pageVersion = Cache.PageVersion[page];
            var bundlePath = string.Format("{0}common/{1}/static", Cdn, Cache.CommonVersion);
            var pageConfigPath = string.Format("{0}pages/{1}/{2}/config.js", Cdn, page, pageVersion);
            indexHtml = Cache.IndexHtml.Replace("%%config.js%%", pageConfigPath).Replace("./static", bundlePath);

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
