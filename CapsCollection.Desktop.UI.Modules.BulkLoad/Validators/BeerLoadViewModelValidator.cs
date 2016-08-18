using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.UI.Modules.BulkLoad.ViewModels;
using FluentValidation;

namespace CapsCollection.Desktop.UI.Modules.BulkLoad.Validators
{
    public class BeerLoadViewModelValidator : AbstractValidator<BeerLoadViewModel>
    {
        public BeerLoadViewModelValidator()
        {
            RuleFor(x => x.SelectedBrewery)
                .Must(CheckBrewery)
                .WithMessage("Brewery cannot be empty");

            RuleFor(x => x.SelectedBeerStyle)
                .Must(CheckBeerStyle)
                .WithMessage("Beer style cannot be empty");

            RuleFor(x => x.SelectedCapType)
                .Must(CheckCapType)
                .WithMessage("Cap type cannot be empty");

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

        private bool CheckBrewery(BreweryDto brewery)
        {
            if (brewery == null || brewery.BreweryId == -1 || brewery.BreweryId == 0)
            {
                return false;
            }
            return true;
        }

        private bool CheckCapType(CapTypeDto capType)
        {
            if (capType == null || capType.CapTypeId == -1 || capType.CapTypeId == 0)
            {
                return false;
            }
            return true;
        }

        private bool CheckBeerStyle(BeerStyleDto beerStyle)
        {
            if (beerStyle == null || beerStyle.BeerStyleId == -1 || beerStyle.BeerStyleId == 0)
            {
                return false;
            }
            return true;
        }
    }
}