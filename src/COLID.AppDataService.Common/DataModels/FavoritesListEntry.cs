using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace COLID.AppDataService.Common.DataModel
{
    public class FavoritesListEntry : EntityBase<int>
    {
        [JsonIgnore]
        [ForeignKey("FavoritesListId")]
        public virtual FavoritesList FavoritesLists { get; set; }

        public string PIDUri { get; set; }

        public string PersonalNote { get; set; }

    }
}
