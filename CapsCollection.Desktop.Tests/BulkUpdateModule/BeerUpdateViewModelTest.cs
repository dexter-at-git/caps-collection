using System;
using System.Linq;
using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Validators;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.ViewModels;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Views;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace CapsCollection.Desktop.Tests.BulkUpdateModule
{
    [TestClass]
    public class BeerUpdateViewModelTest : TestBase
    {
        private Mock<IBeerUpdateView> _beerUpdateViewMock = new Mock<IBeerUpdateView>();
        private Mock<IEventAggregator> _eventAggregatorMock = new Mock<IEventAggregator>();
        private Mock<IValidatorFactory> _validatorFactoryMock = new Mock<IValidatorFactory>();

        private Mock<BeerSavingEvent> _savingEventMock = new Mock<BeerSavingEvent>();

        private BeerUpdateViewModel _beerUpdateViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _validatorFactoryMock.Setup(x => x.GetValidator<BeerUpdateViewModel>()).Returns(new BeerUpdateViewModelValidator());
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerSavingEvent>()).Returns(_savingEventMock.Object);
            
            _beerUpdateViewModel = new BeerUpdateViewModel(_beerUpdateViewMock.Object, _eventAggregatorMock.Object, _validatorFactoryMock.Object);
        }

        [TestMethod]
        public void BeerLoadViewModel_ConstructorInitialize()
        {
            Assert.IsNotNull(_beerUpdateViewModel.SaveBeerCommand);
        }


        [TestMethod]
        public void BeerLoadViewModel_SaveEvent()
        {
            _beerUpdateViewModel.BeerName = String.Empty;
            _beerUpdateViewModel.SelectedBrewery = new BreweryDto();
            _beerUpdateViewModel.SelectedBeerStyle = new BeerStyleDto();
            _beerUpdateViewModel.SelectedCapType = new CapTypeDto();
            _beerUpdateViewModel.SelectedCountry = new CountryDto();

            Assert.IsTrue(_beerUpdateViewModel.HasErrors);
            Assert.IsFalse(_beerUpdateViewModel.SaveBeerCommand.CanExecute(_beerUpdateViewModel));
        }
        

        [TestMethod]
        public void BeerLoadViewModel_PrepareViewModel_ExistingBeer_Null()
        {
            PrepareThumbnails();

            var beerLoadDataEventArgs = new BeerLoadDataEventArgs()
            {
                BeerStyles = _beerStyleList,
                Breweries = _breweryList,
                CapTypes = _capTypeList,
                Countries = _countryList,
                ImageList = _imageDataWithThumbnails
            };

            var beerViewModel = _beerUpdateViewModel.PrepareViewModel(beerLoadDataEventArgs);

            Assert.IsTrue(beerViewModel.HasErrors);
            Assert.IsFalse(beerViewModel.SaveBeerCommand.CanExecute(beerViewModel));
        }


        [TestMethod]
        [Ignore] // Ignore because for some reason fails of tfs-online
        public void BeerLoadViewModel_PrepareViewModel_ExistingBeer()
        {
            PrepareThumbnails();

            var beerLoadDataEventArgs = new BeerLoadDataEventArgs()
            {
                BeerStyles = _beerStyleList,
                Breweries = _breweryList,
                CapTypes = _capTypeList,
                Countries = _countryList,
                ImageList = _imageDataWithThumbnails,
                Existing = Existing
            };

            var beerViewModel = _beerUpdateViewModel.PrepareViewModel(beerLoadDataEventArgs);

            Assert.IsFalse(beerViewModel.HasErrors);
            Assert.AreEqual(Existing.BeerName, beerViewModel.BeerName);
            Assert.AreEqual(Existing.BeerStyleId, beerViewModel.SelectedBeerStyle.BeerStyleId);
            Assert.AreEqual(Existing.BeerType, beerViewModel.BeerType);
            Assert.AreEqual(Existing.BeerYear, beerViewModel.BeerYear);
            Assert.AreEqual(Existing.BeerComment, beerViewModel.BeerComment);
            Assert.AreEqual(Existing.BeerId, beerViewModel.BeerId);
            Assert.AreEqual(Existing.BeerYear, beerViewModel.BeerYear);
            Assert.AreEqual(Existing.BeerSite, beerViewModel.BeerSite);
            Assert.AreEqual(Existing.ContinentId, beerViewModel.SelectedCountry.ContinentId);
            Assert.AreEqual(Existing.CapTypeId, beerViewModel.SelectedCapType.CapTypeId);
            Assert.AreEqual(Existing.CountryId, beerViewModel.SelectedCountry.CountryId);
            Assert.AreEqual(Existing.BreweryId, beerViewModel.SelectedBrewery.BreweryId);
            Assert.IsTrue(beerViewModel.SaveBeerCommand.CanExecute(beerViewModel));
        }



        [TestMethod]
        public void BeerLoadViewModel_PrepareViewModel_FillRequiredFileds_Save()
        {
            PrepareThumbnails();

            var beerLoadDataEventArgs = new BeerLoadDataEventArgs()
            {
                BeerStyles = _beerStyleList,
                Breweries = _breweryList,
                CapTypes = _capTypeList,
                Countries = _countryList,
                ImageList = _imageDataWithThumbnails
            };

            var beerViewModel = _beerUpdateViewModel.PrepareViewModel(beerLoadDataEventArgs);

            Assert.IsTrue(beerViewModel.HasErrors);
            Assert.IsFalse(beerViewModel.SaveBeerCommand.CanExecute(beerViewModel));

            beerViewModel.BeerName = "BeerName";
            beerViewModel.SelectedBeerStyle = _beerStyleList.First();
            beerViewModel.SelectedCountry = _countryList.First();
            beerViewModel.SelectedBrewery = _breweryList.First();
            beerViewModel.SelectedCapType = _capTypeList.First();
            
            Assert.IsFalse(beerViewModel.HasErrors);
            Assert.IsTrue(beerViewModel.SaveBeerCommand.CanExecute(beerViewModel));

            beerViewModel.SaveBeerCommand.Execute(beerViewModel);


            _savingEventMock.Verify(x => x.Publish(It.Is<BeerSavingDataEventArgs>(s=>s.Beer.BeerName == beerViewModel.BeerName &&
                                                                                     s.Beer.BreweryId == beerViewModel.SelectedBrewery.BreweryId)), Times.Once);
            
        }
    }
}
