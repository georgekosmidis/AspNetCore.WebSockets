using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationService.Web.Models.Request
{
    //model properties and validations are just assumptions
    public class LicenseRequestModel
    {
        public string CompanyName { get; set; }

        [Required, MinLength(1), MaxLength(40)]
        public string ContactPerson { get; set; }

        [Required,
         MaxLength(254),
        RegularExpression(@"^([\w\.\-\+!#$%&'*+-/=?^_`{|}~]+)@([\w\-\._]+)((\.(\w){2,25})+)$")]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string LicenseKey { get; set; }
    }
}
