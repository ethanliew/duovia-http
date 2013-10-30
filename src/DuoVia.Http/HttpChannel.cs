using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using ServiceStack.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace DuoVia.Http
{
    public class HttpChannel : Channel
    {
        private Uri _endpoint = null;
        private ServiceMetadata _syncInfo;

        /// <summary>
        /// Creates a connection to the concrete object handling method calls on the server side
        /// </summary>
        /// <param name="endpoint"></param>
        public HttpChannel(Type serviceType, Uri endpoint)
        {
            _serviceType = serviceType;
            _endpoint = endpoint;
            SyncInterface(_serviceType);
        }

        /// <summary>
        /// This method asks the server for a list of identifiers paired with method
        /// names and -parameter types. This is used when invoking methods server side.
        /// </summary>
        protected override void SyncInterface(Type serviceType)
        {
            HttpWebRequest web = PrepareMetadataRequest();
            var webResponse = (HttpWebResponse) web.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    ServiceMetadata[] services = JsonSerializer.DeserializeFromStream<ServiceMetadata[]>(responseStream);
                    foreach (var service in services)
                    {
                        if (service.Name == (serviceType.AssemblyQualifiedName ?? serviceType.Name))
                        {
                            _syncInfo = service;
                            break;
                        }
                    }
                }
            }
            else
            {
                throw new System.Web.HttpException((int) webResponse.StatusCode, webResponse.StatusDescription);
            }
        }

        /// <summary>
        /// Invokes the method with the specified parameters.
        /// </summary>
        /// <param name="parameters">Parameters for the method call</param>
        /// <returns>An array of objects containing the return value (index 0) and the parameters used to call
        /// the method, including any marked as "ref" or "out"</returns>
        protected override object[] InvokeMethod(params object[] parameters)
        {
            HttpWebRequest web = PrepareAppRequest();
            //write multipart form data to request stream
            using (var writeStream = web.GetRequestStream())
            {
                //find the mathing server side method ident
                var callingMethod = (new StackFrame(1)).GetMethod();
                var methodName = callingMethod.Name;
                var methodParams = callingMethod.GetParameters();
                var ident = -1;
                for (int index = 0; index < _syncInfo.Operations.Length; index++)
                {
                    var si = _syncInfo.Operations[index];
                    //first of all the method names must match
                    if (si.Name == methodName)
                    {
                        //second of all the parameter types and -count must match
                        if (methodParams.Length == si.ParameterTypes.Length)
                        {
                            var matchingParameterTypes = true;
                            for (int i = 0; i < methodParams.Length; i++)
                                if (!methodParams[i].ParameterType.FullName.Equals(si.ParameterTypes[i].FullName))
                                {
                                    matchingParameterTypes = false;
                                    break;
                                }
                            if (matchingParameterTypes)
                            {
                                ident = si.Id;
                                break;
                            }
                        }
                    }
                }

                if (ident < 0)
                    throw new Exception(string.Format("Cannot match method '{0}' to its server side equivalent",
                        callingMethod.Name));

                var request = new DvRequest()
                {
                    Service = _syncInfo.Name,
                    Operation = methodName,
                    OperationId = ident,
                    Parameters = parameters
                };

                JsonSerializer.SerializeToStream(request, writeStream);
                writeStream.Flush();
            }

            //execute request with call to GetResponse
            var webResponse = (HttpWebResponse) web.GetResponse();
            try
            {
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        var dvResponse = JsonSerializer.DeserializeFromStream<DvResponse>(responseStream);
                        if (dvResponse.Exception != null) throw dvResponse.Exception;
                        object[] result = new object[dvResponse.Parameters.Length + 1];
                        result[0] = dvResponse.ReturnValue;
                        for (int i = 0; i < dvResponse.Parameters.Length; i++) result[i + 1] = dvResponse.Parameters[i];
                        return result;
                    }
                }
                else
                {
                    throw new System.Web.HttpException((int) webResponse.StatusCode, webResponse.StatusDescription);
                }
            }
            finally
            {
                webResponse.Close();
            }
        }

        private HttpWebRequest PrepareAppRequest()
        {
            var web = (HttpWebRequest) WebRequest.Create(new Uri(_endpoint, "/app"));
            web.ContentType = "application/json";
            web.Accept = "application/json";
            web.Method = "POST";
            return web;
        }

        private HttpWebRequest PrepareMetadataRequest()
        {
            var web = (HttpWebRequest) WebRequest.Create(new Uri(_endpoint, "/metadata"));
            web.ContentType = "application/json";
            web.Accept = "application/json";
            web.Method = "GET";
            return web;
        }
    }
}
