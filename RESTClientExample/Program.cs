using System;
using System.IO;
using Quic.DTO;
using ServiceStack.ServiceClient.Web;

namespace RESTClientExample
{
    class Program
    {

        // Examples using Microsoft's Web client interface
        static void MicrosoftExamples()
        {

        }

        // Examples using ServiceStack 3.9.71 (do not use 4.x)
        static void ServiceStackExamples()
        {
            // Client connection using ServiceStack 3.9.71: Base URL
            var client = new JsonServiceClient(@"http://localhost/CVAService_Dev")
            {
                Timeout = new TimeSpan(0, 5, 0) // Set timeout to 5 minutes
            };

            // Example 1: REST - (counterparty search, uses DTO)
            var response1 = client.Get(new CounterpartySearch { Name = "TEL" });

            //test the portfolio
            var responsePort = client.Get(new Portfolio { Counterparty = "TELSTRA CORP LTD" });

            // Example 2: REST - Value of bNetting flag from Legal document
            var response2 = client.Get(new LegalDocument { Field = "bNetting", Name = "TELSTRA CORP LTD-NAB LIMITED-763-Net" });

            // Example 3: REST - Value of bNetting flag from Legal document of counterparty 
            var response3 = client.Get(new CounterpartyLegalDocuments { Name = "TELSTRA CORP LTD" });

            // Example 4: Counterparty CVA
            //var response4 = client.Get(new CounterpartyCva { CounterpartyName = "TELSTRA CORP LTD" });

            var sr = new StreamReader(@"D:\QLib-IRFixedFloat.xml");

            // Example 5: Trade FpML CVA
            var response5 = client.Get(new ConvertFpml
            {
                Fpml = sr.ReadToEnd(),
            });

            var response6 = client.Get(new TradeCvaFpml
            {
                Fpml = @"D:\QLib-IRFixedFloat.xml",
                NewTrade = true
            });
        }

        private static void Main(string[] args)
        {
            ServiceStackExamples();
        }
    }
}
