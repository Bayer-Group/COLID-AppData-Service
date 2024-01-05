using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class SearchFilterDataMarketplaceDto : DtoBase
    {
#nullable enable
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? SearchTerm { get; set; }
        public string? PidUri { get; set; }

#nullable disable
        public JObject FilterJson { get; set; }
    }
}
