using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DDay.iCal.Test
{
    [TestFixture]
    public class AlarmTest
    {
        private string tzid;

        [TestFixtureSetUp]
        public void InitAll()
        {
            tzid = "US-Eastern";
        }

        public void TestAlarm(string calendar, List<IDateTime> dates, iCalDateTime start, iCalDateTime end)
        {
            var iCal = iCalendar.LoadFromFile(@"Calendars\Alarm\" + calendar)[0];
            ProgramTest.TestCal(iCal);
            var evt = iCal.Events.First();

            // Poll all alarms that occurred between Start and End
            var alarms = evt.PollAlarms(start, end);

            foreach (var alarm in alarms)
            {
                Assert.IsTrue(dates.Contains(alarm.DateTime), "Alarm triggers at " + alarm.Period.StartTime + ", but it should not.");
            }
            Assert.IsTrue(dates.Count == alarms.Count, "There were " + alarms.Count + " alarm occurrences; there should have been " + dates.Count + ".");
        }

        [Test, Category("Alarm")]
        public void Alarm1()
        {
            var dateTimes = new List<IDateTime>();
            dateTimes.AddRange(new[] {new iCalDateTime(2006, 7, 18, 9, 30, 0, tzid)});

            TestAlarm("Alarm1.ics", dateTimes, new iCalDateTime(2006, 7, 1, tzid), new iCalDateTime(2006, 9, 1, tzid));
        }

        [Test, Category("Alarm")]
        public void Alarm2()
        {
            var dateTimes = new List<IDateTime>();
            dateTimes.AddRange(new[]
            {
                new iCalDateTime(2006, 7, 18, 9, 30, 0, tzid), new iCalDateTime(2006, 7, 20, 9, 30, 0, tzid), new iCalDateTime(2006, 7, 22, 9, 30, 0, tzid),
                new iCalDateTime(2006, 7, 24, 9, 30, 0, tzid), new iCalDateTime(2006, 7, 26, 9, 30, 0, tzid), new iCalDateTime(2006, 7, 28, 9, 30, 0, tzid),
                new iCalDateTime(2006, 7, 30, 9, 30, 0, tzid), new iCalDateTime(2006, 8, 1, 9, 30, 0, tzid), new iCalDateTime(2006, 8, 3, 9, 30, 0, tzid),
                new iCalDateTime(2006, 8, 5, 9, 30, 0, tzid)
            });

            TestAlarm("Alarm2.ics", dateTimes, new iCalDateTime(2006, 7, 1, tzid), new iCalDateTime(2006, 9, 1, tzid));
        }

        [Test, Category("Alarm")]
        public void Alarm3()
        {
            var dateTimes = new List<IDateTime>();
            dateTimes.AddRange(new[]
            {
                new iCalDateTime(1998, 2, 11, 9, 0, 0, tzid), new iCalDateTime(1998, 3, 11, 9, 0, 0, tzid), new iCalDateTime(1998, 11, 11, 9, 0, 0, tzid),
                new iCalDateTime(1999, 8, 11, 9, 0, 0, tzid), new iCalDateTime(2000, 10, 11, 9, 0, 0, tzid)
            });

            TestAlarm("Alarm3.ics", dateTimes, new iCalDateTime(1997, 1, 1, tzid), new iCalDateTime(2000, 12, 31, tzid));
        }

        [Test, Category("Alarm")]
        public void Alarm4()
        {
            var dateTimes = new List<IDateTime>();
            dateTimes.AddRange(new[]
            {
                new iCalDateTime(1998, 2, 11, 9, 0, 0, tzid), new iCalDateTime(1998, 2, 11, 11, 0, 0, tzid), new iCalDateTime(1998, 2, 11, 13, 0, 0, tzid),
                new iCalDateTime(1998, 2, 11, 15, 0, 0, tzid), new iCalDateTime(1998, 3, 11, 9, 0, 0, tzid), new iCalDateTime(1998, 3, 11, 11, 0, 0, tzid),
                new iCalDateTime(1998, 3, 11, 13, 0, 0, tzid), new iCalDateTime(1998, 3, 11, 15, 0, 0, tzid), new iCalDateTime(1998, 11, 11, 9, 0, 0, tzid),
                new iCalDateTime(1998, 11, 11, 11, 0, 0, tzid), new iCalDateTime(1998, 11, 11, 13, 0, 0, tzid), new iCalDateTime(1998, 11, 11, 15, 0, 0, tzid),
                new iCalDateTime(1999, 8, 11, 9, 0, 0, tzid), new iCalDateTime(1999, 8, 11, 11, 0, 0, tzid), new iCalDateTime(1999, 8, 11, 13, 0, 0, tzid),
                new iCalDateTime(1999, 8, 11, 15, 0, 0, tzid), new iCalDateTime(2000, 10, 11, 9, 0, 0, tzid), new iCalDateTime(2000, 10, 11, 11, 0, 0, tzid),
                new iCalDateTime(2000, 10, 11, 13, 0, 0, tzid), new iCalDateTime(2000, 10, 11, 15, 0, 0, tzid)
            });

            TestAlarm("Alarm4.ics", dateTimes, new iCalDateTime(1997, 1, 1, tzid), new iCalDateTime(2000, 12, 31, tzid));
        }

        [Test, Category("Alarm")]
        public void Alarm5()
        {
            var dateTimes = new List<IDateTime>();
            dateTimes.AddRange(new[] {new iCalDateTime(1998, 1, 2, 8, 0, 0, tzid)});

            TestAlarm("Alarm5.ics", dateTimes, new iCalDateTime(1997, 7, 1, tzid), new iCalDateTime(2000, 12, 31, tzid));
        }

        [Test, Category("Alarm")]
        public void Alarm6()
        {
            var DateTimes = new List<IDateTime>();
            DateTimes.AddRange(new[]
            {
                new iCalDateTime(1998, 1, 2, 8, 0, 0, tzid), new iCalDateTime(1998, 1, 5, 8, 0, 0, tzid), new iCalDateTime(1998, 1, 8, 8, 0, 0, tzid),
                new iCalDateTime(1998, 1, 11, 8, 0, 0, tzid), new iCalDateTime(1998, 1, 14, 8, 0, 0, tzid), new iCalDateTime(1998, 1, 17, 8, 0, 0, tzid)
            });

            TestAlarm("Alarm6.ics", DateTimes, new iCalDateTime(1997, 7, 1, tzid), new iCalDateTime(2000, 12, 31, tzid));
        }

        [Test, Category("Alarm")]
        public void Alarm7()
        {
            var dateTimes = new List<IDateTime>();
            dateTimes.AddRange(new[]
            {
                new iCalDateTime(2006, 7, 18, 10, 30, 0, tzid), new iCalDateTime(2006, 7, 20, 10, 30, 0, tzid), new iCalDateTime(2006, 7, 22, 10, 30, 0, tzid),
                new iCalDateTime(2006, 7, 24, 10, 30, 0, tzid), new iCalDateTime(2006, 7, 26, 10, 30, 0, tzid), new iCalDateTime(2006, 7, 28, 10, 30, 0, tzid),
                new iCalDateTime(2006, 7, 30, 10, 30, 0, tzid), new iCalDateTime(2006, 8, 1, 10, 30, 0, tzid), new iCalDateTime(2006, 8, 3, 10, 30, 0, tzid),
                new iCalDateTime(2006, 8, 5, 10, 30, 0, tzid)
            });

            TestAlarm("Alarm7.ics", dateTimes, new iCalDateTime(2006, 7, 1, tzid), new iCalDateTime(2006, 9, 1, tzid));
        }
    }
}