using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HttpMediatR
{
    /// <summary>
    /// Provides a common MediatR handler to respond to ASP.NET Core MVC requests.
    /// </summary>
    /// <typeparam name="TRequest">The type of model in the HTTP Request.</typeparam>
    public abstract class HttpHandler<TRequest> : IRequestHandler<TRequest, IActionResult>
        where TRequest : class, IHttpRequest
    {
        private readonly ILogger _logger;

        protected HttpHandler(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Handle(TRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace(nameof(Handle));

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogDebug("Beginning request: {@request}", request);

            var response = await HandleAsync(request, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Got a response: {@response}", response);

            if (!response.Succeeded)
            {
                return new ContentResult
                {
                    Content = response.ErrorMessage,
                    StatusCode = (int)response.HttpStatusCode
                };
            }

            if (response.Model != null)
            {
                return new JsonResult(response.Model)
                {
                    StatusCode = (int)response.HttpStatusCode
                };
            }

            return new StatusCodeResult((int)response.HttpStatusCode);
        }

        /// <summary>
        /// Handler to respond to input.
        /// </summary>
        /// <param name="input">The input passed to MediatR.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task<HttpResponse> HandleAsync(TRequest input, CancellationToken cancellationToken);

        /// <summary>
        /// Respond successfully to a HTTP request with a 200 (OK) response and model.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="response">The type of model to respond with.</param>
        /// <returns></returns>
        protected HttpResponse Ok<TResponse>(TResponse response) where TResponse : class
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return new HttpResponse
            {
                Model = response,
                HttpStatusCode = HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Respond successfully to a HTTP request with a 201 (Created) response.
        /// </summary>
        /// <returns></returns>
        protected HttpResponse Created()
        {
            return new HttpResponse
            {
                HttpStatusCode = HttpStatusCode.Created
            };
        }

        /// <summary>
        /// Respond successfully to a HTTP request with a 201 (Created) response and model.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="response">The type of model to respond with.</param>
        /// <returns></returns>
        protected HttpResponse Created<TResponse>(TResponse response) where TResponse : class
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return new HttpResponse
            {
                Model = response,
                HttpStatusCode = HttpStatusCode.Created
            };
        }

        /// <summary>
        /// Respond successfully to a HTTP request with a 204 (No Content) response.
        /// </summary>
        /// <returns></returns>
        protected HttpResponse NoContent()
        {
            return new HttpResponse
            {
                HttpStatusCode = HttpStatusCode.NoContent
            };
        }

        /// <summary>
        /// Return a 404 (Not Found) response.
        /// </summary>
        /// <returns></returns>
        protected HttpResponse NotFound()
        {
            return new HttpResponse
            {
                ErrorMessage = "Status Code: 404; Not Found",
                HttpStatusCode = HttpStatusCode.NotFound
            };
        }

        /// <summary>
        /// Return a 409 (Conflict) response.
        /// </summary>
        /// <param name="errorMessage">The error message to respond with.</param>
        /// <returns></returns>
        protected HttpResponse Conflict(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }

            return new HttpResponse
            {
                ErrorMessage = errorMessage,
                HttpStatusCode = HttpStatusCode.Conflict
            };
        }
    }
}