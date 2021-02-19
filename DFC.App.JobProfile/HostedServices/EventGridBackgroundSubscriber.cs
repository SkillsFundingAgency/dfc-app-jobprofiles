// TODO: remove me!
//using DFC.App.JobProfile.Data.Contracts
//using DFC.Compui.Telemetry.HostedService
//using Microsoft.Extensions.Hosting
//using Microsoft.Extensions.Logging
//using System.Threading
//using System.Threading.Tasks
//namespace DFC.App.JobProfile.HostedServices
//[
//    public class EventGridBackgroundSubscriber :
//        BackgroundService
//    [
//        private readonly ILogger<EventGridBackgroundSubscriber> _logger
//        private readonly IEventGridSubscriptionService _eventGridSubscription
//        private readonly IHostedServiceTelemetryWrapper _telemetryWrapper
//        public EventGridBackgroundSubscriber(
//            ILogger<EventGridBackgroundSubscriber> logger,
//            IEventGridSubscriptionService eventGridSubscription,
//            IHostedServiceTelemetryWrapper telemetryWrapper)
//        [
//            _logger = logger
//            _eventGridSubscription = eventGridSubscription
//            _telemetryWrapper = telemetryWrapper
//        ]
//        public override Task StartAsync(CancellationToken cancellationToken)
//        [
//            _logger.LogInformation("Event subscription create started")
//            return base.StartAsync(cancellationToken)
//        ]
//        public override Task StopAsync(CancellationToken cancellationToken)
//        [
//            _logger.LogInformation("Event subscription create stopped")
//            return base.StopAsync(cancellationToken)
//        ]
//        protected override Task ExecuteAsync(CancellationToken stoppingToken)
//        [
//            _logger.LogInformation("Event subscription create executing")
//            var task = _telemetryWrapper.Execute(() => _eventGridSubscription.CreateAsync(), nameof(EventGridBackgroundSubscriber))
//            if (!task.IsCompletedSuccessfully)
//            [
//                _logger.LogInformation("Event subscription create didn't complete successfully")
//                if task.Exception != null
//                [
//                    _logger.LogError(task.Exception.ToString())
//                    throw task.Exception
//                ]
//            ]
//            return task
//        ]
//    ]
//]
