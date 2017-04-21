using System;
using System.Collections.Generic;
using System.Linq;
using Quic.DateSchedule;
using Quic.FPML;

namespace Quic.Templates
{
    public partial class BermudanExerciseMap
    {
        public string strTradeId { get; set; }
        public string adtExercise { get; set; }
        public string adtSwapStart { get; set; }
        public string strLongShort { get; set; }

        public BermudanExerciseMap()
        {
            strTradeId = String.Empty;
        }

        public BermudanExerciseMap(string tradeId, FPML.Swaption product, IReadOnlyList<Tuple<string, string>> parties)
            : base(tradeId)
        {
            strTradeId = tradeId;

            strLongShort = product.buyerPartyReference.href == parties[0].Item1 ? "LONG" : "SHORT";

            var exercise = product.Item as BermudaExercise;

            if (exercise != null)
            {
                var adjDates = (exercise.bermudaExerciseDates.Item as AdjustableDates);

                if (adjDates != null)
                {
                    if (adjDates.adjustedDate != null)
                    {
                        adtExercise = String.Join(",",
                            adjDates.adjustedDate.Select(dt => dt.Value.ToString("yyyy/MM/dd")));
                    }
                    else
                    {
                        var bdc = adjDates.dateAdjustments.businessDayConventionSpecified
                            ? (DateSchedule.BusinessDayConventionEnum)
                                (Enum.Parse(typeof (DateSchedule.BusinessDayConventionEnum),
                                    adjDates.dateAdjustments.businessDayConvention.ToString()))
                            : DateSchedule.BusinessDayConventionEnum.NONE;

                        var cals = Utilities.ConvertCalendars(adjDates.dateAdjustments.businessCenters != null
                            ? adjDates.dateAdjustments.businessCenters.businessCenter.Select(s => s.Value)
                            : new String[0]);

                        adtExercise = String.Join(",",
                            adjDates.unadjustedDate.Select(
                                dt => new BusinessDate(dt.Value, new Calendar(cals), bdc).ToString("yyyy/MM/dd")));
                    }

                    var relDates = exercise.relevantUnderlyingDate.Item as RelativeDates;

                    if (relDates != null)
                    {
                        var multiplier = Int32.Parse(relDates.periodMultiplier);
                        var period =
                            (DateSchedule.PeriodEnum)
                                (Enum.Parse(typeof (DateSchedule.PeriodEnum), relDates.period.ToString()));

                        var bdc = relDates.businessDayConventionSpecified
                            ? (DateSchedule.BusinessDayConventionEnum)
                                (Enum.Parse(typeof (DateSchedule.BusinessDayConventionEnum),
                                    relDates.businessDayConvention.ToString()))
                            : DateSchedule.BusinessDayConventionEnum.NONE;

                        var cals =  Utilities.ConvertCalendars(relDates.businessCenters != null
                            ? relDates.businessCenters.businessCenter.Select(s => s.Value)
                            : new String[0]);

                        adtSwapStart = String.Join(",", adtExercise.Split(',')
                            .Select(dt => new BusinessDate(DateTime.Parse(dt), new Calendar(cals), bdc)
                                .Add(multiplier, period, bdc, new Calendar(cals)).ToString("yyyy/MM/dd")));
                    }
                }
            }
        }

    }
}
