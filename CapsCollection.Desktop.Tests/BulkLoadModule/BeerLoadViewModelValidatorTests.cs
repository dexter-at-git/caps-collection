using System;
using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.UI.Modules.BulkLoad.Validators;
using CapsCollection.Desktop.UI.Modules.BulkLoad.ViewModels;
using CapsCollection.Desktop.UI.Modules.BulkLoad.Views;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace CapsCollection.Desktop.Tests.BulkLoadModule
{
    [TestClass]
    public class BeerLoadViewModelValidatorTests
    {
        private Mock<IBeerLoadView> _beerLoadViewMock = new Mock<IBeerLoadView>();
        private Mock<IEventAggregator> _eventAggregatorMock = new Mock<IEventAggregator>();
        private Mock<IValidatorFactory> _validatorFactoryMock = new Mock<IValidatorFactory>();

        private IValidator<BeerLoadViewModel> _validator = new BeerLoadViewModelValidator();
        private BeerLoadViewModelValidator _beerLoadViewModelValidator = new BeerLoadViewModelValidator();

        private BeerLoadViewModel _beerLoadViewModel;
        private string moreFiftyChars = "LoremIpsumissimplydummytextoftheprintingandtypesettingindu";
        private string moreHundredChars = "LoremIpsumissimplydummytextoftheprintingandtypesettinginduLoremIpsumissimplydummytextoftheprintingandtypesettingindu";

        [TestInitialize]
        public void TestInitialize()
        {
            _validatorFactoryMock.Setup(x => x.GetValidator<BeerLoadViewModel>()).Returns(_validator);

            _beerLoadViewModel = new BeerLoadViewModel(_beerLoadViewMock.Object, _eventAggregatorMock.Object, _validatorFactoryMock.Object);
        }


        [TestMethod]
        public void BeerLoadViewModel_SelectionsChecks_Invalid()
        {
            _beerLoadViewModel.SelectedBrewery = new BreweryDto();
            _beerLoadViewModel.SelectedCapType = new CapTypeDto();
            _beerLoadViewModel.SelectedBeerStyle = new BeerStyleDto();

            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBrewery, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedCapType, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBeerStyle, _beerLoadViewModel);

            _beerLoadViewModel.SelectedBrewery = new BreweryDto() { BreweryId = -1 };
            _beerLoadViewModel.SelectedCapType = new CapTypeDto() { CapTypeId = -1 };
            _beerLoadViewModel.SelectedBeerStyle = new BeerStyleDto() { BeerStyleId = -1 };

            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBrewery, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedCapType, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBeerStyle, _beerLoadViewModel);

            _beerLoadViewModel.SelectedBrewery = new BreweryDto() { BreweryId = 0 };
            _beerLoadViewModel.SelectedCapType = new CapTypeDto() { CapTypeId = 0 };
            _beerLoadViewModel.SelectedBeerStyle = new BeerStyleDto() { BeerStyleId = 0 };

            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBrewery, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedCapType, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.SelectedBeerStyle, _beerLoadViewModel);
        }


        [TestMethod]
        public void BeerLoadViewModel_SelectionsChecks_Valid()
        {
            _beerLoadViewModel.SelectedBrewery = new BreweryDto() { BreweryId = 10 };
            _beerLoadViewModel.SelectedCapType = new CapTypeDto() { CapTypeId = 10 };
            _beerLoadViewModel.SelectedBeerStyle = new BeerStyleDto() { BeerStyleId = 10 };

            _beerLoadViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.SelectedBrewery, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.SelectedCapType, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.SelectedBeerStyle, _beerLoadViewModel);
        }


        [TestMethod]
        public void BeerLoadViewModel_StringChecks_Invalid()
        {
            _beerLoadViewModel.BeerName = String.Empty;

            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerName, _beerLoadViewModel);

            _beerLoadViewModel.BeerName = moreFiftyChars;

            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerName, _beerLoadViewModel);

            _beerLoadViewModel.BeerName = moreFiftyChars;
            _beerLoadViewModel.BeerType = moreFiftyChars;
            _beerLoadViewModel.BeerSite = moreHundredChars;
            _beerLoadViewModel.BeerComment = moreHundredChars;
            
            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerName, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerType, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerSite, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldHaveValidationErrorFor(x => x.BeerComment, _beerLoadViewModel);
        }


        [TestMethod]
        public void BeerLoadViewModel_StringChecks_Valid()
        {
            _beerLoadViewModel.BeerType = String.Empty;
            _beerLoadViewModel.BeerSite = String.Empty;
            _beerLoadViewModel.BeerComment = String.Empty;
            
            _beerLoadViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerType, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerSite, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerComment, _beerLoadViewModel);

            _beerLoadViewModel.BeerName = "BeerName";
            _beerLoadViewModel.BeerType = "BeerType";
            _beerLoadViewModel.BeerSite = "BeerSite";
            _beerLoadViewModel.BeerComment = "BeerComment";

            _beerLoadViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerName, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerType, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerSite, _beerLoadViewModel);
            _beerLoadViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BeerComment, _beerLoadViewModel);
        }
    }
}
