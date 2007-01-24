using System;
using System.Collections;
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
using DDay.iCal.Test;
using DDay.iCal.Serialization;

namespace DDay.iCal.Test
{
    [TestFixture]
    public class Program
    {
        static public void Main(string[] args)
        {
            Program p = new Program();
            p.InitAll();
            p.LoadFromFile();
            p.LoadFromUri();

            DDay.iCal.Test.Alarm.DoTests();
            DDay.iCal.Test.Journal.DoTests();
            DDay.iCal.Test.Recurrence.DoTests();
            DDay.iCal.Test.Serialization.DoTests();
            DDay.iCal.Test.Todo.DoTests();

            p.CATEGORIES();
            p.GEO1();
            p.BASE64();
            p.BASE64_1();
            p.BINARY();
            p.MERGE();
            p.UID1();
            p.LoadAndDisplayCalendar();

            p.DisposeAll();
        }
                
        [Test]
        public void LoadAndDisplayCalendar()
        {
             // The following code loads and displays an iCalendar
             // with US Holidays for 2006.
             //
             iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\General\USHolidays.ics");
             Assert.IsNotNull(iCal, "iCalendar did not load.  Are you connected to the internet?");
             iCal.Evaluate(
                 new Date_Time(2006, 1, 1, "US-Eastern", iCal),
                 new Date_Time(2006, 12, 31, "US-Eastern", iCal));
 
             Date_Time dt = new Date_Time(2006, 1, 1, "US-Eastern", iCal);
             while (dt.Year == 2006)
             {
                 // First, display the current date we're evaluating
                 Console.WriteLine(dt.Local.ToShortDateString());
 
                 // Then, iterate through each event in our iCalendar
                 foreach (Event evt in iCal.Events)
                 {
                     // Determine if the event occurs on the specified date
                     if (evt.OccursOn(dt))
                     {
                         // Display the event summary
                         Console.Write("\t" + evt.Summary);
 
                         // Display the time the event happens (unless it's an all-day event)
                         if (evt.Start.HasTime)
                         {
                             Console.Write(" (" + evt.Start.Local.ToShortTimeString() + " - " + evt.End.Local.ToShortTimeString());
                             if (evt.Start.TimeZoneInfo != null)
                                 Console.Write(" " + evt.Start.TimeZoneInfo.TimeZoneName);
                             Console.Write(")");
                         }
 
                         Console.Write(Environment.NewLine);
                     }
                 }
 
                 // Move to the next day
                 dt = dt.AddDays(1);
             }
        }

        private DateTime Start;
        private DateTime End;
        private TimeSpan TotalTime;
        private TZID tzid;

        [TestFixtureSetUp]
        public void InitAll()
        {
            TotalTime = new TimeSpan(0);
            tzid = new TZID("US-Eastern");
        }

        [TestFixtureTearDown]
        public void DisposeAll()
        {
            Console.WriteLine("Total Processing Time: " + Math.Round(TotalTime.TotalMilliseconds) + "ms");
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
            TotalTime = TotalTime.Add(End - Start);
            Console.WriteLine("Time: " + Math.Round(End.Subtract(Start).TotalMilliseconds) + "ms");
        }

        static public void TestCal(iCalendar iCal)
        {
            Assert.IsNotNull(iCal, "The iCalendar was not loaded");
            if (iCal.Events.Count > 0)
                Assert.IsTrue(iCal.Events.Count == 1, "Calendar should contain 1 event; however, the iCalendar loaded " + iCal.Events.Count + " events");
            else if (iCal.Todos.Count > 0)
                Assert.IsTrue(iCal.Todos.Count == 1, "Calendar should contain 1 todo; however, the iCalendar loaded " + iCal.Todos.Count + " todos");
        }

        [Test]
        public void LoadFromFile()
        {
            string path = @"Calendars\General\Test1.ics";
            Assert.IsTrue(File.Exists(path), "File '" + path + "' does not exist.");
            
            iCalendar iCal = iCalendar.LoadFromFile(path);
            Program.TestCal(iCal);
        }

        [Test]
        public void LoadFromUri()
        {
            string path = Directory.GetCurrentDirectory();            
            path = Path.Combine(path, "Calendars/General/Test1.ics").Replace(@"\", "/");
            path = "file:///" + path;            
            Uri uri = new Uri(path);
            iCalendar iCal = iCalendar.LoadFromUri(uri);
            Program.TestCal(iCal);
        }

        [Test]
        public void CATEGORIES()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\General\CATEGORIES.ics");
            Program.TestCal(iCal);
            Event evt = iCal.Events[0];

            ArrayList items = new ArrayList();
            items.AddRange(new string[]
            {
                "One", "Two", "Three",
                "Four", "Five", "Six",
                "Seven", "A string of text with nothing less than a comma, semicolon; and a newline\n."
            });

            Hashtable found = new Hashtable();

            foreach (TextCollection tc in evt.Categories)
            {
                foreach (Text text in tc.Values)
                {
                    if (items.Contains(text.Value))
                        found[text.Value] = true;
                }
            }

            foreach (string item in items)
                Assert.IsTrue(found.ContainsKey(item), "Event should contain CATEGORY '" + item + "', but it was not found.");
        }

        [Test]
        public void GEO1()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\General\GEO1.ics");
            Program.TestCal(iCal);
            Event evt = iCal.Events[0];

            Assert.IsTrue(evt.Geo.Latitude.Value == 37.386013, "Latitude should be 37.386013; it is not.");
            Assert.IsTrue(evt.Geo.Longitude.Value == -122.082932, "Longitude should be -122.082932; it is not.");
        }

        [Test]
        public void BASE64()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\General\BASE64.ics");
            Program.TestCal(iCal);
            Event evt = iCal.Events[0];

            Assert.IsTrue(evt.Attach[0].Value ==
"This is a test to try out base64 encoding without being too large.\r\n" +
"This is a test to try out base64 encoding without being too large.\r\n" +
"This is a test to try out base64 encoding without being too large.\r\n" +
"This is a test to try out base64 encoding without being too large.\r\n" +
"This is a test to try out base64 encoding without being too large.\r\n" +
"This is a test to try out base64 encoding without being too large.\r\n" +
"This is a test to try out base64 encoding without being too large.\r\n" +
"This is a test to try out base64 encoding without being too large.\r\n" +
"This is a test to try out base64 encoding without being too large.\r\n" +
"This is a test to try out base64 encoding without being too large.\r\n" +
"This is a test to try out base64 encoding without being too large.\r\n" +
"This is a test to try out base64 encoding without being too large.", "Attached value does not match.");
        }

        [Test]
        public void BASE64_1()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\General\BASE64_1.ics");
            Program.TestCal(iCal);
            Event evt = iCal.Events[0];

            Assert.IsTrue(evt.UID.Value == "uuid1153170430406", "UID should be 'uuid1153170430406'; it is " + evt.UID.Value);
            Assert.IsTrue(evt.Sequence.Value == 1, "SEQUENCE should be 1; it is " + evt.Sequence.Value);
        }

        [Test]
        public void BINARY()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\General\BINARY.ics");
            Program.TestCal(iCal);
            Event evt = iCal.Events[0];

            Binary b = evt.Attach[0];
            if (b.Uri != null)
                b.LoadDataFromUri();

            MemoryStream ms = new MemoryStream();
            ms.SetLength(b.Data.Length);
            b.Data.CopyTo(ms.GetBuffer(), 0);

            iCalendar ical = iCalendar.LoadFromStream(ms);
            Assert.IsNotNull(ical, "Attached iCalendar did not load correctly");
        }
        
        /// <summary>
        /// The following test is an aggregate of RRULE21() and RRULE22() in the
        /// <see cref="Recurrence"/> class.
        /// </summary>
        [Test]
        public void MERGE()
        {
            iCalendar iCal1 = iCalendar.LoadFromFile(@"Calendars\Recurrence\RRULE21.ics");
            iCalendar iCal2 = iCalendar.LoadFromFile(@"Calendars\Recurrence\RRULE22.ics");            
            
            iCal1.MergeWith(iCal2);
            
            Event evt1 = iCal1.Events[0];
            Event evt2 = iCal1.Events[1];
            evt1.Evaluate(new Date_Time(1996, 1, 1, tzid, iCal1), new Date_Time(2000, 1, 1, tzid, iCal1));

            Date_Time[] DateTimes = new Date_Time[]
            {
                new Date_Time(1997, 9, 10, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 9, 11, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 9, 12, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 9, 13, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 9, 14, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 9, 15, 9, 0, 0, tzid, iCal1),
                new Date_Time(1999, 3, 10, 9, 0, 0, tzid, iCal1),
                new Date_Time(1999, 3, 11, 9, 0, 0, tzid, iCal1),
                new Date_Time(1999, 3, 12, 9, 0, 0, tzid, iCal1),
                new Date_Time(1999, 3, 13, 9, 0, 0, tzid, iCal1),
            };

            string[] TimeZones = new string[]
            {
                "EDT",
                "EDT",
                "EDT",
                "EDT",
                "EDT",
                "EDT",
                "EST",
                "EST",
                "EST",
                "EST"                
            };

            for (int i = 0; i < DateTimes.Length; i++)
            {
                Date_Time dt = (Date_Time)DateTimes[i];
                Assert.IsTrue(evt1.OccursAt(dt), "Event should occur on " + dt);
                Assert.IsTrue(dt.TimeZoneInfo.TimeZoneName == TimeZones[i], "Event " + dt + " should occur in the " + TimeZones[i] + " timezone");
            }

            Assert.IsTrue(evt1.Periods.Count == DateTimes.Length, "There should be exactly " + DateTimes.Length + " occurrences; there were " + evt1.Periods.Count);

            evt2.Evaluate(new Date_Time(1996, 1, 1, tzid, iCal1), new Date_Time(1998, 4, 1, tzid, iCal1));

            Date_Time[] DateTimes1 = new Date_Time[]
            {
                new Date_Time(1997, 9, 2, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 9, 9, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 9, 16, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 9, 23, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 9, 30, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 11, 4, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 11, 11, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 11, 18, 9, 0, 0, tzid, iCal1),
                new Date_Time(1997, 11, 25, 9, 0, 0, tzid, iCal1),
                new Date_Time(1998, 1, 6, 9, 0, 0, tzid, iCal1),
                new Date_Time(1998, 1, 13, 9, 0, 0, tzid, iCal1),
                new Date_Time(1998, 1, 20, 9, 0, 0, tzid, iCal1),
                new Date_Time(1998, 1, 27, 9, 0, 0, tzid, iCal1),
                new Date_Time(1998, 3, 3, 9, 0, 0, tzid, iCal1),
                new Date_Time(1998, 3, 10, 9, 0, 0, tzid, iCal1),
                new Date_Time(1998, 3, 17, 9, 0, 0, tzid, iCal1),
                new Date_Time(1998, 3, 24, 9, 0, 0, tzid, iCal1),
                new Date_Time(1998, 3, 31, 9, 0, 0, tzid, iCal1)
            };

            string[] TimeZones1 = new string[]
            {
                "EDT",
                "EDT",
                "EDT",
                "EDT",                
                "EDT",
                "EST",
                "EST",
                "EST",
                "EST",
                "EST",
                "EST",
                "EST",
                "EST",
                "EST",
                "EST",
                "EST",
                "EST",
                "EST"                
            };

            for (int i = 0; i < DateTimes1.Length; i++)
            {
                Date_Time dt = (Date_Time)DateTimes1[i];
                Assert.IsTrue(evt2.OccursAt(dt), "Event should occur on " + dt);
                Assert.IsTrue(dt.TimeZoneInfo.TimeZoneName == TimeZones1[i], "Event " + dt + " should occur in the " + TimeZones1[i] + " timezone");
            }

            Assert.IsTrue(evt2.Periods.Count == DateTimes1.Length, "There should be exactly " + DateTimes1.Length + " occurrences; there were " + evt2.Periods.Count);
        }

        [Test]
        public void UID1()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\General\BINARY.ics");
            Program.TestCal(iCal);
            
            Event evt = iCal.Events["uuid1153170430406"];
            Assert.IsNotNull(evt, "Event could not be accessed by UID");
        }
    }
}
