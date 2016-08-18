using System;
using System.Collections.Generic;
using System.Windows.Navigation;

namespace CapsCollection.Silverlight.UI.Shell.Navigation
{
    public class SingletonContentLoader : INavigationContentLoader
    {
        static SingletonContentLoader()
        {
            InstantiatedUserControls = new Dictionary<string, object>();
        }

        private string GetTypeNameFromUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                uri = new Uri(new Uri("dummy:///", UriKind.Absolute), uri.OriginalString);
            return Uri.UnescapeDataString(uri.AbsolutePath.Substring(1));
        }

        #region INavigationContentLoader Members

        public bool CanLoad(Uri targetUri, Uri currentUri)
        {
            string typeName = GetTypeNameFromUri(targetUri);
            Type t = Type.GetType(typeName, false, true);
            if (t == null)
                return false;
            var defaultConstructor = t.GetConstructor(new Type[0]);
            if (defaultConstructor == null)
                return false;
            return true;
        }

        public static Dictionary<string, object> InstantiatedUserControls { get; set; }

        public IAsyncResult BeginLoad(Uri targetUri, Uri currentUri, AsyncCallback userCallback, object asyncState)
        {
            try
            {
                var result = new SingletonContentLoaderAsyncResult(asyncState);
                Type t = Type.GetType(GetTypeNameFromUri(targetUri), false, true);
                object instance = null;
                if (t != null)
                {
                    if (InstantiatedUserControls.ContainsKey(t.Name))
                    {
                        instance = InstantiatedUserControls[t.Name];
                    }
                    else
                    {
                        instance = Activator.CreateInstance(t);
                        InstantiatedUserControls[t.Name] = instance;
                    }
                }
                result.Result = instance;
                userCallback(result);

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public LoadResult EndLoad(IAsyncResult asyncResult)
        {
            return new LoadResult(((SingletonContentLoaderAsyncResult)asyncResult).Result);
        }

        public void CancelLoad(IAsyncResult asyncResult)
        {
            return;
        }

        #endregion
    }
}
