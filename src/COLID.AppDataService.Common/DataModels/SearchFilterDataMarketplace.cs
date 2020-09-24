using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.DataModel
{
    public class SearchFilterDataMarketplace : Entity<int>
    {
        public string Name { get; set; }

        public JObject FilterJson { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
