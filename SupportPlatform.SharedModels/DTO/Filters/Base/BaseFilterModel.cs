using Newtonsoft.Json;
using System.Collections.Generic;

namespace SupportPlatform.SharedModels.DTO.Filters.Base
{
    public enum SortOrder { ASC, DESC }

    public class BaseFilterModel
    {
        public string SearchQuery { get; set; } = string.Empty;
        //public List<string> SearchFields { get; set; } = new List<string>();
        public SortOrder OrderBy { get; set; }
        public string OrderField { get; set; }

        public int? Page { get; set; } = 1;
        public int? AmountOnPage { get; set; } = 15;

        [JsonIgnore]
        public bool IsPaginationEnabled => (Page != null && AmountOnPage != null);
        [JsonIgnore]
        public int Take => (AmountOnPage <= 0 ? 1 : AmountOnPage) ?? int.MaxValue;
        [JsonIgnore]
        public int Skip =>
            IsPaginationEnabled ?
                (
                    Page == 1 ? 0 : ((Page - 1) * AmountOnPage ?? 0)
                ) : 0;
    }
}
