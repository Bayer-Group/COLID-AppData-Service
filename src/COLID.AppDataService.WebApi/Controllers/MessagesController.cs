using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using COLID.AppDataService.Common.Constants;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Services.Interfaces;
using Common.DataModels.TransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IMessageService messageService, ILogger<MessagesController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        /// <summary>
        /// Get all messages, that got a sendDate before now.
        /// </summary>
        [Authorize(Roles = ApplicationRoles.Scheduler.FullAccess)]
        [HttpGet("toSend")]
        public IActionResult GetAllMessagesReadyToSend()
        {
            var unreadMessages = _messageService.GetUnreadMessagesToSend();
            return Ok(unreadMessages);
        }

        /// <summary>
        /// Delete all expired messages that has been read or send yet.
        /// </summary>
        [Authorize(Roles = ApplicationRoles.Scheduler.FullAccess)]
        [HttpDelete()]
        public IActionResult DeleteAllMessagesReadyToDelete()
        {
            _messageService.DeleteExpiredMessages();
            return NoContent();
        }

        /// <summary>
        /// Triggers the message broadcast function to send the message to all users.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        [Authorize(Roles = ApplicationRoles.Administration)]
        [HttpPost("broadcastMessage")]
        [Consumes(MediaTypeNames.Application.Json)]
        public IActionResult SendBroadcastMessage(BroadcastMessageDto message)
        {
            _messageService.SendMessageToAllUsers(message);
            return Ok(message);
        }

        /// <summary>
        /// Send message to the author of the resource to norify about distribution endpoint not working
        /// </summary>
        /// <param name="message">Message to be sent</param>
        [HttpPost("notifyUserAboutInvalidDistributionEndpoint")]
        public IActionResult NotifyUserAboutInvalidDistributionEndpoint([FromBody] DistributionEndpointMessageDto message)
        {
            _messageService.SendMessageToUser(message);
            return Ok(message);
        }

        [HttpPost]
        [Route("notifyUserAboutInvalidContacts")]
        public async Task<IActionResult> CreateMessagesOfInvalidContacts([FromBody] ColidEntryContactInvalidUsersDto cec)
        {
            await _messageService.SendInvalidContactsMessageToUser(cec);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpDelete("deleteByAdditionalInfo")]
        public IActionResult DeleteByAdditionalInfo([FromBody] IList<Uri> distributionEndpoints)
        {
            _messageService.DeleteByAdditionalInfo(distributionEndpoints);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distributionEndpoints"></param>
        /// <returns></returns>
        [HttpPost("getByAdditionalInfo")]
        public IActionResult GetByAdditionalInfo([FromBody] IList<Uri> distributionEndpoints)
        {
            var result = _messageService.GetByAdditionalInfo(distributionEndpoints);
            return Ok(result);
        }

        /// <summary>
        /// Send generic message to user
        /// </summary>
        /// <param name="message">Message to be sent</param>
        [HttpPut("sendGenericMessage")]
        [Consumes(MediaTypeNames.Application.Json)]
        public IActionResult SendGenericMessage(MessageUserDto message)
        {
            _messageService.SendGenericMessageToUser(message);
            return Ok(message);
        }
    }
}
