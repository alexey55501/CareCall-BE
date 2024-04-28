using System.Net;

namespace SupportPlatform.SharedModels.DTO.Http
{
    public class StatusResponse<T>
        where T : class
    {
        public T Response { get; set; }
        public HttpStatusCode Status { get; set; }
    }
}
