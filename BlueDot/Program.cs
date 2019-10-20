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
            
            // Upper Limit of Connections
            ServicePointManager.DefaultConnectionLimit = 250;
            ServicePointManager.MaxServicePoints = 60;
            Uri uriRevolico = new Uri("https://www.revolico.com");
            Uri uri2Captcha = new Uri("https://www.2captcha.com");

            sp1 = ServicePointManager.FindServicePoint(uriRevolico);
            sp2 = ServicePointManager.FindServicePoint(uri2Captcha);

            // Configuration ServicePoint Revolico
            sp1.ConnectionLimit = 200;
            sp1.ConnectionLeaseTimeout = 15000;

            // Configuration ServicePoint 2Captcha
            sp2.ConnectionLimit = 200;
            sp2.ConnectionLeaseTimeout = 15000;

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
