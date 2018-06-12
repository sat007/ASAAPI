using ASA.Core.Infrastructure;
using ASA.Core.Interfaces;
using ASA.Core.Repositories;
using System.Data.Entity;
using System.Linq;

namespace ASA.Core.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _client;
        private readonly IUnitOfWork _unitOfWork;

        public ClientService(IClientRepository client, IUnitOfWork unitOfWork)
        {
            this._client = client;
            this._unitOfWork = unitOfWork;
        }

        public Client Add(Client client)
        {
            var newClient = _client.Add(client);
            SaveCommit();
            return newClient;
            
        }

        public bool Update(Client client)
        {
            // _client.Update(client);
            //var status = _client.Update(client); //this return void 
            //ClientRepository rep = new ClientRepository();
            //var status = rep.Update(client);
            //SaveCommit();
            //return status;
            var dbClient = _client.Query().Include(c => c.ClientAddress).Where(i => i.ClientId == client.ClientId).Single();
            if (dbClient != null)
            {
                dbClient.ClientId = client.ClientId;
                dbClient.Name = client.Name;
                dbClient.RegNo = client.RegNo;
                dbClient.VATNo = client.VATNo;
                dbClient.ClientAddress = client.ClientAddress;

                var status = _client.Update(dbClient); 
                SaveCommit();
                return status;
            }
            else { return false; }
        }

        public bool Delete(Client client)
        {
            var dbClient = _client.Query().Include(c => c.ClientAddress).Where(i => i.ClientId == client.ClientId).Single();
            if (dbClient != null)
            {
                dbClient.ClientId = client.ClientId;
                dbClient.Name = client.Name;
                dbClient.RegNo = client.RegNo;
                dbClient.VATNo = client.VATNo;
                dbClient.ClientAddress = client.ClientAddress;

                //var status = _client.Delete(client); //commented out for soft delete implementation 

                dbClient.IsDeleted = true;
                var status = _client.Update(client);
                SaveCommit();
                return status;
            }
            else { return false; }
        }

        public IQueryable<Client>GetClients()
        {
            var clients = _client.Query().Include(c => c.ClientAddress).Where(c => c.IsDeleted == false);
            //_client.Query().Include(c => c.ClientAddress);
            //   return clients.Select(a => new ClientModel() { Id = a.Id, Name = a.Name, RegNo = a.RegNo, VATNo = a.VATNo, ClientAddress = a.ClientAddress });
            return clients;
        }

        public IQueryable<Client> GetClientById(int Id)
        {
            var client = _client.Query().Include(c => c.ClientAddress).Where(i => i.ClientId == Id); //_client.GetById(Id);

            return client;

        }
        public void SaveCommit()
        {
            _unitOfWork.Commit();
        }

    }
}
