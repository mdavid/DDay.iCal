﻿namespace DDay.iCal
{
    public class AssociationUtil
    {
        #region Static Public Methods

        public static void AssociateItem(object item, ICalendarObject objectToAssociate)
        {
            if (item is ICalendarDataType)
                ((ICalendarDataType)item).AssociatedObject = objectToAssociate;
            else if (item is ICalendarObject)
                ((ICalendarObject)item).Parent = objectToAssociate;
        }

        public static void DeassociateItem(object item)
        {
            if (item is ICalendarDataType)
                ((ICalendarDataType)item).AssociatedObject = null;
            else if (item is ICalendarObject)
                ((ICalendarObject)item).Parent = null;
        }

        #endregion
    }
}
