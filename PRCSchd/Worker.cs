using Quartz.Spi;
using Quartz;

namespace PRCSchd
{
    public class Worker : BackgroundService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private IScheduler _scheduler;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, ISchedulerFactory schedulerFactory, IJobFactory jobFactory)
        {
            _logger = logger;
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _scheduler = await _schedulerFactory.GetScheduler(stoppingToken);
            _scheduler.JobFactory = _jobFactory;

            var job = JobBuilder.Create<EmailJob>()
                .WithIdentity("EmailJob")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("EmailJob-trigger")
                .WithCronSchedule("0 0/5 * * * ?") // Every 5 minutes
                .ForJob(job)
                .Build();

            await _scheduler.ScheduleJob(job, trigger, stoppingToken);
            await _scheduler.Start(stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken); // Keep the worker alive
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            if (_scheduler != null)
            {
                await _scheduler.Shutdown(stoppingToken);
            }

            await base.StopAsync(stoppingToken);
        }
    }
}
