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
    /// <typeparam name="TResponse">The type of model to respond with.</typeparam>
    public abstract class HttpHandler<TRequest, TResponse> : IRequestHandler<TRequest, IActionResult>
        where TRequest : class, IHttpRequest
        where TResponse : class
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
        /// <param name="query">The query passed to MediatR.</param>
        /// <returns></returns>
        protected abstract Task<HttpResponse<TResponse>> HandleAsync(TRequest input, CancellationToken cancellationToken);

        /// <summary>
        /// Respond successfully to a HTTP request with a 200 (OK) response.
        /// </summary>
        /// <returns></returns>
        protected HttpResponse<TResponse> Ok()
        {
            return new HttpResponse<TResponse>(HttpStatusCode.OK);
        }

        /// <summary>
        /// Respond successfully to a HTTP request with a 200 (OK) response and model.
        /// </summary>
        /// <param name="response">The type of model to respond with.</param>
        /// <returns></returns>
        protected HttpResponse<TResponse> Ok(TResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return new HttpResponse<TResponse>(response);
        }

        /// <summary>
        /// Respond successfully to a HTTP request with a 201 (Created) response.
        /// </summary>
        /// <returns></returns>
        protected HttpResponse<TResponse> Created()
        {
            return new HttpResponse<TResponse>(HttpStatusCode.Created);
        }

        /// <summary>
        /// Respond successfully to a HTTP request with a 201 (Created) response and model.
        /// </summary>
        /// <param name="response">The type of model to respond with.</param>
        /// <returns></returns>
        protected HttpResponse<TResponse> Created(TResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return new HttpResponse<TResponse>(response, HttpStatusCode.Created);
        }

        /// <summary>
        /// Respond successfully to a HTTP request with a 204 (No Content) response.
        /// </summary>
        /// <param name="response">The type of model to respond with.</param>
        /// <returns></returns>
        protected HttpResponse<TResponse> NoContent(TResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return new HttpResponse<TResponse>(response, HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Return a 404 (Not Found) response.
        /// </summary>
        /// <returns></returns>
        protected HttpResponse<TResponse> NotFound()
        {
            return new HttpResponse<TResponse>("Status Code: 404; Not Found", HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Return a 409 (Conflict) response.
        /// </summary>
        /// <param name="errorMessage">The error message to respond with.</param>
        /// <returns></returns>
        protected HttpResponse<TResponse> Conflict(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }

            return new HttpResponse<TResponse>(errorMessage, HttpStatusCode.Conflict);
        }
    }
}