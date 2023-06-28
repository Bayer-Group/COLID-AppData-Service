using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    public class ApplicationVersionController : ControllerBase
    {
        private readonly IApplicationVersionService _service;
        private readonly ILogger<ApplicationVersionController> _logger;

        public ApplicationVersionController(IApplicationVersionService applicationVersionService, ILogger<ApplicationVersionController> logger)
        {
            _service = applicationVersionService;
            _logger = logger;
        }
        [HttpGet("versions")]
        public ApplicationVersionDto GetApplicationVersions([FromQuery] string application, [FromQuery] int historyLength = 3)
        {
            var result = _service.GetLastVersions(application, historyLength);

            return result;
        }

        [HttpGet("version")]
        public ApplicationVersionDto GetApplicationVersion([FromQuery] string application)
        {
            var result = _service.GetLastVersion(application);

            return result;
        }
    }
}
