using System;
using System.Xml.Schema;
using System.Net;
using System.Web.Services.Description;
using System.IO;
using System.Collections.Generic;

namespace Soapbox
{

	public class DynamicServiceAdapter
	{
		/// <summary>
		/// Extracts webservice information, such as webmethods, inputs/outputs etc.
		/// </summary>
		/// <param name="webserviceUrl">The full url of the webservice</param>
		/// <remarks>
		/// Heavily inspired on pmartin's snippet through http://stackoverflow.com/a/4504351
		/// </remarks>
		public static WebserviceDefinition Extract(string webserviceUrl)
		{
			var returnValue = new WebserviceDefinition ();

			//Build the URL request string
			UriBuilder uriBuilder = new UriBuilder(webserviceUrl);
			uriBuilder.Query = "WSDL";

			HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uriBuilder.Uri);
			webRequest.ContentType = "text/xml;charset=\"utf-8\"";
			webRequest.Method = "GET";
			webRequest.Accept = "text/xml";

			// Submit a web request to get the web service's WSDL
			ServiceDescription serviceDescription;
			using (WebResponse response = webRequest.GetResponse())
			{
				using (Stream stream = response.GetResponseStream())
				{
					serviceDescription = ServiceDescription.Read(stream);
				}
			}

			//Loop through the port types in the service description and list all of the 
			//web service's operations and each operations input/output
			foreach (PortType portType in serviceDescription.PortTypes)
			{
				foreach (Operation operation in portType.Operations)
				{
					var o = new WebserviceOperation () {
						Name = operation.Name
					};

					Console.Out.WriteLine(operation.Name);

					foreach (var message in operation.Messages)
					{
						if (message is OperationInput)
							//Console.Out.WriteLine("Input Message: {0}", ((OperationInput) message).Message.Name);
							o.Inputs.Add (message as OperationInput);
						if (message is OperationOutput)
							//Console.Out.WriteLine("Output Message: {0}", ((OperationOutput) message).Message.Name);
							o.Outputs.Add (message as OperationOutput);

						foreach (Message messagePart in serviceDescription.Messages)
						{
							if (messagePart.Name != ((OperationMessage) message).Message.Name) continue;

							foreach (MessagePart part in messagePart.Parts)
							{
								Console.Out.WriteLine(part.Name);
							}
						}
					}

					returnValue.Operations.Add (o);

				}
			}

			//Drill down into the WSDL's complex types to list out the individual schema elements 
			//and their data types
			// TODO: This requires some more work...
			Types types = serviceDescription.Types;
			XmlSchema xmlSchema = types.Schemas[0];

			// TODO: if types.Schemas[0] is null, there's probably something wrong with the endpoint

			foreach (object item in xmlSchema.Items)
			{
				XmlSchemaElement schemaElement = item as XmlSchemaElement;
				XmlSchemaComplexType complexType = item as XmlSchemaComplexType;

				if (schemaElement != null)
				{
					Console.Out.WriteLine("Schema Element: {0}", schemaElement.Name);

					XmlSchemaType schemaType = schemaElement.SchemaType;
					XmlSchemaComplexType schemaComplexType = schemaType as XmlSchemaComplexType;

					if (schemaComplexType != null)
					{
						XmlSchemaParticle particle = schemaComplexType.Particle;
						XmlSchemaSequence sequence =
							particle as XmlSchemaSequence;
						if (sequence != null)
						{
							foreach (XmlSchemaElement childElement in sequence.Items)
							{
								Console.Out.WriteLine("    Element/Type: {0}:{1}", 
									childElement.Name, //{0}: property name, for example 'ZipCode'
									childElement.SchemaTypeName.Name); //{1}: property type, for example 'string'
							}
						}
					}
				}
				else if (complexType != null)
				{
					Console.Out.WriteLine("Complex Type: {0}", complexType.Name); // complex types, such as arrays/objects
					OutputElements(complexType.Particle);
				}
				Console.Out.WriteLine();
			}

			Console.Out.WriteLine();
			Console.In.ReadLine();

			return returnValue;
		}

		private static void OutputElements(XmlSchemaParticle particle)
		{
			XmlSchemaSequence sequence = particle as XmlSchemaSequence;
			XmlSchemaChoice choice = particle as XmlSchemaChoice;
			XmlSchemaAll all = particle as XmlSchemaAll;

			if (sequence != null) {
				Console.Out.WriteLine ("  Sequence");

				for (int i = 0; i < sequence.Items.Count; i++) {
					XmlSchemaElement childElement = sequence.Items [i] as XmlSchemaElement;
					XmlSchemaSequence innerSequence = sequence.Items [i] as XmlSchemaSequence;
					XmlSchemaChoice innerChoice = sequence.Items [i] as XmlSchemaChoice;
					XmlSchemaAll innerAll = sequence.Items [i] as XmlSchemaAll;

					if (childElement != null) {
						Console.Out.WriteLine ("    Element/Type: {0}:{1}", childElement.Name,
							childElement.SchemaTypeName.Name);                        
					} else
						OutputElements (sequence.Items [i] as XmlSchemaParticle);
				}
			} else if (choice != null) {
				Console.Out.WriteLine ("  Choice");
				for (int i = 0; i < choice.Items.Count; i++) {
					XmlSchemaElement childElement = choice.Items [i] as XmlSchemaElement;
					XmlSchemaSequence innerSequence = choice.Items [i] as XmlSchemaSequence;
					XmlSchemaChoice innerChoice = choice.Items [i] as XmlSchemaChoice;
					XmlSchemaAll innerAll = choice.Items [i] as XmlSchemaAll;

					if (childElement != null) {
						Console.Out.WriteLine ("    Element/Type: {0}:{1}", childElement.Name,
							childElement.SchemaTypeName.Name);
					} else
						OutputElements (choice.Items [i] as XmlSchemaParticle);
				}

				Console.Out.WriteLine ();
			} else if (all != null) {
				Console.Out.WriteLine ("  All");
				for (int i = 0; i < all.Items.Count; i++) {
					XmlSchemaElement childElement = all.Items [i] as XmlSchemaElement;
					XmlSchemaSequence innerSequence = all.Items [i] as XmlSchemaSequence;
					XmlSchemaChoice innerChoice = all.Items [i] as XmlSchemaChoice;
					XmlSchemaAll innerAll = all.Items [i] as XmlSchemaAll;

					if (childElement != null) {
						Console.Out.WriteLine ("    Element/Type: {0}:{1}", childElement.Name,
							childElement.SchemaTypeName.Name);
					} else
						OutputElements (all.Items [i] as XmlSchemaParticle);
				}
				Console.Out.WriteLine ();
			}


		}
	}
}

