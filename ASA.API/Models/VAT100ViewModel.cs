//using FluentValidation;
//using FluentValidation.Attributes;

using System;
using System.ComponentModel.DataAnnotations;

namespace ASA.API.Model
{
    //[Validator(typeof(VAT100ViewModelValidator))]
    public class VAT100ViewModel
    {

        [Required]
        public double VATDueOnOutputs { get; set; }
        [Required]
        public double VATDueOnECAcquisitions { get; set; }
        [Required]
        public double TotalVAT { get; set; }
        [Required]
        public double VATReclaimedOnInputs { get; set; }
        [Required]
        public double NetVAT { get; set; }
        [Required]
        public double NetSalesAndOutputs { get; set; }
        [Required]
        public double NetPurchasesAndInputs { get; set; }
        [Required]
        public double NetECSupplies { get; set; }
        [Required]
        public double NetECAcquisitions { get; set; }
        [Required]
        public DateTime LastUpdated { get; set; }


    }
    //public class VAT100ViewModelValidator: AbstractValidator<VAT100ViewModel>
    //{
    //    public VAT100ViewModelValidator()
    //    {
            
    //        RuleFor(x => x.NetVAT).NotNull().NotEmpty();
    //        RuleFor(x => x.TotalVAT).NotEmpty().NotNull();
    //        RuleFor(x => x.VATDueOnOutputs).NotNull().NotEmpty();
    //        RuleFor(x => x.VATReclaimedOnInputs).NotEmpty().NotEmpty();
    //        RuleFor(x => x.NetSalesAndOutputs).NotEmpty().NotNull();
    //        RuleFor(x => x.NetPurchasesAndInputs).NotNull().NotEmpty();
            
    //    }
    //}
}
