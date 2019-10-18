using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace Kronos.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("kRONOS Running");

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", true);
                    if (args != null) config.AddCommandLine(args);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration);
                    logging.AddConsole();
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddSingleton<IScheduler>((_) => Scheduler.Default);
                    services.AddSingleton<ITimeProvider, ReactiveBasedTimeProvider>();
                    services.AddSingleton<IScheduleProvider, ScheduleProvider>();
                    services.AddSingleton<IHomeAssistantControl, HomeAssistantControl>();
                    services.AddHostedService<SchedulerHostedService>();
                });

            await builder.RunConsoleAsync();
        }
    }
}
