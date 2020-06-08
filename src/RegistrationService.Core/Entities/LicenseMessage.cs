using RegistrationService.SharedKernel.Messages;
using System;

namespace RegistrationService.Core.Entities
{
    public class LicenseMessage
    {
        public Guid Id { get; set; }

        public MessageStatus Status { get; set; } = MessageStatus.New;

        public string CompanyName { get; set; }

        public string ContactPerson { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string LicenseKey { get; set; }

        public string SignedLicenseKey { get; set; }

    }
}
