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
            DDay.iCal.Test.Todo.DoTests();

            p.CATEGORIES();
            p.GEO1();
            p.LoadAndDisplayCalendar();

            p.DisposeAll();
        }
                
        [Test]
        public void LoadAndDisplayCalendar()
        {            
             // The following code loads and displays an iCalendar 
             // with US Holidays for 2006.
             //
             iCalendar iCal = iCalendar.LoadFromUri(new Uri("http://www.applegatehomecare.com/Calendars/USHolidays.ics"));
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
                                 Console.Write(" " + evt.Start.TimeZoneInfo.Name);
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
            Event evt = (Event)iCal.Events[0];

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
            Event evt = (Event)iCal.Events[0];

            Assert.IsTrue(evt.Geo.Latitude.Value == 37.386013, "Latitude should be 37.386013; it is not.");
            Assert.IsTrue(evt.Geo.Longitude.Value == -122.082932, "Longitude should be -122.082932; it is not.");
        }
    }
}
