using System.Text.Json.Serialization;
using COLID.AppDataService.Common.Enums;
using Newtonsoft.Json.Converters;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class MessageTemplateDto : DtoBase
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
