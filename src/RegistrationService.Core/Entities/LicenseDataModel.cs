using RegistrationService.Core.Interfaces;
using RegistrationService.Core.Messages;
using System;

namespace RegistrationService.Core.Entities
{
    public class LicenseDataModel : IDataModel
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public MessageStatus Status { get; set; } = MessageStatus.New;

        public string CompanyName { get; set; }

        public string ContactPerson { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string LicenseKey { get; set; }

        public string SignedLicenseKey { get; set; }

    }
}
