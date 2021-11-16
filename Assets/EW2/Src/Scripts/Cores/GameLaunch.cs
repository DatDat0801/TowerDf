using UnityEngine;
using System;
using EW2.Social;
using Invoke;
using Sirenix.OdinInspector;
using TigerForge;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Zitga.ContextSystem;
using Zitga.Localization;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;
using Zitga.Update;
using ZitgaUtils;

namespace EW2
{
    public class GameLaunch : MonoBehaviour
    {
        private static bool isInitAwake = false;

        public static bool isStart = false;

        public static bool isCheat;

        public bool enableCheat;
        [SerializeField] private SystemLanguage[] supportedLanguages;

        public GameObject reporter;

        public bool isTest;
        [ShowIf("isTest")] public int mapId;

        private Context context;

        public Context Context => this.context;
        private ZitgaServerUtils _zitgaServerUtils;

        private ZitgaServerUtils ZitgaServerUtils
        {
            get
            {
                if (this._zitgaServerUtils == null)
                {
                    this._zitgaServerUtils = new ZitgaServerUtils();
                }

                return this._zitgaServerUtils;
            }
        }

        private void Awake()
        {
            if (isInitAwake)
            {
                return;
            }

            if (isCheat == false)
                isCheat = enableCheat;

            isInitAwake = true;

            context = Context.Current;

            DeviceUtils.IsLowEndDevice();

            DeviceUtils.SetUpQualityLevel(UserData.Instance.SettingData.graphicQuality);

            InitInvokeUtils();

            InitCoroutineUtils();

            InitUpdateSystem();

            InitLocalization();

            InitUiFrame();

            InitFirebase();

            InitFacebook();

            InitToastController();

            InitTouchEffect();

            InitSpawnStaminaController();

            InitializeIronSource();

            // InitATTIos();

            InitRating();

            InitIap();
            
            InvokeProxy.Iinvoke.Invoke(this, InitAppsflyer, 3f);

#if UNITY_IOS
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
        }

        private void Start()
        {
            UIFrame.Instance.SwitchCamera(false);

            GameContainer.Instance.Get<QuestManager>().InitQuest();

            if (isTest == false)
            {
                UIFrame.Instance.OpenWindow(isStart ? ScreenIds.home : ScreenIds.tap_to_start);
            }
            else
            {
                GamePlayController.CampaignId = MapCampaignInfo.GetCampaignId(0, mapId, 0);

                SceneManager.LoadScene(SceneName.GamePlay);
            }
        }

        private void OnEnable()
        {
            this.ZitgaServerUtils.onGetInfo += SetupServerInfo;
        }

        private void OnDisable()
        {
            this.ZitgaServerUtils.onGetInfo -= SetupServerInfo;
        }

        private void SetupServerInfo(int code, LocationInbound locationInbound)
        {
            if (code == 0 && GameLaunch.isCheat == false)
            {
                var timeSpan = TimeSpan.FromMilliseconds(locationInbound.CurrentTimeInMillisecond);
                DateTime serverTime = new DateTime(1970, 1, 1) + timeSpan;
                TimeManager.Setup(serverTime, true);
                Debug.LogWarning($"Server time: {serverTime}");
            }
            else
            {
                TimeManager.Setup(DateTime.UtcNow, false);
            }
        }

        public void SetCheat(bool unlockCheat)
        {
            isCheat = unlockCheat;

            reporter.SetActive(unlockCheat);
        }

        public void SetReporter(bool enable)
        {
            reporter.SetActive(enable);
        }

        private void InitializeIronSource()
        {
#if UNITY_ANDROID
            string appKey = "ee6cb939";
#elif UNITY_IPHONE
        string appKey = "ee6d2c49";
#else
        string appKey = "unexpected_platform";
#endif


            //Debug.Log("unity-script: IronSource.Agent.validateIntegration");
            IronSource.Agent.validateIntegration();

            //Debug.Log("unity-script: unity version" + IronSource.unityVersion());

            // SDK init
            //Debug.Log("unity-script: IronSource.Agent.init");
            IronSource.Agent.init(appKey);
        }

        private void InitUiFrame()
        {
            var result = Resources.Load<UIFrame>("UIFrame");

            if (result)
            {
                var uiFrame = Instantiate(result);

                DontDestroyOnLoad(uiFrame.gameObject);

                uiFrame.gameObject.name = typeof(UIFrame) + " (Singleton)";

                context.GetContainer().Register(uiFrame);
            }
            else
            {
                throw new Exception("UIFrame is not exist");
            }
        }

        private void InitToastController()
        {
            context.GetContainer().Register(new ToastController());
        }

        private void InitSpawnStaminaController()
        {
            var regenStamina = RegenStaminaController.Instance;

            DontDestroyOnLoad(regenStamina.gameObject);

            context.GetContainer().Register(regenStamina);
        }

        private void InitInvokeUtils()
        {
            InvokeCallbackUtils.Instance.Init();

            DontDestroyOnLoad(InvokeCallbackUtils.Instance.gameObject);

            context.GetContainer().Register(InvokeCallbackUtils.Instance);
        }

        private void InitCoroutineUtils()
        {
            DontDestroyOnLoad(CoroutineUtils.Instance.gameObject);

            context.GetContainer().Register(CoroutineUtils.Instance);
        }

        private void InitUpdateSystem()
        {
            DontDestroyOnLoad(GlobalUpdateSystem.Instance.gameObject);

            context.GetContainer().Register(GlobalUpdateSystem.Instance);
        }

        private void InitLocalization()
        {
            var localization = Localization.Current;
            //in case get language data from user data
            var userLanguage = UserData.Instance.SettingData.userLanguage;
            if (userLanguage != SystemLanguage.Unknown)
            {
                localization.localCultureInfo = Locale.GetCultureInfoByLanguage(userLanguage);
                context.GetContainer().Register(localization);
                EventManager.EmitEvent(GamePlayEvent.OnChangeLanggueSuccess);
                return;
            }

            //in case first time init user language data
            var systemLanguage = Application.systemLanguage;
            for (var i = 0; i < supportedLanguages.Length; i++)
            {
                if (supportedLanguages[i] == systemLanguage)
                {
                    localization.localCultureInfo = Locale.GetCultureInfoByLanguage(systemLanguage);
                    UserData.Instance.SettingData.userLanguage = systemLanguage;

                    UserData.Instance.Save();
                    context.GetContainer().Register(localization);
                    return;
                }
            }

            Assert.IsTrue(false, "No languages supported");
        }

        private void InitTouchEffect()
        {
            var objTouchEffect = new GameObject();

            if (objTouchEffect)
            {
                objTouchEffect.AddComponent<TouchEffect>();

                objTouchEffect.name = nameof(TouchEffect);

                DontDestroyOnLoad(objTouchEffect);

                context.GetContainer().Register(objTouchEffect);
            }
        }

        private void InitFirebase()
        {
            var firebaseLogic = FirebaseLogic.Instance;
            firebaseLogic.Init(enableCheat);

            DontDestroyOnLoad(firebaseLogic.gameObject);
        }

        private void InitAppsflyer()
        {
            var appsflyer = AppsflyerUtils.Instance;
            appsflyer.Init(enableCheat);

            DontDestroyOnLoad(appsflyer.gameObject);

            // EventManager.StopListening(GamePlayEvent.StartInitAppsflyer, InitAppsflyer);
        }

        private void InitFacebook()
        {
            var fb = FacebookService.Instance;
            fb.InitFB();

            DontDestroyOnLoad(fb.gameObject);
        }


        private void InitIap()
        {
            InvokeProxy.Iinvoke.Invoke(this, () => {
                Debug.Log("Init IAP");
                ProductsManager.AddProductBundleShop();
                var iap = IapManager.Instance;
                iap.Init(null);
                DontDestroyOnLoad(iap.gameObject);
                context.GetContainer().Register(iap);
            }, 1f);
        }

        private void InitATTIos()
        {
#if UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
        }

        private void InitRating()
        {
            if (!UserData.Instance.AccountData.isCompleteRating)
            {
                var rating = RatingController.Instance;
                DontDestroyOnLoad(rating.gameObject);
                context.GetContainer().Register(rating);
            }
        }
    }
}