using System;
using DuoVia.Http.Host;
using DuoVia.Http.TestShared;
using Microsoft.Owin.Hosting;
using Owin;

namespace DuoVia.Http.HostConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = "http://localhost:12345/";
            var options = new StartOptions(baseUrl);
            using (WebApp.Start<Startup>(options))
            {
                // Launch the browser
                //Process.Start(baseUrl + "dv/metadata");

                // Keep the server going until we're done
                Console.WriteLine("Press Any Key To Exit");
                Console.ReadKey();
            }
        }
    }

    internal class Startup
    {
        // Invoked once at startup to configure your application.
        public void Configuration(IAppBuilder app)
        {
            var test = new MyTest();
            app.Host<IMyTest>(test);
        }
    }

    public class MyTest : IMyTest
    {
        public string GetName(string key)
        {
            return "me " + key;
        }

        public string DoSomething(string work)
        {
            return "w " + work;
        }
    }
}
