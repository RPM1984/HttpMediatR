using HttpMediatR.Samples.AspNetCoreMvc.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace HttpMediatR.Tests
{
    public class Router : Controller
    {
        private readonly IMediator _mediator;

        public Router(IMediator mediator) => _mediator = mediator;

        [HttpGet("/products/{productId:int}")]
        public Task<IActionResult> GetProduct(GetProduct.Input query, CancellationToken cancellationToken)
            => _mediator.Send(query, cancellationToken);

        [HttpPost("/orders")]
        public Task<IActionResult> OrderProduct([FromBody] OrderProduct.Input command, CancellationToken cancellationToken)
            => _mediator.Send(command, cancellationToken);

        [HttpDelete("/products/{productId:int}")]
        public Task<IActionResult> DeleteProduct(DeleteProduct.Input query, CancellationToken cancellationToken)
            => _mediator.Send(query, cancellationToken);
    }
}