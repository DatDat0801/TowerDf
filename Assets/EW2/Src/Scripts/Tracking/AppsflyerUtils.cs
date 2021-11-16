using System.Collections.Generic;
#if TRACKING_APPSFLYER
using AppsFlyerSDK;
using EW2;
using UnityEngine;

#endif

#if UNITY_IOS
using UnityEngine;
#endif

namespace Zitga.TrackingFirebase
{
    public class AppsflyerKey
    {
        public const string AfFirstOpen = "af_first_open";
        public const string AfAdComplete = "af_ad_complete";
        public const string AfPurchase = "af_purchase";
        public const string AfCompleteTut = "af_complete_tut";
        public const string AfCompleteStage = "af_complete_stage_{0}";
    }

    public class AppsflyerUtils : Singleton<AppsflyerUtils>
    {
#if UNITY_IOS
        public bool tokenSent = false;
#endif

        public void Init(bool isDebugMode)
        {
#if TRACKING_APPSFLYER
            AppsFlyer.setIsDebug(isDebugMode);
            AppsFlyer.anonymizeUser(isDebugMode);
#if UNITY_ANDROID
            AppsFlyer.initSDK("Ge7ssp4Hs6ZKXqQ2UU6yeD", "", this);
#elif UNITY_IOS
        AppsFlyer.initSDK("Ge7ssp4Hs6ZKXqQ2UU6yeD", "1571236234");
#endif
            AppsFlyer.startSDK();

#if UNITY_ANDROID && TRACKING_FIREBASE
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
#endif

#if UNITY_IOS
        UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
#endif
#endif
        }

        public void TrackingEvent(string eventName, Dictionary<string, string> eventValues = null)
        {
#if TRACKING_APPSFLYER
            if (eventValues == null)
            {
                eventValues = new Dictionary<string, string>();
            }

            AppsFlyer.sendEvent(eventName, eventValues);
#endif
        }

#if UNITY_IOS && TRACKING_APPSFLYER
        public void Update()
        {

        if (!tokenSent)
        { // tokenSent needs to be defined somewhere (bool tokenSent = false)
            byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;
            if (token != null)
            {
                AppsFlyeriOS.registerUninstall(token);
                tokenSent = true;
            }
        }
        }
#endif

#if UNITY_ANDROID && TRACKING_APPSFLYER && TRACKING_FIREBASE
        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            AppsFlyerAndroid.updateServerUninstallToken(token.Token);
        }
#endif

        public string GetAppsflyerId()
        {
#if TRACKING_APPSFLYER
            return AppsFlyer.getAppsFlyerId();
#else
        return "";
#endif
        }

        #region ConversionData

#if TRACKING_APPSFLYER
        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            // add deferred deeplink logic here
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("onConversionDataFail", error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            Dictionary<string, object> attributionDataDictionary =
                AppsFlyer.CallbackStringToDictionary(attributionData);
            // add direct deeplink logic here
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }
#endif

        #endregion

        #region Event

        public void SetFirstOpen()
        {
#if TRACKING_APPSFLYER
            TrackingEvent(AppsflyerKey.AfFirstOpen);
#endif
        }

        public void PurChase(string productId, string revenue)
        {
#if TRACKING_APPSFLYER
            Dictionary<string, string> PurchaseEvent = new Dictionary<string, string>();
            PurchaseEvent.Add("player_id", UserData.Instance.AccountData.userId);
            PurchaseEvent.Add("product_id", productId);
            PurchaseEvent.Add("revenue", revenue);
            AppsFlyer.sendEvent(AppsflyerKey.AfPurchase, PurchaseEvent);
            Debug.Log($"[Appsflyer] Event Purchase: {productId} | {PurchaseEvent["revenue"]}");
#endif
        }

        public void CompleteTutorialEvent()
        {
#if TRACKING_APPSFLYER
            Dictionary<string, string> PurchaseEvent = new Dictionary<string, string>();
            PurchaseEvent.Add("player_id", UserData.Instance.AccountData.userId);
            AppsFlyer.sendEvent(AppsflyerKey.AfCompleteTut, PurchaseEvent);
#endif
        }

        public void CompleteStageEvent(int stageId)
        {
#if TRACKING_APPSFLYER
            var stageConvert = MapCampaignInfo.GetWorldMapModeId(stageId);
            if (stageConvert.Item2 > 14) return;
            Dictionary<string, string> PurchaseEvent = new Dictionary<string, string>();
            PurchaseEvent.Add("player_id", UserData.Instance.AccountData.userId);
            AppsFlyer.sendEvent(string.Format(AppsflyerKey.AfCompleteStage, stageConvert.Item2 + 1), PurchaseEvent);
#endif
        }

        public void ValidateAndSendInAppPurchaseAndroid(string signature, string purchaseData, string price,
            string currency, Dictionary<string, string> additionalParameters, MonoBehaviour go)
        {
#if TRACKING_APPSFLYER
            AppsFlyer.ValidateAndSendInAppPurchaseAndroid(signature, purchaseData, price, currency,
                additionalParameters, go);
            Debug.Log($"[Appsflyer] Validate And Send In-App Purchase Android : {price}");
#endif
        }

        public void ValidateAndSendInAppPurchaseIos(string productIdentifier, string price, string currency,
            string tranactionId, Dictionary<string, string> additionalParameters, MonoBehaviour go)
        {
#if TRACKING_APPSFLYER
            AppsFlyer.ValidateAndSendInAppPurchaseIos(productIdentifier, price, currency, tranactionId,
                additionalParameters, go);
            Debug.Log($"[Appsflyer] Validate And Send In-App Purchase Ios : {price}");
#endif
        }

        public void CompleteAdsEvent(string location)
        {
#if TRACKING_APPSFLYER
            Dictionary<string, string> PurchaseEvent = new Dictionary<string, string>();
            PurchaseEvent.Add("player_id", UserData.Instance.AccountData.userId);
            PurchaseEvent.Add("ad_type", "reward");
            PurchaseEvent.Add("ad_location", location);
            AppsFlyer.sendEvent(AppsflyerKey.AfAdComplete, PurchaseEvent);
#endif
        }

        #endregion
    }
}