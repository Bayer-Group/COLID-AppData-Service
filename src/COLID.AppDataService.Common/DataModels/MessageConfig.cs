using System.ComponentModel.DataAnnotations.Schema;
using COLID.AppDataService.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace COLID.AppDataService.Common.DataModel
{
    public class MessageConfig : EntityBase<int>
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SendInterval SendInterval { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeleteInterval DeleteInterval { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
