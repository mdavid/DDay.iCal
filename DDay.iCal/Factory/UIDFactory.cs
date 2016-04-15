using System;

namespace DDay.iCal
{
    public class UIDFactory
    {
        public virtual string Build()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
