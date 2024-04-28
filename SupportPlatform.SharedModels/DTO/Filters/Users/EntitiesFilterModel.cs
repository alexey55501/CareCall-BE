using SupportPlatform.SharedModels.DTO.Filters.Base;
using System.Collections.Generic;

namespace SupportPlatform.SharedModels.DTO.Filters.Users
{
    public enum DeletedUsersViewType { OnlyActive, OnlyDeleted, All }
    public class EntitiesFilterModel : BaseFilterModel
    {
        public bool? IsActivated { get; set; }
        public string Id { get; set; }
    }
}
