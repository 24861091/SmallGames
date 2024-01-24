using System;
using System.Runtime.CompilerServices;

namespace Core.Utils
{
    public class TimeUtil
    {
        private static readonly DateTime UtcZero = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static void GetHourMinSec(long secs, out int h, out int m, out int s)
        {
            h    =  (int)(secs / 3600);
            secs -= h * 3600;
            m    =  (int)(secs / 60);
            s    =  (int)(secs - m * 60);
        }

        public static void GetDayHourMinSec(long secs, out int d, out int h, out int m, out int s)
        {
            d    =  (int)(secs / 86400);
            secs -= d * 86400;
            h    =  (int)(secs / 3600);
            secs -= h * 3600;
            m    =  (int)(secs / 60);
            s    =  (int)(secs - m * 60);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ConvertToSecs(DateTime dateTime)
        {
            var ts = dateTime.ToUniversalTime() - UtcZero;
            return (long)ts.TotalSeconds;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetTimeStamp()
        {
            var ts = DateTime.UtcNow - UtcZero;
            return (long)ts.TotalSeconds;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetMillis()
        {
            var ts = DateTime.UtcNow - UtcZero;
            return (long)ts.TotalMilliseconds;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetMillisDouble()
        {
            return (DateTime.UtcNow - UtcZero).TotalMilliseconds;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetTicks()
        {
            return (DateTime.UtcNow - UtcZero).Ticks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetTodayStartSecs()
        {
            var diff = DateTime.Today.ToUniversalTime() - UtcZero;
            return (long)diff.TotalSeconds;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetTomorrowStartSecs()
        {
            var diff = DateTime.Today.AddDays(1).ToUniversalTime() - UtcZero;
            return (long)diff.TotalSeconds;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime GetDayStart(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime GetDayStart(long seconds, DateTimeKind kind = DateTimeKind.Local)
        {
            return GetDayStart(FromSeconds(seconds, kind));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetDayStartSecs(long seconds, DateTimeKind kind = DateTimeKind.Local)
        {
            return GetDayStartSecs(FromSeconds(seconds, kind));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetDayStartSecs(DateTime dateTime)
        {
            return ConvertToSecs(GetDayStart(dateTime).ToUniversalTime());
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime FromSeconds(long seconds, DateTimeKind kind = DateTimeKind.Local)
        {
            var t = UtcZero.AddSeconds(seconds);
            if (kind == DateTimeKind.Local) return t.ToLocalTime();
            return t.ToUniversalTime();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime FromMilliseconds(long milliseconds, DateTimeKind kind = DateTimeKind.Local)
        {
            var t = UtcZero.AddMilliseconds(milliseconds);
            if (kind == DateTimeKind.Local) return t.ToLocalTime();
            return t.ToUniversalTime();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetDayDiff(long ts, DateTimeKind kind = DateTimeKind.Local)
        {
            var today = DateTime.Today;
            return (FromSeconds(ts, kind) - today).Days;
        }
        
    }
}