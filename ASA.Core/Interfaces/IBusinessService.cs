using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core.Interfaces
{
    public interface IBusinessService
    {
        Business Save(Business business);
        bool Update(Business business);
        Task<List<Business>> GetBusinesses(int senderId);
        IQueryable<Business> GetBusinessById(int Id);
    }    
}
