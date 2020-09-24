using System;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Exceptions;

namespace COLID.AppDataService.Repositories.Interface
{
    /// <summary>
    /// Repository to handle all consumer group related operations.
    /// </summary>
    public interface IConsumerGroupRepository : IGenericRepository<ConsumerGroup, int>
    {
        /// <summary>
        /// Fetches a single consumer group, identified by a given uri.
        /// </summary>
        /// <param name="uri">the uri to search for</param>
        /// <exception cref="UriFormatException">If the given uri doesn't consist of a valid uri scheme</exception>
        /// <exception cref="EntityNotFoundException">in case that no consumer group exists for a given uri</exception>
        ConsumerGroup GetOne(Uri uri);

        /// <summary>
        /// Check if a consumer group with the given uri exists and writes in into the out param.
        /// If no consumer group was found, it will be empty.
        /// </summary>
        /// <param name="uri">the consumer group uri to search for</param>
        /// <param name="consumerGroup">the found consumer group otherwise null</param>
        /// <exception cref="UriFormatException">If the given uri doesn't consist of a valid uri scheme</exception>
        /// <return>true if consumer group was found, otherwise false</return>
        bool TryGetOne(Uri uri, out ConsumerGroup consumerGroup);
    }
}
