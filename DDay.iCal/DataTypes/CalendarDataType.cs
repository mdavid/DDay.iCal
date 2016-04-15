using System;
using System.Runtime.Serialization;

namespace DDay.iCal
{
    /// <summary>
    /// An abstract class from which all iCalendar data types inherit.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class CalendarDataType :
        ICalendarDataType
    {
        #region Private Fields

        ICalendarParameterCollection _Parameters;
        ICalendarParameterCollectionProxy _Proxy;
        ServiceProvider _ServiceProvider;

        #endregion

        #region Protected Fields

        protected ICalendarObject _AssociatedObject;

        #endregion

        #region Constructors

        public CalendarDataType()
        {
            Initialize();
        }

        void Initialize()
        {
            _Parameters = new CalendarParameterList();
            _Proxy = new CalendarParameterCollectionProxy(_Parameters);
            _ServiceProvider = new ServiceProvider();
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

        protected virtual void OnDeserialized(StreamingContext context)
        {
        }

        #endregion
    
        #region ICalendarDataType Members

        public virtual Type GetValueType()
        {
            // See RFC 5545 Section 3.2.20.
            if (_Proxy != null && _Proxy.ContainsKey("VALUE"))
            {
                switch (_Proxy.Get("VALUE"))
                {
                    case "BINARY": return typeof(byte[]);
                    case "BOOLEAN": return typeof(bool);
                    case "CAL-ADDRESS": return typeof(Uri);
                    case "DATE": return typeof(IDateTime);
                    case "DATE-TIME": return typeof(IDateTime);
                    case "DURATION": return typeof(TimeSpan);
                    case "FLOAT": return typeof(double);
                    case "INTEGER": return typeof(int);
                    case "PERIOD": return typeof(IPeriod);
                    case "RECUR": return typeof(IRecurrencePattern);
                    case "TEXT": return typeof(string);
                    case "TIME":
                        // FIXME: implement ISO.8601.2004
                        throw new NotImplementedException();
                    case "URI": return typeof(Uri);
                    case "UTC-OFFSET": return typeof(IUTCOffset);
                    default:
                        return null;
                }
            }
            return null;
        }

        public virtual void SetValueType(string type)
        {
            if (_Proxy != null)
                _Proxy.Set("VALUE", type != null ? type : type.ToUpper());
        }

        public virtual ICalendarObject AssociatedObject
        {
            get { return _AssociatedObject; }
            set
            {
                if (!object.Equals(_AssociatedObject, value))
                {
                    _AssociatedObject = value;
                    if (_AssociatedObject != null)
                    {
                        _Proxy.SetParent(_AssociatedObject);
                        if (_AssociatedObject is ICalendarParameterCollectionContainer)
                            _Proxy.SetProxiedObject(((ICalendarParameterCollectionContainer)_AssociatedObject).Parameters);
                    }
                    else
                    {
                        _Proxy.SetParent(null);
                        _Proxy.SetProxiedObject(_Parameters);
                    }
                }
            }
        }

        public virtual IICalendar Calendar
        {
            get
            {
                if (_AssociatedObject != null)
                    return _AssociatedObject.Calendar;
                return null;
            }
        }

        public virtual string Language
        {
            get { return Parameters.Get("LANGUAGE"); }
            set { Parameters.Set("LANGUAGE", value); }
        }

        #endregion

        #region ICopyable Members

        /// <summary>
        /// Copies values from the target object to the
        /// current object.
        /// </summary>
        public virtual void CopyFrom(ICopyable obj)
        {
            if (obj is ICalendarDataType)
            {
                var dt = (ICalendarDataType)obj;                
                _AssociatedObject = dt.AssociatedObject;
                _Proxy.SetParent(_AssociatedObject);
                _Proxy.SetProxiedObject(dt.Parameters);
            }
        }

        /// <summary>
        /// Creates a copy of the object.
        /// </summary>
        /// <returns>The copy of the object.</returns>
        public virtual T Copy<T>()
        {
            ICopyable obj = null;
            var type = GetType();
            obj = Activator.CreateInstance(type) as ICopyable;

            // Duplicate our values
            if (obj is T)
            {
                obj.CopyFrom(this);
                return (T)obj;
            }
            return default(T);
        }

        #endregion

        #region ICalendarParameterCollectionContainer Members

        public virtual ICalendarParameterCollection Parameters
        {
            get { return _Proxy; }
        }

        #endregion

        #region IServiceProvider Members

        public virtual object GetService(Type serviceType)
        {
            return _ServiceProvider.GetService(serviceType);
        }

        public object GetService(string name)
        {
            return _ServiceProvider.GetService(name);
        }

        public T GetService<T>()
        {
            return _ServiceProvider.GetService<T>();
        }

        public T GetService<T>(string name)
        {
            return _ServiceProvider.GetService<T>(name);
        }

        public void SetService(string name, object obj)
        {
            _ServiceProvider.SetService(name, obj);
        }

        public void SetService(object obj)
        {
            _ServiceProvider.SetService(obj);
        }

        public void RemoveService(Type type)
        {
            _ServiceProvider.RemoveService(type);
        }

        public void RemoveService(string name)
        {
            _ServiceProvider.RemoveService(name);
        }

        #endregion
    }
}
