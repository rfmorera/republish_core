using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Republish
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            ServicePoint sp1, sp2;
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(10, 10);
            
            // Upper Limit of Connections
            ServicePointManager.DefaultConnectionLimit = 50;
            ServicePointManager.MaxServicePoints = 25;
            Uri uriRevolico = new Uri("https://www.revolico.com");
            Uri uri2Captcha = new Uri("https://www.2captcha.com");

            sp1 = ServicePointManager.FindServicePoint(uriRevolico);
            sp2 = ServicePointManager.FindServicePoint(uri2Captcha);

            // Configuration ServicePoint Revolico
            sp1.ConnectionLimit = 100;
            sp1.ConnectionLeaseTimeout = 6000;

            // Configuration ServicePoint 2Captcha
            sp2.ConnectionLimit = 100;
            sp2.ConnectionLeaseTimeout = 6000;

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
