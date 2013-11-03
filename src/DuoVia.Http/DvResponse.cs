using System;

namespace DuoVia.Http
{
    public class DvResponse
    {
        public Exception Exception { get; set; }
        public object ReturnValue { get; set; }
        public object[] Parameters { get; set; }
    }
}