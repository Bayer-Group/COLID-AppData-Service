using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COLID.AppDataService.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace COLID.AppDataService.Common.DataModel
{
    public class WelcomeMessage : Entity<int>
    {
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public ColidType Type { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Content { get; set; }
    }
}
