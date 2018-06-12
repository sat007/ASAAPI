using ASA.Core.Infrastructure;

namespace ASA.Core.Repositories
{
    public interface IBusinessRepository : IRepository<Business>
    {
    }

    public interface ISenderRepository : IRepository<Sender>
    {
    }

    public interface IClientRepository:  IRepository<Client>
    {
        new bool Update(Client entity);
        new bool Delete(Client entity);
    
    }
    public interface IInvoiceRepository: IRepository<Invoice>
    {
        new void Update(Invoice entity);
        //new bool Update(Invoice entity);
    }
    public interface IHMRCResponseRepository: IRepository<HMRCResponse>
    {

    }
    public interface IPeriodDataRepository : IRepository<PeriodData> { }
    public interface ISuccessResponseRepository : IRepository<SuccessResponse> { }
}
