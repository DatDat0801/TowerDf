using TigerForge;

namespace EW2
{
    public class PlayQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();
            if (targetType == TargetType.MapCampaignNormal || targetType == TargetType.MapCampaignNightmare)
            {
                EventManager.StartListening(GamePlayEvent.OnPlayCampaign, OnPlayCampaign);
            }
        }

        private void OnPlayCampaign()
        {
            var campaignId = EventManager.GetInt(GamePlayEvent.OnPlayCampaign);
            var campaignIdConvert = MapCampaignInfo.GetWorldMapModeId(campaignId);
            if (campaignIdConvert.Item3 == 0 && targetType == TargetType.MapCampaignNormal)
            {
                if (IsCanIncrease())
                {
                    Count++;
                }
            }
            else if (campaignIdConvert.Item3 == 1 && targetType == TargetType.MapCampaignNightmare)
            {
                if (IsCanIncrease())
                {
                    Count++;
                }
            }
        }

        public override void GoToTarget()
        {
            base.GoToTarget();
            
            if (targetType == TargetType.MapCampaignNormal)
            {
                DirectionGoTo.GotoNormalCampaign();
            }
            else if (targetType == TargetType.MapCampaignNightmare)
            {
                if (UserData.Instance.CampaignData.IsShowUnlockNightmareNotice(0))
                {
                    DirectionGoTo.GotoNighmareCampaign();
                }
                else
                {
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, string.Format(L.popup.spell_unlock_condition, GameConfig.MAX_STAGE_ID_WORLD_1+1) );
                }

            }
        }
    }
}