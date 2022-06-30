using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.Search;

namespace COLID.AppDataService.Services.Interface
{
    /// <summary>
    /// Service to handle communication and authentication with the external SearchService.
    /// </summary>
    public interface IRemoteSearchService
    {
        Task<SearchResultDTO> Search(SearchRequestDto searchRequest);
    }
}
