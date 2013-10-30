using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuoVia.Http
{
	public class ServiceMetadata
	{
		public string Name { get; set; }
		public OperationMetadata[] Operations { get; set; }
	}

	public class OperationMetadata
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Type[] ParameterTypes { get; set; }
	}

	public class DvRequest
	{
		public string Service { get; set; } //TypeName
		public int OperationId { get; set; } // MethodId
		public string Operation { get; set; } // MethodName
		public object[] Parameters { get; set; }
	}

	public class DvResponse
	{
		public Exception Exception { get; set; }
		public object ReturnValue { get; set; }
		public object[] Parameters { get; set; }
	}
}
