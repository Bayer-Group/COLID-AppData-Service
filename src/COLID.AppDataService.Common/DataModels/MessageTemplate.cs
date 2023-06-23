using COLID.AppDataService.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace COLID.AppDataService.Common.DataModel
{
    public class MessageTemplate : EntityBase<int>
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
