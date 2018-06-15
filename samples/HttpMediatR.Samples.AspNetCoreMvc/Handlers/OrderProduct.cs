using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HttpMediatR.Samples.AspNetCoreMvc.Handlers
{
    public static class OrderProduct
    {
        public class Input : IHttpRequest
        {
            public int ProductId { get; set; }
            public bool CauseConflict { get; set; }
        }

        public class Output
        {
            public int OrderId { get; set; }
        }

        public class Handler : HttpHandler<Input, Output>
        {
            private readonly ILogger<Handler> _logger;

            public Handler(ILogger<Handler> logger) : base(logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            protected override Task<HttpResponse<Output>> HandleAsync(Input input)
            {
                _logger.LogTrace(nameof(HandleAsync));

                if (input.ProductId == 0)
                {
                    return Task.FromResult(NotFound());
                }

                if (input.CauseConflict)
                {
                    return Task.FromResult(Conflict("Oops! Conflict occured :("));
                }

                return Task.FromResult(Created(new Output
                {
                    OrderId = input.ProductId + 1
                }));
            }
        }
    }
}