using System;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using Microsoft.Practices.Unity;

namespace CapsCollection.Web.ServiceHost.ServiceBehaviors
{
    public class UnityInstanceProvider : IInstanceProvider
    {
        #region Members

        private readonly Type _serviceType;
        private readonly IUnityContainer _container;

        #endregion


        #region Constructor

        public UnityInstanceProvider(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            _serviceType = serviceType;
            _container = UnityContainer.Current;
        }

        #endregion


        #region IInstance Provider Members

        public object GetInstance(InstanceContext instanceContext, System.ServiceModel.Channels.Message message)
        {
            return _container.Resolve(_serviceType);
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            if (instance is IDisposable)
                ((IDisposable)instance).Dispose();
        }

        #endregion
    }
}
