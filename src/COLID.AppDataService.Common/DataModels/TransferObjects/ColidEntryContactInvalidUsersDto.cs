using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.DataModels.TransferObjects
{
    public class ColidEntryContactInvalidUsersDto
    {
        public string ContactMail { get; set; }

        public List<ColidEntryInvalidUsersDto> ColidEntries { get; set; }

        public ColidEntryContactInvalidUsersDto()
        {
            ColidEntries = new List<ColidEntryInvalidUsersDto>();
        }
    }
}
