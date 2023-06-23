using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using COLID.AppDataService.Services.Graph.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    public class ActiveDirectoryController : ControllerBase
    {
        private readonly IActiveDirectoryService _activeDirectoryService;

        private readonly ILogger<ActiveDirectoryController> _logger;

        public ActiveDirectoryController(IActiveDirectoryService activeDirectoryService, ILogger<ActiveDirectoryController> logger)
        {
            _activeDirectoryService = activeDirectoryService;
            _logger = logger;
        }

        /// <summary>
        /// Check if users are valid (account enabled) or not, determined by the given userEmail param.
        /// </summary>
        /// <param name="userEmails">list of user email adresses</param>
        [HttpPost("users/status")]
        public async Task<IActionResult> CheckUsersValidityAsync([FromBody] ISet<string> userEmails)
        {
            var userList = await _activeDirectoryService.CheckUsersValidityAsync(userEmails);
            return Ok(userList);
        }

        [HttpGet("users")]
        public async Task<IActionResult> FindUsersAsync([FromQuery(Name = "q")] string query)
        {
            var result = await _activeDirectoryService.FindUsersAsync(query);
            return Ok(result);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserAsync([FromRoute] string id)
        {
            var result = await _activeDirectoryService.GetUserAsync(id);
            return Ok(result);
        }

        [HttpGet("users/{id}/manager")]
        public async Task<IActionResult> GetManagerByUserIdAsync([FromRoute] string id)
        {
            var result = await _activeDirectoryService.GetManagerByUserIdAsync(id);
            return Ok(result);
        }

        [HttpGet("groups")]
        public async Task<IActionResult> FindGroupsAsync([FromQuery(Name = "q")] string query)
        {
            var result = await _activeDirectoryService.FindGroupsAsync(query);
            return Ok(result);
        }

        [HttpGet("groups/{id}")]
        public async Task<IActionResult> GetGroupAsync([FromRoute] string id)
        {
            var result = await _activeDirectoryService.GetGroupAsync(id);
            return Ok(result);
        }

        [HttpGet("usersAndGroups")]
        public IActionResult FindUsersAndGroupsAsync([FromQuery(Name = "q")] string query)
        {
            var result = _activeDirectoryService.FindUsersAndGroupsAsync(query);
            return Ok(result);
        }

        [HttpGet("usersAndGroups/{id}")]
        public IActionResult GetUserOrGroupAsync([FromRoute] string id)
        {
            var result = _activeDirectoryService.GetUserOrGroupAsync(id);
            return Ok(result);
        }
    }
}
