using System;
using System.Collections;
using System.Data;
using System.Net;
using System.Configuration;
using System.IO;
using DDay.iCal.Components;
using DDay.iCal.Objects;

namespace DDay.iCal
{
    public class iCalendar : iCalObject
    {
        #region Constructors
        public iCalendar() : base(null) { }
        #endregion

        #region Overrides

        /// <summary>
        /// Adds an <see cref="iCalObject"/>-based component to the
        /// appropriate collection.  Currently, the iCalendar component
        /// supports the following components:
        ///     <list>
        ///         <item>Event</item>
        ///         <item>FreeBusy</item>
        ///         <item>Journal</item>
        ///         <item>TimeZone</item>
        ///         <item>Todo</item>
        ///     </list>
        /// </summary>
        /// <param name="child"></param>
        public override void AddChild(iCalObject child)
        {
            Type type = child.GetType();
            switch (type.Name)
            {
                case "Event": Events.Add(child); break;
                case "FreeBusy": FreeBusy.Add(child); break;
                case "Journal": Journal.Add(child); break;
                case "TimeZone": TimeZone.Add(child); break;
                case "Todo": Todo.Add(child); break;
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
        /// A collection of <see cref="Event"/> components in the iCaledar.
        /// </summary>
        public ArrayList Events
        {
            get { return m_Events; }
            set { m_Events = value; }
        }

        /// <summary>
        /// A collection of <see cref="FreeBusy"/> components in the iCaledar.
        /// </summary>
        public ArrayList FreeBusy
        {
            get { return m_FreeBusy; }
            set { m_FreeBusy = value; }
        }
        
        /// <summary>
        /// A collection of <see cref="Journal"/> components in the iCaledar.
        /// </summary>
        public ArrayList Journal
        {
            get { return m_Journal; }
            set { m_Journal = value; }
        }

        /// <summary>
        /// A collection of <see cref="TimeZone"/> components in the iCaledar.
        /// </summary>
        public ArrayList TimeZone
        {
            get { return m_TimeZone; }
            set { m_TimeZone = value; }
        }

        /// <summary>
        /// A collection of <see cref="Todo"/> components in the iCaledar.
        /// </summary>
        public ArrayList Todo
        {
            get { return m_Todo; }
            set { m_Todo = value; }
        }
        #endregion

        #region Public Methods
        
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
    }
}
