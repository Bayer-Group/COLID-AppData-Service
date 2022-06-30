using COLID.AppDataService.Common.Enums;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class StoredQueryDto : DtoBase
    {
        public ExecutionInterval ExecutionInterval { get; set; }

        public int SearchFilterDataMarketplaceId { get; set; }

    }
}
