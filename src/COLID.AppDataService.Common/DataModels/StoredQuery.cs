using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COLID.AppDataService.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.DataModel
{
    public class StoredQuery : Entity<int>
    {
        public string QueryName { get; set; }

        public JObject QueryJson { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ExecutionInterval ExecutionInterval { get; set; }

        public JObject LastExecutionResult { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NextExecutionAt { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
