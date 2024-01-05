using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DataModels.TransferObjects
{
    public class UpdatedResourceStoredQueryDto
    {
        public string PidUri { get; set; }
        public string ResourceLabel { get; set; }

        public UpdatedResourceStoredQueryDto(string pidUri, string resourceLabel)
        {
            PidUri = pidUri;
            ResourceLabel = resourceLabel;
        }
    }
}
