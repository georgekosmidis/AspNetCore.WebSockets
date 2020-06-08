using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegistrationService.Core.Entities;
using RegistrationService.SharedKernel.Interfaces;
using RegistrationService.Web.Models.Request;
using RegistrationService.Web.Models.Response;

namespace RegistrationService.Web.Controllers
{
    [ApiController]
    [Route("api/Licensing")]
    public class LicensingController : ControllerBase
    {
        private readonly ILogger<LicensingController> _logger;
        private readonly IQueueService<LicenseMessage> _queueService;
        private readonly IStorageService<LicenseDataModel> _storageService;
        private readonly IMapper _mapper;

        public LicensingController(ILogger<LicensingController> logger, IQueueService<LicenseMessage> queueService, IStorageService<LicenseDataModel> storageService, IMapper mapper)
        {
            _logger = logger;
            _queueService = queueService;
            _storageService = storageService;
            _mapper = mapper;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LicenseResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SendLicenseRequest([FromBody] LicenseRequestModel licenseRequestModel)
        {
            var licenseDataModel = _mapper.Map<LicenseDataModel>(licenseRequestModel);

            //queque for the websocket to pickup
            _queueService.Enqueue(
                _mapper.Map<LicenseMessage>(licenseDataModel)
            );

            //store for the consumer to get updates
            _storageService.AddOrUpdate(licenseDataModel);

            //201 response (REST)
            var licenseResponse = _mapper.Map<LicenseResponseModel>(licenseDataModel);
            return CreatedAtAction(nameof(GetLicenseResponse), new { id = licenseResponse.Id }, licenseResponse);
        }


        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LicenseResponseModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult GetLicenseResponse(Guid id)
        {
            //search for a license request
            var licenseDataModel = _storageService.Get(id);

            //404 response if not found (REST)
            if (licenseDataModel == default)
                return NotFound();

            //200 if found
            return Ok(_mapper.Map<LicenseResponseModel>(licenseDataModel));
        }
    }
}
