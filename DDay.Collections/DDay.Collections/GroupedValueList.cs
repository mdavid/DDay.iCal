﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DDay.Collections
{
    public class GroupedValueList<TGroup, TInterface, TItem, TValueType> :
        GroupedList<TGroup, TInterface>,
        IGroupedValueList<TGroup, TInterface, TItem, TValueType>
        where TInterface : class, IGroupedObject<TGroup>, IValueObject<TValueType>
        where TItem : new()        
    {
        #region IKeyedValueList<TGroup, TObject, TValueType> Members

        public virtual void Set(TGroup group, TValueType value)
        {
            Set(group, new TValueType[] { value });
        }

        public virtual void Set(TGroup group, IEnumerable<TValueType> values)
        {
            if (ContainsKey(group))
            {
                var items = AllOf(group);
                if (items != null)
                {
                    // Add a value to the first matching item in the list
                    items.FirstOrDefault().SetValue(values);
                    return;
                }
            }

            // No matching item was found, add a new item to the list
            var obj = Activator.CreateInstance(typeof(TItem)) as TInterface;

            // Set the group for the object
            obj.Group = group;

            // Add the object to the list
            Add(obj);

            // Set the list of values for the object
            obj.SetValue(values);
        }

        public virtual TType Get<TType>(TGroup group)
        {
            var firstItem = AllOf(group).FirstOrDefault();
            if (firstItem != null &&
                firstItem.Values != null)
            {
                return firstItem
                    .Values
                    .OfType<TType>()
                    .FirstOrDefault();
            }
            return default(TType);
        }

        public virtual IList<TType> GetMany<TType>(TGroup group)
        {
            return new GroupedValueListProxy<TGroup, TInterface, TItem, TValueType, TType>(this, group);
        }

        #endregion
    }
}
