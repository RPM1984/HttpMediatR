using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HttpMediatR.Samples.AspNetCoreMvc.Handlers
{
    public static class DeleteProduct
    {
        public class Input : IHttpRequest
        {
            public int ProductId { get; set; }
        }
        
        public class Handler : HttpHandler<Input>
        {
            private readonly ILogger<Handler> _logger;

            public Handler(ILogger<Handler> logger) : base(logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            protected override Task<HttpResponse> HandleAsync(Input input, CancellationToken cancellationToken)
            {
                _logger.LogTrace(nameof(HandleAsync));

                if (input.ProductId == 0)
                {
                    return Task.FromResult(NotFound());
                }

                return Task.FromResult(NoContent());
            }
        }
    }
}