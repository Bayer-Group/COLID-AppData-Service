using System;
using System.Collections.Generic;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;
using COLID.Exception.Models.Business;

namespace COLID.AppDataService.Services.Interface
{
    /// <summary>
    /// Service to handle all consumer group related operations.
    /// </summary>
    public interface IConsumerGroupService : IServiceBase<ConsumerGroup>
    {
        /// <summary>
        /// Fetches a single consumer group, identified by a given uri.
        /// </summary>
        /// <param name="uri">the uri to search for</param>
        /// <exception cref="EntityNotFoundException">in case that no consumer group exists for a given uri</exception>
        ConsumerGroup GetOne(Uri uri);

        /// <summary>
        /// Create a new consumer group asynchronously with a given dto.
        /// </summary>
        /// <param name="consumerGroupDto">the dto to use</param>
        /// <exception cref="UriFormatException">If the given dto doesn't consist of a valid uri scheme</exception>
        /// <exception cref="EntityAlreadyExistsException">If a consumer group with the given uri already exists</exception>
        ConsumerGroup Create(ConsumerGroupDto consumerGroupDto);

        /// <summary>
        /// Delete a consumer group, identified by the given dto.
        /// </summary>
        /// <param name="consumerGroupDto">the dto of a consumer group</param>
        /// <exception cref="UriFormatException">If the given dto doesn't consist of a valid uri scheme</exception>
        /// <exception cref="EntityNotFoundException">in case that no consumer group exists for a given uri</exception>
        void Delete(ConsumerGroupDto consumerGroupDto);
    }
}
