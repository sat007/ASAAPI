using ASA.Core.Infrastructure;
using ASA.Core.Interfaces;
using ASA.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core.Services
{
    public class SenderService : ISenderService 
    {
        private readonly ISenderRepository _senderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SenderService(ISenderRepository SenderRepository, IUnitOfWork UnitOfWork)
        {
            this._senderRepository = SenderRepository;
            this._unitOfWork = UnitOfWork;
        }

        public bool Save(Sender sender)
        {
            try
            {
                _senderRepository.Add(sender);
                SaveCommit();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }           
            
        }

        public bool Update(Sender sender)
        {
            try
            {
                _senderRepository.Update(sender);
                SaveCommit();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
          
        }
        public IQueryable<Sender> GetSenders()
        {
            var senders = _senderRepository.Query();
            return senders;
        }
        public void SaveCommit()
        {
            _unitOfWork.Commit();
        }
    }
}
