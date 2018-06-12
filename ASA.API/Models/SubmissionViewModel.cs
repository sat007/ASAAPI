using ASA.API.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASA.API.Models
{
    public class SubmissionViewModel
    {
        [Required]
        public PeriodViewModel PeriodViewModel { get; set; }
        [Required]
        public VAT100ViewModel VAT100ViewModel { get; set; }
        [Required]
        public BusinessViewModel BussinessViewModel { get; set; }
        [Required]
        public string RunMode { get; set; }

    }
}