using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataModels.TransferObjects
{
    public class ColidEntrySubscriptionDetailsDto
    {
        public Uri ColidPidUri { get; set; }
        public string ResourceLabel { get; set; }
        public string ResourceDefinition { get; set; }
        public string ResourceType { get; set; }

        public ColidEntrySubscriptionDetailsDto(Uri colidPidUri, string resourceLabel, string resourceDefinition, string resourceType)
        {
            ColidPidUri = colidPidUri;
            ResourceLabel = resourceLabel;
            ResourceDefinition = resourceDefinition;
            ResourceType = resourceType;
        }
    }
}
