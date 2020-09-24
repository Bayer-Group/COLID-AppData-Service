using System;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class MessageDto : DtoBase
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        // When the message has been created
        public DateTime CreatedAt { get; set; }

        // When the message has been read
        public DateTime? ReadOn { get; set; }

        // When should the message be send to the user
        public DateTime? SendOn { get; set; }

        // When the message should be deleted
        public DateTime? DeleteOn { get; set; }
    }
}
