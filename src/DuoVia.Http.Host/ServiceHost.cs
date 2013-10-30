using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Reflection;
using ServiceStack.Text;

namespace DuoVia.Http.Host
{
    public class ServiceHost
    {
        private class ServiceInstance
        {
            public string ServiceKey { get; set; }
            public Type InterfaceType { get; set; }
            public object SingletonInstance { get; set; }
            public ConcurrentDictionary<int, MethodInfo> InterfaceMethods { get; set; }
            public ConcurrentDictionary<int, bool[]> MethodParametersByRef { get; set; }
            public ServiceMetadata ServiceMetadata { get; set; }
        }

        private static ConcurrentDictionary<string, ServiceInstance> _services = new ConcurrentDictionary<string, ServiceInstance>(); 

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
                    return HandleOperationRequest(context);
                }
                else if (context.Request.Path.Value.StartsWith("/metadata"))
                {
                    return HandleMetadataRequest(context);
                }
            }
            return Handle404Request(context);
        }

        private Task Handle404Request(IOwinContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = 404;
            context.Response.ReasonPhrase = "resource not found";
            return context.Response.WriteAsync("Unable to locate requested resource.");
        }

        private Task HandleMetadataRequest(IOwinContext context)
        {
            context.Response.ContentType = "application/json";
            var serviceMetadata = (from n in _services select n.Value.ServiceMetadata).ToArray();
            return context.Response.WriteAsync(serviceMetadata.SerializeToString());
        }

        public Task HandleOperationRequest(IOwinContext context)
        {
            var contextLoc = context;
            return Task.Factory.StartNew(() =>
            {
                context.Response.ContentType = "application/json";
                try
                {
                    var dvRequest = JsonSerializer.DeserializeFromStream<DvRequest>(contextLoc.Request.Body);
                    ServiceInstance invokedInstance;
                    if (_services.TryGetValue(dvRequest.Service, out invokedInstance))
                    {
                        //read the method identifier
                        int methodHashCode = dvRequest.OperationId;
                        if (invokedInstance.InterfaceMethods.ContainsKey(methodHashCode))
                        {
                            MethodInfo method;
                            invokedInstance.InterfaceMethods.TryGetValue(methodHashCode, out method);

                            bool[] isByRef;
                            invokedInstance.MethodParametersByRef.TryGetValue(methodHashCode, out isByRef);

                            //invoke the method
                            var response = new DvResponse();
                            try
                            {
                                response.ReturnValue = method.Invoke(invokedInstance.SingletonInstance, dvRequest.Parameters);
                                //the result to the client is the return value (null if void) and the input parameters
                                var returnParameters = new object[dvRequest.Parameters.Length];
                                for (int i = 0; i < dvRequest.Parameters.Length; i++)
                                    returnParameters[i] = isByRef[i] ? dvRequest.Parameters[i] : null;
                                response.Parameters = returnParameters;
                            }
                            catch (Exception ex)
                            {
                                //an exception was caught. Rethrow it client side
                                response.Exception = ex;
                            }
                            contextLoc.Response.Write(response.SerializeToString());
                            return;
                        }
                    }
                    contextLoc.Response.StatusCode = 501;
                    contextLoc.Response.ReasonPhrase = "unknown operation";
                    contextLoc.Response.Write("unknown operation");
                }
                catch (Exception e)
                {
                    contextLoc.Response.StatusCode = 500;
                    contextLoc.Response.ReasonPhrase = "server error";
                    contextLoc.Response.Write(e.SerializeToString());
                }
            });
        }

        public static void AddService<TService>(TService service) where TService : class
        {
            if (null == service) throw new NullReferenceException("service");
            var serviceType = typeof (TService);
            var serviceKey = serviceType.AssemblyQualifiedName ?? serviceType.Name;
            if (_services.ContainsKey(serviceKey)) throw new Exception("Service already added. Only one instance allowed.");
            var instance = CreateMethodMap(serviceKey, serviceType, service);
            _services.TryAdd(serviceKey, instance);
        }

        /// <summary>
        /// Loads all methods from interfaces and assigns an identifier
        /// to each. These are later synchronized with the client.
        /// </summary>
        private static ServiceInstance CreateMethodMap(string serviceKey, Type serviceType, object service)
        {
            var instance = new ServiceInstance()
            {
                ServiceKey = serviceKey,
                InterfaceType = serviceType,
                InterfaceMethods = new ConcurrentDictionary<int, MethodInfo>(),
                MethodParametersByRef = new ConcurrentDictionary<int, bool[]>(),
                SingletonInstance = service
            };

            var currentMethodIdent = 0;
            if (serviceType.IsInterface)
            {
                var methodInfos = serviceType.GetMethods();
                foreach (var mi in methodInfos)
                {
                    instance.InterfaceMethods.TryAdd(currentMethodIdent, mi);
                    var parameterInfos = mi.GetParameters();
                    var isByRef = new bool[parameterInfos.Length];
                    for (int i = 0; i < isByRef.Length; i++)
                        isByRef[i] = parameterInfos[i].ParameterType.IsByRef;
                    instance.MethodParametersByRef.TryAdd(currentMethodIdent, isByRef);
                    currentMethodIdent++;
                }
            }

            var interfaces = serviceType.GetInterfaces();
            foreach (var interfaceType in interfaces)
            {
                var methodInfos = interfaceType.GetMethods();
                foreach (var mi in methodInfos)
                {
                    instance.InterfaceMethods.TryAdd(currentMethodIdent, mi);
                    var parameterInfos = mi.GetParameters();
                    var isByRef = new bool[parameterInfos.Length];
                    for (int i = 0; i < isByRef.Length; i++)
                        isByRef[i] = parameterInfos[i].ParameterType.IsByRef;
                    instance.MethodParametersByRef.TryAdd(currentMethodIdent, isByRef);
                    currentMethodIdent++;
                }
            }


            //Create a list of sync infos from the dictionary
            var syncSyncInfos = new List<OperationMetadata>();
            foreach (var kvp in instance.InterfaceMethods)
            {
                var parameters = kvp.Value.GetParameters();
                var parameterTypes = new Type[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                    parameterTypes[i] = parameters[i].ParameterType;
                syncSyncInfos.Add(new OperationMetadata
                {
                    Id = kvp.Key,
                    Name = kvp.Value.Name,
                    ParameterTypes = parameterTypes
                });
            }

            var serviceSyncInfo = new ServiceMetadata
            {
                Name = serviceKey,
                Operations = syncSyncInfos.ToArray()
            };
            instance.ServiceMetadata = serviceSyncInfo;
            return instance;
        }
    }
}
