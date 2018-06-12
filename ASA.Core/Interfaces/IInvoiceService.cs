using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core.Interfaces
{
    public interface IInvoiceService
    {
        string Save(Invoice invoice);
        bool Update(Invoice invoice);
        //bool Delete(Invoice invoice);
        IQueryable<Invoice> Invoices();
    }
}
