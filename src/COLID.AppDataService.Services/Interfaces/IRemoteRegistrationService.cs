using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Search;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Services.Interfaces
{
    /// <summary>
    /// Service to handle communication and authentication with the external RegistrationService.
    /// </summary>
    public interface IRemoteRegistrationService
    {

        /// <summary>
        /// Register the Saved Search as a PID URI
        /// </summary>
        /// <param name="searchFilterDataMarketplaceDto"></param>
        /// <returns>We get the DTO back with the updated PIDURI value</returns>
        Task<SearchFilterDataMarketplaceDto> RegisterSavedSearches(SearchFilterDataMarketplaceDto searchFilterDataMarketplaceDto);

        /// <summary>
        /// Removes the Saved Search PID URI from Nginx Config
        /// </summary>
        /// <param name="pidUri"></param>
        /// <returns></returns>
        Task RemovePidUriFromConfig(string pidUri);
    }
}
