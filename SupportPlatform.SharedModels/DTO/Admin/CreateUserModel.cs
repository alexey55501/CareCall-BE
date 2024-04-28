using SupportPlatform.SharedModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPlatform.SharedModels.DTO.Admin
{
    public class CreateUserModel : UserBaseFields
    {
        public string Password { get; set; }

        // For Manager
        public List<int> WarehouseIds { get; set; }
    }
}
