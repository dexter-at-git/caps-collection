using System;
using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Validators;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.ViewModels;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Views;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace CapsCollection.Desktop.Tests.BulkUpdateModule
{
    [TestClass]
    public class BeerUpdateViewModelValidatorTests
    {
        private Mock<IBeerUpdateView> _beerUpdateViewMock = new Mock<IBeerUpdateView>();
        private Mock<IEventAggregator> _eventAggregatorMock = new Mock<IEventAggregator>();
        private Mock<IValidatorFactory> _validatorFactoryMock = new Mock<IValidatorFactory>();

        private IValidator<BeerUpdateViewModel> _validator = new BeerUpdateViewModelValidator();
        private BeerUpdateViewModelValidator _beerUpdateViewModelValidator = new BeerUpdateViewModelValidator();

        private BeerUpdateViewModel _beerUpdateViewModel;
        private string moreFiftyChars = "LoremIpsumissimplydummytextoftheprintingandtypesettingindu";
        private string moreHundredChars = "LoremIpsumissimplydummytextoftheprintingandtypesettinginduLoremIpsumissimplydummytextoftheprintingandtypesettingindu";

        [TestInitialize]
        public void TestInitialize()
        {
            _validatorFactoryMock.Setup(x => x.GetValidator<BeerUpdateViewModel>()).Returns(_validator);

            _beerUpdateViewModel = new BeerUpdateViewModel(_beerUpdateViewMock.Object, _eventAggregatorMock.Object, _validatorFactoryMock.Object);
        }


        [TestMethod]
        public void BeerUpdateViewModel_SelectionsChecks_Invalid()
        {
            _beerUpdateViewModel.SelectedBrewery = new BreweryDto();
            _beerUpdateViewModel.SelectedCapType = new CapTypeDto();
            _beerUpdateViewModel.SelectedBeerStyle = new BeerStyleDto();

            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBrewery, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedCapType, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBeerStyle, _beerUpdateViewModel);

            _beerUpdateViewModel.SelectedBrewery = new BreweryDto() { BreweryId = -1 };
            _beerUpdateViewModel.SelectedCapType = new CapTypeDto() { CapTypeId = -1 };
            _beerUpdateViewModel.SelectedBeerStyle = new BeerStyleDto() { BeerStyleId = -1 };

            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBrewery, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedCapType, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBeerStyle, _beerUpdateViewModel);

            _beerUpdateViewModel.SelectedBrewery = new BreweryDto() { BreweryId = 0 };
            _beerUpdateViewModel.SelectedCapType = new CapTypeDto() { CapTypeId = 0 };
            _beerUpdateViewModel.SelectedBeerStyle = new BeerStyleDto() { BeerStyleId = 0 };

            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBrewery, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedCapType, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBeerStyle, _beerUpdateViewModel);
        }


        [TestMethod]
        public void BeerUpdateViewModel_SelectionsChecks_Valid()
        {
            _beerUpdateViewModel.SelectedBrewery = new BreweryDto() { BreweryId = 10 };
            _beerUpdateViewModel.SelectedCapType = new CapTypeDto() { CapTypeId = 10 };
            _beerUpdateViewModel.SelectedBeerStyle = new BeerStyleDto() { BeerStyleId = 10 };

            _beerUpdateViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.SelectedBrewery, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.SelectedCapType, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.SelectedBeerStyle, _beerUpdateViewModel);
        }


        [TestMethod]
        public void BeerUpdateViewModel_StringChecks_Invalid()
        {
            _beerUpdateViewModel.BeerName = String.Empty;

            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerName, _beerUpdateViewModel);

            _beerUpdateViewModel.BeerName = moreFiftyChars;

            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerName, _beerUpdateViewModel);

            _beerUpdateViewModel.BeerName = moreFiftyChars;
            _beerUpdateViewModel.BeerType = moreFiftyChars;
            _beerUpdateViewModel.BeerSite = moreHundredChars;
            _beerUpdateViewModel.BeerComment = moreHundredChars;
            
            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerName, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerType, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerSite, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerComment, _beerUpdateViewModel);
        }


        [TestMethod]
        public void BeerUpdateViewModel_StringChecks_Valid()
        {
            _beerUpdateViewModel.BeerType = String.Empty;
            _beerUpdateViewModel.BeerSite = String.Empty;
            _beerUpdateViewModel.BeerComment = String.Empty;
            
            _beerUpdateViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerType, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerSite, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerComment, _beerUpdateViewModel);

            _beerUpdateViewModel.BeerName = "BeerName";
            _beerUpdateViewModel.BeerType = "BeerType";
            _beerUpdateViewModel.BeerSite = "BeerSite";
            _beerUpdateViewModel.BeerComment = "BeerComment";

            _beerUpdateViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerName, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerType, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerSite, _beerUpdateViewModel);
            _beerUpdateViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerComment, _beerUpdateViewModel);
        }
    }
}
