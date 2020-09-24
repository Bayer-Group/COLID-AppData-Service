using COLID.AppDataService.Common.Enums;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class StoredQueryDto : DtoBase
    {
        public string QueryName { get; set; }

        public JObject QueryJson { get; set; }

        public ExecutionInterval ExecutionInterval { get; set; }
    }
}
