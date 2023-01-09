using System;
using System.Net.Mime;
using COLID.AppDataService.Common.Constants;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    public class MessageTemplatesController : ControllerBase
    {
        private readonly IMessageTemplateService _mtService;

        private readonly ILogger<MessageTemplatesController> _logger;

        public MessageTemplatesController(ILogger<MessageTemplatesController> logger,
            IMessageTemplateService messageTemplateService)
        {
            _logger = logger;
            _mtService = messageTemplateService;
        }

        [HttpGet]
        public IActionResult GetAllMessageTemplates()
        {
            return Ok(_mtService.GetAll());
        }

        [HttpGet("{messageType}")]
        public IActionResult GetMessageTemplate(MessageType messageType)
        {
            var message = _mtService.GetOne(messageType);
            return Ok(message);
        }

        [HttpPost]
        public IActionResult CreateMessageTemplate([FromBody] MessageTemplateDto messageTemplateDto)
        {
            var tplEntity = _mtService.Create(messageTemplateDto);
            return Created($"api/messageTemplates", tplEntity);
        }

        [HttpPut]
        public IActionResult UpdateMessageTemplate([FromBody] MessageTemplateDto messageTemplateDto)
        {
            var message = _mtService.Update(messageTemplateDto);
            return Ok(message);
        }
    }
}
