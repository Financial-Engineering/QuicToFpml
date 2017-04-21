using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quic.CSV;
using Quic.DateSchedule;
using Quic.Templates;
using Frequency = Quic.DateSchedule.PeriodEnum;
using Portfolio = Quic.CSV.Portfolio;
using Schedule = Quic.CSV.Schedules;
using Stub = Quic.DateSchedule.StubEnum;

namespace Quic.Test
{
    public class Program
    {
        private static void DocumentTest()
        {

            //  var observe = new Observables(@"D:\batchsdl-tech\observables.csv");

            // var x = observe["SEK-STIBOR-SIDE-3M-ACT360-Swap"];
            var market = new MarketData(@"D:\batchsdl-tech\marketData.csv");

            var curve = market.GetByCurveName("USD.Yield.USD");

            const string cpty = "TELSTRA CORP-MEL";

            var sched = new Schedule(@"D:\batchsdl-tech\schedules.csv");
            var cph = new CounterpartyHierarchy(@"D:\batchsdl-tech\counterpartyHierarchy.csv");

            var legalDoc = new LegalDocument(@"D:\batchsdl-tech\legalDocuments.csv");
            var port = new Portfolio(@"D:\batchsdl-tech\portfolio.csv");

            var parent = cph.GetParent(cpty);
            var doc = legalDoc.GetByCounterParty(parent);

            var t = port.GetByLegalDocument(doc.First());

            //         port.Add(@"395319CALY-B",@"395319CALY-B,IRGenericQMA,HSBC BANK USA-NYC,HSBC BANK USA-NAB LIMITED-CSA,,NAB,IR Swap,NABLON 3618 SWTU,USD,SWPIR,,TRUE,FALSE,395319CALY-REC,395319CALY-PAY,,");

            //port.WriteByLegalDocument(@"D:\test\portfolio.csv", doc);
            //sched.WriteByLegalDocument(@"D:\test\schedules.csv", doc);

            var children = cph.GetChildren(parent);

            cph.WriteCounterparty(@"D:\test\counterpartyHierarchy.csv", new List<string> {parent});
            legalDoc.WriteCounterparty(@"D:\test\legalDocuments.csv", new List<string> {parent});

        }


        private static void TemplateTest()
        {
            var fpml = File.ReadAllText(@"D:\QuIC\etl\code\Quic.WebService\Quic.FPML\samples\IR Swap.xml");

            var trans = new Transaction(fpml);

            var transaction = trans.GenerateTransaction();
            var schedule = trans.GenerateSchedule();
        }

        private static void ScheduleTest()
        {
            var scheds = new Quic.DateSchedule.DateSchedule(new BusinessDate(new DateTime(2013, 12, 9)),
                new BusinessDate(new DateTime(2015, 12, 9)),
                Frequency.M, 6, DateSchedule.RollConventionEnum.EOM, Stub.NONE, "USD", "JPY");
        }


        private static void Main(string[] args)
        {
            var fpml = File.ReadAllText(@"D:\QuIC\etl\code\Quic.WebService\Quic.FPML\samples\FX\FX Forward.xml");
            var obj = Quic.Config.Utilities.Deserialize<FPML.RequestValuationReport>(fpml);

            var transaction = new Transaction(fpml);

            var transCsv = transaction.GenerateTransaction();
            var schedCsv = transaction.GenerateSchedule();
        }
    }
}
