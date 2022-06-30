using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COLID.AppDataService.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.DataModel
{
    public class StoredQuery : EntityBase<int>
    {
        public virtual SearchFilterDataMarketplace SearchFilterDataMarketplace { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ExecutionInterval ExecutionInterval { get; set; }

        public int NumberSearchResults { get; set; }

        public string SearchResultHash { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LatestExecutionDate { get; set; }

    }
}
