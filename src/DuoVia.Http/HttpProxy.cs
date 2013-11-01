namespace DuoVia.Http
{
    internal static class HttpProxy
    {
        public static TInterface CreateProxy<TInterface>(HttpServiceEndPoint endpoint) where TInterface : class
        {
            return ProxyFactory.CreateProxy<TInterface>(typeof(HttpChannel), typeof(HttpServiceEndPoint), endpoint);
        }
    }
}
