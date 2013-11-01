using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace DuoVia.Http.Host
{
    public class ServiceHostOwinMiddleware : OwinMiddleware
    {
        private string dvAppPath = "/dv/app";
        private string dvMetadataPath = "/dv/metadata";

        public ServiceHostOwinMiddleware(OwinMiddleware next, 
            string appPath = "/dv/app", string metadataPath = "/dv/metadata") : base(next)
        {
            this.dvAppPath = appPath ?? this.dvAppPath;
            this.dvMetadataPath = metadataPath ?? this.dvMetadataPath;
        }

        // Invoked once per request.
        public override Task Invoke(IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (context.Request.Path.HasValue)
            {
                if (context.Request.Path.Value.StartsWith(this.dvAppPath) && context.Request.Method == "POST")
                {
                    return ServiceHost.HandleOperationRequest(context);
                }
                if (context.Request.Path.Value.StartsWith(this.dvMetadataPath) && context.Request.Method == "GET")
                {
                    return ServiceHost.HandleMetadataRequest(context);
                }
            }

            //pass the context to the next middleware component
            return Next.Invoke(context);
        }
    }
}
