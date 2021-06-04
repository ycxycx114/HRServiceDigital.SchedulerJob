using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;
using System;

namespace HRServiceDigital.SchedulerJob.Quartz
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                // base configuration from appsettings.json
                services.Configure<QuartzOptions>(hostContext.Configuration.GetSection("Quartz"));
                // if you are using persistent job store, you might want to alter some options
                services.Configure<QuartzOptions>(options =>
                {
                    options.Scheduling.IgnoreDuplicates = true; // default: false
                    options.Scheduling.OverWriteExistingData = true; // default: true
                });

                // see Quartz.Extensions.DependencyInjection documentation about how to configure different configuration aspects
                services.AddQuartz(q =>
                {
                    // your configuration here

                    q.UseMicrosoftDependencyInjectionJobFactory();

                    q.UseSimpleTypeLoader();

                    q.UsePersistentStore(config =>
                    {
                        config.UseProperties = true;
                        config.RetryInterval = TimeSpan.FromSeconds(15);
                        config.UseSqlServer(sqlServer =>
                        {
                            sqlServer.ConnectionString = hostContext.Configuration.GetConnectionString("QuartzDb");
                        });
                        config.UseJsonSerializer();
                    });

                    q.UseDedicatedThreadPool(tp => tp.MaxConcurrency = 10);
                });

                // Quartz.Extensions.Hosting hosting
                services.AddQuartzHostedService(options =>
                {
                    // when shutting down we want jobs to complete gracefully
                    options.WaitForJobsToComplete = true;
                });
            });
    }
}
