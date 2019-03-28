using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZHSoft.Common
{
    public class DateTimeHelper
    {
        public static long DateTimeConvert2JavaTicks(DateTime time)
        {
            DateTime dt_1970 = new DateTime(1970, 1, 1, 8, 0, 0);
            return (time.Ticks - dt_1970.Ticks) / 10000;
        }
    }
}
