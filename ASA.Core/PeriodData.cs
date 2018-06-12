using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core
{
    [Table("PeriodInfo")]
    public class PeriodData
    {
        private string _periodrefId;
        //private byte _taxQuater;
        private DateTime _startPeriod;
        private DateTime _endPeriod;
        //private DateTime _paymentDue;
        private SubmissionStatus _status;
        //private SubmissionVATType _vatType;
        //private DateTime _lastUpdated;

        //[ForeignKey("Business")]
        [Key]
        public int PeriodId { get; set; }

        [ForeignKey("Business")]
        public int BusinessId { get; set; }
        public Business Business { get; set; } //one to many relation

        public virtual VAT100 VAT100 { get; set; } //one to one relationship 
        //public Business Business { get; set; }
        //    public virtual VATDeclarationRequest VATPeriodData { get; set; }
        public virtual ICollection<HMRCResponse>HmrcResponses { get; set; } 
       // public virtual ICollection<SuccessResponse> SuccessResponse { get; set; }
        public string PeriodrefId
        {
            get
            {
                return this._periodrefId;
            }
            set
            {
                this._periodrefId = value;
            }
        }
    //public byte TaxQuater
    //{
    //  get
    //  {
    //    return this._taxQuater;
    //  }
    //  set
    //  {
    //    this._taxQuater = value;
    //  }
    //}
    public DateTime StartPeriod
    {
      get
      {
        return this._startPeriod;
      }
      set
      {
        this._startPeriod = value;
      }
    }
    public DateTime EndPeriod
    {
      get
      {
        return this._endPeriod;
      }
      set
      {
        this._endPeriod = value;
      }
    }
    //public DateTime PaymentDue
    //{
    //  get
    //  {
    //    return this._paymentDue;
    //  }
    //  set
    //  {
    //    this._paymentDue = value;
    //  }
    //}
    public SubmissionStatus Status
    {
      get
      {
        return this._status;
      }
      set
      {
        this._status = value;
      }
    }
    //public SubmissionVATType VatType
    //{
    //  get
    //  {
    //    return this._vatType;
    //  }
    //  set
    //  {
    //    this._vatType = value;
    //  }
    //}
    //public DateTime LastUpdated
    //{
    //  get
    //  {
    //    return this._lastUpdated;
    //  }
    //  set
    //  {
    //    this._lastUpdated = value;
    //  }
    //}
    public string PeriodDateRange
    {
      get
      {
        return this._startPeriod.ToUniversalTime() + " to " + this._endPeriod.ToUniversalTime();
      }
    }

        

        public PeriodData()
    {
            this.HmrcResponses = new List<HMRCResponse>();
            //this.SuccessResponse = new List<SuccessResponse>();
    }
    public PeriodData(string periodrefId, DateTime periodstartdate, DateTime periodenddate)
        {
            this._periodrefId = periodrefId;
            this._startPeriod = periodstartdate;
            this._endPeriod = periodenddate;

        }
    }
    public enum SubmissionStatus
    {
        Draft,
        Pending,
        Errors,
        Unknown,
        Accepted
    }
    public enum AllowedSubmissionStatus
    {
        Draft = 1<<0,
        Pending = 1<<1,
        Errors = 1<<2,
        Unknown = 1<<3,
        Accepted = 1<<4
    }
    public enum SubmissionVATType
    {
        VAT100,
        VAT101,
    }

}
