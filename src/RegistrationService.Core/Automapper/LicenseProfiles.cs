using AutoMapper;
using RegistrationService.Core.Entities;

namespace RegistrationService.Models.Automapper
{
	public class LicenseProfiles : Profile
	{
		public LicenseProfiles()
		{
			CreateMap<LicenseDataModel, LicenseMessage>().ReverseMap();
		}
	}

}
