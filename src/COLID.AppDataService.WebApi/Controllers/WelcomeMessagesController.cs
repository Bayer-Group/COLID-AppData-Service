using System;
using COLID.AppDataService.Common.Constants;
using COLID.AppDataService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace COLID.AppDataService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WelcomeMessagesController : ControllerBase
    {
        private readonly IWelcomeMessageService _wmService;

        public WelcomeMessagesController(IWelcomeMessageService welcomeMessageService)
        {
            _wmService = welcomeMessageService;
        }

        [HttpGet("editor")]
        public IActionResult GetWelcomeMessageForEditor()
        {
            var message = _wmService.GetWelcomeMessageEditor();
            return Ok(message);
        }

        [HttpGet("datamarketplace")]
        public IActionResult GetWelcomeMessageForDatamarketplace()
        {
            var message = _wmService.GetWelcomeMessageDataMarketplace();
            return Ok(message);
        }

        [HttpPut("editor")]
        [Authorize(Roles = ApplicationRoles.Administration)]
        public IActionResult UpdateWelcomeMessageForEditor([FromBody] string content)
        {
            var message = _wmService.UpdateWelcomeMessageEditor(content);
            return Ok(message);
        }

        [HttpPut("datamarketplace")]
        [Authorize(Roles = ApplicationRoles.Administration)]
        public IActionResult UpdateWelcomeMessageForDataMarketplace([FromBody] string content)
        {
            var message = _wmService.UpdateWelcomeMessageDataMarketplace(content);
            return Ok(message);
        }
    }
}
