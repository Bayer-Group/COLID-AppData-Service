using System;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Enums;

namespace COLID.AppDataService.Repositories.Interface
{
    /// <summary>
    /// Repository to handle all welcome message related operations.
    /// </summary>
    public interface IWelcomeMessageRepository : IGenericRepository<WelcomeMessage, int>
    {
        /// <summary>
        /// Fetches a single welcome message, determined by the given type.
        /// </summary>
        /// <param name="type">the colid type to search for</param>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        WelcomeMessage GetOne(ColidType type);
    }
}
