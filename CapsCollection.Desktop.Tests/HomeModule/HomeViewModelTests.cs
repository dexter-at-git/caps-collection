using System;
using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.UI.Modules.Home.Validators;
using CapsCollection.Desktop.UI.Modules.Home.ViewModels;
using CapsCollection.Desktop.UI.Modules.Home.Views;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace CapsCollection.Desktop.Tests.HomeModule
{
    [TestClass]
    public class HomeViewModelTests : TestBase
    {
        private HomeViewModel _homeViewModel;
        private Mock<IHomeView> _homeViewMock = new Mock<IHomeView>();
        private Mock<IEventAggregator> _eventAggregatorMock = new Mock<IEventAggregator>();
        private Mock<IValidatorFactory> _validatorFactoryMock = new Mock<IValidatorFactory>();
        private Mock<IImageTypeAggregator> _imageTypeAggragator = new Mock<IImageTypeAggregator>();


        private IValidator<HomeViewModel> _validator = new HomeViewModelValidator();

        private readonly Mock<BusyEvent> _busyEventMock = new Mock<BusyEvent>();
        private Action<bool> _busyCallback;

        private Mock<ImagesProcessingEvent> _imageProcessingEventMock = new Mock<ImagesProcessingEvent>();

        [TestInitialize]
        public void TestInitialize()
        {
            _validatorFactoryMock.Setup(x => x.GetValidator<HomeViewModel>()).Returns(_validator);
            _imageTypeAggragator.Setup(x => x.CombineImages()).Returns(_combinedImages);
            _eventAggregatorMock.Setup(x => x.GetEvent<BusyEvent>()).Returns(_busyEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ImagesProcessingEvent>()).Returns(_imageProcessingEventMock.Object);


            _busyEventMock.Setup(x => x.Subscribe(It.IsAny<Action<bool>>(),
                                                             It.IsAny<ThreadOption>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<Predicate<bool>>()))
                                     .Callback<Action<bool>, ThreadOption, bool, Predicate<bool>>(
                                                (e, t, b, a) => _busyCallback = e);

            _homeViewModel = new HomeViewModel(_homeViewMock.Object, _eventAggregatorMock.Object, _validatorFactoryMock.Object, _imageTypeAggragator.Object);
        }


        [TestMethod]
        public void HomeViewModel_ConstructorInitialize()
        {
            Assert.IsNotNull(_homeViewModel);
            Assert.IsNotNull(_homeViewModel.ProcessImagesCommand);
            //Assert.IsNotNull(_homeViewModel.ImageTypeStatistics);
            Assert.IsFalse(_homeViewModel.IsBusy);
            //Assert.IsFalse(_homeViewModel.HasErrors);
        }


        [TestMethod]
        public void HomeViewModel_ValidLookupPathes_NoErrorsCanExecute()
        {
            TestValidLookups(_bottlesPath, _capsPath, _labelsPath);

            TestValidLookups(_bottlesPath, _capsPath, String.Empty);
            TestValidLookups(_bottlesPath, String.Empty, _labelsPath);
            TestValidLookups(_bottlesPath, String.Empty, String.Empty);

            TestValidLookups(String.Empty, _capsPath, _labelsPath);
            TestValidLookups(String.Empty, _capsPath, String.Empty);

            TestValidLookups(_bottlesPath, String.Empty, _labelsPath);
            TestValidLookups(String.Empty, String.Empty, _labelsPath);
        }

        private void TestValidLookups(string bottlePath, string capPath, string labelPath)
        {
            _homeViewModel.BottlesLookupPath = bottlePath;
            _homeViewModel.CapsLookupPath = capPath;
            _homeViewModel.LabelsLookupPath = labelPath;

            bool canExecute = _homeViewModel.ProcessImagesCommand.CanExecute();

            Assert.IsTrue(canExecute);
            Assert.IsFalse(_homeViewModel.HasErrors);
        }


        [TestMethod]
        public void HomeViewModel_InvalidLookupPathes_HasErrorsCannotExecute()
        {
            TestInvalidLookups(_fakePath, _capsPath, _labelsPath);
            TestInvalidLookups(_bottlesPath, _fakePath, _labelsPath);
            TestInvalidLookups(_bottlesPath, _capsPath, _fakePath);
        }

        private void TestInvalidLookups(string bottlePath, string capPath, string labelPath)
        {
            _homeViewModel.BottlesLookupPath = bottlePath;
            _homeViewModel.CapsLookupPath = capPath;
            _homeViewModel.LabelsLookupPath = labelPath;

            bool canExecute = _homeViewModel.ProcessImagesCommand.CanExecute();

            Assert.IsFalse(canExecute);
            Assert.IsTrue(_homeViewModel.HasErrors);
        }


        [TestMethod]
        public void HomeViewModel_EmptyLookupPathes_CannotExecute()
        {
            _homeViewModel.BottlesLookupPath = String.Empty;
            _homeViewModel.CapsLookupPath = String.Empty;
            _homeViewModel.LabelsLookupPath = String.Empty;

            bool canExecute = _homeViewModel.ProcessImagesCommand.CanExecute();

            Assert.IsFalse(canExecute);
        }


        [TestMethod]
        public void HomeViewModel_WhenImageProcessingExecuted_EventMustBeCalledImageProcessingDataShouldBePublished()
        {
            _homeViewModel.ProcessImagesCommand.Execute();

            _imageTypeAggragator.Verify(x => x.CombineImages(), Times.Once);
            _imageProcessingEventMock.Verify(x => x.Publish(It.IsAny<ImageProcessingDataEventArgs>()), Times.Once, "Images must be send to processing only once.");
        }


        [TestMethod]
        public void HomeViewModel_OnBusyStatusRecieved()
        {
            _busyCallback.Invoke(true);
            Assert.IsTrue(_homeViewModel.IsBusy);

            _busyCallback.Invoke(false);
            Assert.IsFalse(_homeViewModel.IsBusy);
        }
    }
}
