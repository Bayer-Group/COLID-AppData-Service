using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.AppDataService.Common.DataModel
{
    public class AdSearchResult
    {
        public IList<AdGroup> Groups { get; set; }

        public IList<AdUser> Users { get; set; }

        public AdSearchResult(IList<AdGroup> groups, IList<AdUser> users)
        {
            Groups = groups;
            Users = users;
        }
    }
}
