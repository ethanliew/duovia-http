namespace DuoVia.Http
{
    public static class HttpClient
    {
        public static TInterface Create<TInterface>(HttpServiceEndPoint endpoint) where TInterface : class
        {
            var proxy = HttpProxy.CreateProxy<TInterface>(endpoint);
            return proxy;
        }
    }
}
