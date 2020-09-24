using System;
using System.Collections.Generic;
using System.Net.Mime;
using COLID.AppDataService.Common.Constants;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Controllers
{
    [Authorize(Roles = ApplicationRoles.Scheduler.FullAccess)]
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    public partial class SchedulerController : ControllerBase
    {
        private readonly IGenericService<StoredQuery, int> _storedQueryService;
        private readonly IGenericService<ColidEntrySubscription, int> _colidEntrySubscriptionService;
        private readonly IMessageService _messageService;
        private readonly ILogger<SchedulerController> _logger;

        public SchedulerController(IGenericService<StoredQuery, int> storedQueryService,
            IGenericService<ColidEntrySubscription, int> colidEntrySubscriptionService,
            IMessageService messageService,
            ILogger<SchedulerController> logger)
        {
            _storedQueryService = storedQueryService;
            _colidEntrySubscriptionService = colidEntrySubscriptionService;
            _messageService = messageService;
            _logger = logger;
        }

        // TODO?: separate in different Controllers
        [HttpGet("storedQueries")] public IActionResult GetAllStoredQueriesReadyForExectution() { throw new NotImplementedException(); }

        [HttpGet("subscriptions")]
        public IActionResult GetAllSubscribedColidEntriesReadyToExecute()
        {
            throw new NotImplementedException();
        }

        [HttpPut("subscriptions")]
        public IActionResult UpdateExecutionDateForColidEntries([FromBody] IList<ColidEntrySubscriptionDto> entryList)
        {
            // on execution: Set the next exectuion date, based on the 'execution interval' value
            throw new NotImplementedException();
        }

        [HttpGet("messages/toSend")]
        public IActionResult GetAllMessagesReadyToSend()
        {
            var unreadMessages = _messageService.GetUnreadMessagesToSend();
            return Ok(unreadMessages);
        }

        [HttpDelete("messages")]
        public IActionResult DeleteAllMessagesReadyToDelete()
        {
            throw new NotImplementedException();
        }
    }
}
