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
            var client = HttpClient.Create<IMyTest>(new Uri("http://localhost:12345/"));
            var name = client.GetName("heyhey");
            var sot = client.DoSomething("today");

            Console.WriteLine(name);
            Console.WriteLine(sot);

            Console.WriteLine("Press Any Key To Exit");
            Console.ReadKey();
        }
    }
}
