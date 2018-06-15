using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HttpMediatR.Samples.AspNetCoreMvc.Handlers
{
    public static class GetProduct
    {
        public class Input : IHttpRequest
        {
            public int Id { get; set; }
        }

        public class Output
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Handler : HttpHandler<Input, Output>
        {
            private readonly ILogger<Handler> _logger;

            public Handler(ILogger<Handler> logger) : base(logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            protected override Task<HttpResponse<Output>> HandleAsync(Input input)
            {
                _logger.LogTrace(nameof(HandleAsync));

                if (input.Id == 0)
                {
                    return Task.FromResult(NotFound());
                }

                return Task.FromResult(Ok(new Output
                {
                    Id = input.Id,
                    Name = $"Product #{input.Id}"
                }));
            }
        }
    }
}