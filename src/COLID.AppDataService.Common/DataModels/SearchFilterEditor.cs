using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.DataModel
{
    public class SearchFilterEditor : Entity<int>
    {
        public JObject FilterJson { get; set; }

        [JsonIgnore]
        public ICollection<User> Users { get; set; }
    }
}
