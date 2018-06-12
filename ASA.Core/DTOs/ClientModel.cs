using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core.DTOs
{
    public class ClientModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegNo { get; set; }
        public string VATNo { get; set; }
        public ClientAddressModel ClientAddressModel { get; set; }
    }

    public class ClientAddressModel
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
    }
}
