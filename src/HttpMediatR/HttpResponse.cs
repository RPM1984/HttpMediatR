using System;
using System.Net;

namespace HttpMediatR
{
    /// <summary>
    /// Encapsulates a Mediated HTTP response.
    /// </summary>
    /// <typeparam name="T">The type of model in the response.</typeparam>
    public class HttpResponse<T> where T : class
    {
        /// <summary>
        /// Create a response with just a status code.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        public HttpResponse(HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Create a response with a status code and model.
        /// </summary>
        /// <param name="model">The model to send in the response.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        public HttpResponse(T model, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Create a failed response.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        public HttpResponse(string errorMessage = "Sorry, an error occured :(",
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }

            HttpStatusCode = httpStatusCode;
            ErrorMessage = errorMessage;
        }

        public T Model { get; }
        public HttpStatusCode HttpStatusCode { get; }
        public string ErrorMessage { get; }
        public bool Succeeded => ErrorMessage == null;
    }
}