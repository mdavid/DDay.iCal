using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace DDay.iCal.DataTypes
{
    /// <summary>
    /// Contains a collection of <see cref="Text"/> objects.
    /// When parsing a TextCollection, <see cref="Text"/> objects 
    /// are separated by commas.
    /// <example>
    ///     For example, <c>CATEGORIES:Business,Personal,Something with a comma\,</c>
    /// </example>
    /// </summary>    
    public class TextCollection : iCalDataType, ICollection
    {
        #region Private Fields

        private List<Text> m_Values;        

        #endregion

        #region Public Properties

        public List<Text> Values
        {
            get { return m_Values; }
            set { m_Values = value; }
        }

        #endregion

        #region Constructors

        public TextCollection() { Values = new List<Text>(); }
        public TextCollection(string value)
            : this()
        {            
            CopyFrom(Parse(value));            
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            string[] values = new string[Values.Count];
            for (int i = 0; i < Values.Count; i++)
                values[i] = Values[i].Value;

            return string.Join(",", values);
        }

        public override void CopyFrom(object obj)
        {            
            if (obj is TextCollection)
            {
                TextCollection tc = (TextCollection)obj;
                Text[] array = new Text[tc.Values.Count];
                tc.CopyTo(array, 0);
                
                Values.Clear();
                Values.AddRange(array);
            }
            base.CopyFrom(obj);
        }

        public override bool TryParse(string value, ref object obj)
        {
            TextCollection tc = (TextCollection)obj;
            MatchCollection matches = Regex.Matches(value, @"[^\\](,)");
            Values.Clear();
            
            int i = 0;            
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    tc.Values.Add(new Text(value.Substring(i, match.Index - i + 1)));
                    i = match.Index + 2;
                }
                else return false;
            }

            if (i < value.Length)
                tc.Values.Add(new Text(value.Substring(i, value.Length - i)));

            return true;            
        }

        #endregion

        #region Public Accessors

        [Browsable(false)]
        public Text this[object obj]
        {
            get
            {                
                if (obj is int)
                    return Values[(int)obj];
                else return null;
            }
        }

        #endregion

        #region ICollection Members

        [Browsable(false)]
        public void CopyTo(Array array, int index)
        {
            if (array.GetType().GetElementType() == typeof(Text))
                Values.CopyTo((Text[])array, index);
            else throw new ArgumentException("Array must be a Text array", "array");
        }

        [Browsable(false)]
        public int Count
        {
            get { return Values.Count; }
        }

        [Browsable(false)]
        public bool IsSynchronized
        {
            get { return false; }
        }

        [Browsable(false)]
        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        #endregion
    }
}
