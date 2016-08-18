using CapsCollection.Silverlight.Infrastructure.Models;
using FluentValidation;

namespace CapsCollection.Silverlight.UI.Modules.Geography.Validators
{
    public class CountryModelValidator : AbstractValidator<CountryWithFlags>
    {
        private CountryModelValidator()
        {
            RuleFor(x => x.ContinentId)
                .NotEmpty().WithMessage("English country name cannot be empty");

            RuleFor(x => x.EnglishCountryName)
                .NotEmpty().WithMessage("English country name cannot be empty")
                .Length(1, 100).WithMessage("Must be between 1-100 characters.");

            RuleFor(x => x.EnglishCountryFullName)
                .Length(0, 100).WithMessage("Must be between 0-100 characters.");

            RuleFor(x => x.NationalCountryName)
                .NotEmpty().WithMessage("National country name cannot be empty")
                .Length(1, 100).WithMessage("Must be between 1-100 characters.");

            RuleFor(x => x.NationalCountryFullName)
                .Length(0, 100).WithMessage("Must be between 0-100 characters.");

            RuleFor(x => x.Alpha2)
                .Length(0, 2).WithMessage("Must be between 0-2 characters.");

            RuleFor(x => x.Alpha3)
                .NotEmpty().WithMessage("Alpha 3 cannot be empty")
                .Length(3).WithMessage("Must be 3 characters.");

            RuleFor(x => x.ISO)
                .Length(0, 2).WithMessage("Must be between 0-3 characters.");

            RuleFor(x => x.PreciseLocation)
                .Length(0, 100).WithMessage("Must be between 0-100 characters.");
        }

        public static CountryModelValidator Create()
        {
            return new CountryModelValidator();
        }
    }
}
