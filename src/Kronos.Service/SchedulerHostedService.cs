using Kronos.Framework.Schedule;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kronos.Service
{
    public class SchedulerHostedService : IHostedService
    {
        private readonly ILogger<SchedulerHostedService> _logger;
        private readonly TimeSpan _tickPeriod;
        private readonly IScheduler _scheduler;
        private readonly ITimeProvider _timeProvider;
        private readonly ICollection<ScheduledBoolean> _scheduledItems;
        private readonly IHomeAssistantControl _homeAssistantControl;
        private IDisposable _tickSubscription;
        private bool _heatingLastState = false;
        private bool _firstRun = true;

        public SchedulerHostedService(
            ILogger<SchedulerHostedService> logger,
            IScheduler scheduler,
            ITimeProvider timeProvider,
            IScheduleProvider scheduleProvider,
            IHomeAssistantControl homeAssistantControl)
        {
            _logger = logger;
            _tickPeriod = TimeSpan.FromSeconds(1);
            _scheduler = scheduler;
            _timeProvider = timeProvider;
            _scheduledItems = scheduleProvider.GetScheduledItems();
            _homeAssistantControl = homeAssistantControl;

            _logger.LogInformation("Scheduled Items:");
            foreach (var item in _scheduledItems)
            {
                _logger.LogInformation(item.ToString());
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _tickSubscription = Observable
                .Interval(_tickPeriod, _scheduler)
                .Subscribe(Tick);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tickSubscription.Dispose();
            return Task.CompletedTask;
        }

        private void Tick(long tickNumber)
        {
            _logger.LogDebug($"Tick:{tickNumber}");

            if (_firstRun)
            {
                _firstRun = false;
                _homeAssistantControl.SwitchHotwaterAsync(false).Wait();
                _homeAssistantControl.SwitchHeatingAsync(false).Wait();
            }

            var now = _timeProvider.NowUtc();
            foreach (var item in _scheduledItems)
            {
                var output = item.GetOutput(now);

                switch (item.Name)
                {
                    case "HotWater":
                        if (output.lastValue != output.currentValue)
                        {
                            _logger.LogInformation($"'{item.Name}' From:'{(output.currentValue ? "On" : "Off")} ' to:'{(output.lastValue ? "On" : "Off")}' Now:'{now}'");
                            _homeAssistantControl.SwitchHotwaterAsync(output.currentValue)
                                .Wait();
                        }
                        break;
                    case "Heating":
                        if (output.currentValue == false)
                        {
                            if (output.lastValue != output.currentValue || _heatingLastState)
                            {
                                _logger.LogInformation($"'{item.Name}' From:'{(output.currentValue ? "On" : "Off")} ' to:'{(output.lastValue ? "On" : "Off")}' Now:'{now}'");

                                _homeAssistantControl.SwitchHeatingAsync(false).Wait();
                                _heatingLastState = false;
                            }
                        }
                        else
                        {
                            //See if we need to turn it on.
                            var currentTemp = _homeAssistantControl.GetCurrentHouseTemperature();
                            var targetTemp = _homeAssistantControl.GetSetTemperature();

                            if (currentTemp > targetTemp)
                            {
                                if (_heatingLastState)
                                {
                                    _logger.LogInformation($"{currentTemp} > {targetTemp} Turning Heating off");
                                    _homeAssistantControl.SwitchHeatingAsync(false).Wait();
                                    _heatingLastState = false;
                                }
                            }
                            else
                            {
                                if (!_heatingLastState)
                                {
                                    _logger.LogInformation($"{currentTemp} < {targetTemp} Turning Heating on");
                                    _homeAssistantControl.SwitchHeatingAsync(true).Wait();
                                    _heatingLastState = true;
                                }
                            }
                        }

                        break;
                    default:
                        _logger.LogWarning($"Unknown Schedule Target:'{item.Name}'");
                        break;
                }
            }
        }

        
    }
}


