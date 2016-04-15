using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Diagnostics;
using DDay.Collections;

namespace DDay.iCal
{
    [DebuggerDisplay("{Name}={string.Join(\",\", Values)}")]
#if !SILVERLIGHT
    [Serializable]
#endif
    public class CalendarParameter : 
        CalendarObject,
        ICalendarParameter
    {
        #region Private Fields

        List<string> _Values;

        #endregion

        #region Constructors

        public CalendarParameter() 
        {
            Initialize();
        }

        public CalendarParameter(string name)
            : base(name)
        {
            Initialize();
        }

        public CalendarParameter(string name, string value)
            : base(name)
        {
            Initialize();
            AddValue(value);
        }

        public CalendarParameter(string name, IEnumerable<string> values)
            : base(name)
        {
            Initialize();
            foreach (var v in values)
                AddValue(v);                
        }

        void Initialize()
        {
            _Values = new List<string>();
        }

        #endregion

        #region Overrides

        protected override void OnDeserializing(StreamingContext context)
        {
            base.OnDeserializing(context);

            Initialize();
        }

        public override void CopyFrom(ICopyable c)
        {
            base.CopyFrom(c);

            var p = c as ICalendarParameter;
            if (p != null)
            {
                if (p.Values != null)
                    _Values = new List<string>(p.Values);
            }
        }

        #endregion

        #region IValueObject<string> Members

        [field:NonSerialized]
        public event EventHandler<ValueChangedEventArgs<string>> ValueChanged;

        protected void OnValueChanged(IEnumerable<string> removedValues, IEnumerable<string> addedValues)
        {
            if (ValueChanged != null)
                ValueChanged(this, new ValueChangedEventArgs<string>(removedValues, addedValues));
        }

        public virtual IEnumerable<string> Values
        {
            get { return _Values; }
        }

        public virtual bool ContainsValue(string value)
        {
            return _Values.Contains(value);
        }

        public virtual int ValueCount
        {
            get
            {
                return _Values != null ? _Values.Count : 0;
            }
        }

        public virtual void SetValue(string value)
        {
            if (_Values.Count == 0)
            {
                // Our list doesn't contain any values.  Let's add one!
                _Values.Add(value);
                OnValueChanged(null, new string[] { value });
            }
            else if (value != null)
            {                
                // Our list contains values.  Let's set the first value!
                var oldValue = _Values[0];
                _Values[0] = value;
                OnValueChanged(new string[] { oldValue }, new string[] { value });
            }
            else
            {
                // Remove all values
                var values = new List<string>(Values);
                _Values.Clear();
                OnValueChanged(values, null);
            }
        }

        public virtual void SetValue(IEnumerable<string> values)
        {                        
            // Remove all previous values
            var removedValues = _Values.ToList();
            _Values.Clear();
            _Values.AddRange(values);
            OnValueChanged(removedValues, values);
        }

        public virtual void AddValue(string value)
        {
            if (value != null)
            {
                _Values.Add(value);
                OnValueChanged(null, new string[] { value });
            }
        }

        public virtual void RemoveValue(string value)
        {
            if (value != null &&
                _Values.Contains(value) &&
                _Values.Remove(value))
            {
                OnValueChanged(new string[] { value }, null);
            }
        }

        #endregion

        #region ICalendarParameter Members

        public virtual string Value
        {
            get
            {
                if (Values != null)
                    return Values.FirstOrDefault();
                return default(string);
            }
            set
            {
                SetValue(value);
            }
        }

        #endregion
    }
}
