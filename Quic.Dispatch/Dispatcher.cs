using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Quic.Config;
using Quic.Config.Schemas;
using Quic.CSV;
#if DISPATCH_DLL
using Quic.Integration.QuicDispatch;
#else
using Module = Quic.Config.Module;
using Quic.Dispatch.ResourceManager;
#endif

namespace Quic.Dispatch
{
    public static class Dispatcher
    {
        #region Private Methods

#if DISPATCH_DLL
        private static IQuicDispatch Dispatch { get; set; }

        static Dispatcher()
        {
            Dispatch = QuicDispatchFactory.GetFactoryInstance().CreateIQuicDispatch();
            Dispatch.Init();

            // Move to Config
            Dispatch.SetModulePaths(@"D:\QuIC\modules");
            Dispatch.LoadModule("quic_mtm_instruments", "11");
            Dispatch.LoadModule("quic_cva_driver", "11");
        }
#else
        private static readonly QuicDispatchInterfaceClient Client =
            new QuicDispatchInterfaceClient("BasicHttpBinding_IQuicDispatchInterface");
#endif

#if DISPATCH_DLL
        private static string RunRequest(DispatchRequest request)
#else
        private static string RunRequest(string moduleDir, string module, string version, Config.Schemas.DispatchRequest request)
#endif
        {
            try
            {
                var strXml = Utilities.SerializeFragment(request);
#if DISPATCH_DLL
                var audit = new EngineAudit();
                var progress = new EngineProgress();

                Dispatch.Dispatch(strXml, audit, progress, false);

                return Dispatch.GetLastXMLResult();
            }
#else
    //double progress;
                var status = new JobStatus();
                var jobId = Client.Submit(moduleDir, module, version, strXml);

                do
                {
                    status = Client.GetStatus(jobId);
                    //progress = status.progress;
                } while (status.funcStatus != FunctionStatus.Completed && status.funcStatus != FunctionStatus.Failed);

                return (status.funcStatus == FunctionStatus.Failed) ? Client.GetError(jobId) : Client.GetResults(jobId);
            }
#endif
            catch (Exception ex)
            {
                throw new ArgumentException(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        // Assumes CSV results are in the form: AggScheme/Counterparty/RiskMeasure,result
        private static List<string> GetValue(PricingRequest request, string results, string scheme, string measure)
        {
            var cph = new CounterpartyHierarchy(request.Context.CptyHierarchy);

            var parent = cph.GetParent(request.Cpty);

            var allRegex = new Regex(String.Format(@"{0}/{1},{2},(.*)",
                scheme, String.IsNullOrEmpty(parent) ? request.Cpty : parent, measure));

            var allMatch = allRegex.Match(results);

            return allMatch.Success
                ? allMatch.Groups[1].Value.Split(',').ToList()
                : new List<string>();
        }

        private static Dictionary<string, List<string>> ParseCvaResults(PricingRequest request)
        {
            var cvaResults = File.ReadAllText(request.Context.CsvResults);
            var mapEntries = new Dictionary<string, List<string>>();

            // If no measures supplied default to XVA
            request.Measures = request.Measures ??
                               new[]
                               {"CVA_Collateralized", "DVA_Collateralized", "FVA_Collateralized", "BCVA_Collateralized"};

            var measures = request.Measures.ToList();

            // handle global reporting dates separately (different structure)
            if (measures.Contains("GlobalReportDates"))
            {
                var match = new Regex(@"^GlobalReportDates,(.*)").Match(cvaResults);

                if (match.Success)
                    mapEntries.Add("GlobalReportDates", match.Groups[1].Value.Split(',').ToList());
            }

            foreach (var measure in measures.Where(m => m != "GlobalReportDates"))
            {
                foreach (var scheme in new[] {"AllTradesScheme", "NewTradesScheme", "ExistingTradesScheme"})
                {
                    mapEntries.Add(scheme + "/" + measure, GetValue(request, cvaResults, scheme, measure));
                }
            }

            return mapEntries;
        }

        private static string IRSwapCreditRunningSpread(
            double rCVA,
            double rFVA,
            string strCpty,
            string strCptyFile,
            string strMktDataH5,
            string strHolDataH5,
            string strSchedule,
            Date dtValuation,
            string strCsvLine)
        {
            var request = new DispatchRequest
            {
                Function = new Function
                {
                    Name = "IRSwapCreditRunningSpread",
                    Arguments = new[]
                    {
                        new Variant {Item = rCVA},
                        new Variant {Item = rFVA},
                        new Variant {Item = strCpty},
                        new Variant {Item = strCptyFile},
                        new Variant {Item = strMktDataH5},
                        new Variant {Item = strHolDataH5},
                        new Variant {Item = strSchedule},
                        new Variant {Item = dtValuation},
                        new Variant {Item = strCsvLine}
                    }
                }
            };

#if DISPATCH_DLL
            return RunRequest(request);
#else
            return RunRequest(@"D:/QuIC/modules", @"quic_cva_driver", "11", request);
#endif
        }

        private static string RunCVA_Baseline(
            string strPortfolio,
            string strMarketDataSource,
            string strHolidayDataSource,
            string strModelParam,
            string strModelParameterSet,
            string urnSimParamFile,
            string strListInputsSource,
            string strModuleBindingsSource,
            string strAggregationSchemes,
            string strLegalDoc,
            string strCounterPartyHier,
            string strH5Out,
            string strExportParam,
            string strExportCsv,
            string strRiskMeasurePlugin,
            string strStructureNodePlugin,
            string strWorkDir,
            Boolean bStructuredTrade)
        {
            var request = new DispatchRequest
            {
                Function = new Function
                {
                    Name = "RunCVA_Baseline",
                    Arguments = new[]
                    {
                        new Variant {Item = strPortfolio},
                        new Variant {Item = strMarketDataSource},
                        new Variant {Item = strHolidayDataSource},
                        new Variant {Item = strModelParam},
                        new Variant {Item = strModelParameterSet},
                        new Variant {Item = urnSimParamFile},
                        new Variant {Item = strListInputsSource},
                        new Variant {Item = strModuleBindingsSource},
                        new Variant {Item = strAggregationSchemes},
                        new Variant {Item = strLegalDoc},
                        new Variant {Item = strCounterPartyHier},
                        new Variant {Item = strH5Out},
                        new Variant {Item = strExportParam},
                        new Variant {Item = strExportCsv},
                        new Variant {Item = strRiskMeasurePlugin},
                        new Variant {Item = strStructureNodePlugin},
                        new Variant {Item = strWorkDir},
                        new Variant {Item = bStructuredTrade}
                    }
                }
            };

#if DISPATCH_DLL
            return RunRequest(request);
#else
            return RunRequest(@"D:/QuIC/modules", @"quic_cva_driver", "11", request);
#endif
        }

        // TODO: Create a simparams cache
        private static void UpdateSimParams(string simParams, IDictionary<string, string> dict)
        {
            // Tune the Simulation Parameters
            var lines = File.ReadAllLines(simParams);
            using (var writer = new StreamWriter(simParams))
            {
                foreach (var fields in lines.Select(line => line.Split(',')))
                {
                    if (dict.ContainsKey(fields[0]))
                        fields[1] = dict[fields[0]];

                    writer.WriteLine(String.Join(",", fields));
                }
            }
        }

        private static string UpdateAggregationFields(PricingRequest request, IDictionary<string, string> dict)
        {
            var fields = request.Transaction.Split(',');

            foreach (var key in dict.Keys)
            {
                var idx = new Aggregation(request.Context.Aggregation).FindIndex(key);

                if (idx <= -1) continue;

                fields[idx + 2] = dict[key];
            }

            return String.Join(",", fields);
        }

        #endregion

        // TODO: Memoize this function, i.e. catch results and check for changes to input files. 
        public static DispatchResponse RunBatch(IEnumerable<PricingRequest> requests)
        {
            var response = new DispatchResponse();

            if (!requests.Any())
            {
                response.Status = DispatchResponse.StatusEnum.Failure;
                response.Error = "No Trades to Process, request list empty";
                return response;
            }

            var pr = requests.First();

            var ctx = pr.Context.CreateTemporary(false);

            try
            {
                var cph = new CounterpartyHierarchy(pr.Context.CptyHierarchy);
                var ld = new LegalDocument(pr.Context.LegalDocument);
                var port = new Portfolio(pr.Context.Portfolio, pr.StandAlone);
                var sched = new Schedules(pr.Context.Schedules);

                var tradeIds = new List<string>();

                var cpys = new HashSet<string>();
                var lgds = new HashSet<string>();


                foreach (var pricingRequest in requests)
                {
                    cpys.Add(pricingRequest.Cpty);
                    cpys.Add(cph.GetParent(pricingRequest.Cpty));

                    if (!String.IsNullOrEmpty(pricingRequest.LegalDocumentName))
                        lgds.Add(pricingRequest.LegalDocumentName);
                    else
                    {
                        lgds.UnionWith(ld.GetByCounterParty(pricingRequest.Cpty));
                        lgds.UnionWith(ld.GetByCounterParty(cph.GetParent(pricingRequest.Cpty)));
                    }
                    tradeIds.Add(pricingRequest.TradeId);

                    // Create a set of ignored trades
                    if (pricingRequest.IgnoreTrade)
                        port.IgnoredTrades.Add(pricingRequest.TradeId);

                    // For new trades, set the WhatIf flag to TRUE
                    if (pricingRequest.NewTrade)
                        pricingRequest.Transaction = UpdateAggregationFields(pricingRequest,
                            new Dictionary<string, string> {{"WhatIf", "TRUE"}});

                    // TODO: Clean this up !!!...
                    // Add supplied items to relevant temporary list
                    if (!String.IsNullOrEmpty(pricingRequest.CptyHierarchy))
                        cph.Add(pricingRequest.CptyHierarchy.Split(',')[0], pricingRequest.CptyHierarchy);

                    if (!String.IsNullOrEmpty(pricingRequest.LegalDocument))
                        ld.Add(pricingRequest.LegalDocument.Split(',')[0], pricingRequest.LegalDocument);

                    if (!String.IsNullOrEmpty(pricingRequest.Transaction) && !pricingRequest.IgnoreTrade)
                        port.Add(pricingRequest.Transaction.Split(',')[0], pricingRequest.Transaction);

                    if (!String.IsNullOrEmpty(pricingRequest.Schedules))
                        sched.Add(pricingRequest.Transaction.Split(',')[0], pricingRequest.Schedules);
                }

                var cpList = cpys.SelectMany(cph.GetChildren).Union(cpys);

                // TODO: rewrite to accept list of cpys
                var doc = ld.GetByCounterParties(cpList);

                doc = doc.Where(lgds.Contains);
                
                port.WriteByLegalDocument(ctx.Portfolio, doc);
                sched.WriteByLegalDocument(ctx.Schedules, doc);

                // TODO: change to IEnumerable<string>
                cph.WriteCounterparty(ctx.CptyHierarchy, cpList);
                ld.WriteCounterparty(ctx.LegalDocument, cpList);

                // Tune the Simulation Parameters
                UpdateSimParams(ctx.SimParams, new Dictionary<string, string>
                {
                    {"BreakdownBlocks", "1"},
                    {"RollupBlocks", "1"}
                });

                if (pr.BatchMode == BatchModeEnum.MTM)
                {
                    ctx.RiskMeasurePlugin = ctx.MtmPlugin;
                    ctx.StructureNodePlugin = ctx.MtmPlugin;
                    ctx.H5Results = ctx.MtmH5Results;
                    ctx.CsvResults = ctx.MtmCsvResults;
                    ctx.ExportParams = ctx.MtmExportParams;
                }

                RunCVA_Baseline(ctx.Portfolio,
                    ctx.MarketDataH5,
                    ctx.HolidaysH5,
                    ctx.RiskFactorParamH5,
                    ctx.ModelParamSet,
                    ctx.SimParams,
                    string.Join(",", new[] {ctx.Schedules, ctx.Observables, ctx.SiteInfo}),
                    ctx.ModuleBindings,
                    ctx.Aggregation,
                    ctx.LegalDocument,
                    ctx.CptyHierarchy,
                    ctx.H5Results,
                    ctx.ExportParams,
                    ctx.CsvResults,
                    ctx.RiskMeasurePlugin,
                    ctx.StructureNodePlugin,
                    ctx.Path,
                    true);

                pr.Context = ctx;

                response.Result = ParseCvaResults(pr);
            }
            catch (Exception ex)
            {
                response.Status = DispatchResponse.StatusEnum.Failure;
                response.Error = ex.Message;
            }
#if !DEBUG_FILES
            finally
            {
                ctx.Delete();
                Utilities.DeleteFiles(ctx.Path, ".*");
                Directory.Delete(ctx.Path, true);
            }
#endif
            return response;
        }

        public static DispatchResponse CalcRunningSpread(PricingRequest request)
        {
            var ctx = request.Context.CreateTemporary(false);

            var response = new DispatchResponse();
            try
            {
                var batchResult = RunBatch(new List<PricingRequest> {request});

                if (batchResult.Status == DispatchResponse.StatusEnum.Failure)
                    throw new ArgumentException(batchResult.Error);

                // TODO: move this into a function combine with code in runbatch
                var cph = new CounterpartyHierarchy(request.Context.CptyHierarchy);
                var cpys = new HashSet<string> {request.Cpty, cph.GetParent(request.Cpty)};
                var cpList = cpys.SelectMany(cph.GetChildren).Union(cpys);
                cph.WriteCounterparty(ctx.CptyHierarchy, cpList);

                using (var file = new StreamWriter(ctx.Schedules))
                    file.WriteLine(request.Schedules);

                var incrCva = Convert.ToDouble(batchResult.Result["AllTradesScheme/CVA_Collateralized"][0]) -
                              Convert.ToDouble(batchResult.Result["ExistingTradesScheme/CVA_Collateralized"][0]);

                var incrFva = Convert.ToDouble(batchResult.Result["AllTradesScheme/FVA_Collateralized"][0]) -
                              Convert.ToDouble(batchResult.Result["ExistingTradesScheme/FVA_Collateralized"][0]);

                // Ummm... this is to work around some interesting QS coding in IRSwapCreditRunningSpread (line 1045)
                // Need to prepend 3 dummy fields to the leg map to prevent an index out of bounds exception 
                var transaction = "A,B,C," + request.PricingTransaction;

                var result = IRSwapCreditRunningSpread(
                    incrCva,
                    incrFva,
                    request.Cpty,
                    ctx.CptyHierarchy,
                    ctx.MarketDataH5,
                    ctx.HolidaysH5,
                    string.Join(",", new[] {ctx.Schedules, ctx.Observables, ctx.SiteInfo}),
                    request.ValueDate,
                    transaction);

                response.Result = Utilities.Deserialize<DispatchResult>(result).ToDictionary();
            }
            catch (Exception ex)
            {
                response.Status = DispatchResponse.StatusEnum.Failure;
                response.Error = ex.Message;
            }
#if !DEBUG_FILES
            finally
            {
                ctx.Delete();
                Utilities.DeleteFiles(ctx.Path, ".*");
                Directory.Delete(ctx.Path, true);
            }
#endif
            return response;
        }

        public static DispatchResponse PriceTrade(PricingRequest request)
        {
            var ctx = request.Context.CreateTemporary(false);

            var response = new DispatchResponse();

            try
            {
                using (var file = new StreamWriter(ctx.Schedules))
                    file.WriteLine(request.Schedules);

                var dispReq = new DispatchRequest
                {
                    Function = new Function
                    {
                        Name = ServiceConfig.Products[request.Product].Alias,
                        Arguments = new[]
                        {
                            new Variant
                            {
                                Item = new Vector
                                {
                                    Variant = request.Measures.Select(measure => new Variant {Item = measure}).ToArray()
                                }
                            },
                            new Variant {Item = ctx.MarketDataH5},
                            new Variant {Item = ctx.HolidaysH5},
                            new Variant {Item = string.Join(",", new[] {ctx.Schedules, ctx.Observables, ctx.SiteInfo})},
                            new Variant {Item = (DateTime) request.ValueDate},
                            new Variant {Item = request.PricingTransaction}
                        }
                    }
                };

#if DISPATCH_DLL
                var result = RunRequest(dispReq);
#else
                var result =  RunRequest(@"D:/QuIC/modules", @"quic_cva_driver", "11", dispReq);
#endif
                response.Result = Utilities.Deserialize<DispatchResult>(result).ToDictionary();
            }
            catch (Exception e)
            {
                response.Status = DispatchResponse.StatusEnum.Failure;
                response.Error = (e.InnerException != null) ? e.InnerException.Message : e.Message;
            }
#if !DEBUG_FILES
            finally
            {
                ctx.Delete();
                Utilities.DeleteFiles(ctx.Path, ".*");
                Directory.Delete(ctx.Path, true);
            }
#endif
            return response;
        }
    }
}