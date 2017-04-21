using System;
using System.Collections.Generic;
using System.Linq;
using Quic.FPML;

namespace Quic.Templates
{
    public partial class Swaption
    {
        // TODO: Store these with respective component legs
        public string strExerciseMap { get; set; }
        public string strLeg1Name { get; set; }
        public string strLeg2Name { get; set; }
        public string strSolverPrefs { get; set; }
        public List<SwapLeg> Legs { get; set; } 
        public TradeMap ExerciseMap { get; set; }

        public Swaption()
        {
            Legs = new List<SwapLeg>();
            ExerciseMap = new EuropeanExerciseMap();
        }

        public Swaption(FPML.Trade trade, IReadOnlyList<Tuple<string, string>> parties)
            : base(trade, parties)
        {
            strExerciseMap = String.Format("{0}-{1}", strTradeId, "EARLY-EXERCISE-MAP");
            strLeg1Name = String.Format("{0}-{1}", strTradeId, "REC");
            strLeg2Name = String.Format("{0}-{1}", strTradeId, "PAY");

            strSolverPrefs = String.Empty;

            var product = trade.Item as FPML.Swaption;

            if (product != null)
            {
                Legs = new List<SwapLeg>();

                if (product.Item == null)
                    throw new ArgumentException(
                        @"Exercise type is null, define as: <exercise xsi:type=""BermudaExercise""");

                var bermOverride = false;

                if (product.Item.GetType() == typeof(EuropeanExercise))
                    ExerciseMap = new EuropeanExerciseMap(strExerciseMap,product,parties);
                else if (product.Item.GetType() == typeof(BermudaExercise))
                {
                    ExerciseMap = new BermudanExerciseMap(strExerciseMap,product,parties);
                    strSolverPrefs = product.premium[0].paymentAmount.currency.Value + "BermSwpnModelInfo";
                    strProduct = "IRGenericQMFD";
                    bermOverride = true;
                }
                else
                {
                    throw new ArgumentException(String.Format("Unsupported exercise type: {0}", product.Item.GetType().Name));
                }

                var xccyFlag = Utilities.IsXccySwap(product.swap.swapStream);

                foreach (var interestRateStream in product.swap.swapStream)
                {
                    var swapLeg = Utilities.IsFloat(interestRateStream)
                        ? (SwapLeg)new SwapLegFloat(strTradeId, xccyFlag, bermOverride)
                        : new SwapLegFixed(strTradeId, true);

                    swapLeg.CreateSwapLeg(interestRateStream,parties);

                    Legs.Add(swapLeg);
                }
            }
        }

        public override string GenerateSchedule()
        {
            var swapLegs = Legs.Aggregate("", (current, swapLeg) => current + swapLeg.TransformText());
            return swapLegs + ExerciseMap.TransformText();
        }
    }
}
