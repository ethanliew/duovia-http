using System;

namespace DuoVia.Http
{
    public static class Client
    {
        public static TInterface Create<TInterface>(Uri endpoint) where TInterface : class
        {
            var httpClient = new HttpClient<TInterface>(endpoint);
            return httpClient.Proxy;
        }
    }

    internal sealed class HttpClient<TInterface> where TInterface : class
    {
        private TInterface _proxy;

        public TInterface Proxy { get { return _proxy; } }

        public HttpClient(Uri endpoint)
        {
            _proxy = HttpProxy.CreateProxy<TInterface>(endpoint);
        }
    }
}
