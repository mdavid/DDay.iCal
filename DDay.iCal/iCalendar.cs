using System;
using System.Collections;
using System.Data;
using System.Net;
using System.Configuration;
using System.IO;
using DDay.iCal.Components;
using DDay.iCal.DataTypes;
using DDay.iCal.Objects;

namespace DDay.iCal
{
    /// <summary>
    /// A class that represents an iCalendar object.  To load an iCalendar object, generally a
    /// static LoadFromXXX method is used.
    /// <example>
    ///     For example, use the following code to load an iCalendar object from a URL:
    ///     <code>
    ///        iCalendar iCal = iCalendar.LoadFromUri(new Uri("http://somesite.com/calendar.ics"));
    ///     </code>
    /// </example>
    /// Once created, an iCalendar object can be used to gather relevant information about
    /// events, todos, time zones, journal entries, and free/busy time.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The following is an example of loading an iCalendar and displaying a text-based calendar.
    /// 
    /// <code>
    ///     //
    ///     // The following code loads and displays an iCalendar 
    ///     // with US Holidays for 2006.
    ///     //
    ///     iCalendar iCal = iCalendar.LoadFromUri(new Uri("http://www.applegatehomecare.com/Calendars/USHolidays.ics"));
    ///     iCal.Evaluate(
    ///         new Date_Time(2006, 1, 1, "US-Eastern", iCal),
    ///         new Date_Time(2006, 12, 31, "US-Eastern", iCal));
    /// 
    ///     Date_Time dt = new Date_Time(2006, 1, 1, "US-Eastern", iCal);
    ///     while (dt.Year == 2006)
    ///     {
    ///         // First, display the current date we're evaluating
    ///         Console.WriteLine(dt.Local.ToShortDateString());
    /// 
    ///         // Then, iterate through each event in our iCalendar
    ///         foreach (Event evt in iCal.Events)
    ///         {
    ///             // Determine if the event occurs on the specified date
    ///             if (evt.OccursOn(dt))
    ///             {
    ///                 // Display the event summary
    ///                 Console.Write("\t" + evt.Summary);
    /// 
    ///                 // Display the time the event happens (unless it's an all-day event)
    ///                 if (evt.Start.HasTime)
    ///                 {
    ///                     Console.Write(" (" + evt.Start.Local.ToShortTimeString() + " - " + evt.End.Local.ToShortTimeString());
    ///                     if (evt.Start.TimeZoneInfo != null)
    ///                         Console.Write(" " + evt.Start.TimeZoneInfo.Name);
    ///                     Console.Write(")");
    ///                 }
    /// 
    ///                 Console.Write(Environment.NewLine);
    ///             }
    ///         }
    /// 
    ///         // Move to the next day
    ///         dt = dt.AddDays(1);
    ///     }
    /// </code>
    ///     </para>
    /// </remarks>
    public class iCalendar : iCalObject
    {
        #region Constructors

        /// <summary>
        /// To load an existing an iCalendar object, use one of the provided LoadFromXXX methods.
        /// <example>
        /// For example, use the following code to load an iCalendar object from a URL:
        /// <code>
        ///     iCalendar iCal = iCalendar.LoadFromUri(new Uri("http://somesite.com/calendar.ics"));
        /// </code>
        /// </example>
        /// </summary>
        public iCalendar() : base(null) { }
        #endregion

        #region Overrides

        /// <summary>
        /// Adds an <see cref="iCalObject"/>-based component to the
        /// appropriate collection.  Currently, the iCalendar component
        /// supports the following components:
        ///     <list type="bullet">        
        ///         <item><see cref="Event"/></item>
        ///         <item><see cref="FreeBusy"/></item>
        ///         <item><see cref="Journal"/></item>
        ///         <item><see cref="DDay.iCal.Components.TimeZone"/></item>
        ///         <item><see cref="Todo"/></item>
        ///     </list>
        /// </summary>
        /// <param name="child"></param>
        public override void AddChild(iCalObject child)
        {
            base.AddChild(child);

            Type type = child.GetType();
            switch (type.Name)
            {
                case "Event": Events.Add(child); break;
                case "FreeBusy": FreeBusy.Add(child); break;
                case "Journal": Journals.Add(child); break;
                case "TimeZone": TimeZones.Add(child); break;
                case "Todo": Todos.Add(child); break;
                default: break;
            }                
        }

        #endregion

        #region Private Fields

        private ArrayList m_Events = new ArrayList();
        private ArrayList m_FreeBusy = new ArrayList();
        private ArrayList m_Journal = new ArrayList();
        private ArrayList m_TimeZone = new ArrayList();
        private ArrayList m_Todo = new ArrayList();

        #endregion

        #region Public Properties

        /// <summary>
        /// A collection of <see cref="Event"/> components in the iCalendar.
        /// </summary>
        public ArrayList Events
        {
            get { return m_Events; }
            set { m_Events = value; }
        }

        /// <summary>
        /// A collection of <see cref="DDay.iCal.Components.FreeBusy"/> components in the iCalendar.
        /// </summary>
        public ArrayList FreeBusy
        {
            get { return m_FreeBusy; }
            set { m_FreeBusy = value; }
        }
        
        /// <summary>
        /// A collection of <see cref="Journal"/> components in the iCalendar.
        /// </summary>
        public ArrayList Journals
        {
            get { return m_Journal; }
            set { m_Journal = value; }
        }

        /// <summary>
        /// A collection of <see cref="DDay.iCal.Components.TimeZone"/> components in the iCalendar.
        /// </summary>
        public ArrayList TimeZones
        {
            get { return m_TimeZone; }
            set { m_TimeZone = value; }
        }

        /// <summary>
        /// A collection of <see cref="Todo"/> components in the iCalendar.
        /// </summary>
        public ArrayList Todos
        {
            get { return m_Todo; }
            set { m_Todo = value; }
        }
        #endregion

        #region Static Public Methods
        
        /// <summary>
        /// Loads an <see cref="iCalendar"/> from the file system.
        /// </summary>
        /// <param name="Filepath">The path to the file to load.</param>
        /// <returns>An <see cref="iCalendar"/> object</returns>
        static public iCalendar LoadFromFile(string Filepath)
        {
            FileStream fs = new FileStream(Filepath, FileMode.Open);
            iCalendar iCal = LoadFromStream(fs);
            fs.Close();
            return iCal;
        }

        /// <summary>
        /// Loads an <see cref="iCalendar"/> from an open stream.
        /// </summary>
        /// <param name="s">The stream from which to load the <see cref="iCalendar"/> object</param>
        /// <returns>An <see cref="iCalendar"/> object</returns>
        static public iCalendar LoadFromStream(Stream s)
        {
            iCalLexer lexer = new iCalLexer(s);
            iCalParser parser = new iCalParser(lexer);
            return parser.icalobject();
        }

        /// <summary>
        /// Loads an <see cref="iCalendar"/> from a given Uri.
        /// </summary>
        /// <param name="url">The Uri from which to load the <see cref="iCalendar"/> object</param>
        /// <returns>an <see cref="iCalendar"/> object</returns>
        static public iCalendar LoadFromUri(Uri uri)
        {
            try
            {
                WebClient client = new WebClient();
                client.Credentials = new System.Net.NetworkCredential("dougd", "rummage");

                byte[] bytes = client.DownloadData(uri);
                MemoryStream ms = new MemoryStream();
                ms.SetLength(bytes.Length);
                bytes.CopyTo(ms.GetBuffer(), 0);
                
                return LoadFromStream(ms);
            }
            catch (System.Net.WebException ex)
            {
                return null;
            }
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves the <see cref="DDay.iCal.Components.TimeZone" /> object for the specified
        /// <see cref="TZID"/> (Time Zone Identifier).
        /// </summary>
        /// <param name="tzid">A valid <see cref="TZID"/> object, or a valid <see cref="TZID"/> string.</param>
        /// <returns>A <see cref="TimeZone"/> object for the <see cref="TZID"/>.</returns>
        public DDay.iCal.Components.TimeZone GetTimeZone(TZID tzid)
        {
            foreach (DDay.iCal.Components.TimeZone tz in TimeZones)
            {
                if (tz.TZID.Equals(tzid))
                    return tz;
            }
            return null;
        }

        /// <summary>
        /// Evaluates component recurrences for the given range of time.
        /// <example>
        ///     For example, if you are displaying a month-view for January 2007,
        ///     you would want to evaluate recurrences for Jan. 1, 2007 to Jan. 31, 2007
        ///     to display relevant information for those dates.
        /// </example>
        /// </summary>
        /// <param name="FromDate">The beginning date/time of the range to test.</param>
        /// <param name="ToDate">The end date/time of the range to test.</param>                
        public void Evaluate(Date_Time FromDate, Date_Time ToDate)
        {
            foreach (iCalObject obj in Children)
            {
                if (obj is RecurringComponent)
                    ((RecurringComponent)obj).Evaluate(FromDate, ToDate);
            }            
        }

        public ArrayList GetTodos(string category)
        {
            ArrayList t = new ArrayList();
            foreach (Todo todo in Todos)
            {
                if (todo.Categories != null)
                {
                    foreach (Text cat in todo.Categories)
                    {
                        if (cat.Value == category)
                            t.Add(todo);
                    }
                }
            }

            return t;
        }

        #endregion
    }
}
