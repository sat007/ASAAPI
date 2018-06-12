using System;
using System.Data.Entity;
using ASA.Core.Infrastructure;
using System.Linq;
using System.Data.Entity.Validation;

namespace ASA.Core.Repositories
{
    public class BusinessRepository : RepositoryBase<Business>, IBusinessRepository
    {
        private IDataBaseFactory _databasefactory;
        public BusinessRepository(IDataBaseFactory databasefactory) : base(databasefactory)
        {
            this._databasefactory = databasefactory;
        }
        public override void Update(Business entity)
        {
            using (var ctx = _databasefactory.Get())
            {

                //get business entity from the DB based on Id             
                var bus = ctx.Business.Where(b => b.BusinessId == entity.BusinessId).
                    Include(a => a.BusinessAddress).
                    Include(s => s.Sender).
                    Include(p=>p.Periods).Where(pe=>pe.BusinessId == entity.BusinessId)
                    .FirstOrDefault();
                //reset the db entity values to new values 
                ctx.Entry(bus).CurrentValues.SetValues(entity);
                if (bus.Sender != null) { ctx.Entry(bus.Sender).CurrentValues.SetValues(entity.Sender); }
                var existingPeriod = (PeriodData)null;
                foreach (var period in entity.Periods)
                {
                    existingPeriod = bus.Periods.Where(p => p.PeriodrefId == period.PeriodrefId).SingleOrDefault();
                    //update children 
                    if (existingPeriod != null)
                    {
                        ctx.Entry(existingPeriod).CurrentValues.SetValues(period);
                        if (period.VAT100 != null) //check entity object and add explicityly 
                        {
                            if(existingPeriod.VAT100==null)
                            {
                                existingPeriod.VAT100 = period.VAT100;
                            }
                            else
                            {
                                ctx.Entry(existingPeriod.VAT100).CurrentValues.SetValues(period.VAT100);
                            }
                            
                        }
                        foreach (var resp in period.HmrcResponses)
                        {
                            var existingChild = existingPeriod.HmrcResponses.Where(c => c.HMRCResponseId == resp.HMRCResponseId).SingleOrDefault();
                            //update children 
                            if (existingChild != null)
                            {
                                ctx.Entry(existingChild).CurrentValues.SetValues(resp);

                            }
                            //add new children 
                            else
                            {
                                existingPeriod.HmrcResponses.Add(resp);
                                
                            }
                        }
                       // bus.Periods.Add(existingPeriod);
                    }
                    //if period is new add period to bus entity but need to add responses to the period before adding to  bu 
                    else
                    {
                        //foreach (var response in period.HmrcResponses)
                        //{
                            //var existingResponse = existingPeriod.HmrcResponses.Any(h => h.HMRCResponseId == response.HMRCResponseId);
                            //if(existingResponse)
                            //{
                            //    ctx.Entry(response).CurrentValues.SetValues(response);
                            //}
                            //else
                            //{
                                //existingPeriod.HmrcResponses.Add(response);
                            bus.Periods.Add(period);
                            //}                            
                        //}                         
                    }

                }
              

                ctx.SaveChanges();
            }
        }
    }
    public class SenderRepository : RepositoryBase<Sender>, ISenderRepository
    {
        public SenderRepository(IDataBaseFactory databasefactory) : base(databasefactory)
        {
        }
    }
    public class HMRCResponseRepository : RepositoryBase<HMRCResponse>, IHMRCResponseRepository
    {
        public HMRCResponseRepository(IDataBaseFactory databasefactory) : base(databasefactory)
        {
        }
    }
    //public class SuccessResponseRepository : RepositoryBase<SuccessResponse>, ISuccessResponseRepository
    //{
    //    public SuccessResponseRepository(IDataBaseFactory databasefactory) : base(databasefactory)
    //    {
    //    }
    //}
    public class PeriodDataRepository : RepositoryBase<PeriodData>, IPeriodDataRepository
    {
        private IDataBaseFactory _databasefactory;
        public PeriodDataRepository(IDataBaseFactory databasefactory) : base(databasefactory)
        {
            this._databasefactory = databasefactory;
        }

        public override void Update(PeriodData entity)
        {
            using (var ctx = _databasefactory.Get())
            {
                var per = ctx.PeriodData.Where(p => p.PeriodId == entity.PeriodId).
                                Include(v=>v.VAT100)
                                .Include(h=>h.HmrcResponses).Where(h=>h.PeriodId == entity.PeriodId)
                          .FirstOrDefault();
                if (per != null)
                {
                    ctx.Entry(per).CurrentValues.SetValues(entity);
                    if (per.VAT100 != null) { ctx.Entry(per.VAT100).CurrentValues.SetValues(entity.VAT100); }
                    else { per.VAT100 = entity.VAT100; } //just forcing set values as in some occasions the above statement set VAT100 to null in the per object
                                                         // Delete children
                    foreach (var existingChild in per.HmrcResponses)
                    {
                        if (!entity.HmrcResponses.Any(c => c.HMRCResponseId == existingChild.HMRCResponseId))
                            ctx.HMRCResponse.Remove(existingChild);
                    }
                    foreach (var resp in entity.HmrcResponses)
                    {
                        var existingChild = per.HmrcResponses.Where(c => c.HMRCResponseId == resp.HMRCResponseId).SingleOrDefault();
                        //update children 
                        if (existingChild != null)
                        {
                            ctx.Entry(existingChild).CurrentValues.SetValues(resp);

                        }
                        //add new children 
                        else
                        {
                            per.HmrcResponses.Add(resp);
                        }
                    }
                }
                else
                {
                    ctx.PeriodData.Add(entity);
                }
                //add or update business related entity here before savig the DBContext as its single request per entity the context is disposing if we try another request from controller 
                //var bus = ctx.Business.Where(b => b.BusinessId == bvmEntity.BusinessId).Include(bd => bd.BusinessAddress).FirstOrDefault();
                //if (bus!=null)
                //{
                //    ctx.Entry(bus).CurrentValues.SetValues(bvmEntity);
                //}                         
                try
                {
                    // Your code...
                    // Could also be before try if you know the exception occurs in SaveChanges

                    ctx.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }            
            }
        }
    }
    public class ClientRepository : RepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(IDataBaseFactory databasefactory) : base(databasefactory) { }

        public new bool Update(Client entity)
        {
            try
            {
                base.Update(entity);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public new bool Delete(Client entity)
        {
            try
            {
                base.Delete(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

    }
    public class InvoiceRepository: RepositoryBase<Invoice>, IInvoiceRepository
    {
        private IDataBaseFactory _databasefactory;
        public InvoiceRepository(IDataBaseFactory databasefactory): base(databasefactory) {
            this._databasefactory = databasefactory;
        }         

        public override void Update(Invoice entity)
        {
            using (var ctx = _databasefactory.Get())
            {
                var inv = ctx.Invoice.Where(i => i.InvoiceId == entity.InvoiceId)
                           .Include(d => d.InvoiceDetail)
                           .Include(it => it.InvoiceItems)
                           .FirstOrDefault();
                
                ctx.Entry(inv).CurrentValues.SetValues(entity);
                if(inv.InvoiceDetail != null)
                {
                    ctx.Entry(inv.InvoiceDetail).CurrentValues.SetValues(entity.InvoiceDetail);
                }              

                // Delete children
                foreach (var existingChild in inv.InvoiceItems)
                {
                    if (!entity.InvoiceItems.Any(c => c.Id == existingChild.Id))
                        ctx.InvoiceItem.Remove(existingChild);
                }
                foreach (var item in entity.InvoiceItems)
                {
                    var existingChild = inv.InvoiceItems.Where(c => c.Id == item.Id).SingleOrDefault();
                    //update children 
                    if (existingChild != null) 
                    {
                        ctx.Entry(existingChild).CurrentValues.SetValues(item);

                    }
                    //add new children 
                    else
                    {
                        inv.InvoiceItems.Add(item);
                    }
                }
               // ctx.SaveChanges();                
            }            
        }

    }
}
