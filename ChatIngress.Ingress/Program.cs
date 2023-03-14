using ChatIngress.Slack;
using ChatIngress.Teams;
using Contoso.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using SlackNet.AspNetCore;
using System;

namespace ChatIngress.Ingress
{
    public class Program
    {
        public static IConfiguration Configuration { get; private set; }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Configuration = ConfigBase.Configuration = builder.Configuration;
            builder.WebHost
                .UseUrls("http://0.0.0.0:80")
                .UseNLog();

            ConfigureServices(builder.Services);

            var app = builder.Build();

            //log.LogInformation($"Using KframeUrl {KFrameManager.Config.KframeUrl}");
            //if (KFrameManager.Config.Certificate != null) log.LogInformation($"Using Certificate {KFrameManager.Config.Certificate.Subject}");

            //log.LogInformation("----- DBO -----");
            //using (var conn = new DbService().GetAzureConnection()) conn.Execute("Select 1");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //app.UseStaticFiles();

            app.UseSlackNet(c => c
                .VerifyWith(Config.Slack.Password)
                .MapToPrefix("slack-api"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });

            app.Run();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            var config = new Config();
            services.AddControllers();
            services
                .AddSlackContext(config)
                .AddTeamsContext(config)
                //.AddMediatR(s => s.
                //    typeof(SlackContext),
                //    typeof(TeamsContext))
                .AddSingleton<Func<IMediator>>(c => () => c.GetService<IMediator>())
                .AddMemoryCache();
            //services
            //    .RegisterSlackServices()
            //    .RegisterTeamsServices();
            services.AddSlackNet(c => c
                .UseApiToken(Config.Slack.UserName)
            );
            services.AddLogging(c => c.AddNLog("nlog.config"));
        }
    }
}