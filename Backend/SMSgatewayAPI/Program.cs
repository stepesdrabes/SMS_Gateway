using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SMSgatewayAPI
{
    public static class Program
    {
        // Main method that gets called by the runtime
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Information);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();

                // IP for testing purposes
                webBuilder.UseUrls("http://192.168.1.3:5000/");
            });
    }
}