using ASA.API.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASA.API.Models
{
    public class ClientViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public AddressViewModel Address { get; set; }
        [Required]
        public string RegNo { get; set; }
        [Required]
        public string VATNo { get; set; }

        [Required]
        public int? BusinessId { get; set; }
    }
}