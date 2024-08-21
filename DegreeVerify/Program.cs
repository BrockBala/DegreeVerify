using DegreeVerify.Client.Services.IServices;
using DegreeVerify.Client.Services;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Owin.Hosting.Services;
using System.Threading;

namespace DegreeVerify.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();
            //var serviceProvider = builder.Services.GetRequiredService<IServiceProvider>();
            //var app = serviceProvider.GetRequiredService<HostedProgram>();
            //app.StartAsync(cancellationToken: new System.Threading.CancellationToken(false));
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                string ConnectionString = hostContext.Configuration.GetConnectionString("Cred2DBConnection");
                GlobalConfiguration.Configuration.UseSqlServerStorage(ConnectionString);

                //Registered Services
                services.AddSingleton<HttpClient>();
                services.AddScoped<DegreeVerifyService>();
                services.AddScoped<AppSettings>();
                services.AddScoped<TokenService>();
                services.AddScoped<HealthService>();
                services.AddScoped<HostedProgram>();
                GlobalConfiguration.Configuration.UseActivator(new HangfireJobActivator(services.BuildServiceProvider()));

                using (WebApp.Start("http://localhost:5008", app =>
                {
                    app.UseHangfireDashboard("/hangfire", new DashboardOptions
                    {
                        Authorization = new[] { new MyAuthorizationFilter() }
                    });
                    //app.UseHangfireServer();
                }))
                {
                    Console.WriteLine("Hangfire Dashboard is running on http://localhost:5008/hangfire");

                    //var serviceProvider = services.BuildServiceProvider();
                    //var app = serviceProvider.GetRequiredService<HostedProgram>();
                    //app.StartAsync(cancellationToken: new System.Threading.CancellationToken(false));
                    using (var server = new BackgroundJobServer())
                    {
                        // Start your jobs here
                        //RecurringJob.AddOrUpdate(
                        //    "my-recurring-job",
                        //    () => Console.WriteLine("Hello, Hangfire!"),
                        //    Cron.Minutely);
                        RecurringJob.AddOrUpdate<DegreeVerifyService>("myrecurringjob", job => job.DegreeVerify(), Cron.Minutely);
                        Console.WriteLine("Degree Service has been started...!!");
                        Console.WriteLine("Press Enter to exit...");
                        Console.ReadLine();
                    }
                }


            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                // Configure app configuration
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureHostConfiguration(config =>
            {
                // Optionally configure additional configuration sources
            })
            .UseConsoleLifetime();

    }

}

