using System;
using System.Collections.Generic;
using System.Web.Services.Description;

namespace Soapbox
{
	/// <summary>
	/// A single operation, or webmethod. 
	/// </summary>
	public class WebserviceOperation
	{
		public string Name { get; set; }
		public List<OperationInput> Inputs { get; set; }
		public List<OperationOutput> Outputs { get; set; }

		public WebserviceOperation ()
		{
			Inputs = new List<OperationInput> ();
			Outputs = new List<OperationOutput> ();
		}
	}
}

