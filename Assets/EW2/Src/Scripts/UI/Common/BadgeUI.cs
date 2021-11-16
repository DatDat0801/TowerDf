using System;
using DG.Tweening;
using EW2.Spell;
using Invoke;
using Sirenix.OdinInspector;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public enum BadgeType
    {
        HeroRoom = 0,
        Shop = 1,
        Upgrade = 2,
        Mine = 3,
        HeroTrail = 4,
        Arena = 5,
        Campaign = 6,
        None,
        SaveLoad,
        Login,
        Gacha,
        TabSpellHeroRoom,
        Quest,
        DailyQuest,
        AchievementQuest,
        GachaSpell,
        GachaRune,
        HeroAcademy,
        NewHeroEvent
    }

    public class BadgeUI : MonoBehaviour
    {
        public static int numberTabShop;
        public static int firstOpenSpell;
        public static bool openNewSpell;

        public GameObject goIconNotice;
        public BadgeType type;
        public bool isFresh;
        [ShowIf("isFresh")] public float timeRefresh;


        void Awake()
        {
            EventManager.StartListening(GamePlayEvent.OnUpdateBadge, HandleUpdateBadge);

            numberTabShop = PlayerPrefs.GetInt("number_tab_shop", 0);
            firstOpenSpell = PlayerPrefs.GetInt("first_open_spell", 1);

            InvokeProxy.Iinvoke.InvokeRepeating(this, HandleUpdateBadge, timeRefresh, timeRefresh);
        }

        private void OnDestroy()
        {
            EventManager.StopListening(GamePlayEvent.OnUpdateBadge, HandleUpdateBadge);
        }

        private void OnEnable()
        {
            HandleUpdateBadge();
        }

        private void HandleUpdateBadge()
        {
            if (this.type == BadgeType.HeroRoom)
            {
                HeroRoomBadge();
            }
            else if (type == BadgeType.Shop)
            {
                ShopBadge();
            }
            else if (type == BadgeType.TabSpellHeroRoom)
            {
                TabSpellHeroRoom();
            }
            else if (type == BadgeType.Quest)
            {
                Quest();
            }
            else if (type == BadgeType.DailyQuest)
            {
                DailyQuest();
            }
            else if (type == BadgeType.Gacha)
            {
                Gacha();
            }
            else if (type == BadgeType.GachaRune)
            {
                GachaRune();
            }
            else if (type == BadgeType.GachaSpell)
            {
                GachaSpell();
            }
            else if (type == BadgeType.AchievementQuest)
            {
                AchievementQuest();
            }
            else if (type == BadgeType.HeroAcademy)
            {
                HeroAcademy();
            }else if (type == BadgeType.NewHeroEvent)
            {
                NewHeroEvent();
            }
            else
            {
                UpdateBadge(false);
            }
        }

        #region Handle badge

        private void HeroRoomBadge()
        {
            var heroCollect = GameContainer.Instance.GetHeroCollection();

            var skillUpgrade = GameContainer.Instance.GetHeroSkillUpgrade();

            for (int i = 0; i < heroCollect.heroList.Length; i++)
            {
                var heroId = heroCollect.heroList[i].heroId;


                if (!UserData.Instance.UserHeroData.CheckDisplayFirstNotify(heroId))
                {
                    UpdateBadge(true);
                    return;
                }

                if (!UserData.Instance.UserHeroData.CheckHeroUnlocked(heroId)) continue;

                var heroItem = UserData.Instance.UserHeroData.GetHeroById(heroId);

                var levelSkills = heroItem.levelSkills;

                for (int j = 0; j < levelSkills.Length; j++)
                {
                    if (levelSkills[j] >= GameConfig.HeroSkillLevelMax) continue;


                    var costUpgrade = skillUpgrade.skillUpgradeDatas[levelSkills[j]].cost;

                    if (heroItem.skillPoint >= costUpgrade)
                    {
                        UpdateBadge(true);

                        return;
                    }
                }
            }

            if (firstOpenSpell > 0 || openNewSpell || UserData.Instance.OtherUserData.listSpellCanUpgrades.Count > 0)
            {
                UpdateBadge(true);
                return;
            }

            UpdateBadge(false);
        }

        private void ShopBadge()
        {
            UpdateBadge(numberTabShop != GameConfig.NumberTabShop);
        }

        private void TabSpellHeroRoom()
        {
            var lockSpellClick = PlayerPrefs.GetInt("lock_spell_tab_click", 0);
            if ((firstOpenSpell > 0 && UnlockFeatureUtilities.IsSpellAvailable())|| openNewSpell || UserData.Instance.OtherUserData.listSpellCanUpgrades.Count > 0 ||
                lockSpellClick == 0)
            {
                UpdateBadge(true);
                return;
            }

            UpdateBadge(false);
        }

        private void Quest()
        {
            var dailyQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<DailyQuest>();
            var achievementQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<AchievementQuest>();

            if (dailyQuest.CheckCanReceive() || achievementQuest.CheckCanReceive())
            {
                UpdateBadge(true);
                return;
            }

            UpdateBadge(false);
        }

        private void DailyQuest()
        {
            var dailyQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<DailyQuest>();
            if (dailyQuest.CheckCanReceive())
            {
                UpdateBadge(true);
                return;
            }

            UpdateBadge(false);
        }

        private void Gacha()
        {
            if (UnlockFeatureUtilities.IsSpellAvailable() &&
                (UserData.Instance.OtherUserData.GetTimeRemainNormalSpell() <= 0 ||
                 UserData.Instance.OtherUserData.GetTimeRemainPremiumSpell() <= 0))
            {
                UpdateBadge(true);
                return;
            }

            if (UserData.Instance.OtherUserData.GetTimeRemainNormalRune() <= 0 ||
                UserData.Instance.OtherUserData.GetTimeRemainPremiumRune() <= 0)
            {
                UpdateBadge(true);
                return;
            }

            UpdateBadge(false);
        }

        private void GachaRune()
        {
            if (UserData.Instance.OtherUserData.GetTimeRemainNormalRune() <= 0 ||
                UserData.Instance.OtherUserData.GetTimeRemainPremiumRune() <= 0)
            {
                UpdateBadge(true);
                return;
            }

            UpdateBadge(false);
        }

        private void GachaSpell()
        {
            if (UnlockFeatureUtilities.IsSpellAvailable() &&
                (UserData.Instance.OtherUserData.GetTimeRemainNormalSpell() <= 0 ||
                 UserData.Instance.OtherUserData.GetTimeRemainPremiumSpell() <= 0))
            {
                UpdateBadge(true);
                return;
            }

            UpdateBadge(false);
        }

        private void AchievementQuest()
        {
            var dailyQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<AchievementQuest>();
            if (dailyQuest.CheckCanReceive())
            {
                UpdateBadge(true);
                return;
            }

            UpdateBadge(false);
        }

        private void HeroAcademy()
        {
            if (!UserData.Instance.UserEventData.HeroAcademyUserData.CheckCanShow())
                return;

            var isOn = UserData.Instance.UserEventData.GloryRoadUser.CanClaimAnyReward() ||
                       GameContainer.Instance.Get<QuestManager>().GetQuest<HeroChallengeQuestEvent>()
                           .CheckCanReceive() ||
                       GameContainer.Instance.Get<QuestManager>().GetQuest<HeroAcademyCheckinEvent>().CheckCanReceive();

            if (isOn)
            {
                UpdateBadge(true);
                return;
            }

            UpdateBadge(false);
        }

        private void NewHeroEvent()
        {
            if (!UserData.Instance.UserEventData.NewHeroEventUserData.CheckCanShow())
                return;

            var isOn = GameContainer.Instance.Get<QuestManager>().GetQuest<NewHeroEventQuest>().CheckCanReceive();

            if (isOn)
            {
                UpdateBadge(true);
                return;
            }

            UpdateBadge(false);
        }
        
        #endregion

        public void UpdateBadge(bool isShow)
        {
            if (goIconNotice)
                goIconNotice.SetActive(isShow);
        }
    }
}