using System;

namespace DuoVia.Http
{
    public class OperationMetadata
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Type[] ParameterTypes { get; set; }
    }
}