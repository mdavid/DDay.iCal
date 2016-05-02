using System;
using System.Runtime.Serialization;
using DDay.Collections;

namespace DDay.iCal
{
    /// <summary>
    /// The base class for all iCalendar objects and components.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class CalendarObject : CalendarObjectBase, ICalendarObject
    {
        #region Private Fields

        private ICalendarObject _Parent = null;
        private ICalendarObjectList<ICalendarObject> _Children;
        private ServiceProvider _ServiceProvider;
        private string _Name;

        private int _Line;
        private int _Column;

        #endregion

        #region Constructors

        internal CalendarObject()
        {
            Initialize();
        }

        public CalendarObject(string name) : this()
        {
            Name = name;
        }

        public CalendarObject(int line, int col) : this()
        {
            Line = line;
            Column = col;
        }

        void Initialize()
        {
            _Children = new CalendarObjectList(this);
            _ServiceProvider = new ServiceProvider();

            _Children.ItemAdded += new EventHandler<ObjectEventArgs<ICalendarObject, int>>(_Children_ItemAdded);
            _Children.ItemRemoved += new EventHandler<ObjectEventArgs<ICalendarObject, int>>(_Children_ItemRemoved);
        }

        #endregion

        #region Internal Methods

        [OnDeserializing]
        internal void DeserializingInternal(StreamingContext context)
        {
            OnDeserializing(context);
        }

        [OnDeserialized]
        internal void DeserializedInternal(StreamingContext context)
        {
            OnDeserialized(context);
        }

        #endregion

        #region Protected Methods

        protected virtual void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }

        protected virtual void OnDeserialized(StreamingContext context) {}

        #endregion

        #region Event Handlers

        void _Children_ItemRemoved(object sender, ObjectEventArgs<ICalendarObject, int> e)
        {
            e.First.Parent = null;
        }

        void _Children_ItemAdded(object sender, ObjectEventArgs<ICalendarObject, int> e)
        {
            e.First.Parent = this;
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            var o = obj as ICalendarObject;
            if (o != null)
            {
                return object.Equals(o.Name, Name);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (Name != null)
            {
                return Name.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override void CopyFrom(ICopyable c)
        {
            var obj = c as ICalendarObject;
            if (obj != null)
            {
                // Copy the name and basic information
                this.Name = obj.Name;
                this.Parent = obj.Parent;
                this.Line = obj.Line;
                this.Column = obj.Column;

                // Add each child
                this.Children.Clear();
                foreach (var child in obj.Children)
                {
                    this.AddChild(child.Copy<ICalendarObject>());
                }
            }
        }

        #endregion

        #region ICalendarObject Members

        /// <summary>
        /// Returns the parent iCalObject that owns this one.
        /// </summary>
        public virtual ICalendarObject Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        /// <summary>
        /// A collection of iCalObjects that are children of the current object.
        /// </summary>
        public virtual ICalendarObjectList<ICalendarObject> Children
        {
            get { return _Children; }
        }

        /// <summary>
        /// Gets or sets the name of the iCalObject.  For iCalendar components, this is the RFC 5545 name of the component.
        /// <example>
        ///     <list type="bullet">
        ///         <item>Event - "VEVENT"</item>
        ///         <item>Todo - "VTODO"</item>
        ///         <item>TimeZone - "VTIMEZONE"</item>
        ///         <item>etc.</item>
        ///     </list>
        /// </example>
        /// </summary>        
        public virtual string Name
        {
            get { return _Name; }
            set
            {
                if (!object.Equals(_Name, value))
                {
                    var old = _Name;
                    _Name = value;
                    OnGroupChanged(old, _Name);
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="DDay.iCal.iCalendar"/> that this DDayiCalObject belongs to.
        /// </summary>
        public virtual IICalendar Calendar
        {
            get
            {
                ICalendarObject obj = this;
                while (!(obj is IICalendar) && obj.Parent != null)
                {
                    obj = obj.Parent;
                }

                if (obj is IICalendar)
                {
                    return (IICalendar) obj;
                }
                return null;
            }
            protected set { _Parent = value; }
        }

        public virtual IICalendar iCalendar
        {
            get { return Calendar; }
            protected set { Calendar = value; }
        }

        public virtual int Line
        {
            get { return _Line; }
            set { _Line = value; }
        }

        public virtual int Column
        {
            get { return _Column; }
            set { _Column = value; }
        }

        #endregion

        #region IServiceProvider Members

        public virtual object GetService(Type serviceType)
        {
            return _ServiceProvider.GetService(serviceType);
        }

        public virtual object GetService(string name)
        {
            return _ServiceProvider.GetService(name);
        }

        public virtual T GetService<T>()
        {
            return _ServiceProvider.GetService<T>();
        }

        public virtual T GetService<T>(string name)
        {
            return _ServiceProvider.GetService<T>(name);
        }

        public virtual void SetService(string name, object obj)
        {
            _ServiceProvider.SetService(name, obj);
        }

        public virtual void SetService(object obj)
        {
            _ServiceProvider.SetService(obj);
        }

        public virtual void RemoveService(Type type)
        {
            _ServiceProvider.RemoveService(type);
        }

        public virtual void RemoveService(string name)
        {
            _ServiceProvider.RemoveService(name);
        }

        #endregion

        #region IGroupedObject Members

        [field: NonSerialized]
        public event EventHandler<ObjectEventArgs<string, string>> GroupChanged;

        protected void OnGroupChanged(string @old, string @new)
        {
            if (GroupChanged != null)
            {
                GroupChanged(this, new ObjectEventArgs<string, string>(@old, @new));
            }
        }

        public virtual string Group
        {
            get { return Name; }
            set { Name = value; }
        }

        #endregion
    }
}