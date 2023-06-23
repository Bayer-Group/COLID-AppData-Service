using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.AppDataService.Common.DataModel
{
    public class ApplicationVersion
    {
        public string Application { get; set; }
        public string PipelineId { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
