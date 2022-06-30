using System;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class BroadcastMessageDto : DtoBase
    {
        public string Subject { get; set; }
        public string Body { get; set; }

        public BroadcastMessageDto(string subject, string body)
        {
            Subject = subject;
            Body = body;
        }
    }
}
