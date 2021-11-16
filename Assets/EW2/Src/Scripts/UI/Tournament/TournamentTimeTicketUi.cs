using System;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class TournamentTimeTicketUi : MonoBehaviour
    {
        [SerializeField] private TimeRemainUi txtTimeResetNewDay;

        public void ShowUi()
        {
            var userData = UserData.Instance.TournamentData;
            userData.CheckNewDay();
            this.txtTimeResetNewDay.SetTimeRemain(userData.GetTimeRemainResetNewDay(), TimeRemainFormatType.Hhmmss,
                HandleResetTicket);
        }

        private void HandleResetTicket()
        {
            ShowUi();
        }
    }
}