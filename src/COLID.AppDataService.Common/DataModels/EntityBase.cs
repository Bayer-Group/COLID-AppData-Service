using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace COLID.AppDataService.Common.DataModel
{
    public abstract class EntityBase
    {
        [JsonIgnore]
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }

        [JsonIgnore]
        [DataType(DataType.Date)]
        public DateTime? ModifiedAt { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
