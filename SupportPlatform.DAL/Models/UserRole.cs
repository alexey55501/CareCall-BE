using Microsoft.AspNetCore.Identity;

namespace SupportPlatform.DAL.Models
{
    public class UserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual Role Role { get; set; }
    }
}
