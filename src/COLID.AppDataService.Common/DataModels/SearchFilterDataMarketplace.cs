using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.DataModel
{
    public class SearchFilterDataMarketplace : EntityBase<int>
    {
        public string Name { get; set; }

        public JObject FilterJson { get; set; }

        public string? SearchTerm { get; set; }

        public string? PidUri { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual StoredQuery StoredQuery { get; set; }
        [JsonIgnore]
        public virtual int? StoredQueryId { get; set; }

    }
}
