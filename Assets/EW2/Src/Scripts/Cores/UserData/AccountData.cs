using System;
using System.Collections.Generic;
using EW2.Tutorial.General;
using UnityEngine;
using Zitga.TrackingFirebase;
using ZitgaLog;

namespace EW2
{
    public class OpenGameLog
    {
        public int openGameCount;
        public DateTime lastTimeOpenGame;
    }

    public class AccountData
    {
        public string userName;

        public string userId;

        public bool isChangedName;

        public int avatarId;


        public string tokenId;

        public string name;

        public string google_name;

        public string country;

        public string language;

        public ZitgaLog.AuthProvider authProvider = AuthProvider.NONE;

        public bool isFirstOpen = true;

        public bool isCompleteTrackingTut;

        public DateTime accountCreated;

        public Dictionary<int, List<int>> anyGroupCompleteStepTutorial;

        public bool isCompleteTutTrial;

        public OpenGameLog gameLog;

        public bool isCompleteRating;

        public List<int> mapCompleteShowRating;

        public int idMapShowRating = -1;

        public AccountData()
        {
            // if (LoadSaveUtilities.IsAuthenticated())
            // {
            //     userName = "Player_" + LoadSaveUtilities.GetUserID();
            //
            //     userId = LoadSaveUtilities.GetUserID();
            //token_id = userId;
            //}
            //else
            //{
            if (accountCreated == default || accountCreated.Year < 2021)
            {
                accountCreated = TimeManager.NowUtc;
            }

            userName = "Player_" + Ultilities.GetDeviceUniqueIdentifier().Remove(11);

            userId = Ultilities.GetDeviceUniqueIdentifier();
            //}

            anyGroupCompleteStepTutorial = new Dictionary<int, List<int>>();
            if (gameLog == null)
            {
                gameLog = new OpenGameLog();
            }

            if (this.mapCompleteShowRating == null)
                this.mapCompleteShowRating = new List<int>();
        }

        public void LogOpenGame()
        {
            if (TimeManager.NowUtc.Date != gameLog.lastTimeOpenGame.Date)
            {
                gameLog.openGameCount++;
                gameLog.lastTimeOpenGame = TimeManager.NowUtc;
            }
        }

        /// <summary>
        /// authenticated token
        /// </summary>
        public void SetToken(string token)
        {
            tokenId = token;
            userId = token;
        }

        public void SetProvider(ZitgaLog.AuthProvider provider)
        {
            authProvider = provider;
        }

        public ZitgaLog.AuthProvider GetProvider()
        {
            if (authProvider == AuthProvider.NONE)
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        return ZitgaLog.AuthProvider.ANDROID_DEVICE_ID;
                    case RuntimePlatform.IPhonePlayer:
                        return ZitgaLog.AuthProvider.IOS_DEVICE_ID;
                    default: return AuthProvider.WINDOWS_DEVICE_ID;
                }
            }
            else
            {
                return authProvider;
            }
        }

        public void AddCompletedGroupId(int tutorialId)
        {
            var stepTutorialData = GameContainer.Instance.GetTutorialData().GetStepTutorialData(tutorialId);
            var groupId = stepTutorialData.groupId;
            if (anyGroupCompleteStepTutorial.ContainsKey(groupId))
            {
                if (!anyGroupCompleteStepTutorial[groupId].Contains(tutorialId))
                {
                    anyGroupCompleteStepTutorial[groupId].Add(tutorialId);
                    //Debug.LogAssertion("ADDED tutorial id: " + tutorialId + "into group: " + groupId);
                }
            }
            else
            {
                anyGroupCompleteStepTutorial.Add(groupId, new List<int> {tutorialId});
                //Debug.LogAssertion("ADDED tutorial id: " + tutorialId + " into group: " + groupId);
            }

            //tracking
            if (CheckCompleteGroupTutorial(AnyTutorialConstants.GROUP_0) && !isCompleteTrackingTut)
            {
                AppsflyerUtils.Instance.CompleteTutorialEvent();
                isCompleteTrackingTut = true;
                UserData.Instance.Save();
            }
        }

        public bool CheckCompleteGroupTutorial(int groupId)
        {
            var ok = anyGroupCompleteStepTutorial.ContainsKey(groupId) && anyGroupCompleteStepTutorial[groupId].Count ==
                GameContainer.Instance.GetTutorialData().CalculateCountTutorialInGroup(groupId);
            return ok;
        }

        /// <summary>
        /// Skip game play tutorial
        /// </summary>
        public void AddFullGroupTutorial()
        {
            //anyGroupCompleteStepTutorial.Clear();
            // var tutorialData = GameContainer.Instance.GetTutorialData();
            // foreach (var groupId in tutorialData.CalculateAnyGroups())
            // {
            //     AddFullGroupTutorialFollowId(groupId);
            // }
            anyGroupCompleteStepTutorial[0].Clear();
            AddFullGroupTutorialFollowId(0);
        }

        public void AddFullGroupTutorialFollowId(int groupId)
        {
            //Debug.LogAssertion("AccountData hashcode "+this.GetHashCode());
            var tutorialData = GameContainer.Instance.GetTutorialData();
            var anyStepTutorialData = tutorialData.AnyStepTutorialData;
            if (anyGroupCompleteStepTutorial.ContainsKey(groupId))
            {
                anyGroupCompleteStepTutorial[groupId].Clear();
            }

            foreach (var stepTutorialData in anyStepTutorialData)
            {
                if (stepTutorialData.groupId != groupId)
                {
                    continue;
                }

                if (anyGroupCompleteStepTutorial.ContainsKey(groupId))
                {
                    anyGroupCompleteStepTutorial[groupId].Add(stepTutorialData.tutorialId);
                }
                else
                {
                    anyGroupCompleteStepTutorial.Add(groupId, new List<int> {stepTutorialData.tutorialId});
                }
            }

            //Debug.LogAssertion("ADD FULL TUTORIAL");
        }

        public void ExecuteAutoGroupCompleteTutorial()
        {
            foreach (var groupCompleteStepTutorial in anyGroupCompleteStepTutorial)
            {
                foreach (var stepTutorialId in groupCompleteStepTutorial.Value)
                {
                    var stepTutorialData = GameContainer.Instance.GetTutorialData().GetStepTutorialData(stepTutorialId);
                    if (stepTutorialData.isAutoCompleteGroup)
                    {
                        AddFullGroupTutorialFollowId(stepTutorialData.groupId);

                        break;
                    }
                }
            }
        }
    }
}