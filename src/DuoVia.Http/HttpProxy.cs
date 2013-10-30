using System;

namespace DuoVia.Http
{
    internal static class HttpProxy
    {
        public static TInterface CreateProxy<TInterface>(Uri endpoint) where TInterface : class
        {
            return ProxyFactory.CreateProxy<TInterface>(typeof(HttpChannel), typeof(Uri), endpoint);
        }
    }
}
