using System;
using System.Threading;

namespace CapsCollection.Silverlight.UI.Shell.Navigation
{
    public class SingletonContentLoaderAsyncResult : IAsyncResult
    {
        public object Result { get; set; }

        public SingletonContentLoaderAsyncResult(object asyncState)
        {
            this.AsyncState = asyncState;
            this.AsyncWaitHandle = new ManualResetEvent(true);
        }

        #region IAsyncResult Members

        public object AsyncState
        {
            get;
            private set;
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get;
            private set;
        }

        public bool CompletedSynchronously
        {
            get { return true; }
        }

        public bool IsCompleted
        {
            get { return true; }
        }

        #endregion
    }
}