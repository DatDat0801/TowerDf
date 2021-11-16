using TigerForge;

namespace EW2
{
    public class CountQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();

            if (targetType == TargetType.CheckinDaily)
            {
                EventManager.StartListening(GamePlayEvent.OnClaimCheckinDaily, OnClaimCheckinDaily);
            }
            else if (targetType == TargetType.SaveLoad)
            {
                EventManager.StartListening(GamePlayEvent.OnSaveData, OnSaveData);
            }
            else if (targetType == TargetType.EquipSpell || this.targetType == TargetType.EquipSpellForHero)
            {
                EventManager.StartListening(GamePlayEvent.OnEquipSpell, OnEquipSpell);
            }
            else if (targetType == TargetType.JoinFanpage)
            {
                EventManager.StartListening(GamePlayEvent.OnJointFanpage, OnJointFanpage);
            }
            else if (targetType == TargetType.WatchVideo)
            {
                EventManager.StartListening(GamePlayEvent.OnWatchVideo, OnWatchVideo);
            }
            else if (targetType == TargetType.IAP)
            {
                EventManager.StartListening(GamePlayEvent.OnIAP, OnIAP);
            }
        }

        private void OnIAP()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        private void OnWatchVideo()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        private void OnJointFanpage()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        private void OnEquipSpell()
        {
            var heroId = EventManager.GetInt(GamePlayEvent.OnEquipSpell);

            if (this.targetType == TargetType.EquipSpell)
            {
                if (IsCanIncrease())
                {
                    Count++;
                }
            }
            else if (this.targetType == TargetType.EquipSpellForHero)
            {
                if (heroId == this.victim[0] && IsCanIncrease())
                {
                    Count++;
                }
            }
        }

        private void OnSaveData()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        private void OnClaimCheckinDaily()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        public override void CleanEventQuest()
        {
            if (targetType == TargetType.CheckinDaily)
            {
                EventManager.StopListening(GamePlayEvent.OnClaimCheckinDaily, OnClaimCheckinDaily);
            }
            else if (targetType == TargetType.SaveLoad)
            {
                EventManager.StopListening(GamePlayEvent.OnSaveData, OnSaveData);
            }
            else if (targetType == TargetType.EquipSpell)
            {
                EventManager.StopListening(GamePlayEvent.OnEquipSpell, OnEquipSpell);
            }
            else if (targetType == TargetType.JoinFanpage)
            {
                EventManager.StopListening(GamePlayEvent.OnJointFanpage, OnJointFanpage);
            }
            else if (targetType == TargetType.WatchVideo)
            {
                EventManager.StopListening(GamePlayEvent.OnWatchVideo, OnWatchVideo);
            }
            else if (targetType == TargetType.IAP)
            {
                EventManager.StopListening(GamePlayEvent.OnIAP, OnIAP);
            }
        }

        public override void InsertData(QuestUserData userData)
        {
            base.InsertData(userData);
            if (targetType == TargetType.EquipSpell)
            {
                if (Count == 0)
                {
                    var listHero = UserData.Instance.UserHeroData.GetListHeroes();

                    foreach (var hero in listHero)
                    {
                        if (hero.spellId > 0)
                        {
                            Count++;
                            UserData.Instance.OtherUserData.listHeroEquipedSpell.Add(hero.heroId);
                        }
                    }
                }
            }
        }

        public override void GoToTarget()
        {
            base.GoToTarget();

            if (targetType == TargetType.CheckinDaily)
            {
                DirectionGoTo.GotoCheckinDaily();
            }
            else if (targetType == TargetType.SaveLoad || targetType == TargetType.JoinFanpage)
            {
                DirectionGoTo.GotoProfile();
            }
            else if (targetType == TargetType.EquipSpell || this.targetType == TargetType.EquipSpellForHero)
            {
                DirectionGoTo.GotoSpellHeroRoom();
            }
            else if (targetType == TargetType.WatchVideo)
            {
                DirectionGoTo.GotoWatchVideo();
            }
            else if (targetType == TargetType.IAP)
            {
                DirectionGoTo.GotoBuyNow();
            }
        }
    }
}