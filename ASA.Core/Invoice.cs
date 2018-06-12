using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core
{
    public class Invoice
    {
        [Key]
        public Guid InvoiceId { get; set; }
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client Client { get; set; } //one to many relation

        [ForeignKey("InvoiceDetail")]
        public int InvoiceDetailId { get; set; }
        public InvoiceDetail InvoiceDetail { get; set; }
        //public InvoiceItem InvoiceItem { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
     
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [System.ComponentModel.DefaultValue("getutcdate()")]
        public DateTime ? DateCreated { get; set; }
        public DateTime ? LastUpdated { get; set; }
        public decimal SubTotal { get; set; }
        public decimal VAT { get; set; }
        public decimal Total { get; set; }
        [ForeignKey("Business")]
        public int BusinessId { get; set; }
        public Business Business { get; set; }
        public Invoice()
        {
            this.InvoiceItems = new List<InvoiceItem>();
        }
    }
    public class InvoiceDetail
    {
        [Key]
        public int InvoiceDetailId { get; set; }
        [MaxLength(20)]
        [Index("UQ_InvoiceDetail_No", IsUnique =true)]
        public string No { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Ref { get; set; }
        public string OrderNo { get; set; }
        public string Discount { get; set; }
        public string Note { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [System.ComponentModel.DefaultValue("getutcdate()")]
        public DateTime? DateCreated { get; set; }
    }
    public class InvoiceItem
    {
        [Key]
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string VATRate { get; set; }
        public string VAT { get; set; }
        public string Total { get; set; }
        public string SubTotal { get; set; }
     

    }

}
