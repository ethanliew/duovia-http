using System;
using DuoVia.Http.TestShared;

namespace DuoVia.Http.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://localhost:12345";
            var client = HttpClient.Create<IMyTest>(new HttpServiceEndPoint(url));
            var name = client.GetName("heyhey");
            var thing = client.DoSomething("today");

            Console.WriteLine(name);
            Console.WriteLine(thing);

            Console.WriteLine("Press Any Key To Exit");
            Console.ReadKey();
        }
    }
}
