using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DuoVia.Http.Host;
using DuoVia.Http.TestShared;
using Microsoft.Owin.Hosting;
using System.Collections;

namespace DuoVia.Http.HostConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = "http://localhost:12345/";

            var test = new MyTest();
            ServiceHost.AddService<IMyTest>(test);

            using (WebApp.Start<Startup>(new StartOptions(baseUrl) { ServerFactory = "Microsoft.Owin.Host.HttpListener" }))
            {
                // Launch the browser
                //Process.Start(baseUrl + "metadata");

                // Keep the server going until we're done
                Console.WriteLine("Press Any Key To Exit");
                Console.ReadKey();
            }
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
