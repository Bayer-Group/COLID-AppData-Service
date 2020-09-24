using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using COLID.AppDataService.Common.Constants;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Services.Interface;
using Common.DataModels.TransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    public partial class ColidEntriesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        private readonly IColidEntrySubscriptionService _colidEntrySubscriptionService;

        private readonly ILogger<ColidEntriesController> _logger;

        public ColidEntriesController(IMessageService messageService, IColidEntrySubscriptionService colidEntrySubscriptionService, ILogger<ColidEntriesController> logger)
        {
            _messageService = messageService;
            _colidEntrySubscriptionService = colidEntrySubscriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Triggers the update message creation for subscribed users to the given colid pid uri.
        /// </summary>
        /// <param name="colidEntryDto">dto with the updated pid uri</param>
        [HttpPut]
        [Authorize(Roles = ApplicationRoles.Resource.Notifications)]
        public async Task<IActionResult> NotifyUpdatedColidEntryToSubscribers([FromBody] ColidEntryDto colidEntryDto)
        {
            int amountOfCreatedMessages = await _messageService.CreateUpdateMessagesForColidEntrySubscriptions(colidEntryDto);
            var jsonString = "{ \"messagesCreated\": " + amountOfCreatedMessages + " }";
            return Ok(jsonString);
        }

        /// <summary>
        /// Triggers the delete message creation for subscribed users to the given colid pid uri and deletes the subscription afterwards.
        /// </summary>
        /// <param name="colidEntryDto">Uri to the deleted colid entry</param>
        [HttpDelete]
        [Authorize(Roles = ApplicationRoles.Resource.Notifications)]
        public async Task<IActionResult> NotifyDeletedColidEntryToSubscribers([FromBody] ColidEntryDto colidEntryDto)
        {
            int amountOfCreatedMessages = await _messageService.CreateDeleteMessagesAndRemoveColidEntrySubscriptions(colidEntryDto);
            var jsonString = "{ \"messagesCreated\": " + amountOfCreatedMessages + " }";
            return Ok(jsonString);
        }

        /// <summary>
        /// Triggers the message creation for invalid users to the given dto.
        /// </summary>
        /// <param name="cec">the ColidEntryContactInvalidUsersDto to consider</param>
        [HttpPut("invalidUser")]
        [Authorize(Roles = ApplicationRoles.Scheduler.FullAccess)]
        public async Task<IActionResult> CreateMessagesOfInvalidUsersForContact([FromBody] ColidEntryContactInvalidUsersDto cec)
        {
            await _messageService.CreateMessagesOfInvalidUsersForContact(cec);
            return Created("/api/ColidEntries/invalidUser","created");
        }


        /// <summary>
        /// Calculate the amount of subscriptions per colid pid uri and return the result.
        /// </summary>
        [HttpGet]
        public IActionResult GetColidPidUrisAndAmountSubscriptions()
        {
            var colidEntrySubscriptionAmountDto = _colidEntrySubscriptionService.GetColidPidUrisAndAmountSubscriptions();
            return Ok(colidEntrySubscriptionAmountDto);
        }
    }
}
