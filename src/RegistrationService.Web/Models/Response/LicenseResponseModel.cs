using RegistrationService.SharedKernel.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
