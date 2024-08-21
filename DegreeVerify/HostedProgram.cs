using DegreeVerify.Client.Services;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DegreeVerify.Client
{
    internal sealed class HostedProgram : IHostedService
    {
        private readonly ILogger<HostedProgram> _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IHostEnvironment _environment;
        private readonly DegreeVerifyService _degreeVerifyService;
        private readonly IBackgroundJobClient _backgroundJobServer;
        public HostedProgram(
            ILogger<HostedProgram> logger,
            IHostApplicationLifetime appLifetime,
            IHostEnvironment environment,
             DegreeVerifyService degreeVerifyService,
             IBackgroundJobClient backgroundJobClient
            )
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _environment = environment;
            _degreeVerifyService = degreeVerifyService;
            _backgroundJobServer = backgroundJobClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                RecurringJob.AddOrUpdate<DegreeVerifyService>("myrecurringjob", job => job.DegreeVerify(), Cron.Minutely);
                Console.WriteLine("Degree Service has been started...!!");

            }
            catch(Exception ex)
            {

            }
            //await _degreeVerifyService.DegreeVerify();
            //await _degreeVerifyService.DOAVerify();
            //await _degreeVerifyService.DegreeHistory();
            //await _degreeVerifyService.DOAHistory();
            //await _degreeVerifyService.Cancel();
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }


    }
}
