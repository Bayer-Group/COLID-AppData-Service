using System;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;

namespace COLID.AppDataService.Services.Interface
{
    /// <summary>
    /// Service to handle all consumer group related operations.
    /// </summary>
    public interface IConsumerGroupService : IGenericService<ConsumerGroup, int>
    {
        /// <summary>
        /// Fetches a single consumer group, identified by a given dto.
        /// </summary>
        /// <param name="consumerGroupDto">the consumer group to search for</param>
        /// <exception cref="EntityNotFoundException">in case that no consumer group exists for a given dto</exception>
        ConsumerGroup GetOne(ConsumerGroupDto consumerGroupDto);

        /// <summary>
        /// Fetches a single consumer group, identified by a given uri.
        /// </summary>
        /// <param name="uri">the uri to search for</param>
        /// <exception cref="EntityNotFoundException">in case that no consumer group exists for a given uri</exception>
        ConsumerGroup GetOne(Uri uri);

        /// <summary>
        /// Check if a consumer group for the given dto exists and writes in into the out param.
        /// If no consumer group was found, it will be empty.
        /// </summary>
        /// <param name="consumerGroupDto">the consumer group to search for</param>
        /// <param name="consumerGroup">the found consumer group otherwise null</param>
        /// <return>true if consumer group was found, otherwise false</return>
        bool TryGetOne(ConsumerGroupDto consumerGroupDto, out ConsumerGroup consumerGroup);

        /// <summary>
        /// Create a new consumer group by a given dto.
        /// </summary>
        /// <param name="consumerGroupDto">the dto to use</param>
        /// <exception cref="UriFormatException">If the given dto doesn't consist of a valid uri scheme</exception>
        /// <exception cref="EntityAlreadyExistsException">If a consumer group with the given uri already exists</exception>
        ConsumerGroup Create(ConsumerGroupDto consumerGroupDto);

        /// <summary>
        /// Create a new consumer group asynchronously with a given dto.
        /// </summary>
        /// <param name="consumerGroupDto">the dto to use</param>
        /// <exception cref="UriFormatException">If the given dto doesn't consist of a valid uri scheme</exception>
        /// <exception cref="EntityAlreadyExistsException">If a consumer group with the given uri already exists</exception>
        Task<ConsumerGroup> CreateAsync(ConsumerGroupDto consumerGroupDto);

        /// <summary>
        /// Delete a consumer group, identified by the given dto.
        /// </summary>
        /// <param name="consumerGroupDto">the dto of a consumer group</param>
        /// <exception cref="UriFormatException">If the given dto doesn't consist of a valid uri scheme</exception>
        /// <exception cref="EntityNotFoundException">in case that no consumer group exists for a given uri</exception>
        void Delete(ConsumerGroupDto consumerGroupDto);
    }
}
