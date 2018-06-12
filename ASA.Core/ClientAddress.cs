using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core
{
    public class ClientAddress
    {
        
        [Key, ForeignKey("Client")]
        public int ClientAddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public virtual Client Client { get; set; } //one to one relationship 
        
    }
}
