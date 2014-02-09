using NUnit.Framework;
using System;
using FluentAssertions;
using System.Linq;

namespace Soapbox.Tests
{
	[TestFixture ()]
	public class DynamicServiceAdapterTests
	{
		[Test ()]
		public void Extract_Should_Provide_Webservice_Operations()
		{
			var webservice_url = @"http://wsf.cdyne.com/WeatherWS/Weather.asmx?WSDL";
			var target = DynamicServiceAdapter.Extract (webservice_url);

			target.Should ().BeOfType<WebserviceDefinition>(reason: "incorrect type returned");
			target.Operations.Count.Should ().BeGreaterOrEqualTo (1, reason: "no webmethods (operations) were found");

			//foreach (var operation in target.Operations) {
			//	operation.Inputs.Count.Should ().BeGreaterOrEqualTo (1, reason: "at least one input/request should be found");
			//	operation.Outputs.Count.Should ().BeGreaterOrEqualTo (1, reason: "at least one output/response should be found");
			//}

		}
	}
}

