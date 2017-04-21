using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quic.DateSchedule;

namespace WebServiceUnitTests
{
    [TestClass]
    public class ScheduleUnitTest
    {
        [TestMethod]
        public void TestYearFraction()
        {
            var d1 = new DateTime(2012, 2, 29);
            var d2 = new DateTime(2012, 3, 29);

            DayCount dc = new Act365();
            var fract = dc.YearFraction(d1, d2);
            Assert.AreEqual(fract,1.0);
        }
    }
}
