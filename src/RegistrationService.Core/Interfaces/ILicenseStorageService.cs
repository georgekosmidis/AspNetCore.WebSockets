using RegistrationService.Core.Entities;
using System;

namespace RegistrationService.Core.Interfaces
{
    public interface ILicenseStorageService : IStorageService<LicenseDataModel>
    {
        LicenseDataModel UpdateMessageReceived(Guid id, string signedLicense);
    }
}
