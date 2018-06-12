using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        public string Name { get; set; }
        [StringLength(500)]
        [Index("UQ_RegNo", IsUnique = true)]
        public string RegNo { get; set; }
        public string VATNo { get; set; }
        public ClientAddress ClientAddress { get; set; }
        public Boolean IsDeleted { get; set; } = false;

        [ForeignKey("Business")]
        public int? BusinessId { get; set; }
        public Business Business { get; set; }

        //[ForeignKey("Invoice")]
        //public Guid InvoiceId { get; set; }
        //public virtual Invoice Invoice { get; set; }
        //public Business Business { get; set; }
        //[ForeignKey("Business")]
        //public int BusinessId { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
        //public Client()
        //{
        //    ClientAddresses = new List<ClientAddress>();
        //}

    }    
}
