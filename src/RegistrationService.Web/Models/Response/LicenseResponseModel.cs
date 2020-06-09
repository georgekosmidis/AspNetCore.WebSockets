using RegistrationService.Core.Messages;
using System;


namespace RegistrationService.Web.Models.Response
{
    public class LicenseResponseModel
    {
        public Guid Id { get; set; }

        public MessageStatus Status { get; set; }

        public string CompanyName { get; set; }

        public string ContactPerson { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string LicenseKey { get; set; }

        public string SignedLicenseKey { get; set; }
    }
}
