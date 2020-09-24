using System.Collections.Generic;
using COLID.AppDataService.Common.DataModel;

namespace COLID.AppDataService.Repositories.Interface
{
    /// <summary>
    /// Repository to handle all message related operations.
    /// </summary>
    public interface IMessageRepository : IGenericRepository<Message, int>
    {
        /// <summary>
        /// Fetches all messages and filter out every entry that has no SendOn date set
        /// and where the SendOn date is older than current timestamp.
        /// </summary>
        IList<Message> GetUnreadMessagesToSend();
    }
}
