using System;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    /// <summary>
    /// Custom transfer object for user related messages, with minimized user information.
    /// </summary>
    public class MessageUserDto : DtoBase
    {
        #region message related

        public int Id { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        // When the message has been raed
        public DateTime? ReadOn { get; set; }

        // When should the message be send to the user
        public DateTime? SendOn { get; set; }

        // When the message should be deleted
        public DateTime? DeleteOn { get; set; }

        #endregion message related

        #region user related

        public Guid UserId { get; set; }

        public string UserEmail { get; set; }

        #endregion user related
    }
}
