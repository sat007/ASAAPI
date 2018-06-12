using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

namespace ASA.Core
{
    //not been used anywhere in the solution 
    public class IR68
    {
        //period data fields 
        #region Properties  
        private string _periodId;
        private string _periodStart;
        private string _periodEnd;
        private byte _taxQuater;
        private PeriodData _periodData;
        private VAT100 _vat100Data;

        public string PeriodId
        {
            get { return this._periodId; }
            set { this._periodId = value; }

        }

        public string PeriodStart
        {
            get { return this._periodStart; }
            set { this._periodStart = value; }
        }

        public string PeriodEnd
        {
            get { return this._periodEnd; }
            set { this.PeriodEnd = value; }
        }

        public byte TaxQuater
        {
            get { return this._taxQuater; }
            set { _taxQuater = value; }
        }
        #endregion Properties
        #region Constructor 
       
        public IR68(PeriodData periodData, VAT100 vat100Data)
        {
            this._periodData = periodData;
            this._vat100Data = vat100Data;
        }
        #endregion Contructor 
        #region methods 
        public IRenvelope CreateIREnvelopeBody(Sender sender, Business business)
        {
            IRenvelope irenvelope = new IRenvelope();
            IRheader irheader = new IRheader();

            IRheaderKey irheaderkey = new IRheaderKey();
            irheaderkey.Type = "VATRegNo";
            irheaderkey.Value = business.VATRegNo;
            irheader.PeriodID = _periodData.PeriodrefId;
            irheader.Sender = IRheaderSender.Individual;
            irheader.PeriodStart = _periodData.StartPeriod;
            irheader.PeriodEnd = _periodData.EndPeriod;

            IRheaderKey[] irHeaderKeys = new IRheaderKey[1];
            irHeaderKeys[0] = irheaderkey;
            irheader.Keys = irHeaderKeys;

            VATDeclarationRequest_ContactDetailsStructure contact = new VATDeclarationRequest_ContactDetailsStructure();
            VATDeclarationRequest_ContactDetailsStructureName name =
                new VATDeclarationRequest_ContactDetailsStructureName();
            name.Fore = new[] { sender.ForName1, sender.ForName2 };
            name.Sur = sender.SurName;
            name.Ttl = sender.Title;

            VATDeclarationRequest_ContactDetailsStructureEmail email =
                new VATDeclarationRequest_ContactDetailsStructureEmail();
            email.Preferred = VATDeclarationRequest_YesNoType.yes;
            email.PreferredSpecified = true;
            email.Type = VATDeclarationRequest_WorkHomeType.work;
            email.TypeSpecified = true;
            email.Value = sender.Email;
            contact.Name = name;

            VATDeclarationRequest_ContactDetailsStructureEmail[] Emails =
                new VATDeclarationRequest_ContactDetailsStructureEmail[1];
            Emails[0] = email;
            contact.Email = Emails;

            IRheaderPrincipal principal = new IRheaderPrincipal();
            principal.Contact = contact;
            irheader.Principal = principal;

            IRheaderIRmark irmark = new IRheaderIRmark();
            irmark.Type = IRheaderIRmarkType.generic;
            irmark.Value = "";
            irheader.IRmark = irmark;

            VATDeclarationRequest vatreq = new VATDeclarationRequest();
            vatreq.VATDueOnOutputs = _vat100Data.Box1;
            vatreq.VATDueOnECAcquisitions = _vat100Data.Box2;
            vatreq.TotalVAT = _vat100Data.Box3;
            vatreq.VATReclaimedOnInputs = _vat100Data.Box4;
            vatreq.NetVAT = _vat100Data.Box5;
            vatreq.NetSalesAndOutputs = _vat100Data.Box6;
            vatreq.NetPurchasesAndInputs = _vat100Data.Box7;
            vatreq.NetECSupplies = _vat100Data.Box8;
            vatreq.NetECAcquisitions = _vat100Data.Box9;

            //set values here
            irenvelope.IRheader = irheader;
            irenvelope.VATDeclarationRequest = vatreq;

            return irenvelope;

        }
       
    }
    #endregion methods 
}

  