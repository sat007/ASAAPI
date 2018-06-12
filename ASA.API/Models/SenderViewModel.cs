using ASA.Core;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
//using FluentValidation;
//using FluentValidation.Attributes;
//using System.ComponentModel.DataAnnotations;


namespace ASA.API.Model
{
    //[Validator(typeof(SenderViewModelValidator))]
    //NOT Used 
    public class SenderViewModelTmp //: IValidatableObject
    {
        //private readonly IValidator _validator;
        //#region IValidatableObject

        ///
        /// Determines whether the specified object is valid.
        ///
        ///The validation context.
        ///
        /// A collection that holds failed-validation information.
        ///
        ///
        //public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        //{
        //    if (Email==null)
        //    {
        //        yield return new ValidationResult("Invalid email");
        //    }
        //    //return _validator.Validate(this).ToValidationResult();
        //}

        ////public SenderViewModel() {
        //    this._validator = new SenderViewModelValidator();
        //    _validator.Validate(this);
        //}


        //#endregion
        //[Display(Name="Title")]
        //[Required]
        public string Title;
        //[Display(Name = "ForName1")]
        [Required]
        public string ForName1;
        [Display(Name = "ForName2")]
        public string ForName2;
        [Required]
        public string SurName;
        [Required]
        public string Telephone;
        [Required]
        public string Mobile;
        [Required]
        public string Email;
        [Required]
        public string AddressLine1;
        [Required]
        public string AddressLine2;
        [Required]
        public string AddressLine3;
        [Required]
        public string Postcode;
        [Required]
        public string Country;
        public SenderType Type;
        [Required]
        public string HMRCUserId;
        [Required]
        public string HMRCPassword;
        [Required]
        public string SenderPassword { get; set; }

    }

    //public class SenderViewModelValidator : AbstractValidator<SenderViewModel>
    //{
    //    public SenderViewModelValidator()
    //    {
    //        RuleFor(x => x.Title).NotEmpty();
    //        RuleFor(a => a.ForName1).NotEmpty();
    //        RuleFor(a => a.Type).NotNull();
    //        RuleFor(f => f.ForName2).NotEmpty();
    //    }
    //}
    //public static class FluentValidationExtensions
    //{
    //    /// <summary>
    //    /// Converts the Fluent Validation result to the type the both mvc and ef expect
    //    /// </summary>
    //    /// <param name="validationResult">The validation result.</param>
    //    /// <returns></returns>
    //    public static IEnumerable<ValidationResult> ToValidationResult(
    //        this FluentValidation.Results.ValidationResult validationResult)
    //    {
    //        var results = validationResult.Errors.Select(item => new ValidationResult(item.ErrorMessage, new List<string> { item.PropertyName }));
    //        return results;
    //    }
    //}

}
