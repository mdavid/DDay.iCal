using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Resources;
using System.Web;
using System.Web.UI;
using DDay.iCal.Components;
using DDay.iCal.DataTypes;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace DDay.iCal
{
    [TestFixture]
    public class Program
    {
        static public void Main(string[] args)
        {
            Program p = new Program();
            p.Init();
            p.LoadFromFile();
            p.LoadFromUri();
            p.RRULE1();
            p.RRULE2();
            p.RRULE3();
            p.RRULE4();
            p.RRULE5();
            // p.RRULE6(); FIXME: should work when TimeZones are implemented
            p.RRULE7();
            p.RRULE8();
            p.RRULE9();
            p.RRULE10();
            p.RRULE11();
            p.RRULE12();
            p.RRULE13();
            p.RRULE14();
            p.RRULE15();
            p.RRULE16();
            p.RRULE17();
            p.RRULE18();
            p.RRULE19();
            p.RRULE20();
            p.RRULE21();
            p.RRULE22();
            p.RRULE23();
            p.RRULE24();
            p.RRULE25();
            p.RRULE26();
            p.RRULE27();
            p.RRULE28();
            p.RRULE29();
            p.RRULE30();
            p.RRULE31();
            p.RRULE32();
            p.RRULE33();
            p.RRULE34();
            //p.RRULE35();
            p.RRULE36();
            p.RRULE37();
            p.RRULE38();
            p.RRULE39();
            p.RRULE40();
            p.RRULE41();
            p.LoadAndDisplayCalendar();
        }
                
        [Test, Explicit]
        public void LoadAndDisplayCalendar()
        {            
            iCalendar iCal = iCalendar.LoadFromUri(new Uri("http://www.applegatehomecare.com/Calendars/USHolidays.ics"));

            foreach (Event evt in iCal.Events)
                evt.Evaluate(new DateTime(2006, 1, 1), new DateTime(2006, 12, 31));

            DateTime dt = new DateTime(2006, 1, 1);
            while (dt.Year == 2006)
            {
                Console.WriteLine(dt.ToShortDateString());
                foreach (Event evt in iCal.Events)
                {
                    if (evt.OccursOn(dt))
                        Console.WriteLine("\t" + evt.Summary + (evt.DTStart.HasTime ? " (" + evt.DTStart.Value.ToShortTimeString() + " - " + evt.DTEnd.Value.ToShortTimeString() + ")" : ""));
                }

                dt = dt.AddDays(1);
            }
        }

        private DateTime Start;
        private DateTime End;
        private DateTime StartAll;
        private DateTime EndAll;

        [TestFixtureSetUp]
        public void InitAll()
        {
            StartAll = DateTime.Now;
        }

        [TestFixtureTearDown]
        public void DisposeAll()
        {
            EndAll = DateTime.Now;
            Console.WriteLine("Total Processing Time: " + EndAll.Subtract(StartAll).Milliseconds + "ms");
        }

        [SetUp]
        public void Init()
        {
            Start = DateTime.Now;
        }

        [TearDown]
        public void Dispose()
        {
            End = DateTime.Now;
            Console.WriteLine("Time: " + End.Subtract(Start).Milliseconds + "ms");
        }

        private void TestCal(iCalendar iCal)
        {
            Assert.IsNotNull(iCal, "The iCalendar was not loaded");
            Assert.IsTrue(iCal.Events.Count == 1, "Calendar should contain 1 event; however, the iCalendar loaded " + iCal.Events.Count + " events");
        }

        [Test]
        public void LoadFromFile()
        {
            string path = @"Calendars\Test1.ics";
            Assert.IsTrue(File.Exists(path), "File '" + path + "' does not exist.");
            
            iCalendar iCal = iCalendar.LoadFromFile(path);
            TestCal(iCal);
        }

        [Test]
        public void LoadFromUri()
        {
            string path = Directory.GetCurrentDirectory();            
            path = Path.Combine(path, "Calendars/Test1.ics").Replace(@"\", "/");
            path = "file:///" + path;            
            Uri uri = new Uri(path);
            iCalendar iCal = iCalendar.LoadFromUri(uri);
            TestCal(iCal);
        }

        /// <summary>
        /// See Page 118 of RFC 2445 - RRULE:FREQ=YEARLY;INTERVAL=2;BYMONTH=1;BYDAY=SU;BYHOUR=8,9;BYMINUTE=30
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE1()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE1.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(2006, 1, 1), new DateTime(2011, 1, 1));

            DateTime dt = new DateTime(2006, 1, 1, 8, 30, 0);
            while (dt.Year < 2011)
            {
                if ((dt > evt.DTStart.Value) && 
                    (dt.Year % 2 == 0) &&
                    (dt.Month == 1) &&
                    (dt.DayOfWeek == DayOfWeek.Sunday))
                {
                    DateTime dt1 = dt.AddHours(1);
                    Assert.IsTrue(evt.OccursAt(dt), "Event should occur at " + dt);
                    Assert.IsTrue(evt.OccursAt(dt1), "Event should occur at " + dt1);
                }
                else if (dt.Date != evt.DTStart.Value.Date)
                    Assert.IsFalse(evt.OccursOn(dt), "Event should not occur on " + dt.ToShortDateString());

                dt = dt.AddDays(1);
            }
        }

        /// <summary>
        /// See Page 118 of RFC 2445 - RRULE:FREQ=DAILY;COUNT=10;INTERVAL=2
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE2()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE2.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(2006, 7, 1), new DateTime(2006, 9, 1));

            // FIXME: this example takes into account the change between EST and EDT, see pg. 118 of RFC 2445
            // We need to make sure we do the same
            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(2006, 07, 18, 10, 00, 00, DateTimeKind.Local),
                new DateTime(2006, 07, 20, 10, 00, 00, DateTimeKind.Local),
                new DateTime(2006, 07, 22, 10, 00, 00, DateTimeKind.Local),
                new DateTime(2006, 07, 24, 10, 00, 00, DateTimeKind.Local),
                new DateTime(2006, 07, 26, 10, 00, 00, DateTimeKind.Local),
                new DateTime(2006, 07, 28, 10, 00, 00, DateTimeKind.Local),
                new DateTime(2006, 07, 30, 10, 00, 00, DateTimeKind.Local),
                new DateTime(2006, 08, 01, 10, 00, 00, DateTimeKind.Local),
                new DateTime(2006, 08, 03, 10, 00, 00, DateTimeKind.Local),
                new DateTime(2006, 08, 05, 10, 00, 00, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 118 of RFC 2445 - RRULE:FREQ=DAILY;UNTIL=19971224T000000Z
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE3()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE3.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1997, 9, 1), new DateTime(1998, 1, 1));

            // FIXME: this example takes into account the change between EST and EDT, see pg. 118 of RFC 2445
            // We need to make sure we do the same
            DateTime dt = new DateTime(1997, 9, 2, 9, 0, 0);
            while (dt.Year < 1998)
            {
                if ((dt >= evt.DTStart.Value) &&
                    (dt < new DateTime(1997, 12, 24, 0, 0, 0, DateTimeKind.Utc)))
                    Assert.IsTrue(evt.OccursAt(dt), "Event should occur at " + dt);
                else Assert.IsFalse(evt.OccursAt(dt), "Event should not occur at " + dt);

                dt = dt.AddDays(1);
            }
        }

        /// <summary>
        /// See Page 118 of RFC 2445 - RRULE:FREQ=DAILY;INTERVAL=2
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE4()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE4.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1997, 9, 1), new DateTime(1997, 12, 4));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 4, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 6, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 8, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 12, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 14, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 16, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 18, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 20, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 22, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 24, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 26, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 28, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 4, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 6, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 8, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 12, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 14, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 16, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 18, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 20, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 22, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 24, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 26, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 28, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 3, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 5, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 7, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 9, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 15, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 17, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 19, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 21, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 23, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 25, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 27, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 29, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 3, 9, 0, 0, DateTimeKind.Local)                
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 119 of RFC 2445 - RRULE:FREQ=DAILY;INTERVAL=10;COUNT=5
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE5()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE5.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1997, 9, 1), new DateTime(1998, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 12, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 22, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 12, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 119 of RFC 2445 - RRULE:FREQ=DAILY;UNTIL=20000131T090000Z;BYMONTH=1
        /// FIXME: add alternative RRULE test that is indicated on RFC 2445 for the same recurrence
        /// </summary>
        [Test, Category("RRULE")]
        [Ignore("This should function when TimeZones are implemented")]
        public void RRULE6()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE6.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1997, 9, 1), new DateTime(2000, 2, 1));

            // FIXME: this fails due to the difference in UTC and floating time (since TimeZone's aren't implemented,
            // the DTSTART is treated as floating.  When TimeZones are implemented, this rule should function.
            DateTime dt = new DateTime(1998, 1, 1, 9, 0, 0);
            while (dt.Year < 2001)
            {
                if (dt >= evt.DTStart.Value &&
                    dt.Month == 1 &&
                    dt <= new DateTime(2000, 1, 31, 9, 0, 0))
                    Assert.IsTrue(evt.OccursAt(dt), "Event should occur at " + dt);
                else Assert.IsFalse(evt.OccursAt(dt), "Event should not occur at " + dt);

                dt = dt.AddDays(1);
            }
        }

        /// <summary>
        /// See Page 119 of RFC 2445 - RRULE:FREQ=WEEKLY;COUNT=10
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE7()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE7.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1997, 9, 1), new DateTime(1998, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 9, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 16, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 23, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 7, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 14, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 21, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 28, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 4, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 119 of RFC 2445 - RRULE:FREQ=WEEKLY;UNTIL=19971224T000000Z
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE8()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE8.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1997, 9, 1), new DateTime(1999, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 9, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 16, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 23, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 7, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 14, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 21, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 28, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 4, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 18, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 25, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 9, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 16, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 23, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        
        }

        /// <summary>
        /// See Page 119 of RFC 2445 - RRULE:FREQ=WEEKLY;INTERVAL=2;WKST=SU
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE9()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE9.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1997, 9, 1), new DateTime(1998, 1, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 16, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 14, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 28, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 25, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 9, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 23, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 6, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 20, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 119 of RFC 2445 - RRULE:FREQ=WEEKLY;UNTIL=19971007T000000Z;WKST=SU;BYDAY=TU,TH
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE10()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE10.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1997, 9, 1), new DateTime(1999, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 4, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 9, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 16, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 18, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 23, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 25, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 2, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 120 of RFC 2445 - RRULE:FREQ=WEEKLY;COUNT=10;WKST=SU;BYDAY=TU,TH
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE11()
        {
            iCalendar iCal1 = iCalendar.LoadFromFile(@"Calendars\RRULE10.ics");
            iCalendar iCal2 = iCalendar.LoadFromFile(@"Calendars\RRULE11.ics");
            TestCal(iCal1);
            TestCal(iCal2);            
            Event evt1 = (Event)iCal1.Events[0];
            Event evt2 = (Event)iCal2.Events[0];

            evt1.Evaluate(new DateTime(1997, 9, 1), new DateTime(1999, 1, 1));
            evt2.Evaluate(new DateTime(1997, 9, 1), new DateTime(1999, 1, 1));
            Assert.IsTrue(evt1.Periods.Count == evt2.Periods.Count, "RRULE11 does not match RRULE10 as it should");
            for (int i = 0; i < evt1.Periods.Count; i++)
                Assert.IsTrue(evt1.Periods[i].Equals(evt2.Periods[i]), "PERIOD " + i + " from RRULE10 (" + evt1.Periods[i].ToString() + ") does not match PERIOD " + i + " from RRULE11 (" + evt2.Periods[i].ToString() + ")");
        }

        /// <summary>
        /// See Page 120 of RFC 2445 - RRULE:FREQ=WEEKLY;INTERVAL=2;UNTIL=19971224T000000Z;WKST=SU;BYDAY=MO,WE,FR
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE12()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE12.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1999, 1, 1));

            // FIXME: this example takes into account the change between EST and EDT, see pg. 120 of RFC 2445
            // We need to make sure we do the same
            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 5, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 15, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 17, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 19, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 29, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 3, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 15, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 17, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 27, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 29, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 31, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 12, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 14, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 24, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 26, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 28, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 8, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 12, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 22, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 120 of RFC 2445 - RRULE:FREQ=WEEKLY;INTERVAL=2;COUNT=8;WKST=SU;BYDAY=TU,TH
        /// </summary>
        [Test, Category("RRULE")]        
        public void RRULE13()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE13.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1999, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 4, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 16, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 18, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 14, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 16, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 120 of RFC 2445 - RRULE:FREQ=MONTHLY;COUNT=10;BYDAY=1FR
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE14()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE14.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1999, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 5, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 3, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 7, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 5, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 2, 6, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 6, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 4, 3, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 5, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 6, 5, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 120 of RFC 2445 - RRULE:FREQ=MONTHLY;UNTIL=19971224T000000Z;BYDAY=1FR
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE15()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE15.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1999, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 5, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 3, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 7, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 5, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 120 of RFC 2445 - RRULE:FREQ=MONTHLY;INTERVAL=2;COUNT=10;BYDAY=1SU,-1SU
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE16()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE16.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1999, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 7, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 28, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 4, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 25, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 29, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 5, 3, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 5, 31, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 121 of RFC 2445 - RRULE:FREQ=MONTHLY;COUNT=6;BYDAY=-2MO
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE17()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE17.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1999, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 22, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 20, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 17, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 22, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 19, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 2, 16, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 121 of RFC 2445 - RRULE:FREQ=MONTHLY;BYMONTHDAY=-3
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE18()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE18.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1998, 3, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 28, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 29, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 28, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 29, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 29, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 2, 26, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 121 of RFC 2445 - RRULE:FREQ=MONTHLY;COUNT=10;BYMONTHDAY=2,15
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE19()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE19.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1998, 3, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 15, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 15, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 15, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 15, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 15, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 121 of RFC 2445 - RRULE:FREQ=MONTHLY;COUNT=10;BYMONTHDAY=1,-1
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE20()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE20.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1998, 3, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 31, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 31, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 31, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 2, 1, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 121 of RFC 2445 - RRULE:FREQ=MONTHLY;INTERVAL=18;COUNT=10;BYMONTHDAY=10,11,12,13,14,15
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE21()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE21.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(2000, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 12, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 14, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 15, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 3, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 3, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 3, 12, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 3, 13, 9, 0, 0, DateTimeKind.Local),
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 122 of RFC 2445 - RRULE:FREQ=MONTHLY;INTERVAL=2;BYDAY=TU
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE22()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE22.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1998, 4, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 9, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 16, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 23, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 4, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 18, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 25, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 6, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 20, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 27, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 3, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 17, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 24, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 31, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 122 of RFC 2445 - RRULE:FREQ=YEARLY;COUNT=10;BYMONTH=6,7
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE23()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE23.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(2002, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 6, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 7, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 6, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 7, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 6, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 7, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2000, 6, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2000, 7, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2001, 6, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2001, 7, 10, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 122 of RFC 2445 - RRULE:FREQ=YEARLY;INTERVAL=2;COUNT=10;BYMONTH=1,2,3
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE24()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE24.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(2003, 4, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 3, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 1, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 2, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 3, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2001, 1, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2001, 2, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2001, 3, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2003, 1, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2003, 2, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2003, 3, 10, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 122 of RFC 2445 - RRULE:FREQ=YEARLY;INTERVAL=3;COUNT=10;BYYEARDAY=1,100,200
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE25()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE25.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(2007, 1, 1));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 1, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 4, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 7, 19, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2000, 1, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2000, 4, 9, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2000, 7, 18, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2003, 1, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2003, 4, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2003, 7, 19, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2006, 1, 1, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 123 of RFC 2445 - RRULE:FREQ=YEARLY;BYDAY=20MO
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE26()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE26.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1999, 12, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 5, 19, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 5, 18, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 5, 17, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 123 of RFC 2445 - RRULE:FREQ=YEARLY;BYWEEKNO=20;BYDAY=MO
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE27()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE27.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1999, 12, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 5, 12, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 5, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 5, 17, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 123 of RFC 2445 - RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=TH
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE28()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE28.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1999, 12, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 3, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 3, 20, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 3, 27, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 5, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 12, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 19, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 26, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 3, 4, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 3, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 3, 18, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 3, 25, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 123 of RFC 2445 - RRULE:FREQ=YEARLY;BYDAY=TH;BYMONTH=6,7,8
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE29()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE29.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1999, 12, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 6, 5, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 6, 12, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 6, 19, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 6, 26, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 7, 3, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 7, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 7, 17, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 7, 24, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 7, 31, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 8, 7, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 8, 14, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 8, 21, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 8, 28, 9, 0, 0, DateTimeKind.Local),                
                new DateTime(1998, 6, 4, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 6, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 6, 18, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 6, 25, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 7, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 7, 9, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 7, 16, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 7, 23, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 7, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 8, 6, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 8, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 8, 20, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 8, 27, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 6, 3, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 6, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 6, 17, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 6, 24, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 7, 1, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 7, 8, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 7, 15, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 7, 22, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 7, 29, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 8, 5, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 8, 12, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 8, 19, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 8, 26, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 123 of RFC 2445:
        /// EXDATE;TZID=US-Eastern:19970902T090000
        /// RRULE:FREQ=MONTHLY;BYDAY=FR;BYMONTHDAY=13
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE30()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE30.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(2000, 12, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1998, 2, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 11, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1999, 8, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2000, 10, 13, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 124 of RFC 2445 - RRULE:FREQ=MONTHLY;BYDAY=SA;BYMONTHDAY=7,8,9,10,11,12,13
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE31()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE31.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1998, 6, 30));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 8, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 13, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 2, 7, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 7, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 4, 11, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 5, 9, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 6, 13, 9, 0, 0, DateTimeKind.Local),
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 124 of RFC 2445 - RRULE:FREQ=YEARLY;INTERVAL=4;BYMONTH=11;BYDAY=TU;BYMONTHDAY=2,3,4,5,6,7,8
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE32()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE32.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(2004, 12, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1996, 11, 5, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2000, 11, 7, 9, 0, 0, DateTimeKind.Local),
                new DateTime(2004, 11, 2, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 124 of RFC 2445 - RRULE:FREQ=MONTHLY;COUNT=3;BYDAY=TU,WE,TH;BYSETPOS=3
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE33()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE33.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(2004, 12, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 4, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 7, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 6, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 124 of RFC 2445 - RRULE:FREQ=MONTHLY;BYDAY=MO,TU,WE,TH,FR;BYSETPOS=-2
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE34()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE34.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1998, 3, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 29, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 10, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 11, 27, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 12, 30, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 1, 29, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 2, 26, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1998, 3, 30, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 125 of RFC 2445 - RRULE:FREQ=HOURLY;INTERVAL=3;UNTIL=19970902T170000Z
        /// </summary>
        [Test, Category("RRULE"), Ignore("This should function when TimeZones are implemented")]
        public void RRULE35()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE35.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1998, 3, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 12, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 15, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 125 of RFC 2445 - RRULE:FREQ=MINUTELY;INTERVAL=15;COUNT=6
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE36()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE36.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1997, 9, 2), new DateTime(1997, 9, 3));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 9, 15, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 9, 30, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 9, 45, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 10, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 10, 15, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 125 of RFC 2445 - RRULE:FREQ=MINUTELY;INTERVAL=90;COUNT=4
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE37()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE37.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1998, 12, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 10, 30, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 12, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 13, 30, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 125 of RFC 2445 - RRULE:FREQ=DAILY;BYHOUR=9,10,11,12,13,14,15,16;BYMINUTE=0,20,40
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE38()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE38.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1997, 9, 2), new DateTime(1997, 9, 4));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 9, 2, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 9, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 9, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 10, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 10, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 10, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 11, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 11, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 11, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 12, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 12, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 12, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 13, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 13, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 13, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 14, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 14, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 14, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 15, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 15, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 15, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 16, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 16, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 2, 16, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 9, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 9, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 10, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 10, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 10, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 11, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 11, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 11, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 12, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 12, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 12, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 13, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 13, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 13, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 14, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 14, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 14, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 15, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 15, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 15, 40, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 16, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 16, 20, 0, DateTimeKind.Local),
                new DateTime(1997, 9, 3, 16, 40, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 125 of RFC 2445 - RRULE:FREQ=MINUTELY;INTERVAL=20;BYHOUR=9,10,11,12,13,14,15,16
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE39()
        {
            iCalendar iCal1 = iCalendar.LoadFromFile(@"Calendars\RRULE38.ics");
            iCalendar iCal2 = iCalendar.LoadFromFile(@"Calendars\RRULE39.ics");
            TestCal(iCal1);
            TestCal(iCal2);
            Event evt1 = (Event)iCal1.Events[0];
            Event evt2 = (Event)iCal2.Events[0];

            evt1.Evaluate(new DateTime(1997, 9, 1), new DateTime(1997, 9, 3));
            evt2.Evaluate(new DateTime(1997, 9, 1), new DateTime(1997, 9, 3));
            Assert.IsTrue(evt1.Periods.Count == evt2.Periods.Count, "RRULE39 does not match RRULE38 as it should");
            for (int i = 0; i < evt1.Periods.Count; i++)
                Assert.IsTrue(evt1.Periods[i].Equals(evt2.Periods[i]), "PERIOD " + i + " from RRULE38 (" + evt1.Periods[i].ToString() + ") does not match PERIOD " + i + " from RRULE39 (" + evt2.Periods[i].ToString() + ")");
        }

        /// <summary>
        /// See Page 125 of RFC 2445 - RRULE:FREQ=WEEKLY;INTERVAL=2;COUNT=4;BYDAY=TU,SU;WKST=MO
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE40()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE40.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1998, 12, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 8, 5, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 8, 10, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 8, 19, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 8, 24, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }

        /// <summary>
        /// See Page 125 of RFC 2445 - RRULE:FREQ=WEEKLY;INTERVAL=2;COUNT=4;BYDAY=TU,SU;WKST=SU
        /// This is the same as RRULE40, except WKST is SU, which changes the results.
        /// </summary>
        [Test, Category("RRULE")]
        public void RRULE41()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\RRULE41.ics");
            TestCal(iCal);
            Event evt = (Event)iCal.Events[0];
            evt.Evaluate(new DateTime(1996, 1, 1), new DateTime(1998, 12, 31));

            DateTime[] DateTimes = new DateTime[]
            {
                new DateTime(1997, 8, 5, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 8, 17, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 8, 19, 9, 0, 0, DateTimeKind.Local),
                new DateTime(1997, 8, 31, 9, 0, 0, DateTimeKind.Local)
            };

            foreach (DateTime dt in DateTimes)
                Assert.IsTrue(evt.OccursAt(dt), "Event should occur on " + dt);

            Assert.IsTrue(evt.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt.Periods.Count);
        }
    }
}
