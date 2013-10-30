using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace DuoVia.Http.Host
{
    // Note: By default all requests go through this OWIN pipeline. Alternatively you can turn this 
    // off by adding an appSetting owin:AutomaticAppStartup with value “false”. 
    // With this turned off you can still have OWIN apps listening on specific routes by adding routes 
    // in global.asax file using MapOwinPath or MapOwinRoute extensions on RouteTable.Routes
    public class Startup
    {
        ServiceHost host = new ServiceHost();

        public void Configuration(IAppBuilder app)
        {
            app.Run(Invoke);
        }

        // Invoked once per request.
        public Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.HasValue)
            {
                if (context.Request.Path.Value.StartsWith("/app") && context.Request.Method == "POST")
                {
                    return host.HandleOperationRequest(context);
                }
                else if (context.Request.Path.Value.StartsWith("/metadata"))
                {
                    return host.HandleMetadataRequest(context);
                }
            }
            return host.Handle404Request(context);
        }
    }
}
