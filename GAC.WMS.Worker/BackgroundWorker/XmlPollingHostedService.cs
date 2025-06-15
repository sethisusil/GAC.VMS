using Cronos;
using GAC.WMS.Worker.XmlHelpers;

namespace GAC.WMS.Worker.BackgroundWorker
{
    public class XmlPollingHostedService : BackgroundService
    {
        private readonly ILogger<XmlPollingHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly CronExpression _cron;
        private Timer? _timer;

        public XmlPollingHostedService(
            ILogger<XmlPollingHostedService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;

            // Read CRON expression from config
            var cronExpression = _configuration["XmlPolling:Schedule"];
            _cron = CronExpression.Parse(cronExpression ?? "*/5 * * * *"); // default: every 5 min
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(XmlPollingHostedService)}.{nameof(ExecuteAsync)}: XmlPollingHostedService is starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    var next = _cron.GetNextOccurrence(now);
                    var delay = next?.Subtract(now) ?? TimeSpan.FromMinutes(5);
                    _logger.LogInformation($"{nameof(XmlPollingHostedService)}.{nameof(ExecuteAsync)}: Next execution scheduled at {next?.ToString("o") ?? "unknown"} (in {delay.TotalSeconds} seconds).");
                    await Task.Delay(delay, stoppingToken);

                    _logger.LogInformation($"{nameof(XmlPollingHostedService)}.{nameof(ExecuteAsync)}: XmlPollingHostedService is processing XML files.");
                    using var scope = _serviceProvider.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IXmlFileProcessor>();
                    await processor.ProcessAsync();
                    _logger.LogInformation($"{nameof(XmlPollingHostedService)}.{nameof(ExecuteAsync)}: XmlPollingHostedService has completed processing XML files.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing XML files.");
                }

                _logger.LogInformation($"{nameof(XmlPollingHostedService)}.{nameof(ExecuteAsync)}: XmlPollingHostedService completed processing.");              
            }
        }
    }
}
