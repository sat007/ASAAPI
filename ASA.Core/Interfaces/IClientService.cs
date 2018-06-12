using System.Linq;

namespace ASA.Core.Interfaces
{
    public interface IClientService
    {
        Client Add(Client client);
        bool Update(Client client);
        bool Delete(Client client);
        IQueryable<Client> GetClients();
        IQueryable<Client> GetClientById(int Id);
    }
}
