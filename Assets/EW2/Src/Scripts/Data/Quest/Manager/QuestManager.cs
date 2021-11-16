using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using Zitga.ContextSystem;

namespace EW2
{
    public class QuestManager
    {
        private ServiceContainer container;
        private Dictionary<string, IEventQuest> dictEventQuest = new Dictionary<string, IEventQuest>();

        public QuestManager()
        {
            container = new ServiceContainer();
            EventManager.StartListening(GamePlayEvent.OnLoadDataSuccess, InitQuest);
        }

        public void InitQuest()
        {
            LoadDailyQuest();
            LoadAchievementQuest();
        }

        public T Get<T>() where T : ScriptableObject
        {
            var obj = container.Resolve<T>();
            if (obj == null)
            {
                obj = LoadFromResource<T>();

                container.Register(obj);
            }

            return obj;
        }

        public T GetQuest<T>() where T : IEventQuest
        {
            if (dictEventQuest.ContainsKey(typeof(T).Name))
                return (T)dictEventQuest[typeof(T).Name];

            return default;
        }

        private T LoadFromResource<T>() where T : ScriptableObject
        {
            return Resources.Load<T>($"CSV/Quest/{typeof(T).Name}");
        }

        public QuestItem CreateQuest(QuestType questType)
        {
            QuestItem quest = null;

            switch (questType)
            {
                case QuestType.Play:
                    quest = new PlayQuest();
                    break;
                case QuestType.Use:
                    quest = new UseQuest();
                    break;
                case QuestType.Complete:
                    quest = new CompleteQuest();
                    break;
                case QuestType.Count:
                    quest = new CountQuest();
                    break;
                case QuestType.Summon:
                    quest = new SummonQuest();
                    break;
                case QuestType.Upgrade:
                    quest = new UpgradeQuest();
                    break;
                case QuestType.Online:
                    quest = new OnlineQuest();
                    break;
                case QuestType.Kill:
                    quest = new KillQuest();
                    break;
                case QuestType.KillBy:
                    quest = new KillByQuest();
                    break;
                case QuestType.Earn:
                    quest = new EarnQuest();
                    break;
                case QuestType.Spent:
                    quest = new SpendQuest();
                    break;
                case QuestType.Gacha:
                    quest = new GachaQuest();
                    break;
                default:
                    Log.Error("[QuestType] questtype not found: " + questType);
                    break;
            }

            return quest;
        }

        public void LoadDailyQuest()
        {
            if (!dictEventQuest.ContainsKey(nameof(DailyQuest)))
            {
                var dailyQuest = new DailyQuest();
                dailyQuest.ReadQuests();
                dailyQuest.UpdateDataQuests(UserData.Instance.UserDailyQuest.listQuestData);
                dictEventQuest[nameof(DailyQuest)] = dailyQuest;
            }

            else
            {
                var dailyQuest = dictEventQuest[nameof(DailyQuest)];
                dailyQuest.UpdateDataQuests(UserData.Instance.UserDailyQuest.listQuestData);
            }
        }

        public void LoadAchievementQuest()
        {
            if (!dictEventQuest.ContainsKey(nameof(AchievementQuest)))
            {
                var achievementQuest = new AchievementQuest();
                achievementQuest.ReadQuests();
                achievementQuest.UpdateDataQuests(UserData.Instance.UserAchievementQuest.listQuestData);
                dictEventQuest[nameof(AchievementQuest)] = achievementQuest;
            }

            else
            {
                var achievementQuest = dictEventQuest[nameof(AchievementQuest)];
                achievementQuest.UpdateDataQuests(UserData.Instance.UserAchievementQuest.listQuestData);
            }
        }

        public void LoadHeroChallengeQuest()
        {
            if (!dictEventQuest.ContainsKey(nameof(HeroChallengeQuestEvent)))
            {
                var heroChallengeQuest = new HeroChallengeQuestEvent();
                heroChallengeQuest.ReadQuests();
                var listQuest = heroChallengeQuest.GetAllQuests();
                if (UserData.Instance.UserEventData.HeroChallengeUserData.dictQuestData.Count != listQuest.Count)
                {
                    UserData.Instance.UserEventData.HeroChallengeUserData.InsertData(listQuest);
                    UserData.Instance.Save();
                }

                heroChallengeQuest.UpdateDataQuests(UserData.Instance.UserEventData.HeroChallengeUserData.GetListQuestUserData());
                dictEventQuest[nameof(HeroChallengeQuestEvent)] = heroChallengeQuest;
            }

            else
            {
                var heroChallengeQuest = dictEventQuest[nameof(HeroChallengeQuestEvent)];
                heroChallengeQuest.UpdateDataQuests(UserData.Instance.UserEventData.HeroChallengeUserData.GetListQuestUserData());
            }
        }

        public void LoadHeroAcademyCheckIn()
        {
            if (!dictEventQuest.ContainsKey(nameof(HeroAcademyCheckinEvent)))
            {
                var heroAcademyCheckIn = new HeroAcademyCheckinEvent();
                heroAcademyCheckIn.ReadQuests();

                if (UserData.Instance.UserEventData.HeroAcademyUserData.listQuestData.Count <= 0)
                {
                    UserData.Instance.UserEventData.HeroAcademyUserData.InsertData(heroAcademyCheckIn.GetAllQuests());
                    UserData.Instance.Save();
                }

                heroAcademyCheckIn.UpdateDataQuests(UserData.Instance.UserEventData.HeroAcademyUserData.listQuestData);
                dictEventQuest[nameof(HeroAcademyCheckinEvent)] = heroAcademyCheckIn;
            }

            else
            {
                var heroAcademy = dictEventQuest[nameof(HeroAcademyCheckinEvent)];
                heroAcademy.UpdateDataQuests(UserData.Instance.UserEventData.HeroAcademyUserData.listQuestData);
            }
        }

        public void LoadNewHeroEventQuest()
        {
            if (!dictEventQuest.ContainsKey(nameof(NewHeroEventQuest)))
            {
                var newHeroEventQuest = new NewHeroEventQuest();
                newHeroEventQuest.ReadQuests();

                var listQuest = newHeroEventQuest.GetAllQuests();
                if (UserData.Instance.UserEventData.NewHeroEventUserData.dictQuestData.Count != listQuest.Count)
                {
                    UserData.Instance.UserEventData.NewHeroEventUserData.InsertData(listQuest);
                    UserData.Instance.Save();
                }

                newHeroEventQuest.UpdateDataQuests(UserData.Instance.UserEventData.NewHeroEventUserData
                    .GetListQuestUserData());
                dictEventQuest[nameof(NewHeroEventQuest)] = newHeroEventQuest;
            }

            else
            {
                var newHeroEventQuest = dictEventQuest[nameof(NewHeroEventQuest)];
                newHeroEventQuest.UpdateDataQuests(UserData.Instance.UserEventData.NewHeroEventUserData
                    .GetListQuestUserData());
            }
        }
    }
}