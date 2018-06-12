using ASA.API.Models;
using ASA.Core;
using ASA.Core.DTOs;
using ASA.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ASA.API.Controllers
{
    public class ClientController : ApiController
    {

        private readonly IClientService _clientService;

        public ClientController(IClientService clientService) { this._clientService = clientService; } 

        // GET: api/Clients
        [HttpGet]
        public HttpResponseMessage Get()
        {

            var clientsList = _clientService.GetClients().ToList();
            var cvm = clientsList.Select(c => new ClientViewModel() {
                Id = c.ClientId, Name=c.Name, RegNo=c.RegNo, VATNo=c.VATNo, BusinessId=c.BusinessId,
                Address = new Model.AddressViewModel()
                { Line1 = c.ClientAddress.Address1, Line2=c.ClientAddress.Address2, City=c.ClientAddress.City, PostCode=c.ClientAddress.PostCode, Country=c.ClientAddress.Country } });
            if (clientsList == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return Request.CreateResponse<IEnumerable<ClientViewModel>>(HttpStatusCode.OK, cvm);
        }

        // GET: api/Clients/5
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            var client = _clientService.GetClientById(id);
            var cvm = client.Select(c=> new ClientViewModel() {Id=c.ClientId, Name=c.Name, RegNo=c.RegNo, VATNo=c.VATNo, BusinessId = c.BusinessId, Address = new Model.AddressViewModel() { Line1 = c.ClientAddress.Address1, Line2 = c.ClientAddress.Address2, City = c.ClientAddress.City, PostCode = c.ClientAddress.PostCode, Country = c.ClientAddress.Country } }).Single();
            if (cvm == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return Request.CreateResponse<ClientViewModel>(HttpStatusCode.OK, cvm);
        }

        // POST: api/Clients
        [HttpPost]
        public HttpResponseMessage Post([FromBody]ClientViewModel client)
        {  
            if(ModelState.IsValid)
            {
                var cm = new Client()
                {
                    Name = client.Name,
                    RegNo = client.RegNo,
                    VATNo = client.VATNo,
                    BusinessId = client.BusinessId,
                    ClientAddress = new ClientAddress()
                    {
                        Address1 = client.Address.Line1,
                        Address2 = client.Address.Line2,
                        City = client.Address.City,
                        PostCode = client.Address.PostCode,
                        Country = client.Address.Country
                    }

                };
                var newClient = _clientService.Add(cm);
                if (newClient != null)
                {
                    var msg = new HttpResponseMessage(HttpStatusCode.Created);
                    msg.Headers.Location = new Uri(Request.RequestUri + newClient.ClientId.ToString());
                    return msg;
                }
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }  
            else
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = "Model validation failed"
                });
            }        
          
        }

        // PUT: api/Clients/5
        [HttpPut]
        public HttpResponseMessage Put(int id, [FromBody]ClientViewModel cvm) 
        {
            if(ModelState.IsValid)
            {
                var client = new Client()
                {
                    Name = cvm.Name,
                    ClientId = cvm.Id,
                    RegNo = cvm.RegNo,
                    VATNo = cvm.VATNo,                    
                    ClientAddress = new ClientAddress()
                    {   Address1 = cvm.Address.Line1,
                        Address2 = cvm.Address.Line2,
                        City = cvm.Address.City,
                        Country = cvm.Address.Country,
                        PostCode = cvm.Address.PostCode,
                        ClientAddressId=cvm.Id
                    }
                };
                bool status = _clientService.Update(client);               
                if (status) return new HttpResponseMessage(HttpStatusCode.OK);
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
          return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // DELETE: api/Clients/5
        [HttpDelete]
        public HttpResponseMessage Delete(ClientViewModel cvm)
        {
            if (ModelState.IsValid)
            {
                var client = new Client()
                {
                    Name = cvm.Name,
                    ClientId = cvm.Id,
                    RegNo = cvm.RegNo,
                    VATNo = cvm.VATNo,
                    BusinessId = cvm.BusinessId,
                    ClientAddress = new ClientAddress()
                    {
                        Address1 = cvm.Address.Line1,
                        Address2 = cvm.Address.Line2,
                        City = cvm.Address.City,
                        Country = cvm.Address.Country,
                        PostCode = cvm.Address.PostCode,
                        ClientAddressId = cvm.Id
                    }
                };

                var isDeleted = _clientService.Delete(client);
                if (isDeleted) return new HttpResponseMessage(HttpStatusCode.OK);
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }
    }
}
