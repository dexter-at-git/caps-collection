using System;
using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.UI.Modules.Home.Validators;
using CapsCollection.Desktop.UI.Modules.Home.ViewModels;
using CapsCollection.Desktop.UI.Modules.Home.Views;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentValidation.TestHelper;
using Prism.Events;

namespace CapsCollection.Desktop.Tests.HomeModule
{
    [TestClass]
    public class HomeViewModelValidatorTests : TestBase
    {
        private HomeViewModelValidator _homeViewModelValidator = new HomeViewModelValidator();

        private Mock<IHomeView> _homeViewMock = new Mock<IHomeView>();
        private Mock<IEventAggregator> _eventAggregatorMock = new Mock<IEventAggregator>();
        private Mock<IValidatorFactory> _validatorFactoryMock = new Mock<IValidatorFactory>();
        private Mock<IImageTypeAggregator> _imageTypeAggragator = new Mock<IImageTypeAggregator>();

        private IValidator<HomeViewModel> _validator = new HomeViewModelValidator();

        private readonly Mock<BusyEvent> _busyEventMock = new Mock<BusyEvent>();

        private Mock<ImagesProcessingEvent> _imageProcessingEventMock = new Mock<ImagesProcessingEvent>();

        private HomeViewModel _homeViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _validatorFactoryMock.Setup(x => x.GetValidator<HomeViewModel>()).Returns(_validator);
            _imageTypeAggragator.Setup(x => x.CombineImages()).Returns(_combinedImages);
            _eventAggregatorMock.Setup(x => x.GetEvent<BusyEvent>()).Returns(_busyEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ImagesProcessingEvent>()).Returns(_imageProcessingEventMock.Object);

            _homeViewModel = new HomeViewModel(_homeViewMock.Object, _eventAggregatorMock.Object, _validatorFactoryMock.Object, _imageTypeAggragator.Object);
        }


        [TestMethod]
        public void HomeViewModelValidator_ValidPath_Ok()
        {
            _homeViewModel.BottlesLookupPath = _bottlesPath;
            _homeViewModel.CapsLookupPath = _bottlesPath;
            _homeViewModel.LabelsLookupPath = _bottlesPath;

            _homeViewModelValidator.ShouldNotHaveValidationErrorFor(x=>x.BottlesLookupPath, _homeViewModel);
            _homeViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.CapsLookupPath, _homeViewModel);
            _homeViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.LabelsLookupPath, _homeViewModel);
        }


        [TestMethod]
        public void HomeViewModelValidator_EmptyPath_Ok()
        {
            _homeViewModel.BottlesLookupPath = String.Empty;
            _homeViewModel.CapsLookupPath = String.Empty;
            _homeViewModel.LabelsLookupPath = String.Empty;

            _homeViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.BottlesLookupPath, _homeViewModel);
            _homeViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.CapsLookupPath, _homeViewModel);
            _homeViewModelValidator.ShouldNotHaveValidationErrorFor(x => x.LabelsLookupPath, _homeViewModel);
        }


        [TestMethod]
        public void HomeViewModelValidator_InvalidPath_Error()
        {
            _homeViewModel.BottlesLookupPath = _fakePath;
            _homeViewModel.CapsLookupPath = _fakePath;
            _homeViewModel.LabelsLookupPath = _fakePath;

            _homeViewModelValidator.ShouldHaveValidationErrorFor(x => x.BottlesLookupPath, _homeViewModel);
            _homeViewModelValidator.ShouldHaveValidationErrorFor(x => x.CapsLookupPath, _homeViewModel);
            _homeViewModelValidator.ShouldHaveValidationErrorFor(x => x.LabelsLookupPath, _homeViewModel);
        }
    }
}
