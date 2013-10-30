using System;

namespace DuoVia.Http
{
    public sealed class Client<TInterface> : IDisposable where TInterface : class
    {
        private TInterface _proxy;

        public TInterface Proxy { get { return _proxy; } }

        public Client(Uri endpoint)
        {
            _proxy = HttpProxy.CreateProxy<TInterface>(endpoint);
        }

        #region IDisposable Members

        private bool _disposed = false;

        public void Dispose()
        {
            //MS recommended dispose pattern - prevents GC from disposing again
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true; //prevent second call to Dispose
                if (disposing)
                {
                    (_proxy as HttpChannel).Dispose();
                }
            }
        }

        #endregion
    }
}
