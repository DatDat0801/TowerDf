using System;
using System.Collections;
using Invoke;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public enum TimeRemainFormatType
    {
        Hhmmss,
        Ddhhmmss,
    }

    public class TimeRemainUi : MonoBehaviour
    {
        public Text timeLb;

        private long timeRemain;

        private TimeRemainFormatType formatType;

        private Action callbackWhenEndTime;

        private Coroutine coroutineCooldown;

        public void SetTimeRemain(long timeRemain, TimeRemainFormatType type = TimeRemainFormatType.Hhmmss,
            Action cbEndTime = null)
        {
            this.timeRemain = timeRemain + 1;

            if (coroutineCooldown != null)
                CoroutineUtils.Instance.StopCoroutine(coroutineCooldown);

            coroutineCooldown = CoroutineUtils.Instance.StartCoroutine(Cooldown());

            this.formatType = type;

            this.callbackWhenEndTime = cbEndTime;
        }

        public void StopUpdateTimeRemain()
        {
            if (coroutineCooldown != null)
                CoroutineUtils.Instance.StopCoroutine(coroutineCooldown);
        }

        public void SetText(string text)
        {
            timeLb.text = text;
        }

        public void SetActiveText(bool isActive)
        {
            timeLb.gameObject.SetActive(isActive);
        }

        private IEnumerator Cooldown()
        {
            while (timeRemain > 0)
            {
                UpdateTimeRemainUi();

                yield return new WaitForSecondsRealtime(1);
            }
        }

        void UpdateTimeRemainUi()
        {
            timeRemain -= 1;

            if (timeLb != null)
            {
                if (this.formatType == TimeRemainFormatType.Hhmmss)
                {
                    var timeString = TimeManager.FormatSecondsToHHMMSS((long)Mathf.Max(0, timeRemain));

                    if (timeLb)
                        timeLb.text = timeString;

                    if (timeRemain <= 0)
                    {
                        callbackWhenEndTime?.Invoke();

                        InvokeCallbackUtils.Instance.CancelInvoke(this, UpdateTimeRemainUi);
                    }
                }
                else if (this.formatType == TimeRemainFormatType.Ddhhmmss)
                {
                    string day = L.common.format_day;

                    string hour = L.common.format_hour;

                    string minute = L.common.format_minutes;

                    string second = L.common.format_seconds;

                    var timeString = TimeManager.FormatSecondsToDdhhmmss((long)Mathf.Max(0, timeRemain),
                        day, hour, minute, second);

                    if (timeLb)
                        timeLb.text = timeString;

                    if (timeRemain <= 0)
                    {
                        callbackWhenEndTime?.Invoke();

                        InvokeCallbackUtils.Instance.CancelInvoke(this, UpdateTimeRemainUi);
                    }
                }
            }
        }

        public void SetActive(bool v)
        {
            gameObject.SetActive(v);
        }
    }
}