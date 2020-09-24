using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class AdUserDto
    {
        public string Id { get; set; }

        public string Mail { get; set; }

        public bool AccountEnabled { get; set; }

        public AdUserDto(string id, string mail, bool accEnabled)
        {
            Id = id;
            Mail = mail;
            AccountEnabled = accEnabled;
        }
    }
}
