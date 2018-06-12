using ASA.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace ASA.Core.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new ASAEntitiesContext())
            {
                //Sender sen = new Sender()
                //{
                //    Title = "mr",
                //    ForName1 = "Sat",
                //    ForName2 = "Ch",
                //    SurName = "Gat",
                //    Email = "test@mail.com",
                //    AddressLine1 = "awdas",
                //    AddressLine2 = "asdasdsa",
                //    AddressLine3 = "www",
                //    Country = "UK",
                //    Postcode = "E11",
                //    Mobile = "01111",
                //    SenderPassword = "test",
                //    Telephone = "5131",
                //    Type = SenderType.Individual,
                //    HMRCUserId = "VATDEC180a01",
                //    HMRCPassword = "testing1"

                //};
                //ctx.Sender.Add(sen);
                //ctx.SaveChanges();
                GovTalkMessage msg = new GovTalkMessage()
                {
                    
                };
                //SuccessResponseIRmarkReceipt ir = new SuccessResponseIRmarkReceipt
                //{

                //};
                //SuccessResponse resp = new SuccessResponse()
                //{
                //    //IRmarkReceipt = ir, 
                //    AcceptedTime = DateTime.Now,
                //    AcceptedTimeSpecified = true,
                //    ResponseData_IsEmpty = true, 
                    


                //};

                //ctx.SucessResponse.Add(resp);
                ctx.SaveChanges();

                Client clt = new Client()
                {
                    Name = "London Borough Of Redbride",
                    RegNo = "Reg123456789",
                    VATNo = "VAT123456"
                    
                                        
                };

                ctx.Client.Add(clt);
                ctx.SaveChanges();

                Business bu = new Business()
                {
                    BusinessName = "TEST",
                    BusinessId=1,
                    TradingName="TEST",
                    RegisteredDate= DateTime.Now,
                    VATRegNo= "123456",
                    SenderId = 1                    
                };
                BusinessAddress bud = new BusinessAddress()
                {
                    Line1="test",
                    Line2="test2",
                    Line3="test3",
                    Line4="test5",
                    Postcode="IG1",
                    Country="UK"                     
                };
                bu.BusinessAddress = bud;
               
                //ctx.BusinessAddress.Add(bud);
                //ctx.Business.Add(bu);
                // ctx.SaveChanges();

               
                
          
                PeriodData per = new PeriodData()
                {
                    StartPeriod = DateTime.Now.AddMonths(-3),
                    EndPeriod = DateTime.Now,
                    PeriodrefId = "2017-04",
                    Status = SubmissionStatus.Draft                   
                };
                VAT100 vat = new VAT100()
                {

                };
                //per.VAT100 = vat;


                bu.Periods.Add(per);
        
                Client cl = new Client()
                {
                    //Id = 1,
                    Name = "consoleC",
                    RegNo = "123",
                    VATNo = "1234567"
                };
                
                ClientAddress cd = new ClientAddress()
                {
                    Address1 = "First line of add",
                    Address2 = "Second line of add",
                    City = "Lonodn",
                    ClientAddressId = 1, 
                    Country = "UK",
                    PostCode = "SS1",
                    
                };
                
                PeriodData per1 = new PeriodData()
                {
                    StartPeriod = DateTime.Now.AddMonths(-3),
                    EndPeriod = DateTime.Now,
                    PeriodrefId = "2017-07",
                    Status = SubmissionStatus.Draft
                };
                bu.Periods.Add(per1);

                //works above 


                cl.ClientAddress = cd;
                //ctx.Client.Add(cl);
                //ctx.SaveChanges();
                //ctx.SaveChanges();
                //var cup = ctx.ClientAddress.FirstOrDefault();
                //cup.Address1 = "New Update address";
                //var c = ctx.Client.FirstOrDefault();

                //c.ClientAddress = cup;

                //ctx.Client.Add(c);


                // ctx.Client.Add(cl);
                InvoiceDetail invd = new InvoiceDetail();
                invd.No = "LBR-1";
                invd.IssueDate = DateTime.Now;
                invd.DueDate = DateTime.Now.AddDays(7);
                invd.Note = "test";
                invd.Ref = "nothing";
                invd.Discount = "10%";

                InvoiceItem item = new InvoiceItem();
                var itemguid = Guid.Parse("00000000-0000-0000-0000-000000000000");
                item.Id = itemguid;
                item.Description = "apple";
                item.Quantity = "1";
                item.Price = "1";
                item.Total = "1";
                item.SubTotal = "1";
                item.VAT = "0";
                item.VATRate = "0";



                List<InvoiceItem> items = new List<InvoiceItem>();
                items.Add(item);
                var myguid = Guid.Parse("3551A09E-DE21-A325-6DF4-40281A4FEBE0");
                //var inv = ctx.Invoice.Where(i => i.InvoiceId == myguid)
                //            .Include(d => d.InvoiceDetail)
                //            .Include(it => it.InvoiceItems)
                //            .FirstOrDefault();
                

                //ctx.Entry(inv).CurrentValues.SetValues(inv);
                //foreach (var it in items)
                //{
                //    var existingChild = inv.InvoiceItems.Where(c => c.Id == it.Id).SingleOrDefault();
                //    if(existingChild!=null)
                //    {
                //        ctx.Entry(existingChild).CurrentValues.SetValues(it);

                //    }
                //    else
                //    {
                //        Console.WriteLine("add new item");
                //        inv.InvoiceItems.Add(item);
                //    }
                //}

                //ctx.Entry(inv).State = EntityState.Modified;
                
                //Invoice myinvoice = new Invoice();
                //myinvoice.ClientId = 3;
                //myinvoice.InvoiceDetail = invd;
                //myinvoice.InvoiceItems = items;
                //ctx.Invoice.Add(myinvoice);
                //ctx.SaveChanges();
                // bu.Periods.Add(per1);

                // ctx.Business.Add(bu);

                //cl.ClientAddress = cd;
                //ctx.Client.Add(cl);
                //ctx.SaveChanges();            
            }
        }
    }
}
