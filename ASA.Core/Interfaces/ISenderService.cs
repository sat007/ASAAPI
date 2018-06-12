using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core.Interfaces
{
    public interface ISenderService
    {
        bool Save(Sender sender);
        bool Update(Sender sender);
        IQueryable<Sender> GetSenders();

    }
}
