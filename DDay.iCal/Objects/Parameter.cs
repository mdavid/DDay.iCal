using System;
using System.Collections;
using System.Text;

namespace DDay.iCal.Objects
{
    /// <summary>
    /// A class that provides additional information about a <see cref="ContentLine"/>.
    /// </summary>
    /// <remarks>
    /// <example>
    /// For example, a DTSTART line may look like this: <c>DTSTART;VALUE=DATE:20060116</c>.  
    /// The <c>VALUE=DATE</c> portion is a <see cref="Parameter"/> of the DTSTART value.
    /// </example>
    /// </remarks>
    public class Parameter : iCalObject
    {
        #region Public Fields

        public ArrayList Values = new ArrayList();

        #endregion

        #region Constructors

        public Parameter(iCalObject parent) : base(parent) { }
        public Parameter(iCalObject parent, string name) : base(parent, name)
        {
            AddToParent();
        }

        #endregion

        #region Protected Methods

        protected void AddToParent()
        {
            if (Parent != null &&
                Parent is ContentLine &&
                Name != null &&
                !((ContentLine)Parent).Parameters.ContainsKey(Name))
                ((ContentLine)Parent).Parameters[Name] = this;
        }

        #endregion
    }
}
