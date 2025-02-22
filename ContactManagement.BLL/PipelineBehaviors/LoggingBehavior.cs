using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactManagement.BLL.PipelineBehaviors
{

    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }


        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Log request details here
            _logger.LogInformation($"Handling {typeof(TRequest).Name}");

            var response = await next();

            // Log response details here
            _logger.LogInformation($"Handled {typeof(TResponse).Name}");

            return response;
        }
    }
}
