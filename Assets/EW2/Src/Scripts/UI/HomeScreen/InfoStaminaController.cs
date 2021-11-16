using System;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class InfoStaminaController : MonoBehaviour
    {
        [SerializeField] private GameObject bgr;

        [SerializeField] private TimeRemainUi timeRemainNext;

        [SerializeField] private TimeRemainUi timeRemainFull;

        private void OnEnable()
        {
            ShowUi();

            EventManager.StartListening(GamePlayEvent.OnMoneyChange(ResourceType.Money, MoneyType.Stamina), ShowUi);
        }

        private void ShowUi()
        {
            var timeNow = TimeManager.NowInSeconds;

            if (timeRemainFull)
            {
                var timeRemainFullStamina = UserData.Instance.RegenStamina.timeRegenFullSeconds - timeNow;

                if (timeRemainFullStamina > 0)
                {
                    timeRemainFull.SetTimeRemain(timeRemainFullStamina);
                }
                else
                {
                    timeRemainFull.StopUpdateTimeRemain();
                    timeRemainFull.timeLb.text = TimeManager.FormatSecondsToHHMMSS(0);
                }
            }

            if (timeRemainNext)
            {
                var timeRemainReceiveStamina = UserData.Instance.RegenStamina.nextTimeRegenSeconds - timeNow;

                if (timeRemainReceiveStamina > 0)
                {
                    timeRemainNext.SetTimeRemain(timeRemainReceiveStamina);
                }
                else
                {
                    timeRemainNext.StopUpdateTimeRemain();
                    timeRemainNext.timeLb.text = TimeManager.FormatSecondsToHHMMSS(0);
                }
            }
        }

        private void Update()
        {
            HideIfClickedOutside(bgr);
        }

        private void HideIfClickedOutside(GameObject panel)
        {
            if (Input.GetMouseButton(0) && gameObject.activeSelf &&
                !RectTransformUtility.RectangleContainsScreenPoint(
                    panel.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    Camera.main))
            {
                gameObject.SetActive(false);
            }
        }
    }
}