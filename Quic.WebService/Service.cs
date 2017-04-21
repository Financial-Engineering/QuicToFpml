using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Quic.Config;
using Quic.CSV;
using Quic.Dispatch;
using Quic.DTO;
using Quic.FPML;
using Quic.Templates;
using ServiceStack.Common;
using ServiceStack.Common.Extensions;
using ServiceStack.Html;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using Exception = System.Exception;
using LegalDocument = Quic.DTO.LegalDocument;
using MarketData = Quic.DTO.MarketData;
using Observables = Quic.DTO.Observables;
using Portfolio = Quic.DTO.Portfolio;
using Schedules = Quic.DTO.Schedules;
using StringExtensions = ServiceStack.Common.StringExtensions;
using nab.QR.Persistence;
using nab.QR.SQLPersistence;

namespace Quic.WebService
{
    public class Service : IService
    {
        #region Private Methods

        private static TradeResponse RunTradeBatch(IEnumerable<PricingRequest> requests)
        {
            var response = new TradeResponse();

            try
            {
                var cph = new CounterpartyHierarchy(ServiceConfig.Context.CptyHierarchy);

                var request = requests.First();

                if (!cph.ContainsKey(request.Cpty) && (string.IsNullOrEmpty(request.CptyHierarchy) &&
                        string.IsNullOrEmpty(request.LegalDocument)))
                        throw new ArgumentException(String.Format("ERROR: Counterparty: {0}, does not exist",
                            request.Cpty));

                var exp = new ExportParams(ServiceConfig.Context.ExportParams);
                foreach (var measure in requests.SelectMany(pricingRequest => pricingRequest.Measures.Where(measure => !exp.ContainsKey(measure))))
                {
                    throw new ArgumentException(String.Format("ERROR: Invalid Measure {0} supplied", measure));
                }

                var cvaResult = Dispatcher.RunBatch(requests);

                if (cvaResult.Status == DispatchResponse.StatusEnum.Failure)
                    throw new ArgumentException(cvaResult.Error);

                response.Results.Add(new TradeResponse.TradeResult {Result = cvaResult.Result});
                response.Status = Response.StatusEnum.Success;
            }
            catch (Exception e)
            {
                response.Status = Response.StatusEnum.Failure;
                response.Error = (e.InnerException != null) ? e.InnerException.Message : e.Message;
            }

            return response;
        }


        private static CounterpartyBatchResponse RunCounterpartyBatch(PricingRequest request)
        {
            var response = new CounterpartyBatchResponse();

            try
            {
                var cph = new CounterpartyHierarchy(ServiceConfig.Context.CptyHierarchy);
                if (!cph.ContainsKey(request.Cpty))
                {
                    throw new ArgumentException(String.Format("ERROR: Counterparty: {0}, does not exist", request.Cpty));
                }

                var cvaResult = Dispatcher.RunBatch(new List<PricingRequest> { request });

                if (cvaResult.Status == DispatchResponse.StatusEnum.Failure)
                    throw new ArgumentException(cvaResult.Error);

                response.Status = Response.StatusEnum.Success;
                response.Result = cvaResult.Result;
            }
            catch (Exception e)
            {
                response.Status = Response.StatusEnum.Failure;
                response.Error = (e.InnerException != null) ? e.InnerException.Message : e.Message;
            }

            return response;
        }

        private TradeResponse RunPricing(IEnumerable<PricingRequest> requests)
        {
            var response = new TradeResponse();

            var priceResponse = new DispatchResponse();

            Parallel.ForEach(requests,
                (pricingRequest, state) =>
                {
                    priceResponse = Dispatcher.PriceTrade(pricingRequest);

                    if (priceResponse.Status == DispatchResponse.StatusEnum.Failure)
                        state.Stop();
                    else
                    {
                        response.Results.Add(new TradeResponse.TradeResult
                        {
                            TradeId = pricingRequest.Transaction.Split(',')[0],
                            Result = priceResponse.Result
                        });
                    }
                }
                );

            if (priceResponse.Status == DispatchResponse.StatusEnum.Failure)
            {
                response.Status = Response.StatusEnum.Failure;
                response.Error = priceResponse.Error;
            }

            return response;
        }

        #endregion

        
        public object Any(TradePricingFpml pricing)
        {
            return RunPricing(Conversion.ToPricingRequest(pricing)).UpCast<TradePricingFpmlResponse>();
        }

        public object Any(TradePricingCsv pricing)
        {
            return RunPricing(Conversion.ToPricingRequest(pricing)).UpCast<TradePricingCsvResponse>();
        }

        /// <summary>
        /// Calculates the running spread of swap trades
        /// </summary>
        /// <param name="pricing"></param>
        /// <returns></returns>
        public object Any(TradeRunningSpreadCsv pricing)
        {
            var request = Conversion.ToPricingRequest(pricing);

            var pricingRequest = request.ToArray()[0];

            var response = new TradeRunningSpreadCsvResponse();

            try
            {
                if (pricingRequest.Product != "IRGenericQMA")
                {
                    throw new ArgumentException("ERROR: Running Spread Calculation Only Valid for Swaps");
                }

                pricingRequest.Measures = new[] { "CVA_Collateralized", "FVA_Collateralized" };

                var cvaResult = Dispatcher.CalcRunningSpread(pricingRequest);

                if (cvaResult.Status == DispatchResponse.StatusEnum.Failure)
                    throw new ArgumentException(cvaResult.Error);

                response.Status = Response.StatusEnum.Success;
                response.Results = new List<TradeCsvResponse.TradeResult>
                {
                    new TradeCsvResponse.TradeResult {Result = cvaResult.Result}
                };

                var priceResponse = new DispatchResponse();

                if (priceResponse.Status == DispatchResponse.StatusEnum.Failure)
                {
                    response.Status = Response.StatusEnum.Failure;
                    response.Error = priceResponse.Error;
                }
            }
            catch (Exception e)
            {
                response.Status = Response.StatusEnum.Failure;
                response.Error = (e.InnerException != null) ? e.InnerException.Message : e.Message;
            }

            return response;
        }

        /// <summary>
        /// Calculates the running spread of swap trades
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Any(TradeRunningSpreadFpml request)
        {
            var response = RunTradeBatch(Conversion.ToPricingRequest(request)).UpCast<TradeRunningSpreadFpmlResponse>();

            var priceResponse = new DispatchResponse();

            if (priceResponse.Status == DispatchResponse.StatusEnum.Failure)
            {
                response.Status = Response.StatusEnum.Failure;
                response.Error = priceResponse.Error;
            }

            return response;
        }

        /// <summary>
        /// Credit charge run for CSV formatted trades
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Any(TradeCvaCsv request)
        {
            return RunTradeBatch(Conversion.ToPricingRequest(request)).UpCast<TradeCvaCsvResponse>();
        }

        /// <summary>
        /// Translate FpML to CSV
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Any(ConvertFpml request)
        {
            var response = new ConvertFpmlResponse();

            try
            {
                var transaction = new Transaction(request.Fpml);

                var transCsv = transaction.GenerateTransaction();
                var schedCsv = transaction.GenerateSchedule();

                response.Result = new Dictionary<string, string>
                {
                    {"transaction", transCsv},
                    {"schedules", schedCsv},
                };
            }
            catch (Exception e)
            {
                response.Status = Response.StatusEnum.Failure;
                response.Error = (e.InnerException != null) ? e.InnerException.Message : e.Message;
            }

            return response;
        }

        public object Any(TradeCvaFpml request)
        {
            var response = RunTradeBatch(Conversion.ToPricingRequest(request)).UpCast<TradeCvaFpmlResponse>();

            return response;
        }

        public object Any(TradeMtmFpml mtm)
        {
            var response = RunTradeBatch(Conversion.ToPricingRequest(mtm)).UpCast<TradeMtmFpmlResponse>();

            return response;
        }

        public object Any(TradeMtmCsv request)
        {
            var response = RunTradeBatch(Conversion.ToPricingRequest(request)).UpCast<TradeMtmCsvResponse>();

            return response;
        }

        public object Any(CounterpartyCva request)
        {
            return RunCounterpartyBatch(Conversion.ToPricingRequest(request)).UpCast<CounterpartyCvaResponse>();
        }

        public object Any(CounterpartyMtm request)
        {
            return RunCounterpartyBatch(Conversion.ToPricingRequest(request)).UpCast<CounterpartyMtmResponse>();
        }

        /// <summary>
        /// Various portfolio related queries
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Any(Portfolio request)
        {
            var response = new PortfolioResponse();
            var portfolio = new CSV.Portfolio(ServiceConfig.Context.Portfolio);

            if (!StringExtensions.IsNullOrEmpty(request.TradeId))
                response.Result = portfolio.GetByTrade(request.TradeId).ToArray();
            else if (!StringExtensions.IsNullOrEmpty(request.Counterparty))
                response.Result = portfolio.GetByCounterparty(request.Counterparty).ToArray();
            else if (!StringExtensions.IsNullOrEmpty(request.LegalDocument))
                response.Result = portfolio.GetByLegalDocument(request.LegalDocument).ToArray();
            else
                response.Result = portfolio.GetAll().ToArray();

            return response;
        }

        public object Any(Schedules request)
        {
            return new SchedulesResponse 
                {
                    Result = Conversion.LoadTrade(new TradeCsv.Trade
                    {
                        TransactionId = request.TradeId
                    }).SchedulesCsv
                };
        }

        /// <summary>
        /// Returns a list of legal documents for a given counterparty
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Any(CounterpartyLegalDocuments request)
        {
            var cph = new CounterpartyHierarchy(ServiceConfig.Context.CptyHierarchy);
            var parent = cph.GetParent(request.Name);

            var children = cph.GetChildren(parent);

            var list = children.ToList();

            list.Add(parent);

            return new CounterpartyLegalDocumentsResponse
            {
                Result = new CSV.LegalDocument(ServiceConfig.Context.LegalDocument).GetByCounterParties(list.ToArray()).ToArray()
            };
        }

        /// <summary>
        /// Returns all information or specified field for a given legal document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Any(LegalDocument request)
        {
            var response = new LegalDocumentResponse();

            if (!string.IsNullOrEmpty(request.Name))
            {
                response.Result = !string.IsNullOrEmpty(request.Field)
                    ? new CSV.LegalDocument(ServiceConfig.Context.LegalDocument).GetFieldByLegalDocument(request.Name, request.Field)
                    : new CSV.LegalDocument(ServiceConfig.Context.LegalDocument).GetByLegalDocument(request.Name);
            }

            return response;
        }

        /// <summary>
        /// Returns all information or specified field for a given counterparty
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Any(Counterparty request)
        {
            return new CounterpartyResponse
            {
                Result = String.IsNullOrEmpty(request.Field)
                    ? new CounterpartyHierarchy(ServiceConfig.Context.CptyHierarchy).GetByCounterParty(request.Name) 
                    : new CounterpartyHierarchy(ServiceConfig.Context.CptyHierarchy).GetField(request.Name, request.Field,DocumentStructure.FieldKeys.Counterparty)
            };
        }

        public object Any(MarketData request)
        {
            return new MarketDataResponse
            {
                Result = StringExtensions.IsNullOrEmpty(request.CurveName)
                    ? StringExtensions.IsNullOrEmpty(request.Currency) ? new CSV.MarketData(ServiceConfig.Context.MarketDataCsv).GetByCurveType(request.CurveType)
                    : new CSV.MarketData(ServiceConfig.Context.MarketDataCsv).GetByCurrency(request.Currency)
                    : new CSV.MarketData(ServiceConfig.Context.MarketDataCsv).GetByCurveName(request.CurveName)
            };
        }

        public object Any(Observables request)
        {
            var response = new ObservablesResponse();
            var observables = new CSV.Observables(ServiceConfig.Context.Observables);

            if (StringExtensions.IsNullOrEmpty(request.Name) && StringExtensions.IsNullOrEmpty(request.Currency))
                response.Result = observables.GetAll().ToArray();
            else if (!StringExtensions.IsNullOrEmpty(request.Name))
                response.Result = new []{observables.GetByName(request.Name)};
            else
                response.Result = observables.GetByCurrency(request.Currency);

            return response;
        }

        public object Any(CounterpartySearch request)
        {
            return new CounterpartySearchResponse
            {
                Result = new CounterpartyHierarchy(ServiceConfig.Context.CptyHierarchy).Match(request.Name).ToArray()
            };
        }

        public object Any(CounterpartyRegexpSearch request)
        {
            return new CounterpartyRegexpSearchResponse
            {
                Result = new CounterpartyHierarchy(ServiceConfig.Context.CptyHierarchy).RegexMatch(request.Pattern).ToArray()
            };
        }

        public object Any(PricingMeasures request)
        {
            return new PricingMeasuresResponse
            {
                Result = ServiceConfig.Products[request.Instrument].SupportedMeasures.Select(e=>e.ToString()).ToArray()
            };
        }

        public object Any(DeskUseTradeSave request)
        {
            var ret = new DeskUseTradeSaveResponse();
            var strret = "";
            try
            {
                var trd = request.Trades.Select(x => x.SerializeToString()).ToArray();
                var nm = request.SaveName;
                var a = ServiceConfig.Context.PricingDB;
                
                var cmd = new PersistCommand("CVA_Pricing", "dbo.Save");
                cmd.TimeoutSeconds = 300;  //change the default time out from 120 secs
                var message = "";
                for (var idx = 0; idx != trd.Count(); idx++)
                {
                    cmd.SetParam("saveName", request.SaveName);
                    cmd.SetParam("trade", trd[idx]);
                    cmd.SetParam("overRide", request.OverRide? 1:0);
                    cmd.SetParam("message", "");
                    cmd.Params["message"].Type = PersistParam.ParamType.Out;

                    IPersistSink rows = new PersistSinks();
                    //cmd.Execute(rows);
                    message = cmd.Execute(rows)["message"].StringValue;
                    
                    strret = strret + string.Format("when saving trade {0} db return status {1} \r\n",
                        request.Trades[idx].TransactionCsv.Split(',')[0], message);
                }

                cmd = new PersistCommand("CVA_Pricing", "dbo.SaveLock");
                cmd.TimeoutSeconds = 300;
                cmd.SetParam("saveName", request.SaveName);
                cmd.Execute();
            }
            catch (Exception ex)
            {
                ret.Error = ex.Message;
                ret.Status = Response.StatusEnum.Failure;
                ret.Result = "FAILURE! :" +ret.Error;
                return ret;
            }
            ret.Status = Response.StatusEnum.Success;
            ret.Result = strret;
            
            return ret;
        }

        public object Any(DeskUseTradeLoad request)
        {
            var ret = new DeskUseTradeLoadResponse();

            if (string.IsNullOrEmpty(request.SaveName))
            {
                try
                {
                    var cmdl = new PersistCommand("CVA_Pricing", "dbo.LoadSavedNames") {TimeoutSeconds = 300};
                    var rowsl = new PersistSinks();
                    cmdl.Execute(rowsl);
                    var lst = new string[rowsl.Count];
                    for (int idx = 0; idx != rowsl.Count; idx++)
                    {
                        lst[idx] = rowsl[idx]["save_name"].StringValue;
                    }
                    ret.SavedNames = lst;
                    ret.Status = Response.StatusEnum.Success;
                }
                catch (Exception ex)
                {
                    ret.Error = ex.Message;
                    ret.Status = Response.StatusEnum.Failure;
                }
                return ret;
            }

            try
            {
                var cmd = new PersistCommand("CVA_Pricing", "dbo.Load");
                cmd.TimeoutSeconds = 300;  //change the default time out from 120 secs

                var rows = new PersistSinks();
                cmd.SetParam("saveName", request.SaveName);
                cmd.Execute(rows);
                
                var rc = rows.Count;
                var trds = new TradeCsv.Trade[rc];

                for (int idx = 0; idx != rc; idx++)
                {
                    trds[idx] = (TradeCsv.Trade)JsonSerializer.DeserializeFromString(rows[idx]["trade"].StringValue, typeof(TradeCsv.Trade));
                }

                ret.Status = Response.StatusEnum.Success;
                ret.Trades = trds;
            }
            catch (Exception ex)
            {
                ret.Error = ex.Message;
                ret.Status = Response.StatusEnum.Failure;
            }


            return ret;
        }
    }
}
