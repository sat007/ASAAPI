using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASA.API.Models
{
    public class CalcVATViewModel
    {
        [Required]
        public DateTime StartPeriod { get; set; }
        [Required]
        public DateTime EndPeriod { get; set; }
        [Required]
        public string VATRate { get; set; }
        [Required]
        public string DayRate { get; set; }
        [Required]
        public string DaysOff { get; set; }
        [Required]
        public string AdditionalDays { get; set; }

    }
}