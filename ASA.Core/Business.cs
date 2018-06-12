using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core
{
    public class Business
    {
        [Key]
        public int BusinessId { get; set; }

        private string _businessName;
        private string _tradingName;
        private DateTime _registeredDate;
        private string _vatRegNo;
        private string _vatRate;
        public string RegNo { get; set; }
        public DateTime NextQuaterStartDate { get; set; }
        public DateTime NextQuaterEndDate { get; set; }

                                           //[ForeignKey("PeriodData")]
                                           //public int FKPeriodId { get; set; }
                                           //public PeriodData PeriodData { get; set; }

        public virtual BusinessAddress BusinessAddress { get; set; } //one to one relationship prop

        //[ForeignKey("Sender")]
        //public int FKSenderId { get; set; } //just defining custom property instead of default prop to understand easly at sql side column, even rename to FKColumnName to identify foreign key naming conventions 
        //if its nullable then we can use ?ColumnId which will make nullable column in sql
        //public Sender Sender { get; set; } //one to may relationship configured by adding a Sender navigation property

        //public virtual ICollection<BusinessAddress> AddressList { get; set; }
        //public virtual ICollection<PeriodData> PeriodsList { get; set; }
        [ForeignKey("Sender")]
        public int SenderId { get; set; }
        public Sender Sender { get; set; }

        public ICollection<Client> Clients { get; set; } 
        public ICollection<PeriodData> Periods { get; set; }
        public string VATRate
        {
            get { return this._vatRate; }
            set { this._vatRate = value; }
        }
        public string VATRegNo
        {
            get { return this._vatRegNo; }
            set { this._vatRegNo = value; }
        }
        public DateTime RegisteredDate
        {
            get { return this._registeredDate; }
            set { this._registeredDate = value; }
        }

        public string BusinessName
        {
            get { return this._businessName; }
            set { this._businessName = value; }
        }

        public string TradingName
        {
            get { return this._tradingName; }
            set { this._tradingName = value; }
        }
        public Boolean IsDeleted { get; set; } = false;
        //public Address AddressDetails
        //{
        //    get { return this._address; }
        //    set
        //    {
        //        this._address = value;
        //    }
        //}
        public string FullCompanyName
        {
            get
            {
                string str = this._businessName;
                if (this._tradingName.Length > 0)
                    str = str + " {trading as " + this._tradingName + "}";
                return str;
            }
        }

        public Business() {
            //this.Senders = new List<Sender>();
            this.Periods = new List<PeriodData>();
            this.Clients = new List<Client>();
        }
        //public Business(string vatregno, DateTime vatregistereddate, string businessname, string tradingname, Address address)
        //{
        //    // this._address = new Address();
        //    this._businessName = businessname;
        //    this._vatRegNo = vatregno;
        //    this._registeredDate = vatregistereddate;
        //    this._tradingName = tradingname;
        //    //this._address = address;
            
        //}
    }
}

