using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using COLID.AppDataService.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace COLID.AppDataService.Common.DataModel
{
    public class WelcomeMessage : EntityBase<int>
    {
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public ColidType Type { get; set; }

        [Required]
        [Column(TypeName = "text")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}
