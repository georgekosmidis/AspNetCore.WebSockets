using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegistrationService.Web.Models.Request;
using RegistrationService.Web.Models.Response;

namespace RegistrationService.Web.Controllers
{
    [ApiController]
    [Route("api/Licensing")]    
    public class LicensingController : ControllerBase
    {
        private readonly ILogger<LicensingController> _logger;
        private readonly IMapper _mapper;

        public LicensingController(ILogger<LicensingController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }


        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LicenseResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SendLicenseRequest([FromBody] LicenseRequestModel licenseRequestModel)
        {
            //PLAN:
            //1. queque for the websocket to pickup
            //2. store for the consumer to get updates
            //3. respond 201 when all done

            //basic REST
            return CreatedAtAction(nameof(GetLicenseResponse), new { id = Guid.Empty }, new LicenseResponseModel());

        }

        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LicenseResponseModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //No Cache since the plan is for long polling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult GetLicenseResponse(Guid id)
        {
            //PLAN:
            //1. search for the license
            //2. return 404 if not found
            //3. respond 200 if found

            //basic rest
            return Ok(new LicenseResponseModel());

        }
    }
}
