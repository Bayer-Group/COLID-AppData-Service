using System.Net.Mime;
using System.Threading.Tasks;
using COLID.AppDataService.Common.Constants;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    public partial class ConsumerGroupsController : ControllerBase
    {
        private readonly IConsumerGroupService _consumerGroupService;

        private readonly ILogger<ConsumerGroupsController> _logger;

        public ConsumerGroupsController(IConsumerGroupService consumerGroupService, ILogger<ConsumerGroupsController> logger)
        {
            _consumerGroupService = consumerGroupService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = ApplicationRoles.Administration)]
        public IActionResult GetAll()
        {
            return Ok(_consumerGroupService.GetAll());
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.ConsumerGroup.FullAccess)]
        public async Task<IActionResult> Create([FromBody] ConsumerGroupDto dto)
        {
            var cgEntity = await _consumerGroupService.CreateAsync(dto);
            return Created($"api/consumergroups", cgEntity);
        }

        [HttpDelete]
        [Authorize(Roles = ApplicationRoles.ConsumerGroup.FullAccess)]
        public IActionResult Delete([FromBody] ConsumerGroupDto dto)
        {
            _consumerGroupService.Delete(dto);
            return Ok($"Consumer Group with URI {dto.Uri} has been deleted");
        }
    }
}
