using System;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class UserDto : DtoBase
    {
        public Guid Id { get; set; }

        public string EmailAddress { get; set; }
    }
}
