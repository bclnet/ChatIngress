using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace ChatIngress.Ingress
{
    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(b =>
                {
                    b.UseUrls("http://0.0.0.0:80");
                    b.UseNLog();
                    b.UseStartup<Startup>();
                });
    }
}