using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using FluentValidation;

namespace CapsCollection.Silverlight.UI.Modules.Geography.Validators
{
    public class RegionModelValidator : AbstractValidator<RegionDto>
    {
        private RegionModelValidator()
        {
            RuleFor(x => x.EnglishRegionName)
                .NotEmpty().WithMessage("English region name cannot be empty")
                .Length(1, 100).WithMessage("Must be between 1-100 characters.");

            RuleFor(x => x.NationalRegionName)
                .NotEmpty().WithMessage("National region name cannot be empty")
                .Length(1, 100).WithMessage("Must be between 1-100 characters.");
        }

        public static RegionModelValidator Create()
        {
            return new RegionModelValidator();
        }
    }
}
