using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASA.API.Models
{
    public class SignupViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }

        
    }
}