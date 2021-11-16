using System;
using System.Collections.Generic;
using EW2.CampaignInfo.HeroSelect;
using UnityEngine;

namespace EW2
{
    public class UserHeroDefenseData
    {
        public int numberTicket;
        public int numberTrial;
        public long startTimeSeason;
        public long endTimeSeason;
        public long timeResetNewDay;
        public int timesWatchAds;
        public int defensePointId;
        public int currMapId;
        public int highestScore;
        public int highestScoreOld;
        public List<HeroSelectedData> listHeroSelected;
        public List<int> listDefensePointUnlocked;

        public bool showTrialPopupOneTime;
        public bool showFlowPopupOne;
        public long battleId;

        public UserHeroDefenseData()
        {
            var dataConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>()
                .GetDataConfig();
            this.numberTicket = dataConfig.ticketFree;
            this.timesWatchAds = dataConfig.numberWatchAds;
            this.numberTrial = dataConfig.numberTrial;
            if (this.listHeroSelected == null)
            {
                this.listHeroSelected = new List<HeroSelectedData>();
            }

            if (this.listDefensePointUnlocked == null)
            {
                this.listDefensePointUnlocked = new List<int>() {8001};
            }
        }

        public bool IsOutOfDFSPTrial()
        {
            return this.numberTrial <= 0 && !this.showTrialPopupOneTime;
        }

        public void SetHighestScore(int highestWave)
        {
            this.highestScoreOld = this.highestScore;

            if (highestWave > this.highestScore)
            {
                this.highestScore = highestWave;
            }
        }

        public int GetHighestScore()
        {
            return this.highestScore;
        }

        public bool IsNewRecord()
        {
            return this.highestScore > this.highestScoreOld;
        }

        private void HandleResetNewDay()
        {
            var dataConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>();
            if (dataConfig != null)
            {
                var data = dataConfig.GetDataConfig();
                this.numberTicket = data.ticketFree;
                this.timesWatchAds = data.numberWatchAds;
            }
        }

        public long GetTimeRemainSeason()
        {
            return endTimeSeason - TimeManager.NowInSeconds;
        }

        public long GetTimeRemainResetNewDay()
        {
            return timeResetNewDay - TimeManager.NowInSeconds;
        }

        private bool CheckCanResetNewDay()
        {
            return (timeResetNewDay - TimeManager.NowInSeconds) <= 0;
        }

        public void SetTimeResetNewDay()
        {
            timeResetNewDay = TimeManager.EndTimeOfDaySeconds;
        }

        public void CheckNewDay()
        {
            if (CheckCanResetNewDay())
            {
                SetTimeResetNewDay();
                HandleResetNewDay();
            }
        }

        public void CheckTimeSeason()
        {
            var dataConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>()
                .GetDataConfig();

            if (dataConfig != null && this.startTimeSeason == 0)
            {
                var startTimeSeconds = dataConfig.seasonStartTime;
                var numbSeason = (TimeManager.NowInSeconds - startTimeSeconds) / dataConfig.seasonDuration;
                Debug.LogWarning($"[Hero Defense] numbSeason: {numbSeason}");
                this.startTimeSeason = startTimeSeconds + numbSeason * dataConfig.seasonDuration;
                this.endTimeSeason = this.startTimeSeason + dataConfig.seasonDuration;
                Debug.LogWarning($"[Hero Defense] startTimeSeason: {startTimeSeason} | endTimeSeason: {endTimeSeason}");
                this.currMapId = CaculateMapId((int)numbSeason);
            }
            else if (GetTimeRemainSeason() <= 0)
            {
                HandleResetNewSeason();
            }
        }

        public void HandleResetNewSeason()
        {
            var dataConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>()
                .GetDataConfig();
            this.startTimeSeason = this.endTimeSeason;
            this.endTimeSeason = this.startTimeSeason + dataConfig.seasonDuration;
            this.numberTrial = dataConfig.numberTrial;
            var startTimeSeconds = dataConfig.seasonStartTime;
            var numbSeason = (TimeManager.NowInSeconds - startTimeSeconds) / dataConfig.seasonDuration;
            this.currMapId = CaculateMapId((int)numbSeason);
            HandleResetNewDay();
        }

        public bool CheckDefensePointUnlocked(int defensePointIdCheck)
        {
            return this.listDefensePointUnlocked.Contains(defensePointIdCheck);
        }

        public void UnlockDefensePoint(int defensePointIdUnlock)
        {
            if (!this.listDefensePointUnlocked.Contains(defensePointIdUnlock))
            {
                this.listDefensePointUnlocked.Add(defensePointIdUnlock);
            }
        }

        public List<int> GetListHeroes()
        {
            var listHeroIds = new List<int>();
            foreach (var heroSelected in this.listHeroSelected)
            {
                listHeroIds.Add(heroSelected.heroId);
            }

            return listHeroIds;
        }

        private int CaculateMapId(int season)
        {
            var dataConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>()
                .defendModeDataConfigs;

            var indexStart = 0;
            var totalMap = dataConfig.Length;

            return (((season - 1) + (indexStart - 1)) % totalMap);
        }

        public List<HeroItem> GetSelectedHeroItems()
        {
            List<HeroItem> heroItems = new List<HeroItem>();
            for (var i = 0; i < listHeroSelected.Count; i++)
            {
                UserData.Instance.UserHeroData.DictHeroes.TryGetValue(listHeroSelected[i].heroId,
                    out HeroItem heroItem);
                heroItems.Add(heroItem);
            }

            return heroItems;
        }
    }
}