using ASA.API.Model;
using ASA.API.Models;
using ASA.Core;
using ASA.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ASA.API.Controllers
{
    public class InvoiceController : ApiController
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPeriodService _periodService; 
        public InvoiceController(IInvoiceService invoiceService, IPeriodService periodService)
        {
            this._invoiceService = invoiceService;
            this._periodService = periodService;
        }
        // GET: api/Invoice
        [HttpGet]
        public HttpResponseMessage Get(int businessId)
        {
            //return new string[] { "value1", "value2" };
            var invoices = _invoiceService.Invoices().Where(i => i.BusinessId == businessId).ToList();
            var periods = _periodService.GetPeriodsByBusinessId(businessId);
            //var denorPeriods = from p in periods
            //                   group p.PeriodrefId by p.BusinessId into g
            //                   select new DenormPeriodView() { Quater = g.Key, Number = g.ToList() };
            if (invoices.Count > 0)
            {
                var invmlist = invoices.Select(i => new InvoiceViewModel()
                {
                    Id = i.InvoiceId,
                    ClientId = i.ClientId,
                    Details = new InvoiceDetailModel() { No = i.InvoiceDetail.No, InvoiceDetailId = i.InvoiceDetailId, Discount = i.InvoiceDetail.Discount, DueDate = i.InvoiceDetail.DueDate, IssueDate = i.InvoiceDetail.IssueDate, Note = i.InvoiceDetail.Note, OrderNo = i.InvoiceDetail.OrderNo, Ref = i.InvoiceDetail.Ref },
                    SubTotal = i.SubTotal,
                    VAT = i.VAT,
                    Total = i.Total,
                    BusinessId = i.BusinessId,
                    Items = i.InvoiceItems.Select(item => new InvoiceItemModal()
                    {
                        Description = item.Description,
                        Id = item.Id,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        SubTotal = item.SubTotal,
                        Total = item.Total,
                        VAT = item.VAT,
                        VATRate = item.VATRate
                    }).ToList()
                });
                if (periods.Count() > 0)
                {
                    var periodList = periods.Select(p => new PeriodViewModel()
                    {
                        PeriodId = p.PeriodId,
                        StartPeriod = p.StartPeriod,
                        EndPeriod = p.EndPeriod,
                        PeriodRefId = p.PeriodrefId,
                        Status = p.Status
                    }).ToList();
                    var dashboardModel = new DashboardViewModel()
                    {
                        Invoices = invmlist,
                        Periods = periodList
                    };
                    return Request.CreateResponse<DashboardViewModel>(HttpStatusCode.OK, dashboardModel);
                }
            }
            return Request.CreateResponse(HttpStatusCode.NoContent, "empty");
        }
    
        // GET: api/Invoice/5
        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int Id = Int16.Parse(id);
                var invoices = _invoiceService.Invoices().Where(i => i.BusinessId == Id).ToList();
                if (invoices.Count>0)
                {
                    var invmlist = invoices.Select(i => new InvoiceViewModel()
                    {
                        Id = i.InvoiceId,
                        ClientId = i.ClientId,
                        Details = new InvoiceDetailModel() { No = i.InvoiceDetail.No, InvoiceDetailId = i.InvoiceDetailId, Discount = i.InvoiceDetail.Discount, DueDate = i.InvoiceDetail.DueDate, IssueDate = i.InvoiceDetail.IssueDate, Note=i.InvoiceDetail.Note, OrderNo=i.InvoiceDetail.OrderNo, Ref=i.InvoiceDetail.Ref },
                        SubTotal = i.SubTotal,
                        VAT = i.VAT,
                        Total = i.Total,
                        BusinessId = i.BusinessId,
                        Items = i.InvoiceItems.Select(item => new InvoiceItemModal()
                        {
                            Description = item.Description,
                            Id = item.Id,
                            Price = item.Price,
                            Quantity = item.Quantity,
                            SubTotal = item.SubTotal,
                            Total = item.Total,
                            VAT = item.VAT,
                            VATRate = item.VATRate
                        }).ToList()
                    });
                    return Request.CreateResponse<IEnumerable<InvoiceViewModel>>(HttpStatusCode.OK, invmlist);
                }
                else
                {                   
                    return Request.CreateResponse(HttpStatusCode.NoContent, "empty");
                }
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        // POST: api/Invoice
        [HttpPost]
        public HttpResponseMessage Post([FromBody]InvoiceViewModel invoiceModal)
        {
            if (ModelState.IsValid)
            {
                var inv = new Invoice()
                {
                    InvoiceId = invoiceModal.Id,
                    InvoiceDetail = new InvoiceDetail() { No = invoiceModal.Details.No, DueDate = invoiceModal.Details.DueDate, IssueDate = invoiceModal.Details.IssueDate, Discount = invoiceModal.Details.Discount, Note = invoiceModal.Details.Note, OrderNo = invoiceModal.Details.OrderNo, Ref = invoiceModal.Details.Ref },
                    ClientId = invoiceModal.ClientId,
                    SubTotal = invoiceModal.SubTotal,
                    VAT = invoiceModal.VAT,
                    Total = invoiceModal.Total,
                    BusinessId = invoiceModal.BusinessId,
                    DateCreated = invoiceModal.DateCreated,
                    InvoiceItems = invoiceModal.Items.Select(i => new InvoiceItem()
                    {
                        Description = i.Description, Price = i.Price, Quantity = i.Quantity, SubTotal = i.SubTotal, Total = i.Total, VAT = i.VAT, VATRate = i.VATRate, Id=i.Id
                    }).ToList()
                };
                string invOrderId = _invoiceService.Save(inv);
                if (!(string.IsNullOrEmpty(invOrderId)))
                {
                    var msg = Request.CreateResponse(HttpStatusCode.Created, invOrderId);
                    msg.Headers.Location = new Uri(Request.RequestUri +"/"+ invOrderId);
                    return msg;
                }
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = "Model validation failed"
                });
            }
        }

        // PUT: api/Invoice/5
        [HttpPut]
        public HttpResponseMessage Put(string id, [FromBody]InvoiceViewModel invoiceModal)
        {
            if (ModelState.IsValid)
            {
                var inv = new Invoice()
                {
                    InvoiceId = invoiceModal.Id,
                    InvoiceDetailId = invoiceModal.Details.InvoiceDetailId,
                    SubTotal = invoiceModal.SubTotal,
                    VAT = invoiceModal.VAT,
                    Total = invoiceModal.Total,
                    BusinessId = invoiceModal.BusinessId,
                    InvoiceDetail = new InvoiceDetail()
                                    {
                                        No = invoiceModal.Details.No,
                                        DueDate = invoiceModal.Details.DueDate,
                                        IssueDate = invoiceModal.Details.IssueDate,
                                        Discount = invoiceModal.Details.Discount,
                                        Note = invoiceModal.Details.Note,
                                        OrderNo = invoiceModal.Details.OrderNo,
                                        Ref = invoiceModal.Details.Ref,
                                        InvoiceDetailId = invoiceModal.Details.InvoiceDetailId
                                    },
                    ClientId = invoiceModal.ClientId,
                    InvoiceItems = invoiceModal.Items.Select(i => new InvoiceItem()
                    {
                        Description = i.Description,
                        Price = i.Price,
                        Quantity = i.Quantity,
                        SubTotal = i.SubTotal,
                        Total = i.Total,
                        VAT = i.VAT,
                        VATRate = i.VATRate,
                        Id = i.Id
                    }).ToList()
                };
                var status = _invoiceService.Update(inv);
                if (status) return new HttpResponseMessage(HttpStatusCode.OK);
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // DELETE: api/Invoice/5
        public void Delete(int id)
        {
        }
    }
}
