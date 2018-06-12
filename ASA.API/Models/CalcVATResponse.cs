using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASA.API.Models
{
    public class CalcVATResponse
    {
        public int TotalWorkingDay { get; set; }
        public double GrossExcludingVAT { get; set; }
        public double GrossIncludingVAT { get; set; }
        public double TotalVAT { get; set; }

        public double VATRate { get; set; }

    }
}