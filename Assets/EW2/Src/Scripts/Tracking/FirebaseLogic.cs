using System;
using System.Collections;
using System.Collections.Generic;
using EW2;
using EW2.CampaignInfo.HeroSelect;
using Firebase.Analytics;
using SocialTD2;
using UnityEngine;


namespace Zitga.TrackingFirebase
{
    public class FirebaseLogic : Singleton<FirebaseLogic>
    {
        private FirebaseUtils firebase;

        public void Init(bool isDebug)
        {
            firebase = new FirebaseUtils(isDebug);
        }

        public void SetPlayerId()
        {
            var id = LoadSaveUtilities.GetUserID();
            this.firebase.SetUserProperty(FirebaseProperty.PlayerId, string.IsNullOrEmpty(id) ? null : id);
            this.firebase.SetUserId(string.IsNullOrEmpty(id) ? null : id);
        }

        public void SetIapCount(string numberPackages)
        {
            firebase.SetUserProperty(FirebaseProperty.IapCount, numberPackages);
        }

        public void SetRevenue(string totalRevenue)
        {
            firebase.SetUserProperty(FirebaseProperty.IapRevenue, totalRevenue);
        }

        public void SetRemainingCrystal(string crystal)
        {
            firebase.SetUserProperty(FirebaseProperty.RemainingCrystal, crystal);
        }

        public void SetRemainingGem(string gem)
        {
            firebase.SetUserProperty(FirebaseProperty.RemainingGem, gem);
        }

        public void TrackingOnlineTime()
        {
            StartCoroutine(ITrackingOnlineTime());
        }

        private IEnumerator ITrackingOnlineTime()
        {
            int delayTime = 10;

            yield return new WaitForSeconds(delayTime);

            UserData.Instance.SettingData.countOnlineTime += delayTime;

            UserData.Instance.Save();

            firebase.SetUserProperty(FirebaseProperty.OnlineTime,
                UserData.Instance.SettingData.countOnlineTime.ToString());
        }

        public void SetMaxCampaign(int campaignId)
        {
            var current = UserData.Instance.SettingData.maxCampaignId;

            if (campaignId > current)
            {
                UserData.Instance.SettingData.maxCampaignId = campaignId;

                UserData.Instance.Save();

                firebase.SetUserProperty(FirebaseProperty.MaxNormalStage, campaignId.ToString());
            }
        }

        public void SetCurrency(int moneyType, long value)
        {
            try
            {
                var key = string.Empty;

                switch (moneyType)
                {
                    case MoneyType.Stamina:
                        key = FirebaseProperty.RemainingStamina;
                        break;
                    case MoneyType.SliverStar:
                        key = FirebaseProperty.RemainingStar1;
                        break;
                    case MoneyType.GoldStar:
                        key = FirebaseProperty.RemainingStar2;
                        break;
                }

                if (string.IsNullOrEmpty(key))
                    return;

                firebase.SetUserProperty(key, value.ToString());
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void SetStageStart(int campaignId, List<int> heroList)
        {
            Parameter[] parameters = new Parameter[heroList.Count * 2 + 1];

            parameters[heroList.Count * 2] = new Parameter("stage", campaignId);

            for (int i = 0; i < heroList.Count; i++)
            {
                int heroId = heroList[i];
                var heroData = UserData.Instance.UserHeroData.GetHeroById(heroId);

                parameters[i * 2] = new Parameter($"hero_{i + 1}", heroId);
                parameters[i * 2 + 1] = new Parameter($"level_hero_{i + 1}", heroData != null ? heroData.level : 0);
            }

            firebase.SetUserEvent(FirebaseEvent.StageStart, parameters);
        }

        public void SetStageEnd(int campaignId, bool isWin, int lastWave, long lifePoint, int star, long remainingGold,
            int envInteract, int numberSpellUse)
        {
            var campaignParam = new Parameter("stage", campaignId);
            var winParam = new Parameter("win", isWin ? 1 : 0);
            var lastWaveParam = new Parameter("last_wave", lastWave);
            var lifePointParam = new Parameter("life_point", lifePoint);
            var starParam = new Parameter("star", star);
            var remainingGoldParam = new Parameter("remaining_gold", remainingGold);
            var envInteractParam = new Parameter("env_interact", envInteract);
            var spellNumberParam = new Parameter("spell_number", numberSpellUse);

            firebase.SetUserEvent(FirebaseEvent.StageEnd,
                campaignParam, winParam, lastWaveParam,
                lifePointParam, spellNumberParam, starParam, remainingGoldParam, envInteractParam);
        }

        public void SetStageTower(int campaignId, bool isWin, int towerId, int towerLevel, int levelSkill1,
            int levelSkill2)
        {
            var campaignParam = new Parameter("stage", campaignId);
            var winParam = new Parameter("win", isWin ? 1 : 0);
            var towerIdParam = new Parameter("tower_id", towerId);
            var towerLevelParam = new Parameter("tower_level", towerLevel);
            var skillLevel1Param = new Parameter("skill_1", levelSkill1);
            var skillLevel2Param = new Parameter("skill_2", levelSkill2);

            firebase.SetUserEvent(FirebaseEvent.StageTower,
                campaignParam, winParam, towerIdParam,
                towerLevelParam, skillLevel1Param, skillLevel2Param);
        }

        public void SetStageHero(int campaignId, bool isWin, int heroId, int useSkill, int revive, int move)
        {
            var campaignParam = new Parameter("stage", campaignId);
            var winParam = new Parameter("win", isWin ? 1 : 0);
            var heroIdParam = new Parameter("hero_id", heroId);
            var skillsParam = new Parameter("skills", useSkill);
            var reviveParam = new Parameter("revive", revive);
            var moveParam = new Parameter("move", move);

            firebase.SetUserEvent(FirebaseEvent.StageHero,
                campaignParam, winParam, heroIdParam,
                skillsParam, reviveParam, moveParam);
        }

        public void SetCallWave(int campaignId, int wave)
        {
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter("stage", campaignId));
            fParamters.Add(new Parameter("wave", wave));
            firebase.SetUserEvent(FirebaseEvent.EarlyWave, fParamters.ToArray());
        }

        public void LogResource(string itemCategory, string itemBranch, string itemId, string itemName, float value,
            float valueRemaining, string source, string sourceId, bool isPurchase)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    var quantityPush = Mathf.Abs(value);
                    if (string.IsNullOrEmpty(source))
                    {
                        Log.Warning("[Firebase] Source null");
                        return;
                    }

                    if (isPurchase)
                    {
                        if (firebase != null)
                            firebase.BuyResource(itemCategory, itemBranch, itemId, itemName, quantityPush,
                                valueRemaining,
                                source, sourceId);
                    }
                    else
                    {
                        if (firebase != null)
                        {
                            if (value > 0)
                                firebase.EarnResource(itemCategory, itemBranch, itemId, itemName, quantityPush,
                                    valueRemaining,
                                    source, sourceId);
                            else if (value < 0)
                                firebase.SpendResource(itemCategory, itemBranch, itemId, itemName, quantityPush,
                                    valueRemaining,
                                    source, sourceId);
                        }
                    }
                }
            }
        }

        public void LogSpellLevelUp(string resourceUse, string spellId, string spellLevel)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    firebase.UpgradeSpell(resourceUse, spellId, spellLevel);
                }
            }
        }

        public void LogHeroStatChange(int heroId)
        {
            var heroData = UserData.Instance.UserHeroData.GetHeroById(heroId);
            if (firebase != null && heroData != null)
            {
                if (firebase.Initialized)
                {
                    List<string> levelSkill = new List<string>();
                    for (int i = 0; i < heroData.levelSkills.Length; i++)
                    {
                        levelSkill.Add(heroData.levelSkills[i].ToString());
                    }

                    //runes
                    List<string> runeEquips = new List<string>();
                    for (var i = 0; i < heroData.runeEquips.Length; i++)
                    {
                        var runeUserData = UserData.Instance.GetInventory(InventoryType.Rune, heroData.runeEquips[i]);
                        if (runeUserData != null)
                        {
                            runeEquips.Add(((RuneItem)runeUserData).RuneIdConvert.ToString());
                        }
                    }

                    if (heroData.spellId > 0)
                    {
                        var spellData = UserData.Instance.GetInventory(InventoryType.Spell, heroData.spellId);
                        firebase.HeroStatsChange($"{heroId}", $"{heroData.level}", "0", levelSkill,
                            heroData.spellId.ToString(), spellData.Level.ToString(),
                            runeEquips);
                    }
                    else
                    {
                        firebase.HeroStatsChange($"{heroId}", $"{heroData.level}", "0", levelSkill,
                            heroData.spellId.ToString(), "0", runeEquips);
                    }
                }
            }
        }

        public void WaveEnd(int stageId, int waveNumber, int lifePoint, int spellNumber)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    firebase.WaveEnd(stageId, waveNumber, lifePoint, spellNumber);
                }
            }
        }

        public void StageSpell(int stageId, int win, int spell, int spell_level, int number, int hero_id)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    firebase.StageSpell(stageId, win, spell, spell_level, number, hero_id);
                }
            }
        }

        public void ButtonClick(string category, string buttonName, int id)
        {
            if (firebase.Initialized)
            {
                var categoryPara = new Parameter("category", category);
                var buttonNamePara = new Parameter("name", buttonName);
                var idPara = new Parameter("id", id);
                firebase.SetUserEvent(FirebaseEvent.ButtonClick, categoryPara, buttonNamePara, idPara);
            }
        }

        public void StageTowerSell(int stage, int wave, int towerId, int towerLevel,
            int skill1Level, int skill2Level)
        {
            if (firebase != null)
                if (firebase.Initialized)
                {
                    var stageParameter = new Parameter("stage", stage);
                    var waveParameter = new Parameter("wave", wave);
                    var towerIdParameter = new Parameter("tower_id", towerId);
                    //var towerNameParameter = new Parameter("tower_name", towerName);
                    var towerLevelParameter = new Parameter("tower_level", towerLevel);
                    var skill1LevelParameter = new Parameter("skill_1_level", skill1Level);
                    var skill2Parameter = new Parameter("skill_2_level", skill2Level);
                    firebase.SetUserEvent(FirebaseEvent.StageTowerSell, stageParameter, waveParameter, towerIdParameter,
                        towerLevelParameter, skill1LevelParameter, skill2Parameter);
                }
        }

        public void PassTutorial(int stepId, string stepName)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    var stepIdParameter = new Parameter("step_id", stepId);
                    var stepNameParameter = new Parameter("step_name", stepName);
                    firebase.SetUserEvent(FirebaseEvent.Tutorial, stepIdParameter, stepNameParameter);
                }
            }
        }

        public void TapToStart()
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    firebase.SetUserEvent(FirebaseEvent.TapToStart);
                }
            }
        }

        public void PurchaseClick(string productId)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    firebase.SetUserEvent(FirebaseEvent.PurchaseClick, new Parameter("product_id", productId));
                }
            }
        }


        #region Ads Tracking

        public void AdClick(string source, string sourceId)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    firebase.SetUserEvent(FirebaseEvent.AdClick, new Parameter("source", source),
                        new Parameter("source_id", sourceId));
                }
            }
        }

        public void AdReward(string source, string sourceId)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    firebase.SetUserEvent(FirebaseEvent.AdReward, new Parameter("source", source),
                        new Parameter("source_id", sourceId));
                }
            }
        }

        #endregion

        #region Remote config

        public int GetIapDataId()
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    return firebase.GetIAPConfig();
                }
            }

            return 0;
        }

        public int GetCampaignLevelDesignDataId()
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    return firebase.GetCampaignLevelDesignId();
                }
            }

            return 0;
        }

        #endregion

        #region Hero Defense

        public void DefenseModeStart(long battleId, int dfpId, List<HeroSelectedData> listHero)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    var listHeroIds = new List<int>();
                    for (int i = 0; i < 5; i++)
                    {
                        if (i < listHero.Count)
                        {
                            listHeroIds.Add(listHero[i].heroId);
                        }
                        else
                        {
                            listHeroIds.Add(0);
                        }
                    }

                    firebase.HeroDefenseStart(battleId, dfpId, listHeroIds);
                }
            }
        }

        public void DefenseModeEnd(long battleId, int waveCount, long remainingGolds)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    firebase.HeroDefenseEnd(battleId, waveCount, remainingGolds);
                }
            }
        }

        public void DefenseModeBuffEarn(long battleId, int waveCount, string buffId, int buffNumber)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    firebase.HeroDefenseBuffErn(battleId, waveCount, buffId, buffNumber);
                }
            }
        }

        #endregion

        #region Tournament

        public void StartATournamentGame(List<HeroSelectedData> listHero, int bandSpellId, int nerfHeroId, int bandTowerId,
            int statBuffId, int nerfStatId)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    var listHeroIds = new List<int>();
                    for (int i = 0; i < 3; i++)
                    {
                        if (i < listHero.Count)
                        {
                            listHeroIds.Add(listHero[i].heroId);
                        }
                        else
                        {
                            listHeroIds.Add(0);
                        }
                    }

                    firebase.PlayATournamentGame(listHeroIds, bandSpellId, nerfHeroId, bandTowerId, statBuffId, nerfStatId);
                }
            }
        }

        public void EndATournamentGame(int scored, int waveSurvived, int tournamentMapId, int previousRank,
            int remainingGold, float playDuration, Dictionary<int, int> builtTowers)
        {
            if (firebase != null)
            {
                if (firebase.Initialized)
                {
                    firebase.ResultATournamentGame(scored, waveSurvived, tournamentMapId, previousRank, remainingGold,
                        playDuration, builtTowers);
                }
            }
        }

        #endregion
    }
}