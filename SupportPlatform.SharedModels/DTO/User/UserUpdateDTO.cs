using SupportPlatform.SharedModels.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupportPlatform.SharedModels.DTO.User
{
    public class UserUpdateDTO //: UserBaseFields
    {
        public string Name { get; set; }
        public string ImageBase64 { get; set; } // max 1mb
        public string Description { get; set; }
    }
}
