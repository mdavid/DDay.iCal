using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Resources;
using System.Web;
using System.Web.UI;
using DDay.iCal.Components;
using DDay.iCal.DataTypes;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace DDay.iCal.Test
{
    [TestFixture]
    public class Journal
    {
        private TZID tzid;

        [TestFixtureSetUp]
        public void InitAll()
        {
            tzid = new TZID("US-Eastern");
        }

        static public void DoTests()
        {
            Journal j = new Journal();
            j.InitAll();
            j.JOURNAL1();
        }

        [Test, Category("Journal")]
        public void JOURNAL1()
        {
            iCalendar iCal = iCalendar.LoadFromFile(@"Calendars\Journal\JOURNAL1.ics");
            Program.TestCal(iCal);
            DDay.iCal.Components.Journal j = (DDay.iCal.Components.Journal)iCal.Journals[0];

            Assert.IsNotNull(j, "Journal entry was null");
            Assert.IsTrue(j.Status == JournalStatus.DRAFT, "Journal entry should have been in DRAFT status, but it was in " + j.Status.ToString() + " status.");
            Assert.IsTrue(j.Class == "PUBLIC", "Journal class should have been PUBLIC, but was " + j.Class + ".");
            Assert.IsNull(j.DTStart);
        }
    }
}
