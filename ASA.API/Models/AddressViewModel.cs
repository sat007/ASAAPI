using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.API.Model
{
    public class AddressViewModel
    {
        [Required]
        public string Line1;
        [Required]
        public string Line2;
        //[Required]
        //public string Line3;
        //public string Line4;
        [Required]
        public string City { get; set; }
        [Required]
        public string PostCode;
        [Required]
        public string Country;

    }
}
