using System;
using System.Linq;
using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.UI.Modules.BulkLoad.Validators;
using CapsCollection.Desktop.UI.Modules.BulkLoad.ViewModels;
using CapsCollection.Desktop.UI.Modules.BulkLoad.Views;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace CapsCollection.Desktop.Tests.BulkLoadModule
{
    [TestClass]
    public class BeerLoadViewModelTest : TestBase
    {
        private Mock<IBeerLoadView> _beerLoadViewMock = new Mock<IBeerLoadView>();
        private Mock<IEventAggregator> _eventAggregatorMock = new Mock<IEventAggregator>();
        private Mock<IValidatorFactory> _validatorFactoryMock = new Mock<IValidatorFactory>();

        private Mock<BeerSavingEvent> _savingEventMock = new Mock<BeerSavingEvent>();

        private BeerLoadViewModel _beerLoadViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _validatorFactoryMock.Setup(x => x.GetValidator<BeerLoadViewModel>()).Returns(new BeerLoadViewModelValidator());
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerSavingEvent>()).Returns(_savingEventMock.Object);
            
            _beerLoadViewModel = new BeerLoadViewModel(_beerLoadViewMock.Object, _eventAggregatorMock.Object, _validatorFactoryMock.Object);
        }

        [TestMethod]
        public void BeerLoadViewModel_ConstructorInitialize()
        {
            Assert.IsNotNull(_beerLoadViewModel.SaveBeerCommand);
        }


        [TestMethod]
        public void BeerLoadViewModel_SaveEvent()
        {
            _beerLoadViewModel.BeerName = String.Empty;
            _beerLoadViewModel.SelectedBrewery = new BreweryDto();
            _beerLoadViewModel.SelectedBeerStyle = new BeerStyleDto();
            _beerLoadViewModel.SelectedCapType = new CapTypeDto();
            _beerLoadViewModel.SelectedCountry = new CountryDto();

            Assert.IsTrue(_beerLoadViewModel.HasErrors);
            Assert.IsFalse(_beerLoadViewModel.SaveBeerCommand.CanExecute(_beerLoadViewModel));
        }
        

        [TestMethod]
        public void BeerLoadViewModel_PrepareViewModel()
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

            var beerViewModel = _beerLoadViewModel.PrepareViewModel(beerLoadDataEventArgs);

            Assert.IsTrue(beerViewModel.HasErrors);
            Assert.IsFalse(beerViewModel.SaveBeerCommand.CanExecute(beerViewModel));
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

            var beerViewModel = _beerLoadViewModel.PrepareViewModel(beerLoadDataEventArgs);

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
