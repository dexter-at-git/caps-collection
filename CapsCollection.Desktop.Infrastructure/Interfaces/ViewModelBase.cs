using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using CapsCollection.Desktop.Infrastructure.Models;
using Prism.Mvvm;

namespace CapsCollection.Desktop.Infrastructure.Interfaces
{
    public class ViewModelBase : BindableBase, IViewModel, INotifyDataErrorInfo
    {
        public IView View { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                base.SetProperty(ref _isBusy, value);
                OnPropertyChanged(() => IsBusy);
            }
        }
        
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private readonly ConcurrentDictionary<string, List<ErrorInfo>> _currentErrors = new ConcurrentDictionary<string, List<ErrorInfo>>();

        public ViewModelBase(IView view)
        {
            View = view;
            View.ViewModel = this;
        }

        public IEnumerable GetErrors([CallerMemberName]string propertyName = "")
        {
            List<ErrorInfo> propertyErrors = new List<ErrorInfo>();
            if (!String.IsNullOrEmpty(propertyName))
            {
                _currentErrors.TryGetValue(propertyName, out propertyErrors);
            }
            return propertyErrors;
        }

        public bool HasErrors
        {
            get { return _currentErrors.Count > 0; }
        }
        
        public bool CheckValidProperty([CallerMemberName]string propertyName = "")
        {
            List<ErrorInfo> propertyErrors = new List<ErrorInfo>();
            if (!String.IsNullOrEmpty(propertyName))
            {
                _currentErrors.TryGetValue(propertyName, out propertyErrors);
            }
            return propertyErrors.Count > 0;
        }
        
        private void FireErrorsChanged(string property)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(property));
            }
        }

        protected void ClearErrorFromProperty(string property)
        {
            List<ErrorInfo> existingErrors;
            _currentErrors.TryRemove(property, out existingErrors);

            FireErrorsChanged(property);
        }

        protected void AddErrorForProperty(string property, ErrorInfo error)
        {
            var results = new List<ErrorInfo>();
            _currentErrors.AddOrUpdate(property, new List<ErrorInfo>() { error },
                    (key, existingVal) => { return new List<ErrorInfo>() { error }; });

            FireErrorsChanged(property);
        }
    }
}
