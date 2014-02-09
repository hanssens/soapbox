using System;
using System.Collections.Generic;

namespace Soapbox
{
	/// <summary>
	/// Wraps the definition of an entire webservice.
	/// </summary>
	public class WebserviceDefinition
	{
		public List<WebserviceOperation> Operations { get; set; }

		public WebserviceDefinition ()
		{
			Operations = new List<WebserviceOperation> ();
		}

	}
}

