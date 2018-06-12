using ASA.Core.Infrastructure;
using ASA.Core.Interfaces;
using ASA.Core.Repositories;
using System.Linq;
using System.Data.Entity;


namespace ASA.Core.Services
{
    public class InvoiceService: IInvoiceService 
    {
        public readonly IInvoiceRepository _invoiceRepository;
        public readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork)
        {
            this._invoiceRepository = invoiceRepository;
            this._unitOfWork = unitOfWork;
        }
        public string Save(Invoice invoice)
        {
            _invoiceRepository.Add(invoice);
            _unitOfWork.Commit();
            return _invoiceRepository.Query().Include(invd => invd.InvoiceDetail).Where(i => i.InvoiceId == invoice.InvoiceId).SingleOrDefault().InvoiceDetailId.ToString();            
        }
        public IQueryable<Invoice> Invoices()
        {
            var invoices = _invoiceRepository.Query().Include(invd => invd.InvoiceDetail).Include(it => it.InvoiceItems);

            return invoices;
        }
        public bool Update(Invoice invoice)
        {
            try
            {
                _invoiceRepository.Update(invoice);
                //_unitOfWork.Commit(); //context disposed here not sure why so commented out for now todo:// need to investigate why
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

    }
}
