using COLID.AppDataService.Common.Enums;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class WelcomeMessageDto : DtoBase
    {
        public ColidType Type { get; set; }
        public string Content { get; set; }
    }
}
