using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;
using FluentValidation;

namespace CapsCollection.Silverlight.UI.Modules.Collection.Validators
{
    public class BeerModelValidator : AbstractValidator<BeerDto>
    {
        private BeerModelValidator()
        {
            RuleFor(x => x.BreweryId)
                .NotEqual(-1).WithMessage("Brewery cannot be empty")
                .NotEmpty().WithMessage("Brewery cannot be empty");

            RuleFor(x => x.BeerStyleId)
                .NotEqual(-1).WithMessage("Beer style cannot be empty")
                .NotEmpty().WithMessage("Beer style cannot be empty");

            RuleFor(x => x.CapTypeId)
                .NotEqual(-1).WithMessage("Cap type cannot be empty")
                .NotEmpty().WithMessage("Cap type cannot be empty");

            RuleFor(x => x.BeerName)
                .NotEmpty().WithMessage("Beer name cannot be empty")
                .Length(1, 50).WithMessage("Must be between 1-50 characters.");

            RuleFor(x => x.BeerType)
                .Length(0, 50).WithMessage("Must be between 0-50 characters.");

            RuleFor(x => x.BeerSite)
                .Length(0, 100).WithMessage("Must be between 1-100 characters.");

            RuleFor(x => x.BeerComment)
                .Length(0, 100).WithMessage("Must be between 1-100 characters.");
        }

        public static BeerModelValidator Create()
        {
            return new BeerModelValidator();
        }
    }
}
