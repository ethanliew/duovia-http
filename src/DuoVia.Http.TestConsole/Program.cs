using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuoVia.Http.TestShared;

namespace DuoVia.Http.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new Client<IMyTest>(new Uri("http://localhost:12345/")))
            {
                var name = client.Proxy.GetName("heyhey");
                Console.WriteLine(name);
                var sot = client.Proxy.DoSomething("today");
                Console.WriteLine(sot);
            }

            Console.ReadKey();
        }
    }
}
