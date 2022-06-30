using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace COLID.AppDataService.Common.DataModel
{
    public class ConsumerGroup : EntityBase<int>
    {
        [Required]
        public Uri Uri { get; set; }

        [JsonIgnore]
        public ICollection<User> Users { get; set; }
    }
}
