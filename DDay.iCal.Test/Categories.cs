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
    public class Categories
    {
        private TZID tzid;

        [TestFixtureSetUp]
        public void InitAll()
        {
            tzid = new TZID("US-Eastern");
        }

        static public void DoTests()
        {
            Categories c = new Categories();
            c.InitAll();
            c.CATEGORIES();
        }

        /// <summary>        
        /// </summary>
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
    }
}
