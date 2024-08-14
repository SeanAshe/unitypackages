using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace Cosmos
{
    public enum eDateTimeFormatType
    {
        Simple,
        Full,
        OnlyTime,
    }
    public enum eTimeSign
    {
        Year,
        Month,
        Day,
        Hour,
        Minutes,
        Seconds,
    }
    public static class CTime
    {
        public static TimeSpanFormat TimeSpanFormat = AssetDatabase.LoadAssetAtPath<TimeSpanFormat>(@"Assets/Scripts/StaticConfig/TimeSpanFormat.asset");
        public static DateFormat DateFormat = AssetDatabase.LoadAssetAtPath<DateFormat>(@"Assets/Scripts/StaticConfig/DateFormat.asset");
        /// <summary>
        /// Unix时间戳(ms)到本地时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime UtcToLocalDateTime(long timeStamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timeStamp).DateTime.ToLocalTime();
        }
        /// <summary>
        /// 本地时间到Unix时间戳(ms)
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static double LocalDateTimeToUtc(in DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
        /// <summary>
        /// 将日期格式化输出
        /// </summary>
        public static string ToFormatString(this DateTime dt, eDateTimeFormatType type = eDateTimeFormatType.Simple, bool withtime = false, bool is12 = false)
        {
            string monthday = dt.Month + DateFormat.Month + dt.Day + DateFormat.Day;
            var hour = is12 ? dt.Hour % 12 : dt.Hour;
            var sub = is12 ? dt.Hour >= 12 ? DateFormat.Afternoon : DateFormat.Morning : "";
            string time = sub + hour.ToString() + DateFormat.Hour + dt.Minute + DateFormat.Minutes + dt.Second + DateFormat.Seconds;
            if (type == eDateTimeFormatType.Simple)
                return monthday + (withtime ? time : "");
            string year = dt.Year + DateFormat.Year;
            if (type == eDateTimeFormatType.Full)
                return year + " " + monthday + (withtime ? " " + time : "");
            if (type == eDateTimeFormatType.OnlyTime)
                return time;
            return monthday + (withtime ? time : "");
        }
        /// <summary>
        /// 将一段时间转化为描述字符串
        /// </summary>
        /// <param name="count">保留的（年、月、天、时、分、秒）个数</param>
        /// <param name="maxUnit">显示的最高单位（年、月、天、时、分、秒）</param>
        /// <returns></returns>
        public static string ToFormatString(this TimeSpan ts, int count = 3, eTimeSign maxUnit = eTimeSign.Hour)
        {
            if (count > 6) count = 6;
            else if (count < 1) count = 1;

            int[] timeticks = GetTimeTicks(ts);
            string buff = "";
            int curUnit = (int)maxUnit;

            for (int i = 0; i < count; i++)
            {
                var num = 0;
                for (int j = 0; j <= curUnit; j++)
                {
                    if (j == curUnit)
                    {
                        num = timeticks[j];
                        timeticks[j] = 0;
                        break;
                    }
                    timeticks[j + 1] += timeticks[j] * TimeSpanFormat.carry[j];
                    timeticks[j] = 0;
                }
                buff += num.ToString() + TimeSpanFormat.list[curUnit];
                curUnit += 1;
                if (curUnit >= timeticks.Length) break;
            }
            return buff;
        }
        /// <summary>
        /// 将一段时间转化为描述字符串 "00:00:00"
        /// </summary>
        public static string ToFormatStringSimple(this TimeSpan ts, in string sep = ":") => $"{ts.Hours}" + sep + $"{ts.Minutes}" + sep + $"{ts.Seconds}";
        /// <summary>
        /// 获得一个（年、月、天、时、分、秒）的数组
        /// </summary>
        public static int[] GetTimeTicks(in TimeSpan ts)
        {
            var Days = (int)ts.TotalDays;
            var Year = Days / 365;
            Days -= Year * 365;
            var Month = Days / 30;
            Days -= Month * 30;
            return new int[]
            {
                Year, Month, Days, ts.Hours, ts.Minutes, ts.Seconds
            };
        }
        /// <summary>
        /// 两个时间间的天数差值
        /// </summary>
        /// <param name="lasttime">前一天</param>
        /// <param name="now">后一天</param>
        /// <param name="offsetSeconds">刷新时间秒数，例如6点刷新，则为6*3600</param>
        /// <returns></returns>
        public static int GetDayDiff(in DateTime lasttime, in DateTime now, int offsetSeconds = 0)
        {
            var d1 = lasttime - new TimeSpan(0, 0, offsetSeconds);
            var d2 = now - new TimeSpan(0, 0, offsetSeconds);
            d1 = new DateTime(d1.Year, d1.Month, d1.Day);
            d2 = new DateTime(d2.Year, d2.Month, d2.Day);
            return (int)d2.Subtract(d1).TotalDays;
        }
        /// <summary>
        /// 两个时间间的周数差值
        /// </summary>
        /// <param name="lasttime"></param>
        /// <param name="now"></param>
        /// <param name="startDay"></param>
        /// <returns></returns>
        public static int GetWeekDiff(in DateTime lasttime, in DateTime now, in DayOfWeek startDay = DayOfWeek.Monday, int offsetSeconds = 0)
        {
            var d1 = lasttime.Subtract(new TimeSpan(0, 0, offsetSeconds)).Date;
            var d2 = now.Subtract(new TimeSpan(0, 0, offsetSeconds)).Date;
            d1 = setDayToDayOfWeek(d1, startDay);
            d2 = setDayToDayOfWeek(d2, startDay);
            return (int)((d2 - d1).TotalDays / 7);
        }
        /// <summary>
        /// 两个时间间的月数差值
        /// </summary>
        /// <param name="lasttime"></param>
        /// <param name="now"></param>
        /// <param name="startDay"></param>
        /// <returns></returns>
        public static int GetMonthDiff(in DateTime lasttime, in DateTime now, int offsetSeconds = 0)
        {
            var d1 = lasttime.Subtract(new TimeSpan(0, 0, offsetSeconds)).Date;
            var d2 = now.Subtract(new TimeSpan(0, 0, offsetSeconds)).Date;
            d1 = new DateTime(d1.Year, d1.Month, 1);
            d2 = new DateTime(d2.Year, d2.Month, 1);
            int diff = 12 * (d2.Year - d1.Year) + d2.Month - d1.Month;
            return diff;
        }
        private static DateTime setDayToDayOfWeek(in DateTime date, in DayOfWeek dayOfWeek)
        {
            int day1 = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
            int day2 = date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
            int dayOffset = day1 - day2;
            if (dayOffset > 0) dayOffset -= 7;
            return date.AddDays(dayOffset);
        }
        /// <summary>
        /// 判断某个日期是否在某段日期范围内
        /// </summary>
        /// <param name="dt">要判断的日期</param>
        /// <param name="dtBegin">开始日期</param>
        /// <param name="dtEnd">结束日期</param>
        /// <returns></returns>
        public static bool IsInDateRange(in DateTime dt, in DateTime dtBegin, in DateTime dtEnd)
        {
            return dt.CompareTo(dtBegin) >= 0 && dt.CompareTo(dtEnd) <= 0;
        }
        /// <summary>
        /// 判断某个日期是否是今天
        /// </summary>
        /// <param name="dt">要判断的日期</param>
        /// <returns></returns>
        public static bool IsToday(in DateTime dt)
        {
            return dt.Subtract(DateTime.Now).Days == 0;
        }
        public static DateTime GetDayStartTime(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0, dt.Kind);
        }
        public static DateTime GetDayEndTime(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999, dt.Kind);
        }
    }
}
