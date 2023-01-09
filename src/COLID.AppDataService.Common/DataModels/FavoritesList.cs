using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace COLID.AppDataService.Common.DataModel
{
    public class FavoritesList : EntityBase<int>
    {
        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string Name { get; set; }

        public ICollection<FavoritesListEntry> FavoritesListEntries { get; set; }
    }
}
