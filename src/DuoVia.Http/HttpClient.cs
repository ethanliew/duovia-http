using System;

namespace DuoVia.Http
{
    public static class HttpClient
    {
        public static TInterface Create<TInterface>(Uri endpoint) where TInterface : class
        {
            var proxy = HttpProxy.CreateProxy<TInterface>(endpoint);
            return proxy;
        }
    }
}
