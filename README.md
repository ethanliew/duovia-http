duovia-http
==========

DuoVia.Http
----------
### An OWIN-based Lightweight Services Library for .NET.

#### Exciting News 
DuoVia.Http.Host is now an OWIN middleware component rather than a stand alone app. This makes it easy to use DuoVia.Http side-by-side with other OWIN frameworks and applications. Special thanks to Chris Ross for make the recommendation.

> Based on the feature set of [DuoVia.Net][1] but with a focus on using HTTP via [OWIN][2] with JSON rather than binary serialization. Initial testing shows that this library keeps pace with the binary serialization. This is in large part due to the use the ultra-fast ServiceStack.Text serializer.

> There are two NuGet libraries that you will need. The client proxy library [DuoVia.Http][3] and the OWIN hosting library [DuoVia.Http.Host][4]. 

> The host exposes a raw "metadata" endpoint that defines the services being hosted, their operations and their parameters in JSON. In the future we want to add a nice HTML response when the Accept header is not application/json.

> While not recommended, this allows you to build your own client or access your service with a non-.NET client. 

> And one very cool thing you probably won't find in any other RPC-style library is support for method overloads and out and ref parameters.

> Using the library is easy. Write your interface and DTO's into one assembly and your implementation into another assembly. This allows you to avoid distributing your implementation to clients.

> Now just hook up the host and the client as shown below. Check out the integration projects. Also note that you will need the OWIN self host NuGet package to create a similar console app that hosts your service.
 
First, your code that hosts your service.

```C#
    // must have this reference to provide Host extension method
    using DuoVia.Http.Host;
    // other using and namespace declarations removed for brevity

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

    // your startup class for application initialization
    internal class Startup
    {
        // Invoked once at startup to configure your application.
        public void Configuration(IAppBuilder app)
        {
            // create the service implementation
            IMyTest test = new MyTest();

            // use the middelware extension to host it
            app.Host<IMyTest>(test);

            // NOTE: host as many services as you want
            // but host only one instance of an interface
        }
    }
```
Second, your code that accesses your service (the client). What could be easier.

```C#
    static void Main(string[] args)
    {
        var url = "http://localhost:12345";
        var client = HttpClient.Create<IMyTest>(url);

        // now call methods on the interface as if it were local
        var name = client.GetName("heyhey");
        var thing = client.DoSomething("today");

        Console.WriteLine("{0} {1}", name, thing);

        Console.WriteLine("Press Any Key To Exit");
        Console.ReadKey();
    }
```

[1]: https://github.com/duovia/duovia-net                "DuoVia.Net"
[2]: http://www.owin.org                                 "OWIN"
[3]: http://www.nuget.org/packages/DuoVia.Http/          "DuoVia.Http"
[4]: http://www.nuget.org/packages/DuoVia.Http.Host/     "DuoVia.Http.Host"
