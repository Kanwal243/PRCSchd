using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PRCSchd;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Spi;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Configure Quartz services
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
        });

        services.AddSingleton<IJobFactory, MicrosoftDependencyInjectionJobFactory>();
        services.AddSingleton<EmailJob>(); // Register EmailJob
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

        // Add the worker to manage the Quartz scheduler
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
