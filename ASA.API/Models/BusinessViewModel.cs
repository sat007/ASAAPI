using ASA.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ASA.API.Model
{
    public class BusinessViewModel
    {
        public int BusinessId { get; set; }
        [Required]
        public string VATRegNo;
        [Required]
        public DateTime RegisteredDate;
        [Required]
        public string BusinessName;
        [Required]
        public string TradingName;
        [Required]
        public BusinessAdressViewModel BusinessAddress;
        [Required]
        public DateTime NextQuaterStartDate { get; set; }
        [Required]
        public DateTime NextQuaterEndDate { get; set; }
        [Required]
        public string RegNo { get; set; }
        public int SenderId { get; set; }
        [Required]
        public SenderViewModel Sender;
        //[Required]
        public List<PeriodViewModel> Periods { get; set; }
        
        public PeriodViewModel Period { get; set; }
        [Required]
        public string VATRate { get; set; }
    }
    public class BusinessAdressViewModel {
        
        public int BusinessAddressId { get; set; }
        [Required]
        public string Line1 { get; set; }
        [Required]
        public string Line2 { get; set; }

        public string Line3 { get; set; }
        public string Line4 { get; set; }
        [Required]
        public string Postcode { get; set; }
        [Required]
        public string Country { get; set; }

    }
    public class SenderViewModel {
        public int SenderId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string ForName1 { get; set; }
        public string ForName2 { get; set; }
        [Required]
        public string SurName { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public SenderType Type { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string SenderPassword { get; set; }
        [Required]
        public string HMRCUserId { get; set; }
        [Required]
        public string HMRCPassword { get; set; }

    }


    //Line1
    //        Line2: "",
    //        Line3: "",
    //        Line4: "",
    //        Postcode: "",
    //        Country: ""
}
