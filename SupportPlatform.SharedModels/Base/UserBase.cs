using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupportPlatform.SharedModels.Base
{
    public enum PlatformRole { Volunteer, Requester }

    public interface IUserBaseFields
    {
        public string Name { get; set; }
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
    }
    public class UserBaseFields : IUserBaseFields
    {
        public string Name { get; set; }
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
    }

    public class UserBase : UserBaseFields
    {
        public string Id { get; set; }
    }
}
