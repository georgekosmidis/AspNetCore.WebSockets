using RegistrationService.Core.Entities;
using RegistrationService.Core.Interfaces;
using RegistrationService.Core.Messages;
using RegistrationService.Infrastructure.Abstractions;
using System;

namespace RegistrationService.Infrastructure.Implementations
{
    public class LicenseStorageService : DemoStorageService<LicenseDataModel>, ILicenseStorageService
    {
        public LicenseDataModel UpdateMessageReceived(Guid id, string signedLicense)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (signedLicense == null)
            {
                throw new ArgumentNullException(nameof(signedLicense));
            }

            var license = base.Get(id);
            license.Status =  MessageStatus.Complete;
            license.SignedLicenseKey = signedLicense;
            base.AddOrUpdate(license);
            
            return license;
        }
    }
}
