using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataModels.TransferObjects
{
    public class ColidEntryMostSubscriptionsDto
    {
        public Uri ResourcePidUri { get; set; }
        public int SubscriptionsCount { get; set; }
    }
}
