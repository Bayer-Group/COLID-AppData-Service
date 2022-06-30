using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace COLID.AppDataService.Common.Search
{
    public enum SearchIndex
    {
        [EnumMember(Value = "draft")]
        Draft,

        [EnumMember(Value = "published")]
        Published,

        [EnumMember(Value = "all")]
        All
    }
}
