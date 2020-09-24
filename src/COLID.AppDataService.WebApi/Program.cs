using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace COLID.AppDataService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            PrintStartMessage();

            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");

            Console.WriteLine("- ASPNETCORE_URLS = " + urls);
            return WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(urls);
        }

        public static void PrintStartMessage()
        {
            Console.Write(@"
     __________  __    ________              ___                ____        __
    / ____/ __ \/ /   /  _/ __ \            /   |  ____  ____  / __ \____ _/ /_____ _
   / /   / / / / /    / // / / /  ______   / /| | / __ \/ __ \/ / / / __ `/ __/ __ `/
  / /___/ /_/ / /____/ // /_/ /  /_____/  / ___ |/ /_/ / /_/ / /_/ / /_/ / /_/ /_/ /
  \____/\____/_____/___/_____/           /_/  |_/ .___/ .___/_____/\__,_/\__/\__,_/
                                               /_/   /_/
");
        }
    }
}
