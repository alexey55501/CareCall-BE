using System.Linq;
using System.Web.Mvc;

namespace SupportPlatform.CORE.Helpers
{
    public static class ValidationHelper
    {
        public static object Validate(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                var errors = modelState.Select(x => x.Value.Errors)
                              .Where(y => y.Count > 0)
                              .ToList();
                return new { errors = errors.Select(t => t[0]) };
            }
            return null;
        }
    }
}
