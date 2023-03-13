using ChatIngress.Slack;
using ChatIngress.Teams;
using Contoso.Extensions;
using KFrame;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using SlackNet;
using SlackNet.AspNetCore;
using System;

namespace ChatIngress.Ingress
{
    public class Startup
    {
        public Startup(IConfiguration config) => Configuration = ConfigBase.Configuration = config;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new Config();
            services.AddControllers();
            services
                //.AddSfdcContext(config)
                //.AddSfccContext(config)
                //.AddSlackContext(config)
                //.AddKanbanContext(config)
                //.AddMediatR(
                //    typeof(SlackContext),
                //    typeof(KanbanContext))
                .AddSingleton<Func<IMediator>>(c => () => c.GetService<IMediator>())
                .AddMemoryCache();
            //services
            //    .RegisterBackendServices()
            //    .RegisterCoreServices()
            //    .RegisterCrmServices()
            //    .RegisterFinanceServices()
            //    .RegisterKanbanServices()
            //    .RegisterKudoServices()
            //    //.RegisterLibraryServices()
            //    .RegisterO365Services()
            //    .RegisterPlanServices()
            //    .RegisterSfccServices()
            //    .RegisterSlackServices()
            //    .RegisterStaffServices();
            services.AddSlackNet(c => c
                .UseApiToken(Config.ApiToken)
            //.RegisterCoreServices()
            //.RegisterCrmServices()
            //.RegisterFinanceServices()
            //.RegisterKanbanServices()
            //.RegisterKudoServices()
            ////.RegisterLibraryServices()
            //.RegisterO365Services()
            //.RegisterPlanServices()
            //.RegisterSfccServices()
            //.RegisterSlackServices()
            //.RegisterStaffServices()
            );
            services.AddLogging(c => c.AddNLog("nlog.config"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> log)
        {
            SlackExtensions.Log = log;
            TeamsExtensions.Log = log;
            //FinanceService.DbName = "Unanet";
            //KanbanService.DbName = "Unanet";
            //PlanService.DbName = "Unanet";
            //SlackService.DbName = "Unanet";

            log.LogInformation($"Using KframeUrl {KFrameManager.Config.KframeUrl}");
            if (KFrameManager.Config.Certificate != null) log.LogInformation($"Using Certificate {KFrameManager.Config.Certificate.Subject}");

            //log.LogInformation("----- DBO -----");
            //using (var db = new DbService().GetAzureConnection()) db.Execute("Select 1");

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else app.UseHttpsRedirection();

            app.UseSlackNet(c => c
                .VerifyWith(Config.VerificationToken)
                .MapToPrefix("slack-api"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}