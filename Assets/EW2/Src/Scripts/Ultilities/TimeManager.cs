using System;
using System.Globalization;
using UnityEngine;

namespace EW2
{
    public sealed class TimeManager
    {
        private const float TimeOfDaySeconds = 86400;

        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static long DeltaMilisecondsWithServer { get; set; }

        public static bool IsSyncedTimeWithServer { get; private set; }

        private static DateTime hadDateTime;

        private static DateTime baseTime;

        private static string formatDDHHMMSS;

        /// <summary>
        /// Get current time in UTC
        /// </summary>
        public static DateTime NowUtc
        {
            get
            {
                if (GameLaunch.isCheat)
                {
                    return DateTime.Now;
                }
                if (IsSyncedTimeWithServer)
                {
                    return baseTime.AddSeconds(TimeManager.GetRealtimeSinceStartup())
                        .AddMilliseconds(DeltaMilisecondsWithServer);
                }
                else
                {
                    return DateTime.UtcNow.AddMilliseconds(DeltaMilisecondsWithServer);
                }
            }
        }

        public static long NowInSeconds
        {
            get { return (long) (NowUtc - Jan1St1970).TotalSeconds; }
        }

        public static long NowInDays
        {
            get { return (long) (NowUtc - Jan1St1970).TotalDays; }
        }

        public static DateTime EndTimeOfDay
        {
            get
            {
                var now = NowUtc;
                return new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Utc);
            }
        }

        public static long EndTimeOfDaySeconds
        {
            get { return (long) (EndTimeOfDay - Jan1St1970).TotalSeconds; }
        }

        public static DateTime StartTimeOfDay
        {
            get
            {
                var now = NowUtc;
                return new DateTime(now.Year, now.Month, now.Day, 00, 00, 00, 999, DateTimeKind.Utc);
            }
        }

        public static long StartTimeOfDaySeconds
        {
            get { return (long) (StartTimeOfDay - Jan1St1970).TotalSeconds; }
        }

        #region Setup

        public static void Setup(DateTime timeServer, bool isSyncSuccess)
        {
            if (IsSyncedTimeWithServer) return;
            IsSyncedTimeWithServer = isSyncSuccess;
            if (isSyncSuccess)
            {
                baseTime = timeServer;
                DeltaMilisecondsWithServer = ToMiliseconds(timeServer) - ToMiliseconds(baseTime);
                Debug.Log("[Time] Delta Seconds With Server: " + DeltaMilisecondsWithServer / 1000f);
            }

            Debug.Log("[Time] Current time UTC: " + NowUtc);
            Debug.Log("[Time] Is sync time success: " + isSyncSuccess);
        }

        public static void SetTimeCountCurrent()
        {
            hadDateTime = NowUtc;
        }

        #endregion

        #region Convert time

        public static DateTime ToDateTimeFromLongString(string timeString)
        {
            try
            {
                return DateTime.ParseExact(timeString, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return NowUtc;
            }
        }

        public static long ToSeconds(DateTime dateTime)
        {
            return (long) (dateTime.ToUniversalTime() - Jan1St1970).TotalSeconds;
        }

        public static long ToDays(DateTime dateTime)
        {
            return (long) (dateTime.ToUniversalTime() - Jan1St1970).TotalDays;
        }

        public static long ToDays(long seconds)
        {
            var dateTime = ToDateTimeFromSeconds(seconds);
            return ToDays(dateTime);
        }

        public static long ToMiliseconds(DateTime dateTime)
        {
            return (long) (dateTime.ToUniversalTime() - Jan1St1970).TotalMilliseconds;
        }

        public static DateTime ToDateTimeFromSeconds(long seconds)
        {
            return Jan1St1970.AddSeconds(seconds).ToUniversalTime();
        }

        public static DateTime ToDateTimeFromMiliseconds(long miliseconds)
        {
            return Jan1St1970.AddMilliseconds(miliseconds);
        }

        public static long GetTimeCountSeconds()
        {
            TimeSpan time = TimeSpan.Zero;
            if (hadDateTime != null)
            {
                time = NowUtc - hadDateTime;
            }

            return (long) time.TotalSeconds;
        }

        public static DateTime GetMidnightTime()
        {
            DateTime midNightTime = new DateTime(NowUtc.Year, NowUtc.Month, NowUtc.Day, 0, 0, 0);
            return midNightTime;
        }

        #endregion

        #region Format time

        /// <summary>
        /// Format time to long, last value is day
        /// </summary>
        public static long FormatTimeToDays(DateTime timeInput)
        {
            return long.Parse(timeInput.ToString("yyyyMMdd"));
        }

        public static long FormatTimeToSeconds(DateTime timeInput)
        {
            return long.Parse(timeInput.ToString("yyyyMMddhhmmss"));
        }

        public static string FormatSecondsToYyyymmdd(long time)
        {
            var dateTime = Jan1St1970.AddSeconds(time);
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string FormatSecondsToDdhhmmss(long time, string formatDay, string formatHour,
            string formatMinutes,
            string formatSeconds)
        {
            if (string.IsNullOrEmpty(formatDDHHMMSS))
                formatDDHHMMSS = L.gameplay.format_dd_hh_mm_ss;
            try
            {
                TimeSpan t = TimeSpan.FromSeconds(time);
                return string.Format(formatDDHHMMSS, t.Days + formatDay, t.Hours + formatHour,
                    t.Minutes + formatMinutes, t.Seconds + formatSeconds);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return string.Format(formatDDHHMMSS, 00, 00, 00, 00);
            }
        }

        internal static long GetEndTimeOfDaySeconds()
        {
            var dayOffset = (EndTimeOfDaySeconds - NowInSeconds) / TimeOfDaySeconds;
            return TimeManager.NowInSeconds + (long) (dayOffset * TimeOfDaySeconds);
        }

        public long GetEndTimeOfDaySecondsRemain()
        {
            return GetEndTimeOfDaySeconds() - TimeManager.NowInSeconds;
        }

        internal long GetEndTimeOfWeekSeconds()
        {
            var dt = TimeManager.NowUtc;
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            var lastDay = dt.AddDays(-diff + 7).Date;
            var endTimeSeconds = (long) (lastDay - Jan1St1970).TotalSeconds;
            var dayOffset = (endTimeSeconds - NowInSeconds) / TimeOfDaySeconds;
            return NowInSeconds + (long) (dayOffset * TimeOfDaySeconds);
        }

        internal long GetStartTimeOfDaySeconds()
        {
            var dayOffset = (TimeManager.StartTimeOfDaySeconds - TimeManager.NowInSeconds) /
                            TimeOfDaySeconds;
            return TimeManager.NowInSeconds + (long) (dayOffset * TimeOfDaySeconds);
        }

        public static long GetEndTimeOfDaySeconds(int day)
        {
            var endTime = GetEndTimeOfDaySeconds();
            return endTime + day * (long) TimeOfDaySeconds;
        }

        public static string FormatSecondsToHHMMSS(long duration)
        {
            if (duration <= 0)
            {
                return "00:00:00";
            }

            TimeSpan t = TimeSpan.FromSeconds(duration);
            return string.Format("{0:00}:{1:00}:{2:00}", (int) t.TotalHours, t.Minutes, t.Seconds);
        }

        public static string FormatToDDMMYYYY(DateTime timeInput)
        {
            return string.Format("{0}/{1}/{2}", timeInput.Day, timeInput.Month, timeInput.Year);
        }

        public static string FormatToDDMMYYYYHHMM(DateTime timeInput)
        {
            return string.Format("{0}/{1}/{2} {3}:{4}", timeInput.Day, timeInput.Month, timeInput.Year, timeInput.Hour,
                timeInput.Minute);
        }

        public static float GetDeltaTime()
        {
            float delta = Time.deltaTime;
            if (delta >= 0)
            {
                return delta;
            }
            else
            {
                return Time.timeScale / Application.targetFrameRate;
            }
        }

        public static void SetTimeScale(float time)
        {
            Time.timeScale = time;
        }

        internal static float GetTimeScale()
        {
            return Time.timeScale;
        }

        public static float GetRealtimeSinceStartup()
        {
            return Mathf.Max(Time.realtimeSinceStartup, 0);
        }

        #endregion

        public static long DayFromTime(long seconds)
        {
            var date = TimeSpan.FromSeconds(seconds);
            return (long) date.TotalDays;
        }
    }
}