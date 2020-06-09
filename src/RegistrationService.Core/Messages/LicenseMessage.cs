using System;

namespace RegistrationService.Core.Messages
{
    public class LicenseMessage
    {
        public Guid Id { get; set; }

        public string LicenseKey { get; set; }

        public string SignedLicenseKey { get; set; }

    }
}
