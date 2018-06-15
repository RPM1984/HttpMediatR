using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HttpMediatR
{
    /// <summary>
    /// Marker interface to be used for MediatR queries/commands.
    /// </summary>
    public interface IHttpRequest : IRequest<IActionResult>
    {
    }
}