using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace ContactManagement.DAL.Singletons
{
    public class LoggerSingleton
    {
        private static readonly Lazy<LoggerSingleton> _instance =
            new(() => new LoggerSingleton());

        public static LoggerSingleton Instance => _instance.Value;

        private readonly ILogger<LoggerSingleton> _logger;

        private LoggerSingleton()
        {
            // Create a ServiceCollection to get an ILogger instance
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .BuildServiceProvider();

            _logger = serviceProvider.GetRequiredService<ILogger<LoggerSingleton>>();
        }

        public void Log(string message)
        {
            _logger.LogInformation($"[LOG]: {message}");
        }
    }
}
