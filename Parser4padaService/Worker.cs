using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AngleSharp;
using AngleSharp.Html.Parser;

namespace Parser4padaService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private PgSqlWrapper sql;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var url = @"https://4pda.ru/";
            var client = new WebClient();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding win1251 = Encoding.GetEncoding("windows-1251");
            client.Encoding = win1251;
            sql = new PgSqlWrapper(Environment.GetEnvironmentVariable("connstring"));
            var par = new HtmlParser();

            while (!stoppingToken.IsCancellationRequested)
            {
                var response = client.DownloadString(url);
                var doc = par.ParseDocument(response);
                var posts = doc.QuerySelectorAll("article[class='post']");
                foreach (var item in posts.Skip(1))
                {
                    string itemid = item.GetAttribute("itemid");
                    string title = item.QuerySelector("div").QuerySelector("a").GetAttribute("title");
                    string date = item.QuerySelector("div").QuerySelector("div[class='v-panel']").QuerySelector("div[class='p-description']").QuerySelector("em[class='date']").TextContent;
                    string link = item.QuerySelector("div[class='more-box']").QuerySelector("a[class='btn-more']").GetAttribute("href");
                    sql.Update(itemid, title, link, date);
                }
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(3600000, stoppingToken);
            }
        }
    }
}
