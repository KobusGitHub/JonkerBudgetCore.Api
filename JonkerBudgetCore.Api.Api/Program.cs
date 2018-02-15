using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace JonkerBudgetCore.Api.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var template = "{Timestamp:yyyy-MM-dd HH:mm:ss} {Level} {Username} {Message}{NewLine}{Exception}";
            var logFileLocation = @"logs\log-{Date}.txt";            

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Information()
               .Enrich.FromLogContext()
               .WriteTo.LiterateConsole(outputTemplate: template)
               .WriteTo.RollingFile(logFileLocation, outputTemplate: template, retainedFileCountLimit: 31)                  
               .CreateLogger();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://0.0.0.0:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()                
                .Build();

            host.Run();
        }
    }
}
