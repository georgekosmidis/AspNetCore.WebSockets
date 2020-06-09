using RegistrationService.Core.Entities;
using RegistrationService.Core.Messages;
using System;

namespace RegistrationService.Core.Interfaces
{
    public interface ILicenseQueueService : IQueueService<LicenseMessage>
    {
    }
}
