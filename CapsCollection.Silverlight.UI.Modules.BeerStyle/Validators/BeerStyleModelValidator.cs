using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;
using FluentValidation;

namespace CapsCollection.Silverlight.UI.Modules.BeerStyle.Validators
{
    public class BeerStyleModelValidator : AbstractValidator<BeerStyleDto>
    {
        private BeerStyleModelValidator()
        {
            RuleFor(x => x.BeerStyleName)
                .NotEmpty().WithMessage("Beer style name cannot be empty");
        }

        public static BeerStyleModelValidator Create()
        {
            return new BeerStyleModelValidator();
        }
    }
}
