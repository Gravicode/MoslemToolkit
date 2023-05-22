using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class DateHelper
    {

        public static int GetSemesterFromDate(DateTime date)
        {
            var startDate = new DateTime(date.Year,1,1);
            var endDate = new DateTime(date.Year,6,1).AddMonths(1).AddDays(-1);
            return date >= startDate && date <= endDate ? 1 : 2;
        }
        static CultureInfo ci = new CultureInfo("id-ID");
        public static string GetMonthName(int month)
        {
            if (month < 1 || month > 12) return "";
            var monthName = ci.DateTimeFormat.GetMonthName(month);
            return monthName;
        }
        public static string GetDayName(DayOfWeek day)
        {
            var dayName = ci.DateTimeFormat.GetDayName(day);
            return dayName;
        }
        public static DateTime GetLocalTimeNow()
        {
            var localTimeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "SE Asia Standard Time" : "Asia/Jakarta";
            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(localTimeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone);
        }
    }
}
