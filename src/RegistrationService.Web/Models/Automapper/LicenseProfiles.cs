using AutoMapper;
using RegistrationService.Core.Entities;
using RegistrationService.Core.Messages;
using RegistrationService.Web.Models.Request;
using RegistrationService.Web.Models.Response;

namespace RegistrationService.Models.Automapper
{
	public class LicenseProfiles : Profile
	{
		public LicenseProfiles()
		{
			CreateMap<LicenseRequestModel, LicenseDataModel>();
			CreateMap<LicenseDataModel, LicenseResponseModel>().ReverseMap();
			CreateMap<LicenseDataModel, LicenseMessage>();
		}
	}

}
