using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core.Interfaces
{
    public interface IPeriodService
    {
        IQueryable<PeriodData> GetPeriodsByBusinessId(int Id);
    }
}
