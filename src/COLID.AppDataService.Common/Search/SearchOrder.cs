using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace COLID.AppDataService.Common.Search
{
    public enum SearchOrder
    {
        [EnumMember(Value = "asc")]
        Asc,

        [EnumMember(Value = "desc")]
        Desc
    }
}
