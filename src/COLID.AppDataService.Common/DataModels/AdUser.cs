using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.AppDataService.Common.DataModel
{
    public class AdUser : AdObject
    {
        public string GivenName { get; set; }

        public string SurName { get; set; }

        public string Cwid { get; set; }

        public string Department { get; set; }
    }
}
