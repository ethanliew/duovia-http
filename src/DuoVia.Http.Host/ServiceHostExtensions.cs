using System;
using Owin;

namespace DuoVia.Http.Host
{
    public static class ServiceHostExtensions
    {
        public static IAppBuilder Host<TService>(this IAppBuilder app, TService service, 
            string dvAppPath = "/dv/app", string dvMetadataPath = "/dv/metadata") where TService : class
        {
            if (app == null) throw new ArgumentNullException("app");
            if (service == null) throw new ArgumentNullException("service");
            ServiceHost.AddService<TService>(service);
            return app.Use<ServiceHostOwinMiddleware>(dvAppPath, dvMetadataPath);
        }
    }
}
