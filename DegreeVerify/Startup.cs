using DegreeVerify.Client.Services;
using DegreeVerify.Client.Services.IServices;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Net.Http;


namespace DegreeVerify.Client
{
    public class Startup
    {
        //public void Configuration(IAppBuilder app)
        //{
        //    app.UseHangfireDashboard();
        //    app.UseHangfireServer();
        //}
        //public IConfiguration _configuration { get; }
        //public Startup(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddSingleton<HttpClient>();
            services.AddScoped<IAppSettings, AppSettings>();
            services.AddScoped<DegreeVerifyService>();
            services.AddScoped<TokenService>();
            services.AddScoped<HealthService>();
            //services.AddScoped<HostedProgram>();
        }

        //public void Configure(IHostApplicationLifetime appLifetime, IServiceProvider serviceProvider)
        //{
        //    // Configure the application to perform tasks on startup and shutdown
        //    appLifetime.ApplicationStarted.Register(() =>
        //    {
        //        // Enqueue a job to be executed
        //        var backgroundJobClient = serviceProvider.GetRequiredService<IBackgroundJobClient>();
        //        backgroundJobClient.Enqueue(() => Console.WriteLine("Hello, Hangfire!"));
        //    });
        //}


    }
}
