using System;
using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.StatusBar.Resources;
using CapsCollection.Desktop.UI.Modules.StatusBar.ViewModels;
using CapsCollection.Desktop.UI.Modules.StatusBar.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace CapsCollection.Desktop.Tests.StatusBarModule
{
    [TestClass]
    public class StatusBarViewModelTests
    {
        private StatusBarViewModel _statusBarViewModel;
        private Mock<IStatusBarView> _statusBarView = new Mock<IStatusBarView>();
        private Mock<IEventAggregator> _eventAggregatorMock = new Mock<IEventAggregator>();
        
        private readonly Mock<BeerLoadingStatusEvent> _loadingStatusEventMock = new Mock<BeerLoadingStatusEvent>();
        private Action<string> _loadingStatusCallback;

        private Mock<BeerErrorEvent> _errorEventMock = new Mock<BeerErrorEvent>();
        private Action<BeerErrorEventArgs> _errorCallback;

        private Mock<BeerLoadingInProgressEvent> _progressEventMock = new Mock<BeerLoadingInProgressEvent>();
        private Action<LoadingProgress> _progressCallback;
        

        [TestInitialize]
        public void TestInitialize()
        {
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerLoadingStatusEvent>()).Returns(_loadingStatusEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerErrorEvent>()).Returns(_errorEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerLoadingInProgressEvent>()).Returns(_progressEventMock.Object);
            
            _loadingStatusEventMock.Setup(x => x.Subscribe(It.IsAny<Action<string>>(),It.IsAny<ThreadOption>(),It.IsAny<bool>(),It.IsAny<Predicate<string>>()))
                                     .Callback<Action<string>, ThreadOption, bool, Predicate<string>>((e, t, b, a) => _loadingStatusCallback = e);

            _errorEventMock.Setup(x => x.Subscribe(It.IsAny<Action<BeerErrorEventArgs>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<BeerErrorEventArgs>>()))
                                     .Callback<Action<BeerErrorEventArgs>, ThreadOption, bool, Predicate<BeerErrorEventArgs>>((e, t, b, a) => _errorCallback = e);

            _progressEventMock.Setup(x => x.Subscribe(It.IsAny<Action<LoadingProgress>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<LoadingProgress>>()))
                                     .Callback<Action<LoadingProgress>, ThreadOption, bool, Predicate<LoadingProgress>>((e, t, b, a) => _progressCallback = e);

            _statusBarViewModel = new StatusBarViewModel(_statusBarView.Object, _eventAggregatorMock.Object);
        }
        

        [TestMethod]
        public void StatusBarViewModel_ConstructorInitialize()
        {
            Assert.IsNotNull(_statusBarViewModel);
            Assert.AreEqual(StatusBarModuleStrings.Ready, _statusBarViewModel.Message);
            Assert.AreEqual(0, _statusBarViewModel.CurrentProgress);
            Assert.AreEqual(0, _statusBarViewModel.MaximumProgress);
            Assert.IsFalse(_statusBarViewModel.IsLoading);
        }


        [TestMethod]
        public void StatusBarViewModel_OnStatusMessageRecieved()
        {
            var message = "loading";

            _loadingStatusCallback.Invoke(message);

            Assert.AreEqual(message, _statusBarViewModel.Message);
        }


        [TestMethod]
        public void StatusBarViewModel_OnErrorMessageRecieved()
        {
            var error = new BeerErrorEventArgs() { Message = "message", UserMessage = "user message"};

            _errorCallback.Invoke(error);

            StringAssert.Contains(_statusBarViewModel.Message, error.UserMessage);
            StringAssert.Contains(_statusBarViewModel.Message, error.Message);
        }


        [TestMethod]
        public void StatusBarViewModel_OnProgressChangeRecieved_InProgress()
        {
            var progress = new LoadingProgress() { CurrentProgress = 5, MaximumProgress = 20};

            _progressCallback.Invoke(progress);

            Assert.IsTrue(_statusBarViewModel.IsLoading);
            Assert.AreEqual(progress.CurrentProgress, _statusBarViewModel.CurrentProgress);
            Assert.AreEqual(progress.MaximumProgress, _statusBarViewModel.MaximumProgress);
        }
        

        [TestMethod]
        public void StatusBarViewModel_OnProgressChangeRecieved_Finished()
        {
            var progress = new LoadingProgress() { CurrentProgress = 20, MaximumProgress = 20 };

            _progressCallback.Invoke(progress);

            Assert.IsFalse(_statusBarViewModel.IsLoading);
            Assert.AreEqual(0, _statusBarViewModel.CurrentProgress);
            Assert.AreEqual(progress.MaximumProgress, _statusBarViewModel.MaximumProgress);
            StringAssert.Contains(_statusBarViewModel.Message, StatusBarModuleStrings.LoadingTime);
        }
    }
}
