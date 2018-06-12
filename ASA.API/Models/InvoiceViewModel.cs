using ASA.API.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASA.API.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<InvoiceViewModel> Invoices { get; set; }
        public IEnumerable<PeriodViewModel> Periods { get; set; }

    }

    public class InvoiceViewModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public int ClientId { get; set; }
        [Required]
        public InvoiceDetailModel Details { get; set; }
        [Required]
        public IList<InvoiceItemModal> Items { get; set; }
        [Required]
        public decimal SubTotal { get; set; }
        [Required]
        public decimal VAT { get; set; }
        [Required]
        public decimal Total { get; set; }
        [Required]
        public int BusinessId { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
       // [Required]
        public DateTime? LastUpdated { get; set; }

        //public List<PeriodViewModel> PeriodViewModel { get; set; }

    }
    public class InvoiceDetailModel
    {
       
        public int InvoiceDetailId { get; set; }
        [Required]
        public string No { get; set; }
        [Required]
        public DateTime IssueDate { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public string Ref { get; set; }
        public string OrderNo { get; set; }
        public string Discount { get; set; }
        public string Note { get; set; }
    }
    public class InvoiceItemModal
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Quantity { get; set; }
        [Required]
        public string Price { get; set; }
        [Required]
        public string VATRate { get; set; }
        public string VAT { get; set; }
        public string Total { get; set; }
        public string SubTotal { get; set; }
    }
}