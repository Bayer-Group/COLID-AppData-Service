using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class DistributionEndpointMessageDto : DtoBase
    {
        public string UserEmail { get; set; }

        public string ResourceLabel { get; set; }
        public Uri DistributionEndpoint { get; set; }
        public string ColidEntryPidUri { get; set; }

        public string DistributionEndpointPidUri { get; set; }
    }
}
