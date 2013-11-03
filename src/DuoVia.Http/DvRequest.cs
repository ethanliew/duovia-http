namespace DuoVia.Http
{
    public class DvRequest
    {
        public string Service { get; set; } //TypeName
        public int OperationId { get; set; } // MethodId
        public string Operation { get; set; } // MethodName
        public object[] Parameters { get; set; }
    }
}