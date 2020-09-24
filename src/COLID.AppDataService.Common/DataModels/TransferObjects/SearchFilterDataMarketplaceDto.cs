using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class SearchFilterDataMarketplaceDto : DtoBase
    {
#nullable enable
        public string? Name { get; set; }
#nullable disable

        public JObject FilterJson { get; set; }
    }
}
