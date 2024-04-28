using Microsoft.AspNetCore.Identity;
using SupportPlatform.API.DAL.Models;
using SupportPlatform.SharedModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SupportPlatform.DAL.Models
{
    public class ApplicationUser : IdentityUser, IUserBaseFields
    {
        public ApplicationUser()
        {
            UserRoles = new HashSet<UserRole>();
            Requests = new HashSet<Request>();
        }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Role { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime RegisterDate { get; set; }

        public PlatformRole PlatformRole { get; set; }
        public bool IsActivated { get; set; }

        public string ImageBase64 { get; set; } // max 1mb
        public string Description { get; set; }

        // Other
        public DateTime? BanDate { get; set; }
        public string BanReason { get; set; }
        [StringLength(256)]
        public string RoleBeforeBan { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public bool IsBanned => BanDate != null;
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<Request> Requests { get; set; }

    }
}
