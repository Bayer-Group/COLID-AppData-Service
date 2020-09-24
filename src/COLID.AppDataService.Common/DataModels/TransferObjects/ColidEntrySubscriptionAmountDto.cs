using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class ColidEntrySubscriptionAmountDto
    {
        public Uri ColidPidUri { get; set; }

        public int Subscribers { get; set; }
    }
}
