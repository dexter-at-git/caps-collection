using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CapsCollection.Silverlight.Infrastructure.Models;
using Microsoft.Practices.Prism.ViewModel;

namespace CapsCollection.Silverlight.Infrastructure.ViewModels
{
    public class ViewModelBase : NotificationObject, INotifyDataErrorInfo
    {
        bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
            set
            {
                _isAuthenticated = value;
                RaisePropertyChanged(() => IsAuthenticated);
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private readonly Dictionary<string, List<ErrorInfo>> _currentErrors;
        
        public ViewModelBase()
        {

            _currentErrors = new Dictionary<string, List<ErrorInfo>>();
        }

        #region INotifyDataErrorInfo methods

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return (_currentErrors.Values);
            }

            MakeOrCreatePropertyErrorList(propertyName);
            return (_currentErrors[propertyName]);
        }

        public bool HasErrors
        {
            get
            {
                return (_currentErrors.Values.Sum(x => x.Count) > 0);
            }
        }

        #endregion

        void MakeOrCreatePropertyErrorList(string propertyName)
        {
            if (!_currentErrors.ContainsKey(propertyName))
            {
                _currentErrors[propertyName] = new List<ErrorInfo>();
            }
        }

        void FireErrorsChanged(string property)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(property));
            }
        }

        protected void ClearErrorFromProperty(string property)
        {
            MakeOrCreatePropertyErrorList(property);

            ErrorInfo error = _currentErrors[property].SingleOrDefault();

            if (error != null)
            {
                _currentErrors[property].Remove(error);
                FireErrorsChanged(property);
            }
        }

        protected void AddErrorForProperty(string property, ErrorInfo error)
        {
            MakeOrCreatePropertyErrorList(property);

            if (_currentErrors[property].SingleOrDefault(e => e.ErrorCode == error.ErrorCode) == null)
            {
                _currentErrors[property].Add(error);
                FireErrorsChanged(property);
            }
        }
    }
}
