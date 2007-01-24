using System;
using System.Collections;
using System.Text;
using System.Reflection;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Objects
{
    /// <summary>
    /// The base class for all iCalendar objects, components, and data types.
    /// </summary>
    public class iCalObject
    {
        #region Public Fields

        public iCalObject Parent = null;
        public Hashtable Properties = new Hashtable();
        public event EventHandler Load;

        #endregion

        #region Private Fields

        private ArrayList m_Children = new ArrayList();
        private string m_name;

        #endregion

        #region Public Properties

        /// <summary>
        /// A collection of <see cref="iCalObject"/>s that are children 
        /// of the current object.
        /// </summary>
        public ArrayList Children
        {
            get { return m_Children; }
            set { m_Children = value; }
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="iCalObject"/>.  For iCalendar components,
        /// this is the RFC 2445 name of the component.
        /// <example>
        ///     <list type="bullet">
        ///         <item>Event - "VEVENT"</item>
        ///         <item>Todo - "VTODO"</item>
        ///         <item>TimeZone - "VTIMEZONE"</item>
        ///         <item>etc.</item>
        ///     </list>
        /// </example>
        /// </summary>        
        virtual public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        /// Returns the <see cref="DDay.iCal.iCalendar"/> that this <see cref="iCalObject"/>
        /// belongs to.
        /// </summary>
        public iCalendar iCalendar
        {
            get
            {
                iCalObject obj = this;
                while (obj.Parent != null)
                    obj = obj.Parent;

                if (obj is iCalendar)
                    return obj as iCalendar;
                return null;
            }
        }

        #endregion

        #region Constructors

        internal iCalObject() { }
        public iCalObject(iCalObject parent)
        {
            this.Parent = parent;
            if (parent != null)
            {
                if (!(this is Property) &&
                    !(this is Parameter))
                    parent.AddChild(this);
            }
        }
        public iCalObject(iCalObject parent, string name)
            : this(parent)
        {
            Name = name;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an <see cref="iCalObject"/>-based object as a child
        /// of the current object.
        /// </summary>
        /// <param name="child">The <see cref="iCalObject"/>-based object to add.</param>
        virtual public void AddChild(iCalObject child)
        {
            Children.Add(child);
        }

        /// <summary>
        /// For iCalendar components, automatically finds and retrieves fields that
        /// match the field specified in the <see cref="ContentLine"/>, and sets
        /// their value.
        /// <example>
        /// For example, if a public DTStart field exists in the specified component,
        /// (i.e. <c>public Date_Time DTStart;</c>)
        /// and a content line of <c>DTSTART;TZID=US-Eastern:20060830T090000</c> is
        /// encountered, this method will automatically set the value of the
        /// DTStart field to Aug. 30, 2006, 9:00 AM in the US-Eastern TimeZone.
        /// </example>
        /// <note>
        ///     It should not be necessary to invoke this method manually as it
        ///     is handled automatically during the iCalendar parsing.
        /// </note>
        /// </summary>
        /// <param name="cl">The <see cref="ContentLine"/> to process.</param>
        virtual public void SetContentLineValue(ContentLine cl)
        {
            if (cl.Name != null)
            {
                string name = cl.Name.Replace("-","");
                Type type = GetType();

                FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                if (field != null)
                {
                    object value = field.GetValue(this);
                    Type elementType = field.FieldType.IsArray ? field.FieldType.GetElementType() : field.FieldType;

                    // If it's an iCalDataType, or an array of iCalDataType, then let's fill it!
                    if (field.FieldType.IsSubclassOf(typeof(iCalDataType)) ||
                        (field.FieldType.IsArray && field.FieldType.GetElementType().IsSubclassOf(typeof(iCalDataType))))
                    {   
                        iCalDataType icdt = null;
                        if (!field.FieldType.IsArray)
                            icdt = (iCalDataType)value;
                        if (icdt == null)
                            icdt = (iCalDataType)Activator.CreateInstance(elementType);

                        // Set the content line for the object.  On most objects, this
                        // triggers the object to parse the content line with Parse().
                        icdt.ContentLine = cl;

                        // It's an array, let's add an item to the end
                        if (field.FieldType.IsArray)
                        {
                            ArrayList arr = new ArrayList();
                            if (value != null)
                                arr.AddRange((ICollection)value);                            
                            arr.Add(icdt);
                            field.SetValue(this, arr.ToArray(elementType));
                        }                        
                        // Otherwise, set the value directly!
                        else field.SetValue(this, icdt);
                    }
                    else
                    {
                        FieldInfo minValue = field.FieldType.GetField("MinValue");
                        object minVal = (minValue != null) ? minValue.GetValue(null) : null;

                        if (field.FieldType.IsArray)
                        {
                            ArrayList arr = new ArrayList();
                            if (value != null)
                                arr.AddRange((ICollection)value);
                            arr.Add(cl.Value);
                            field.SetValue(this, arr.ToArray(elementType));
                        }
                        // Always assign enum values
                        else if (field.FieldType.IsEnum)
                            field.SetValue(this, Enum.Parse(field.FieldType, cl.Value.Replace("-","_")));
                        // Otherwise, set the value directly!
                        else if (value == null || value.Equals(minVal))
                            field.SetValue(this, cl.Value);
                        else ;// FIXME: throw single-value exception, if "strict" parsing is enabled
                    }
                }
            }
        }

        /// <summary>
        /// Invokes the <see cref="Load"/> event handler when the object has been fully loaded.
        /// This is automatically called when processing objects that inherit from 
        /// <see cref="ComponentBase"/> (i.e. all iCalendar components).
        /// </summary>        
        virtual public void OnLoad(EventArgs e)
        {
            if (this.Load != null)
                this.Load(this, e);
        }

        #endregion
    }
}
