using System;
using System.Collections;
using System.Text;
using System.Reflection;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Objects
{
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

        public ArrayList Children
        {
            get { return m_Children; }
            set { m_Children = value; }
        }
        
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

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

        virtual public void AddChild(iCalObject child)
        {
            Children.Add(child);
        }

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
                        // Otherwise, set the value directly!
                        else if (value == null || value.Equals(minVal))
                            field.SetValue(this, cl.Value);
                        else ;// FIXME: throw single-value exception, if "strict" parsing is enabled
                    }
                }
            }
        }

        virtual public void OnLoad(EventArgs e)
        {
            if (this.Load != null)
                this.Load(this, e);
        }

        #endregion
    }
}
