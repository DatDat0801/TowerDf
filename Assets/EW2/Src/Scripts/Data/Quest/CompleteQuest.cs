using System;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class CompleteQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();
            if (targetType == TargetType.AllDailyQuest)
            {
                EventManager.StartListening(GamePlayEvent.OnCompleteAllDailyQuest, OnCompleteAllDailyQuest);
            }
            else if (targetType == TargetType.DailyQuest)
            {
                EventManager.StartListening(GamePlayEvent.OnCompleteDailyQuest, OnCompleteDailyQuest);
            }
            else if (targetType == TargetType.AnyMapNormal || targetType == TargetType.AnyMapNightmare ||
                     targetType == TargetType.Complete3StarNormal || targetType == TargetType.Complete3StarNightmare ||
                     this.targetType == TargetType.Complete3StarNormalWithHero ||
                     this.targetType == TargetType.Complete3StarNightmareWithHero)
            {
                EventManager.StartListening(GamePlayEvent.OnMapComplete, OnCompleteMap);
            }
        }

        private void OnCompleteMap()
        {
            var star = EventManager.GetInt(GamePlayEvent.OnMapComplete);

            if (GamePlayData.Instance.CurrentMapCampaign.modeId == (int)ModeCampaign.Normal)
            {
                if (targetType == TargetType.AnyMapNormal)
                {
                    if (IsCanIncrease())
                    {
                        Count++;
                    }
                }
                else if (targetType == TargetType.Complete3StarNormal)
                {
                    if (this.victim.Length > 0 && this.victim[0] >= 0)
                    {
                        if (GamePlayData.Instance.CurrentMapCampaign.stageId != this.victim[0]) return;
                    }

                    if (IsCanIncrease() && star >= 3)
                    {
                        Count++;
                    }
                }
                else if (this.targetType == TargetType.Complete3StarNormalWithHero)
                {
                    if (IsCanIncrease() && star >= 3 && CheckPassConditionCompleteMapWithHero())
                    {
                        Count++;
                    }
                }
            }
            else if (GamePlayData.Instance.CurrentMapCampaign.modeId == (int)ModeCampaign.Nightmare)
            {
                if (targetType == TargetType.AnyMapNightmare)
                {
                    if (IsCanIncrease())
                    {
                        Count++;
                    }
                }
                else if (targetType == TargetType.Complete3StarNightmare)
                {
                    if (this.victim.Length > 0 && this.victim[0] >= 0)
                    {
                        if (GamePlayData.Instance.CurrentMapCampaign.stageId != this.victim[0]) return;
                    }

                    if (IsCanIncrease() && star >= 3)
                    {
                        Count++;
                    }
                }
                else if (this.targetType == TargetType.Complete3StarNightmareWithHero)
                {
                    if (IsCanIncrease() && star >= 3 && CheckPassConditionCompleteMapWithHero())
                    {
                        Count++;
                    }
                }
            }
        }

        private bool CheckPassConditionCompleteMapWithHero()
        {
            if (GamePlayData.Instance.CurrentMapCampaign.stageId == this.victim[0])
            {
                foreach (var heroSelected in UserData.Instance.UserHeroData.SelectedHeroes)
                {
                    if (heroSelected.heroId == this.victim[1])
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        private void OnCompleteDailyQuest()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        private void OnCompleteAllDailyQuest()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        public override void CleanEventQuest()
        {
            if (targetType == TargetType.AllDailyQuest)
            {
                EventManager.StopListening(GamePlayEvent.OnCompleteAllDailyQuest, OnCompleteAllDailyQuest);
            }
            else if (targetType == TargetType.DailyQuest)
            {
                EventManager.StopListening(GamePlayEvent.OnCompleteDailyQuest, OnCompleteDailyQuest);
            }
            else if (targetType == TargetType.AnyMapNormal || targetType == TargetType.AnyMapNightmare ||
                     targetType == TargetType.Complete3StarNormal || targetType == TargetType.Complete3StarNightmare ||
                     this.targetType == TargetType.Complete3StarNormalWithHero ||
                     this.targetType == TargetType.Complete3StarNightmareWithHero)
            {
                EventManager.StopListening(GamePlayEvent.OnMapComplete, OnCompleteMap);
            }
        }

        public override void GoToTarget()
        {
            base.GoToTarget();

            if (targetType == TargetType.AllDailyQuest || targetType == TargetType.DailyQuest)
            {
                DirectionGoTo.GotoDailyQuest();
            }
            else if (targetType == TargetType.AnyMapNormal || targetType == TargetType.Complete3StarNormal ||
                     this.targetType == TargetType.Complete3StarNormalWithHero)
            {
                DirectionGoTo.GotoNormalCampaign(this.victim.Length > 0 ? this.victim[0] : -1);
            }
            else if (targetType == TargetType.AnyMapNightmare || targetType == TargetType.Complete3StarNightmare ||
                     this.targetType == TargetType.Complete3StarNightmareWithHero)
            {
                DirectionGoTo.GotoNighmareCampaign(this.victim.Length > 0 ? this.victim[0] : -1);
            }
        }
    }
}