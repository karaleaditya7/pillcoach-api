using System;

namespace OntrackDb.Helper
{
    public class UtilityHelper
    {
        private static readonly Random _random = new Random();

        public static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static DateTime GetStartDateByMonth(DateTime endDate,int month)
        {
            return endDate.AddMonths((-1) * month);
        }
    }
}
