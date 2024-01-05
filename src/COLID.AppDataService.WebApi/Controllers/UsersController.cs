using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using COLID.AppDataService.Common.Constants;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using COLID.AppDataService.Common.DataModel;

namespace COLID.AppDataService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public partial class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger,
            IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        #region [User]

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var all = await _userService.GetAllAsync();
            return Ok(all);
        }

        // TODO SL: Check with middleware if userId is in token (if role is admin, also allowed)
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var user = await _userService.GetOneAsync(userId);
            return Ok(user);
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateAsync([FromBody] UserDto userDto)
        {
            var userEntity = await _userService.CreateAsync(userDto);
            return Created($"api/users/{userEntity.Id}", userEntity);
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = ApplicationRoles.Administration)]
        public IActionResult Delete(Guid userId)
        {
            _userService.Delete(userId);
            return Ok();
        }

        [HttpPut("{userId}/emailAddress")]
        [Consumes(MediaTypeNames.Text.Plain)]
        public async Task<IActionResult> SetEmailAddress(Guid userId, [FromBody] string email)
        {
            var updatedEntity = await _userService.UpdateEmailAddressAsync(userId, email);
            return Ok(updatedEntity);
        }

        [HttpPut("{userId}/lastLoginEditor")]
        public async Task<IActionResult> SetLastLoginEditor(Guid userId, [FromBody] DateTime time)
        {
            var updatedEntity = await _userService.UpdateLastLoginColidAsync(userId, time);
            return Ok(updatedEntity);
        }

        [HttpPut("{userId}/lastLoginDataMarketplace")]
        public async Task<IActionResult> SetLastLoginDataMarketplace(Guid userId, [FromBody] DateTime time)
        {
            var updatedEntity = await _userService.UpdateLastLoginDataMarketplaceAsync(userId, time);
            return Ok(updatedEntity);
        }

        [HttpPut("{userId}/lastTimeChecked")]
        public async Task<IActionResult> SetLastTimeChecked(Guid userId, [FromBody] DateTime time)
        {
            var updatedEntity = await _userService.UpdateLastTimeCheckedAsync(userId, time);
            return Ok(updatedEntity);
        }

        #endregion [User]

        #region [Default Consumer Group]

        [HttpGet("{userId}/defaultConsumerGroup")]
        public async Task<IActionResult> GetDefaultConsumerGroup(Guid userId)
        {
            var cg = await _userService.GetDefaultConsumerGroupAsync(userId);
            return Ok(cg);
        }

        [HttpPut("{userId}/defaultConsumerGroup")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> SetDefaultConsumerGroup(Guid userId, [FromBody] ConsumerGroupDto consumerGroup)
        {
            var updatedUser = await _userService.UpdateDefaultConsumerGroupAsync(userId, consumerGroup);
            return Ok(updatedUser);
        }

        [HttpDelete("{userId}/defaultConsumerGroup")]
        public async Task<IActionResult> RemoveDefaultConsumerGroup(Guid userId)
        {
            var updatedUser = await _userService.RemoveDefaultConsumerGroupAsync(userId);
            return Ok(updatedUser);
        }

        #endregion [Default Consumer Group]

        #region [Search Filter - Editor]

        [HttpGet("{userId}/searchFilterEditor")]
        public async Task<IActionResult> GetSearchFilterEditor(Guid userId)
        {
            var searchFilterResult = await _userService.GetSearchFilterEditorAsync(userId);
            return Ok(searchFilterResult);
        }

        [HttpPut("{userId}/searchFilterEditor")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> SetSearchFilterEditor(Guid userId, [FromBody] JObject json)
        {
            var searchFilterResult = await _userService.UpdateSearchFilterEditorAsync(userId, json);
            return Ok(searchFilterResult);
        }

        [HttpDelete("{userId}/searchFilterEditor")]
        public async Task<IActionResult> RemoveSearchFilterEditor(Guid userId)
        {
            var searchFilterResult = await _userService.RemoveSearchFilterEditorAsync(userId);
            return Ok(searchFilterResult);
        }

        #endregion [Search Filter - Editor]

        #region [Default Search Filter - Data Marketplace]

        [HttpGet("{userId}/defaultSearchFilterDataMarketplace")]
        public async Task<IActionResult> GetDefaultSearchFilterDataMarketplace(Guid userId)
        {
            var searchFilter = await _userService.GetDefaultSearchFilterDataMarketplaceAsync(userId);
            return Ok(searchFilter);
        }

        [HttpPut("{userId}/defaultSearchFilterDataMarketplace/{id}")]
        public async Task<IActionResult> SetDefaultSearchFilterDataMarketplace(Guid userId, int id)
        {
            var updatedUser = await _userService.UpdateDefaultSearchFilterDataMarketplaceAsync(userId, id);
            return Ok(updatedUser);
        }

        [HttpDelete("{userId}/defaultSearchFilterDataMarketplace")]
        public async Task<IActionResult> RemoveDefaultSearchFilterDataMarketplace(Guid userId)
        {
            var updatedUser = await _userService.RemoveDefaultSearchFilterDataMarketplaceAsync(userId);
            return Ok(updatedUser);
        }

        [HttpGet("searchAllFiltersDataMarketplace")]
        public async Task<IActionResult> GetAllSearchFiltersDataMarketplace()
        {
            var searchFilter = await _userService.GetAllSearchFiltersDataMarketplaceAsync();
            return Ok(searchFilter);
        }

        #endregion [Default Search Filter - Data Marketplace]

        #region [Search Filter - Data Marketplace]

        [HttpGet("{userId}/searchFiltersDataMarketplace")]
        public async Task<IActionResult> GetAllSearchFiltersDataMarketplace(Guid userId)
        {
            var searchFilters = await _userService.GetSearchFiltersDataMarketplaceAsync(userId);
            return Ok(searchFilters);
        }

        [HttpGet("{userId}/searchFiltersDataMarketplace/{id}")]
        public async Task<IActionResult> GetSingleSearchFilterDataMarketplace(Guid userId, int id)
        {
            var searchFilter = await _userService.GetSearchFilterDataMarketplaceAsync(userId, id);
            return Ok(searchFilter);
        }

        [HttpPut("{userId}/searchFiltersDataMarketplace")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddSearchFilterDataMarketplace(Guid userId, [FromBody] SearchFilterDataMarketplaceDto searchFilterDto)
        {
            var user = await _userService.AddSearchFilterDataMarketplaceAsync(userId, searchFilterDto);
            return Ok(user);
        }

        [HttpDelete("{userId}/searchFiltersDataMarketplace/{id}")]
        public async Task<IActionResult> RemoveSearchFilterDataMarketplace(Guid userId, int id)
        {
            var user = await _userService.RemoveSearchFilterDataMarketplaceAsync(userId, id);
            return Ok(user);
        }

        #endregion [Search Filter - Data Marketplace]

        #region [Stored Queries]

        [HttpPost("processStoredQueries")]
        public async Task<IActionResult> ProcessStoredQueries()
        {
            await _userService.ProcessStoredQueries();
            return NoContent();
        }

        [HttpGet("allSubscribedSearchFiltersDataMarketplace")]
        public async Task<IActionResult> GetSearchFiltersDataMarketplaceOnlyWithStoredQueriesAsync()
        {
            var searchFiltersWithStoredQueries = await _userService.GetAllSearchFiltersDataMarketplaceOnlyWithStoredQueriesAsync();
            return Ok(searchFiltersWithStoredQueries);
        }

        [HttpGet("allSubscribedSearchFiltersDataMarketplaceCount")]
        public async Task<IActionResult> GetSearchFiltersDataMarketplaceCount()
        {
            var searchFiltersWithStoredQueries = await _userService.GetSearchFiltersDataMarketplaceCount();
            return Ok(searchFiltersWithStoredQueries);
        }

        [HttpGet("{userId}/subscribedSearchFiltersDataMarketplace")]
        public async Task<IActionResult> GetSearchFiltersDataMarketplaceOnlyWithStoredQueriesAsync(Guid userId)
        {
            var searchFiltersWithStoredQueries = await _userService.GetSearchFiltersDataMarketplaceOnlyWithStoredQueriesAsync(userId);
            return Ok(searchFiltersWithStoredQueries);
        }

        [HttpPut("{userId}/subscribeToSearchFilterDataMarketplace")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddStoredQueryToSearchFiltersDataMarketplaceAync(Guid userId, StoredQueryDto storedQueryDto)
        {
            var searchFilterWithStoredQueries = await _userService.AddStoredQueryToSearchFiltersDataMarketplaceAync(userId,storedQueryDto);
            return Ok(searchFilterWithStoredQueries);
        }

        [HttpDelete("{userId}/removeSubscriptionFromSearchFilterDataMarketplace/{id}")]
        public async Task<IActionResult> RemoveStoredQuery(Guid userId, int id)
        {
            var searchFilterWithStoredQueries = await _userService.RemoveStoredQueryFromSearchFiltersDataMarketplaceAync(userId, id);
            return Ok(searchFilterWithStoredQueries);
        }

        #endregion [Stored Queries]

        #region [COLID Entry Subsciption]

        [HttpGet("{userId}/colidEntrySubscriptions")]
        public async Task<IActionResult> GetAllColidEntrySubscriptions(Guid userId)
        {
            var colidEntrySubscriptionDto = await _userService.GetColidEntrySubscriptionsAsync(userId);
            return Ok(colidEntrySubscriptionDto);
        }

        [HttpGet("{userId}/latestSubscriptionsWithDetails")]
        public async Task<IActionResult> GetLatestColidEntrySubscriptionsOfUser(Guid userId)
        {
            var latestColidSubscriptions = await _userService.GetLatestColidEntrySubscriptionsOfUserAsync(userId);
            return Ok(latestColidSubscriptions);
        }

        [HttpGet("mostSubscribedResourceDetails")]
        public async Task<IActionResult> x()
        {
            var mostSubscribedResources = await _userService.GetMostSubscribedColidEntrySubscriptions();
            return Ok(mostSubscribedResources);
        }

        [HttpPut("{userId}/colidEntrySubscriptions")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddColidEntrySubscription(Guid userId, [FromBody] ColidEntrySubscriptionDto colidEntrySubscriptionDto)
        {
            var user = await _userService.AddColidEntrySubscriptionAsync(userId, colidEntrySubscriptionDto);
            return Ok(user);
        }

        [HttpDelete("{userId}/colidEntrySubscriptions")]
        public async Task<IActionResult> RemoveColidEntrySubscription(Guid userId, [FromBody] ColidEntrySubscriptionDto colidEntrySubscriptionDto)
        {
            var user = await _userService.RemoveColidEntrySubscriptionAsync(userId, colidEntrySubscriptionDto);
            return Ok(user);
        }

        #endregion [COLID Entry Subsciption]

        #region [Message Config]

        [HttpGet("{userId}/messageConfig")]
        public async Task<IActionResult> GetMessageConfig(Guid userId)
        {
            var messageConfig = await _userService.GetMessageConfigAsync(userId);
            return Ok(messageConfig);
        }

        [HttpPut("{userId}/messageConfig")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateMessageConfig(Guid userId, MessageConfigDto userMessageConfigDto)
        {
            var updatedEntity = await _userService.UpdateMessageConfigAsync(userId, userMessageConfigDto);
            return Ok(updatedEntity);
        }

        #endregion [Message Config]

        #region [Messages]

        [HttpGet("{userId}/messages")]
        public async Task<IActionResult> GetAllMessages(Guid userId)
        {
            var messages = await _userService.GetMessagesAsync(userId);
            return Ok(messages);
        }

        [HttpPut("{userId}/messages/markRead")]
        public async Task<IActionResult> MarkMessagesAsRead(Guid userId, [FromBody] ICollection<int> ids)
        {
            var message = await _userService.MarkMessagesAsReadAsync(userId, ids);
            return Ok(message);
        }

        [HttpDelete("{userId}/messages/{id}")]
        public async Task<IActionResult> DeleteMessage(Guid userId, int id)
        {
            await _userService.DeleteMessageAsync(userId, id);
            return Ok();
        }

        [HttpPut("{userId}/messages/{id}/markSent")]
        public async Task<IActionResult> MarkMessageAsSent(Guid userId, int id)
        {
            var message = await _userService.MarkMessageAsSentAsync(userId, id);
            return Ok(message);
        }

        [HttpPut("{userId}/messages/{id}/markRead")]
        public async Task<IActionResult> MarkMessageAsRead(Guid userId, int id)
        {
            var message = await _userService.MarkMessageAsReadAsync(userId, id);
            return Ok(message);
        }

        #endregion [Messages]
 
        #region [Favorites List]

        /// <summary>
        /// Fetches the favorites lists and its entries associated to an user. 
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <returns>A list containing an overview of FavoriteLists Names and its entries</returns>
        /// <response code="200">A list containing an overview of FavoriteLists Names and its entries</response>
        /// <response code="404">If user is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [ProducesResponseType(typeof(ICollection<FavoritesList>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("favoritesList/{userId}")]
        public async Task<IActionResult> GetFavoritesList(Guid userId)
        {
            var favoritesList = await _userService.GetFavoritesListsAsync(userId);
            return Ok(favoritesList);
        }

        [HttpGet("getAllFavoritesListCount")]
        public async Task<IActionResult> GetAllFavoritesListCount()
        {
            var favoritesList = await _userService.GetAllFavoritesListCount();
            return Ok(favoritesList);
        }

        /// <summary>
        /// Creates an entry into the favorites lists. If just the List Name is passed, creates a blank list 
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="favoritesListDto">the list name. PIDURI and Personal Note for List Entry</param>
        /// <remarks>
        ///   <br><b>Note:</b>If only Name is provided then it creates an empty favorite list. The PIDURI and PersonalNote are mandatory to make a favorite list entry</br>
        /// </remarks>
        /// <returns>A list containing an overview of FavoriteLists Names and its entries</returns>
        /// <response code="200">A list containing an overview of FavoriteLists Names and its entries</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPut("favoritesList/{userId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddFavoritesList(Guid userId, [FromBody] FavoritesListDto favoritesListDto)
        {
            var user = await _userService.AddFavoritesListAsync(userId, favoritesListDto);
            return Ok(user);
        }

        /// <summary>
        /// Insert multiple entries into favorites lists.
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="favoritesListEntriesDto">the list id. PIDURIs of the entries to be added </param>
        /// <returns>A list containing an overview of FavoriteLists Names and its entries</returns>
        /// <response code="200">A list containing an overview of FavoriteLists Names and its entries</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPut("favoritesListEntries/{userId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddFavoritesListEntries(Guid userId, [FromBody] IList<FavoritesListEntriesDTO> favoritesListEntriesDto)
        {
            var user = await _userService.AddFavoritesListEntriesAsync(userId, favoritesListEntriesDto);
            return Ok(user);
        }

        /// <summary>
        /// Creates a single entry into a favorite list provided by the ID of the favoritelist
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="favoritesListId"> the list ID for which entry needs to be made.</param>
        /// <param name="favoritesListDto">PIDURI and Personal Note for List Entry.</param>
        /// <remarks>
        ///   <br><b>Note:</b>The List Name in the DTO can be ignored as we are providing favoritesListId and only valid PIDURI and Personal Note are expected</br>
        /// </remarks>
        /// <returns>The Favorite List and the newly created entry</returns>
        /// <response code="200">The Favorite List and the newly created entry</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpPost("favoritesList/{userId}/{favoritesListId}")]
        [ProducesResponseType(typeof(ICollection<FavoritesList>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddFavoritesListEntryPerID(Guid userId, int favoritesListId, [FromBody] FavoritesListDto favoritesListDto)
        {
            var user = await _userService.AddFavoritesListEntryPerID(userId, favoritesListId, favoritesListDto);
            return Ok(user);
        }

        /// <summary>
        /// Returns all the PIDURIs for users' favorites lists
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <returns>List of PID URIs</returns>
        /// <response code="200">List of PID URIs</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet("favoritesListPIDUris/{userId}")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFavoritesListPIDUris(Guid userId)
        {
            var favoritesListPIDUris = await _userService.GetFavoritesListPIDUris(userId);
            return Ok(favoritesListPIDUris);
        }

        /// <summary>
        /// Provides the details of the resources which are part of Users' Favorites List
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="favoritesListId"> the favoritesListId for which resource details need to be fetched</param>
        /// <returns>A dictionary containing PIDURI and its details as part of Elastic Json Response</returns>
        /// <response code="200">A dictionary containing PIDURI and its details as part of Elastic Json Response</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [ProducesResponseType(typeof(Dictionary<string, JObject>), 200)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("favoritesListDetails/{userId}/{favoritesListId}")]
        public async Task<IActionResult> GetFavoritesListDetails(Guid userId, int favoritesListId)
        {
            var favoritesListDetails = await _userService.GetFavoritesListDetails(userId, favoritesListId);
            return Ok(favoritesListDetails);
        }

        /// <summary>
        /// Provides the IDs of all the favorites lists of a resource.
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="pidUri"> the PID URI of the resource</param>
        /// <returns>A list containing IDs of favorite lists</returns>
        /// <response code="200">A list containing IDs of favorite lists which resource is part of.</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [ProducesResponseType(typeof(List<int>), 200)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("favoritesList/resourceFavoritesLists/{userId}/{pidUri}")]
        public async Task<IActionResult> GetResourceFavoritesList(Guid userId, string pidUri)
        {
            var getResourceFavoritesList = await _userService.GetResourceFavoritesList(userId, pidUri);
            return Ok(getResourceFavoritesList);
        }

        /// <summary>
        /// Updates the User Favorite List (eg. Name)
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="favoritesListId"> the favoritesListId to be changed.</param>
        /// <param name="favoritesListDto">Favorite List Name to be changed.</param>
        /// <remarks>
        ///   <br><b>Note:</b>Only the List Name in the DTO is expected. Personal Note and PIDURI can be ignored</br>
        /// </remarks>
        /// <returns>The updated Favorite List Details</returns>
        /// <response code="200">The updated Favorite List Deatils</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [ProducesResponseType(typeof(FavoritesList), 200)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPut("favoritesList/{userId}/{favoritesListId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> SetFavoritesListName(Guid userId, int favoritesListId, [FromBody] FavoritesListDto favoritesListDto)
        {
            var updatedEntity = await _userService.SetFavoritesListName(userId, favoritesListId, favoritesListDto);
            return Ok(updatedEntity);
        }

        /// <summary>
        /// Updates the User Favorite List Entry (eg. Personal Note)
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="favoritesListEntryId"> the favoritesListEntryId to be changed.</param>
        /// <param name="favoritesListDto">Favorite List Entry Note to be changed.</param>
        /// <remarks>
        ///   <br><b>Note:</b>Only the List Entry PersonalNote in the DTO is expected. List Name and PIDURI can be ignored</br>
        /// </remarks>
        /// <returns>The updated Favorite List Entry Details</returns>
        /// <response code="200">The updated Favorite Entry List Deatils</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [ProducesResponseType(typeof(FavoritesList), 200)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPut("favoritesListEntry/{userId}/{favoritesListEntryId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> SetFavoritesListEntryNote(Guid userId, int favoritesListEntryId, [FromBody] FavoritesListDto favoritesListDto)
        {
            var updatedEntity = await _userService.SetFavoritesListEntryNote(userId, favoritesListEntryId, favoritesListDto);
            return Ok(updatedEntity);
        }

        /// <summary>
        /// Deletes entire favorite list and all its corresponding entries.
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="favoritesListId"> the favorite list ID to be removed.</param>
        /// <returns>Returns a user favorites lists according to the result</returns>
        /// <response code="200">Returns a user favorites lists according to the result</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpDelete("{userId}/favoritesList/{favoritesListId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFavoritesList(Guid userId, int favoritesListId)
        {
            var user = await _userService.RemoveFavoritesListAsync(userId, favoritesListId);
            return Ok(user);
        }

        /// <summary>
        /// Deletes a favorite list entry from a favorite list
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="favoritesListEntryId"> the favorite list entry ID to be removed.</param>
        /// <returns>Returns a remaining entries according to the result</returns>
        /// <response code="200">Returns remaining entries according to the result</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpDelete("{userId}/favoritesListEntries/{favoritesListEntryId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFavoritesListEntry(Guid userId, int favoritesListEntryId)
        {
            var favoritesList = await _userService.RemoveFavoritesEntryAsync(userId, favoritesListEntryId);
            return Ok(favoritesList);
        }

        /// <summary>
        /// Delete multiple entries from favorites lists.
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="favoritesListEntriesId">the list ids entries to be removed </param>
        /// <returns>A list containing an overview of FavoriteLists Names and its entries</returns>
        /// <response code="200">A list containing an overview of FavoriteLists Names and its entries</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPost("removeFavoritesListEntries/{userId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> RemoveFavoritesListEntries(Guid userId, [FromBody] IList<int> favoritesListEntriesId)
        {
            var user = await _userService.RemoveFavoritesListEntriesAsync(userId, favoritesListEntriesId);
            return Ok(user);
        }

        #endregion [Favorites List]
    }
}
