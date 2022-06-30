using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using COLID.AppDataService.Common.Constants;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

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
        [Authorize(Roles = ApplicationRoles.Administration)]
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
 
    }
}
