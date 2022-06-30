using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Exceptions;

namespace COLID.AppDataService.Services.Interface
{
    /// <summary>
    /// Service to handle all message template related operations.
    /// </summary>
    public interface IMessageTemplateService : IServiceBase<MessageTemplate>
    {
        /// <summary>
        /// Fetches a single message template, identified by a given type.
        /// </summary>
        /// <param name="type">the type to search for</param>
        /// <exception cref="EntityNotFoundException">if no entity was found</exception>
        MessageTemplate GetOne(MessageType type);

        /// <summary>
        /// Create a new message template by a given dto.
        /// </summary>
        /// <param name="messageTemplateDto">the dto to use</param>
        /// <exception cref="EntityAlreadyExistsException">if the message type is already assigned to another template</exception>
        MessageTemplate Create(MessageTemplateDto messageTemplateDto);

        /// <summary>
        /// Updates the message template.
        /// </summary>
        /// <param name="messageTemplateDto">the template to set</param>
        /// <exception cref="EntityNotFoundException">if no entity was found</exception>
        /// <exception cref="EntityNotChangedException">if no entity was found</exception>
        MessageTemplate Update(MessageTemplateDto messageTemplateDto);
    }
}
