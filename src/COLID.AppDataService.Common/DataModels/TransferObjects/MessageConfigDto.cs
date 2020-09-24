using COLID.AppDataService.Common.Enums;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class MessageConfigDto : DtoBase
    {
        public SendInterval SendInterval { get; set; }

        public DeleteInterval DeleteInterval { get; set; }
    }
}
