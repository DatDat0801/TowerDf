using System;
using System.Collections.Generic;

namespace EW2
{
    public class UserDailyCheckin
    {
        public List<CheckinTime> checkinTimes;

        [NonSerialized] public const int CIRCLE_DAY = 30;

        /// <summary>
        /// Get the last circle
        /// </summary>
        /// <returns></returns>
        public CheckinTime GetLastCircleCheckinTime()
        {
            if (checkinTimes == null)
            {
                checkinTimes = new List<CheckinTime>();
                var defaultCheckinTime = CheckinTime.CreateDefault();

                checkinTimes.Add(defaultCheckinTime);
                return defaultCheckinTime;
            }

            if (checkinTimes.Count <= 0)
            {
                var defaultCheckinTime = CheckinTime.CreateDefault();
                checkinTimes.Add(defaultCheckinTime);
                return defaultCheckinTime;
            }

            var circleCheckinAllDays = checkinTimes.Find(o => o.lastDayCheckedIn == CIRCLE_DAY);
            var uncompletedCircle = checkinTimes.Find(o => o.lastDayCheckedIn < CIRCLE_DAY);
            //Case: all circle are checked in
            if (circleCheckinAllDays != null && uncompletedCircle == null)
            {
                var defaultCheckinTime = CheckinTime.CreateDefault();
                if (circleCheckinAllDays.lastTimeCheckedIn.Date.Equals(TimeManager.NowUtc))
                {
                    defaultCheckinTime.lastTimeCheckedIn = circleCheckinAllDays.lastTimeCheckedIn;
                }
                defaultCheckinTime.lastTimeCheckedIn = checkinTimes[0].lastTimeCheckedIn;
                checkinTimes.Add(defaultCheckinTime);
                return defaultCheckinTime;
            }

            return checkinTimes[checkinTimes.Count - 1];
        }

        /// <summary>
        /// Get the circle showing on UI
        /// </summary>
        /// <returns></returns>
        public CheckinTime GetCurrentShowCheckinTime()
        {
            if (checkinTimes == null)
            {
                checkinTimes = new List<CheckinTime>();
                var defaultCheckinTime = CheckinTime.CreateDefault();
                checkinTimes.Add(defaultCheckinTime);
                return defaultCheckinTime;
            }

            if (checkinTimes.Count <= 0)
            {
                var defaultCheckinTime = CheckinTime.CreateDefault();
                checkinTimes.Add(defaultCheckinTime);
                return defaultCheckinTime;
            }

            if (checkinTimes[0].lastDayTaken == CIRCLE_DAY)
            {
                var defaultCheckinTime = CheckinTime.CreateDefault();
                
                checkinTimes.Add(defaultCheckinTime);
                return defaultCheckinTime;
            }

            return checkinTimes[0];
        }

        public void CheckIn()
        {
            for (var i = 0; i < checkinTimes.Count; i++)
            {
                if (checkinTimes[i].lastDayCheckedIn < CIRCLE_DAY)
                {
                    checkinTimes[i].lastDayCheckedIn++;
                    checkinTimes[i].lastTimeCheckedIn = TimeManager.NowUtc;
                    return;
                }
            }
        }

        public void TakeReward()
        {
            var current = GetCurrentShowCheckinTime();

            current.lastDayTaken = current.lastDayCheckedIn;
            current.lastTimeTaken = TimeManager.NowUtc;
            if (current.lastDayTaken == CIRCLE_DAY)
            {
                checkinTimes.Remove(current);
            }
        }

        /// <summary>
        /// Check in all days in the last circle
        /// </summary>
        public void CheckInAllDays()
        {
            var last = GetLastCircleCheckinTime();
            last.lastDayCheckedIn = CIRCLE_DAY;
            last.lastTimeCheckedIn = TimeManager.NowUtc;
        }
    }

    [Serializable]
    public class CheckinTime
    {
        /// <summary>
        /// Auto generated id
        /// </summary>
        public string circleId;

        public DateTime lastTimeCheckedIn;

        /// <summary>
        /// 1-30
        /// </summary>
        public int lastDayCheckedIn;

        /// <summary>
        /// 1-30
        /// </summary>
        public int lastDayTaken;

        public DateTime lastTimeTaken;

        public static CheckinTime CreateDefault()
        {
            return new CheckinTime()
            {
                circleId = ShortId.Generate(10), lastTimeCheckedIn = default, lastDayTaken = 0,
                lastTimeTaken = default
            };
        }
    }
}