using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASA.Core.Infrastructure;
using ASA.Core.Interfaces;
using ASA.Core.Repositories;
using System.Data.Entity;

namespace ASA.Core.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BusinessService(IBusinessRepository businessRepository, IUnitOfWork unitOfWork)
        {
            this._businessRepository = businessRepository;
            this._unitOfWork = unitOfWork;
        }
        public Business Save(Business business)
        {
            var bu = _businessRepository.Add(business);            
            SaveCommit();
            return bu;
        }
        public bool Update(Business business)
        {
            try
            {
                _businessRepository.Update(business);
                //SaveCommit();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public async Task<List<Business>> GetBusinesses(int senderId)
        {
            //var businesses = _businessRepository.Query().Include(b => b.BusinessAddress).Include(s => s.Sender).Include(p => p.Periods).Where(b => b.IsDeleted == false);
            var businesses = await _businessRepository.GetAllIncluding(
                b => b.BusinessAddress, 
                s => s.Sender, 
                p => p.Periods).Where(b => b.IsDeleted == false && b.SenderId == senderId).ToListAsync();           
            return businesses;
        }
        public IQueryable<Business> GetBusinessById(int Id)
        {
            var business = _businessRepository.Query()
                .Include(a => a.BusinessAddress)
                .Include(p=>p.Periods).Where(pi=>pi.BusinessId ==Id)
                .Include(s => s.Sender).Where(b => b.BusinessId == Id && b.IsDeleted == false); //_client.GetById(Id);
            return business;
        }

        private void SaveCommit()
        {
            _unitOfWork.Commit();
        }
    }
}
