using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SupportPlatform.API.BLL.Helpers
{
    public static class HttpHelper
    {
        public static HttpStatusCode GetHttpStatusCode(IActionResult functionResult)
        {
            try
            {
                return (HttpStatusCode)functionResult
                    .GetType()
                    .GetProperty("StatusCode")
                    .GetValue(functionResult, null);
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
