using System;
using System.Collections.Generic;
using System.Text;

namespace RegistrationService.Core.Settings
{
    public class LicenseSignatureWebSocketSettings
    {
        public List<string> AllowedOrigins { get; set; }
        public string Path { get; set; }
    }
}
