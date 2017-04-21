using System;
using System.Collections.Generic;
using System.Linq;
using Quic.Config;
using Quic.CSV;
using Quic.Dispatch;
using Quic.DTO;
using Quic.FPML;
using Quic.Templates;
using Portfolio = Quic.CSV.Portfolio;
using Schedules = Quic.CSV.Schedules;
using Utilities = Quic.Config.Utilities;

namespace Quic.WebService
{
    /// <summary>
    /// Functions to convert CSV and FpML to pricing requests
    /// </summary>
    public static class Conversion
    {
       
        /// <summary>
        /// Loads an existing trade from the portfolio and schedules files 
        /// </summary>
        /// <param name="trade">Trade object with transaction ID defined</param>
        /// <returns>New trade object with Transaction and Schedule string</returns>
        public static TradeCsv.Trade LoadTrade(TradeCsv.Trade trade)
        {
            var port = new Portfolio(ServiceConfig.Context.Portfolio);
            var sched = new Schedules(ServiceConfig.Context.Schedules);

            var entry = port.GetByTrade(trade.TransactionId);

            if (!entry.Any())
                throw new ArgumentException(String.Format("Transaction ID: {0} does not exist in portfolio: {1}", 
                    trade.TransactionId, ServiceConfig.Context.Portfolio));

            var legs = port.GetLegs(trade.TransactionId);

            trade.TransactionCsv = port.GetByTrade(trade.TransactionId).First();
            trade.SchedulesCsv = legs.Aggregate("", (current, leg) => current + sched[leg]);

            return trade;
        }

        /// <summary>
        /// Convert Trade CSV DTO object into a list of pricing requests
        /// <seealso cref="TradeCsv"/>
        /// </summary>
        /// <param name="pricing">Trade CSV</param>
        /// <returns>Enumerable list of PricingRequest object</returns>
        public static IEnumerable<PricingRequest> ToPricingRequest(TradeCsv pricing)
        {
            // If a trade has the transactionId defined then it is an existing trade so load details, otherwise pass it through
            var tempList = pricing.Trades.Select(trade => !String.IsNullOrWhiteSpace(trade.TransactionId) ? LoadTrade(trade) : trade).ToList();

            var valueDate = pricing.ValueDate == new DateTime()
                ? new CSV.MarketData(ServiceConfig.Context.MarketDataCsv).GetValuationDate()
                : pricing.ValueDate;

            var result = (from trade in tempList
                          where trade.TransactionCsv != null
                          let transaction = Utilities.PackCsv(Utilities.RemoveControlChars(trade.TransactionCsv))
                          let fields = transaction.Split(',')
                          where fields[0] != null
                          select new PricingRequest
                          {
                              Product = fields[1],
                              BatchMode = pricing.GetType().Name == "TradeMtmCsv" ? BatchModeEnum.MTM : BatchModeEnum.CVA,
                              Measures = pricing.Measures,
                              Context = ServiceConfig.Context,
                              ValueDate = (Date)valueDate,
                              PricingTransaction = new Aggregation(ServiceConfig.Context.Aggregation).RemoveFields(transaction),
                              Transaction = transaction,
                              Schedules = trade.SchedulesCsv,
                              CptyHierarchy = trade.CptyHierarchyCsv,
                              LegalDocument = trade.LegalDocumentCsv,
                              NewTrade = trade.NewTrade,
                              IgnoreTrade = trade.IgnoreTrade,
                              Cpty = fields[2],
                              TradeId = fields[0],
                              StandAlone = pricing.StandAlone
                          });

            return result;
        }

        /// <summary>
        /// Convert FpML DTO object into a list of pricing requests
        /// <seealso cref="TradeFpml"/>
        /// </summary>
        /// <param name="pricing">Trade Fpml</param>
        /// <returns>Enumerable list of PricingRequest object</returns>
        public static IEnumerable<PricingRequest> ToPricingRequest(TradeFpml pricing)
        {
            var transaction = new Transaction(pricing.Fpml);

            return (from trade in transaction
                let transCsv = trade.GenerateTransaction()
                let schedCsv = trade.GenerateSchedule()
                let valueDate = transaction.ValuationDate == new DateTime() ? new CSV.MarketData(ServiceConfig.Context.MarketDataCsv).GetValuationDate() : transaction.ValuationDate
                select new PricingRequest
                {
                    Product = trade.strProduct, 
                    BatchMode = pricing.GetType().Name == "TradeMtmFpml" ? BatchModeEnum.MTM : BatchModeEnum.CVA, //TODO: Convert to lambda
                    Measures = transaction.Measures, 
                    Context = ServiceConfig.Context, 
                    ValueDate = (Date) valueDate, 
                    PricingTransaction = new Aggregation(ServiceConfig.Context.Aggregation).RemoveFields(transCsv), 
                    Transaction = transCsv, 
                    Schedules = schedCsv, 
                    Cpty = trade.Parties[1].Item2, 
                    StandAlone = pricing.StandAlone
                }).ToList();
        }

        // TODO: make this work for pricing as well?
        public static PricingRequest ToPricingRequest(CounterpartyBatch pricing)
        {
            var priceRequest = new PricingRequest
            {
                BatchMode = pricing.GetType().Name == "CounterpartyMtm" ? BatchModeEnum.MTM : BatchModeEnum.CVA,
                Context = ServiceConfig.Context,
                Measures = pricing.Measures,
                Cpty = pricing.CounterpartyName,
                LegalDocumentName = pricing.LegalDocumentName
            };
            return priceRequest;
        }
    }
}