using System;
using System.Globalization;

namespace WebRubiksCubeTimer.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ConvertFromString(this DateTime dateTime, string toBeParsed, bool upperLimit = false)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string format = "d";

            if (upperLimit)
            {
                try
                {
                    DateTime date = toBeParsed != null ? DateTime.ParseExact(toBeParsed, format, provider) : new DateTime(3000, 1, 1, 0, 0, 0);
                    return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                }
                catch
                {
                    return new DateTime(3000, 1, 1, 0, 0, 0);
                }
            }
            else
            {
                try
                {
                    DateTime date = toBeParsed != null ? DateTime.ParseExact(toBeParsed, format, provider) : new DateTime(1, 1, 1, 0, 0, 0);
                    return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                }
                catch
                {
                    return new DateTime(1, 1, 1, 0, 0, 0);
                }
            }
        }
    }
}
