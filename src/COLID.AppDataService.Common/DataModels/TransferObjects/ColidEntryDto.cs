using System;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class ColidEntryDto : DtoBase
    {
        public Uri ColidPidUri { get; set; }

        public string Label { get; set; }
    }
}
