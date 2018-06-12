using ASA.Core.Interfaces;
using ASA.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core.Services
{
    public class PeriodService: IPeriodService 
    {
        private readonly IPeriodDataRepository _periodDataRepository;
        public PeriodService(IPeriodDataRepository periodDataRepository)
        {
            this._periodDataRepository = periodDataRepository;
        }
        public IQueryable<PeriodData> GetPeriodsByBusinessId(int Id)
        {
            return _periodDataRepository.Query().Include(h => h.HmrcResponses).Where(p => p.BusinessId == Id);
        }

    }
}
