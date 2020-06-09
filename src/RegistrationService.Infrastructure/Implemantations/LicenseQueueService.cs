using RegistrationService.Core.Interfaces;
using RegistrationService.Core.Messages;
using RegistrationService.Infrastructure.Abstractions;

namespace RegistrationService.Infrastructure.Implemantations
{
    public class LicenseQueueService : DemoQueueService<LicenseMessage>, ILicenseQueueService
    {
    }
}
