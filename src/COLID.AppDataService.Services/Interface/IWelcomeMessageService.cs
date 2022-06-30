using COLID.AppDataService.Common.DataModel;

namespace COLID.AppDataService.Services.Interface
{
    /// <summary>
    /// Service to handle all welcome message related operations.
    /// </summary>
    public interface IWelcomeMessageService : IServiceBase<WelcomeMessage>
    {
        /// <summary>
        /// Fetches the welcome message for editor.
        /// </summary>
        WelcomeMessage GetWelcomeMessageEditor();

        /// <summary>
        /// Fetches the welcome message for data marketplace.
        /// </summary>
        WelcomeMessage GetWelcomeMessageDataMarketplace();

        /// <summary>
        /// Updates the welcome message for editor.
        /// </summary>
        /// <param name="content">the content to set</param>
        WelcomeMessage UpdateWelcomeMessageEditor(string content);

        /// <summary>
        /// Updates the welcome message for data marketplace.
        /// </summary>
        /// <param name="content">the content to set</param>
        WelcomeMessage UpdateWelcomeMessageDataMarketplace(string content);
    }
}
