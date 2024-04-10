using System;
using UnityEngine;

namespace Code.Core.Utils
{
    public static class NowTime
    {
        private static long _serverTimeDiffMilli;

        public static long Milliseconds
        {
            get => DateTime.Now.Millisecond;
        }

        // public static long Milliseconds
        // {
        //     get => TimeUtil.GetMillis() + _serverTimeDiffMilli;
        //     set
        //     {
        //         _serverTimeDiffMilli = value - TimeUtil.GetMillis();
        //         Debug.Log($"_serverTimeDiffMilli {_serverTimeDiffMilli}");
        //     }
        // }

        public static long   Seconds                 => Milliseconds / 1000;
        public static long   LocalSeconds            => LocalMilliseconds / 1000;
        public static long   LocalMilliseconds       => TimeUtil.GetMillis();
        public static double LocalMillisecondsDouble => TimeUtil.GetMillisDouble();
        public static long   LocalTicks              => TimeUtil.GetTicks();
    }
}