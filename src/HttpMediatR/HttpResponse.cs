using System.Net;

namespace HttpMediatR
{
    /// <summary>
    /// Encapsulates a Mediated HTTP response.
    /// </summary>
    public class HttpResponse
    {
        public object Model { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool Succeeded => ErrorMessage == null;
    }
}