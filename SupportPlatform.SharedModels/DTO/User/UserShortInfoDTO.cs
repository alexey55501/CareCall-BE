using SupportPlatform.SharedModels.Base;
using System;
using System.Collections.Generic;

namespace SupportPlatform.SharedModels.DTO.User
{
    public class UserShortInfoDTO : UserBase
    {
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public dynamic Skills { get; set; }
    }
}
