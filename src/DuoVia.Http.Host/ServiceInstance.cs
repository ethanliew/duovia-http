using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace DuoVia.Http.Host
{
    internal class ServiceInstance
    {
        public string ServiceKey { get; set; }
        public Type InterfaceType { get; set; }
        public object SingletonInstance { get; set; }
        public ConcurrentDictionary<int, MethodInfo> InterfaceMethods { get; set; }
        public ConcurrentDictionary<int, bool[]> MethodParametersByRef { get; set; }
        public ServiceMetadata ServiceMetadata { get; set; }
    }
}