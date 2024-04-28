using SupportPlatform.SharedModels.Base;
using SupportPlatform.SharedModels.DTO.Filters.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPlatform.SharedModels.DTO.Filters.Recruitment
{
    public class RequestsFilterModel : BaseFilterModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public CategoryType? Category { get; set; } 
    }
}
