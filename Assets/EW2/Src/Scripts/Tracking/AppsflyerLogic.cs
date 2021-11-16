using EW2;
using Zitga.ContextSystem;

namespace Zitga.TrackingFirebase
{
    public class AppsflyerLogic : Singleton<AppsflyerLogic>
    {
        private AppsflyerUtils appsflyer;

        public void Init(bool isDebug)
        {
            appsflyer = AppsflyerUtils.Instance;
            appsflyer?.Init(isDebug);
        }

        public void SetFirstOpen()
        {
#if TRACKING_APPSFLYER
            appsflyer.TrackingEvent(AppsflyerKey.AfFirstOpen);
#endif
        }

        public void PurchaseEvent(string productId, string revenue)
        {
#if TRACKING_APPSFLYER
            appsflyer.PurChase(productId, revenue);
#endif
        }

        public void CompleteTutorial()
        {
#if TRACKING_APPSFLYER
            appsflyer.CompleteTutorialEvent();
#endif
        }

        public void CompleteStage(int stageId)
        {
#if TRACKING_APPSFLYER
            var stageConvert = MapCampaignInfo.GetWorldMapModeId(stageId);
            if (stageConvert.Item2 > 14) return;
            appsflyer.CompleteStageEvent(stageConvert.Item2 + 1);
#endif
        }
    }
}