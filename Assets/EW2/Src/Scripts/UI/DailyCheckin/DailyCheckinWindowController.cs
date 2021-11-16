using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using Coffee.UIExtensions;
using EW2.Tools;
using Hellmade.Sound;
using Lean.Pool;
using Sirenix.OdinInspector;
using SocialTD2;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2.DailyCheckin
{
    public class DailyCheckinWindowController : AWindowController
    {
        [SerializeField] private Transform itemContainer;
        [SerializeField] private DailyCheckinItemUI itemPrefab;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button claimButton;
        [SerializeField] private Text progress;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private TimeRemainUi timer;
        [SerializeField] private Text claimTxt;
        [SerializeField] private int testIndex;
        private int firstCanTakeIndex = -1;
        private WaitForSeconds wait = new WaitForSeconds(0.1f);
        private List<Reward> canTakeRewards;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(CloseClick);
            claimButton.onClick.AddListener(ClaimClick);

            //init cheating
            if (GameLaunch.isCheat)
            {
                cheatCheckInAllDay.gameObject.SetActive(true);
                cheatCheckInOneDay.gameObject.SetActive(true);
                cheatCheckInAllDay.onClick.AddListener(UnlockAllDays);
                cheatCheckInOneDay.onClick.AddListener(CheckInNewDay);
            }
            else
            {
                cheatCheckInAllDay.gameObject.SetActive(false);
                cheatCheckInOneDay.gameObject.SetActive(false);
            }

        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            Initialize();
        }

        private void Initialize()
        {
            itemContainer.DestroyAllChildren();
            canTakeRewards = new List<Reward>();
            var dailyCheckinDb = GameContainer.Instance.GetDailyCheckinDb();
            var userData = UserData.Instance.UserDailyCheckin;

            var currentCircle = userData.GetCurrentShowCheckinTime();

            var lastCheckinDay = currentCircle.lastDayCheckedIn;
            var lastTakenDay = currentCircle.lastDayTaken;
            if (progress != null)
            {
                progress.text = string.Format(L.popup.checkin_progress, lastCheckinDay,
                    dailyCheckinDb.rewardItems.Length);
            }

            for (var i = 0; i < dailyCheckinDb.rewardItems.Length; i++)
            {
                var day = dailyCheckinDb.rewardItems[i].day;
                var item = LeanPool.Spawn(itemPrefab, itemContainer);
                if (day <= lastTakenDay)
                {
                    item.Repaint(day, dailyCheckinDb.rewardItems[i].reward, CheckinUIItemState.Taken);
                }
                else if (day > lastTakenDay && day <= lastCheckinDay)
                {
                    var r = dailyCheckinDb.rewardItems[i].reward;
                    if (firstCanTakeIndex < i)
                    {
                        firstCanTakeIndex = i;
                    }
                    canTakeRewards.Add(r);
                    item.Repaint(day, r, CheckinUIItemState.CanTake);
                }
                else
                {
                    item.Repaint(day, dailyCheckinDb.rewardItems[i].reward, CheckinUIItemState.Alive);
                }
            }

            //repaint Claim button
            if (canTakeRewards.Count <= 0) //currentCircle.lastTimeTaken.Date.CompareTo(TimeManager.NowUtc.Date) == 0
            {
                claimTxt.enabled = false;
                claimButton.interactable = false;
                claimButton.GetComponent<UIEffect>().enabled = true;


                var utcNow = TimeManager.NowUtc;
                var remainingTime = new TimeSpan(1, 0, 0, 0, 0) -
                                    new TimeSpan(0, utcNow.Hour, utcNow.Minute, utcNow.Second, utcNow.Millisecond);
                timer.SetTimeRemain((long)remainingTime.TotalSeconds, TimeRemainFormatType.Hhmmss, delegate
               {
                   AutoCheckin();
                   Initialize();
               });
                timer.gameObject.SetActive(true);
            }
            else
            {
                claimTxt.enabled = true;
                timer.gameObject.SetActive(false);
                claimButton.GetComponent<UIEffect>().enabled = false;
                claimButton.interactable = true;
            }
            this.gameObject.SetActive(true);
            StartCoroutine(ScrollToCanTakeReward());
        }
        private IEnumerator ScrollToCanTakeReward()
        {
            yield return wait;
            if (firstCanTakeIndex >= 12)
            {
                if (firstCanTakeIndex >= 24)
                {
                    firstCanTakeIndex = 23;
                }
                scrollRect.SnapTo((RectTransform)scrollRect.content.GetChild(firstCanTakeIndex), 300, true);
            }
        }
        private void ClaimClick()
        {
            try
            {
                if (canTakeRewards.Count == 1)
                {
                    PopupUtils.ShowReward(canTakeRewards[0]);
                }
                else if (canTakeRewards.Count > 1)
                {
                    PopupUtils.ShowReward(canTakeRewards.ToArray());
                }

                EventManager.EmitEvent(GamePlayEvent.OnClaimCheckinDaily);
                Reward.AddToUserData(canTakeRewards.ToArray(), "daily_check_in");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }

            var userData = UserData.Instance.UserDailyCheckin;
            // var currentCircle = userData.GetCurrentShowCheckinTime();
            // currentCircle.lastDayTaken = currentCircle.lastDayCheckedIn;
            // currentCircle.lastTimeTaken = TimeManager.NowUtc;
            userData.TakeReward();
            UserData.Instance.Save();
            LoadSaveUtilities.AutoSave(false);

            Initialize();
            EventManager.EmitEventData(GamePlayEvent.DAILY_CHECKIN, false);
        }

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }

        #region Check-in

        public static void AutoCheckin()
        {
            var userData = UserData.Instance.UserDailyCheckin;
            var currentCircle = userData.GetLastCircleCheckinTime();
            //make sure the last circle and the current circle is not same day

            //Auto checkin if possible
            if (currentCircle.lastTimeCheckedIn.Date.CompareTo(TimeManager.NowUtc.Date) != 0)
            {
                userData.CheckIn();
                UserData.Instance.Save();
                EventManager.EmitEventData(GamePlayEvent.DAILY_CHECKIN, true);
                //Notify here
            }
            else
            if (currentCircle.lastDayTaken < currentCircle.lastDayCheckedIn)
            {
                EventManager.EmitEventData(GamePlayEvent.DAILY_CHECKIN, true);
            }
        }

        #endregion


        #region Cheats

        [SerializeField] private Button cheatCheckInOneDay;
        [SerializeField] private Button cheatCheckInAllDay;

        [Button]
        void CheckInNewDay()
        {
            var userData = UserData.Instance.UserDailyCheckin;
            userData.CheckIn();
            var currentCircle = userData.GetLastCircleCheckinTime();
            if (currentCircle.lastTimeTaken != default)
            {
                currentCircle.lastTimeTaken = currentCircle.lastTimeTaken.Subtract(TimeSpan.FromDays(1));
            }

            UserData.Instance.Save();
            Initialize();
            EventManager.EmitEventData(GamePlayEvent.DAILY_CHECKIN, true);
        }

        [Button]
        void UnlockAllDays()
        {
            var userData = UserData.Instance.UserDailyCheckin;
            userData.CheckInAllDays();

            var currentCircle = userData.GetLastCircleCheckinTime();
            if (currentCircle.lastTimeTaken != default)
            {
                currentCircle.lastTimeTaken = currentCircle.lastTimeTaken.Subtract(TimeSpan.FromDays(1));
            }

            EventManager.EmitEventData(GamePlayEvent.DAILY_CHECKIN, true);
            UserData.Instance.Save();
            Initialize();
        }

        #endregion
    }
}
