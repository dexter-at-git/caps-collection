using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using FluentValidation;

namespace CapsCollection.Silverlight.UI.Modules.Geography.Validators
{
    public class CityModelValidator : AbstractValidator<CityDto>
    {
        private CityModelValidator()
        {
            RuleFor(x => x.EnglishCityName)
                .NotEmpty().WithMessage("English city name cannot be empty")
                .Length(1, 100).WithMessage("Must be between 1-100 characters.");

            RuleFor(x => x.NationalCityName)
                .NotEmpty().WithMessage("National city name cannot be empty")
                .Length(1, 100).WithMessage("Must be between 1-100 characters.");
        }

        public static CityModelValidator Create()
        {
            return new CityModelValidator();
        }
    }
}
