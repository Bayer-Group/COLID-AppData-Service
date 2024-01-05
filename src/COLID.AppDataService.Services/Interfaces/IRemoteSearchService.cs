using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.Search;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Services.Interfaces
{
    /// <summary>
    /// Service to handle communication and authentication with the external SearchService.
    /// </summary>
    public interface IRemoteSearchService
    {
        Task<SearchResultDTO> Search(SearchRequestDto searchRequest);

        /// <summary>
        /// Get Documents By PID URIs
        /// </summary>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        Task<IDictionary<string, IEnumerable<JObject>>> GetDocumentsByIds(IEnumerable<string> identifiers, bool includeDraft = false);
    }
}
