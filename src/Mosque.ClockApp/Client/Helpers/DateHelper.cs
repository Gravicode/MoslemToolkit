using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Mosque.ClockApp.Client.Helpers
{
    public class DateHelper
    {
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
            var localTimeZoneId = "Asia/Bangkok";//"SE Asia Standard Time";
            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(localTimeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone);
        }

        public static string ConvertDateCalendar(DateTime DateConv, string Calendar, string DateLangCulture)
        {
            DateTimeFormatInfo DTFormat;
            DateLangCulture = DateLangCulture.ToLower();
            /// We can't have the hijri date writen in English. We will get a runtime error

            if (Calendar == "Hijri" && DateLangCulture.StartsWith("en-"))
            {
                DateLangCulture = "ar-sa";
            }

            /// Set the date time format to the given culture
            DTFormat = new System.Globalization.CultureInfo(DateLangCulture, false).DateTimeFormat;

            /// Set the calendar property of the date time format to the given calendar
            switch (Calendar)
            {
                case "Hijri":
                    DTFormat.Calendar = new System.Globalization.HijriCalendar();
                    break;

                case "Gregorian":
                    DTFormat.Calendar = new System.Globalization.GregorianCalendar();
                    break;

                default:
                    return "";
            }

            /// We format the date structure to whatever we want
            DTFormat.ShortDatePattern = "dd/MM/yyyy";
            return (DateConv.Date.ToString("f", DTFormat));
            //string date = ConvertDateCalendar(DateTime.Now, "Hijri", "en-US")
        }
    }
}
