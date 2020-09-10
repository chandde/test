using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartPageServer
{
    public class SubdomainRootHandler
    {
        public SubdomainRootHandler(AppConfiguration config)
        {
            configuration = config;
            cache = new Cache()
            {
                PageVersion = new Dictionary<string, string>(),
            };
        }

        public AppConfiguration configuration { get; }
        public Cache cache { get; }

        public async Task HandleRequest(HttpContext context, ILogger<Startup> logger)
        {
            var host = context.Request.Host.Host.ToLower();
            var subDomain = host.Substring(0, host.IndexOf(configuration.ShortDomain) - 1);

            bool commonVersionCacheUpdated = false;
            bool indexHtmlCacheUpdated = false;
            bool pageVersionCacheUpdated = false;

            Func<Task> ensureCommon = async () =>
            {
                if (string.IsNullOrEmpty(cache.CommonVersion))
                {
                    commonVersionCacheUpdated = true;

                    logger.LogInformation("Cache missed: common version");

                    cache.CommonVersion = await GetCommonVersion();
                }
                if (string.IsNullOrEmpty(cache.IndexHtml))
                {
                    indexHtmlCacheUpdated = true;

                    logger.LogInformation("Cache missed: index.html");

                    var indexHtmlPath = string.Format("{0}common/index.{1}.html", configuration.Blob, cache.CommonVersion);
                    cache.IndexHtml = await GetTextContent(indexHtmlPath);
                }
            };

            Func<Task> ensurePage = async () =>
            {
                if (!cache.PageVersion.ContainsKey(subDomain))
                {
                    pageVersionCacheUpdated = true;

                    logger.LogInformation("Cache missed: page version for " + subDomain);

                    cache.PageVersion[subDomain] = await GetPageVersion(subDomain);
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
                    logger.LogInformation("Cache hit: common version, lazy updating cache");
                    cache.CommonVersion = await GetCommonVersion();
                }
                if (!indexHtmlCacheUpdated)
                {
                    logger.LogInformation("Cache hit: index html, lazy updating cache");
                    var indexHtmlPath = string.Format("{0}common/index.{1}.html", configuration.Blob, cache.CommonVersion);
                    cache.IndexHtml = await GetTextContent(indexHtmlPath);
                }
                if (!pageVersionCacheUpdated)
                {
                    logger.LogInformation("Cache hit: page version, lazy updating cache " + subDomain);

                    cache.PageVersion[subDomain] = await GetPageVersion(subDomain);
                }
            }).ConfigureAwait(false);
        }

        public async Task<string> GetCommonVersion()
        {
            var url = String.Format("{0}common/version.txt", configuration.Blob);
            return await GetTextContent(url);
        }

        public async Task<string> GetPageVersion(string page)
        {
            var url = string.Format("{0}pages/{1}/version.txt", configuration.Blob, page);
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

        public string PopulateIndexHtml(string page)
        {
            var indexHtml = "";
            var pageVersion = cache.PageVersion[page];
            var bundlePath = string.Format("{0}common/", configuration.Cdn);
            var pageConfigPath = string.Format("{0}pages/{1}/config_{2}.js", configuration.Cdn, page, pageVersion);
            indexHtml = cache.IndexHtml.Replace("%%config.js%%", pageConfigPath).Replace("./", bundlePath);

            return indexHtml;
        }
    }
}
