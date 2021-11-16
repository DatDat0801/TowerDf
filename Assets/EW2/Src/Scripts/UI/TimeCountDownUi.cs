using System;
using System.Collections;
using System.Collections.Generic;
using EW2;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountDownUi : MonoBehaviour
{
    [SerializeField] private Text titleTimeCountdown;

    [SerializeField] private TimeRemainUi timeRemainUi;

    private Action callbackWhenEndTime;

    public void SetData(long timeRemain, TimeRemainFormatType formatType = TimeRemainFormatType.Hhmmss, Action callbackEnd = null)
    {
         this.callbackWhenEndTime = callbackEnd;
        if (timeRemainUi)
        {
            timeRemainUi.SetTimeRemain(timeRemain, formatType, callbackWhenEndTime);
            timeRemainUi.gameObject.SetActive(true);
        }
        else
        {
            timeRemainUi.gameObject.SetActive(false);
        }
    }

    public void SetTitle(string title = "")
    {
        if (titleTimeCountdown)
        {
            if (title.Length == 0)
                titleTimeCountdown.text = L.button.free_name;
            else
                titleTimeCountdown.text = title;

            titleTimeCountdown.gameObject.SetActive(true);
        }
        else
        {
            titleTimeCountdown.gameObject.SetActive(false);
        }
    }
}