using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;
using FluentValidation;

namespace CapsCollection.Silverlight.UI.Modules.Brewery.Validators
{
    public class BreweryModelValidator : AbstractValidator<BreweryDto>
    {
        private BreweryModelValidator()
        {
            RuleFor(x => x.CityId)
                .NotEqual(-1).WithMessage("City name cannot be empty")
                .NotEmpty().WithMessage("City name cannot be empty");

            RuleFor(x => x.Brewery)
                .NotEmpty().WithMessage("Brewery name cannot be empty")
                .Length(1, 50).WithMessage("Must be between 1-50 characters.");

            RuleFor(x => x.Site)
                .Length(0, 50).WithMessage("Must be between 0-50 characters.");

            RuleFor(x => x.Comment)
                .Length(0, 50).WithMessage("Must be between 0-100 characters.");
        }

        public static BreweryModelValidator Create()
        {
            return new BreweryModelValidator();
        }
    }
}
