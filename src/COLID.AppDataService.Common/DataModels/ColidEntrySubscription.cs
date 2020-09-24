using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace COLID.AppDataService.Common.DataModel
{
    public class ColidEntrySubscription : Entity<int>
    {
        public Uri ColidPidUri { get; set; }

        // Users can add Notes to subscribed entries for admins
        public string Note { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
