using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace DDay.iCal.DataTypes
{
    class ByDay : iCalDataType, IComparable
    {
        public int Num;
        public DayOfWeek DayOfWeek;

        public ByDay()
        {
             Num = int.MinValue;
        }

        public ByDay(string value)
            : this()
        {
            CopyFrom((ByDay)Parse(value));
        }

        public ByDay(DayOfWeek day)
            : this()
        {
            this.DayOfWeek = day;
        }

        public override void CopyFrom(object obj)
        {
            if (obj is ByDay)
            {
                ByDay bd = (ByDay)obj;
                this.Num = bd.Num;
                this.DayOfWeek = bd.DayOfWeek;
            }
        }

        public override bool TryParse(string value, ref object obj)
        {
            ByDay bd = (ByDay)obj;

            Match bdMatch = Regex.Match(value, @"(\+|-)?(\d{1,2})?(\w{2})");
            if (bdMatch.Success)
            {
                if (bdMatch.Groups[2].Success)
                {
                    bd.Num = Convert.ToInt32(bdMatch.Groups[2].Value);
                    if (bdMatch.Groups[1].Success && bdMatch.Groups[1].Value.Contains("-"))
                        bd.Num *= -1;
                }
                bd.DayOfWeek = Recur.GetDayOfWeek(bdMatch.Groups[3].Value);
                return true;
            }
            return false;            
        }

        public bool CheckValidDate(Recur r, Date_Time Date)
        {    
            bool valid = false;
            
            if (this.DayOfWeek == Date.Value.DayOfWeek)
                valid = true;

            if (valid && this.Num != int.MinValue)
            {
                int mult = (this.Num < 0) ? -1 : 1;
                int offset = (this.Num < 0) ? 1 : 0;
                int abs = Math.Abs(this.Num);

                switch (r.Frequency)
                {
                    case Recur.FrequencyType.MONTHLY:
                        {
                            DateTime mondt = new DateTime(Date.Value.Year, Date.Value.Month, 1, Date.Value.Hour, Date.Value.Minute, Date.Value.Second, Date.Value.Kind);
                            DateTime.SpecifyKind(mondt, Date.Value.Kind);
                            if (offset > 0)
                                mondt = mondt.AddMonths(1).AddDays(-1);

                            while (mondt.DayOfWeek != this.DayOfWeek)
                                mondt = mondt.AddDays(mult);

                            for (int i = 1; i < abs; i++)
                                mondt = mondt.AddDays(7 * mult);

                            if (Date.Value.Date != mondt.Date)
                                valid = false;
                        } break;

                    case Recur.FrequencyType.YEARLY:
                        {
                            // If BYMONTH is specified, then offset our tests
                            // by those months; otherwise, begin with Jan. 1st.
                            // NOTE: fixes USHolidays.ics eval
                            ArrayList months = new ArrayList();
                            if (r.ByMonth.Count == 0)
                                months.Add(1);
                            else months = r.ByMonth;

                            bool found = false;
                            foreach (int month in months)
                            {
                                DateTime yeardt = new DateTime(Date.Value.Year, month, 1, Date.Value.Hour, Date.Value.Minute, Date.Value.Second, Date.Value.Kind);
                                DateTime.SpecifyKind(yeardt, Date.Value.Kind);
                                if (offset > 0)
                                    yeardt = yeardt.AddYears(1).AddDays(-1);

                                while (yeardt.DayOfWeek != this.DayOfWeek)
                                    yeardt = yeardt.AddDays(mult);

                                for (int i = 1; i < abs; i++)
                                    yeardt = yeardt.AddDays(7 * mult);

                                if (Date.Value == yeardt)
                                    found = true;
                            }

                            if (!found)
                                valid = false;
                        } break;

                    // Ignore other frequencies
                    default: break;
                }
            }
            return valid;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            ByDay bd = null;
            if (obj is string)
                bd = new ByDay(obj as string);
            else if (obj is ByDay)
                bd = (ByDay)obj;

            if (bd == null)
                throw new ArgumentException();
            else return this.DayOfWeek.CompareTo(bd.DayOfWeek);
        }

        #endregion
    }
}
