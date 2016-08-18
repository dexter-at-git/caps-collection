using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using CapsCollection.Common.Settings;

namespace CapsCollection.Desktop.UI.Modules.Services.WcfService
{
    public class ServiceClientWrapper<TChannel> : ClientBase<TChannel>, IDisposable where TChannel : class
    {
        public ServiceClientWrapper()
        {
        }

        public ServiceClientWrapper(Binding binding, EndpointAddress endpointAddress) : base(binding, endpointAddress)
        {
            this.ClientCredentials.UserName.UserName = CapsCollectionEndpointSettings.ServiceCredentials.UserName;
            this.ClientCredentials.UserName.Password = CapsCollectionEndpointSettings.ServiceCredentials.Password;
        }

        public TChannel Client
        {
            get { return Channel; }
        }

        public new void Close()
        {
            ((IDisposable)this).Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (State != CommunicationState.Closed)
                    base.Close();
            }
            catch (CommunicationException)
            {
                base.Abort();
            }
            catch (TimeoutException)
            {
                base.Abort();
            }
            catch
            {
                base.Abort();
                throw;
            };
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
