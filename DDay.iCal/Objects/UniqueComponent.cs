using System;
using System.Collections;
using System.Text;
using DDay.iCal.Components;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Objects
{
    /// <summary>
    /// Represents a unique component, a component with a unique UID,
    /// which can be used to uniquely identify the component.
    /// </summary>
    public class UniqueComponent : ComponentBase
    {
        #region Constructors

        public UniqueComponent(iCalObject parent) : base(parent) {}
        public UniqueComponent(iCalObject parent, string name) : base(parent, name) { }

        #endregion

        #region Public Fields

        public Text UID;

        #endregion        

        #region Static Public Methods

        static public Text NewUID()
        {
            return new Text(Guid.NewGuid().ToString());
        }

        #endregion
    }
}
