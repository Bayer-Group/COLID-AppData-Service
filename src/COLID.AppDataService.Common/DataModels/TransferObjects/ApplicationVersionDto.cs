using System;
using System.Collections.Generic;
using System.Text;
using COLID.AppDataService.Common.DataModel;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class ApplicationVersionDto
    {
        public IList<ApplicationVersion> ApplicationVersion { get; set; }
        public ApplicationVersion ColidVersion { get; set; }

    }
}
