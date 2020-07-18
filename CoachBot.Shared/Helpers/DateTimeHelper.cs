using System;

namespace CoachBot.Domain.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime RemoveSeconds(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, DateTimeKind.Utc);
        }
    }
}