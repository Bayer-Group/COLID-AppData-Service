using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;

namespace COLID.AppDataService.Repositories.Interface
{
    /// <summary>
    /// Repository to handle all message template related operations.
    /// </summary>
    public interface IMessageTemplateRepository : IGenericRepository<MessageTemplate, int>
    {
        /// <summary>
        /// Fetches a single message template, determined by the given type.
        /// </summary>
        /// <param name="type">the message type to search for</param>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        MessageTemplate GetOne(MessageType type);

        /// <summary>
        /// Check if a entity exists which has the same information as the given
        /// dto and writes in into the out param. If no entity was found, it will be empty.
        /// </summary>
        /// <param name="dto">the dto containing values to search for</param>
        /// <param name="entity">the found entity otherwise null</param>
        /// <return>true if entity was found, otherwise false</return>
        bool TryGetOne(MessageTemplateDto dto, out MessageTemplate entity);

        /// <summary>
        /// Check if a entity exists for the given type and writes in into the out param.
        /// If no entity was found, it will be empty.
        /// </summary>
        /// <param name="messageType">the message type to search for</param>
        /// <param name="entity">the found entity otherwise null</param>
        /// <return>true if entity was found, otherwise false</return>
        bool TryGetOne(MessageType messageType, out MessageTemplate entity);
    }
}
