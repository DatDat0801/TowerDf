using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using SocialTD2;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;
using ZitgaGiftCode;

namespace EW2
{
    public class GiftCodeWindowController : AWindowController
    {
        [SerializeField] private InputField inputGiftCode;

        [SerializeField] private Button btnConfirm;

        //[SerializeField] private Button btnConfirmDisable;
        [SerializeField] private Button btnClose;

        private Social.GiftCode.ZitgaGiftCode giftCode;
        private AuthProvider provider;

        private const string SECRET_CODE = "ZCHEATCODE123";
        private const string IGNORE_IAP_CODE = "ignoreiap6789";
        private const string TEST_IAP_CODE = "testiap6789";
        private const string ENABLE_LOG = "enablelog6789";
        private const string ENABLE_CHEAT = "batcheatz704";

        protected override void Awake()
        {
            base.Awake();

            btnConfirm.onClick.AddListener(ConfirmClick);

            btnClose.onClick.AddListener(CloseClick);
            giftCode = new Social.GiftCode.ZitgaGiftCode();
            provider = Social.GiftCode.ZitgaGiftCode.GetProvider();
        }

        private void OnEnable()
        {
            giftCode.onCheckGiftCode += OnCheckGiftCode;
            giftCode.onClaimGiftCode += OnClaimGiftCode;
        }


        private void OnDisable()
        {
            giftCode.onCheckGiftCode -= OnCheckGiftCode;
            giftCode.onClaimGiftCode -= OnClaimGiftCode;
        }

        private async void OnClaimGiftCode(int logicCode, string giftCodeData)
        {
            switch (logicCode)
            {
                case 0:
                    HandleSpecialGiftCode();
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.claim_giftcode);
                    await UniTask.Delay(1000);
                    RewardContainer rewards = JsonConvert.DeserializeObject<RewardContainer>(giftCodeData);
                    var rewardGen = Reward.GenRewards(rewards.items);
                    if (rewardGen != null)
                    {
                        Reward.AddToUserData(rewardGen, AnalyticsConstants.SourceGiftCode);
                        PopupUtils.ShowReward(rewardGen);
                    }

                    inputGiftCode.text = string.Empty;
                    break;
                case 51:
                case 52:
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.used_giftcode_toast);
                    break;
                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                case 85:
                case 50:
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.claim_giftcode_failed);
                    break;
                default:
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, $"{L.popup.error_occured} Code: {logicCode}");
                    break;
            }
        }

        private void OnCheckGiftCode(int logicCode, string giftCodeData)
        {
        }

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }


        private void ConfirmClick()
        {
            if (inputGiftCode.text.Equals(SECRET_CODE))
            {
                FindObjectOfType<GameLaunch>().SetCheat(true);
                LoadSceneUtils.LoadScene(SceneName.Start);
                return;
            }

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.giftcode_check_internet);
                return;
            }

            if (!LoadSaveUtilities.IsAuthenticated())
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.giftcode_login_first);
                return;
            }

            //check gift code
            var userId = UserData.Instance.AccountData.tokenId;
            //giftCode.CheckGiftCode(provider, userId, inputGiftCode.text);
            //claim gift code
            giftCode.ClaimGiftCode(provider, userId, inputGiftCode.text);
        }

        [Button]
        private void TestSerializeGiftCode()
        {
            Reward reward = Reward.Create(ResourceType.Inventory, 4003, 60);

            var json = JsonConvert.SerializeObject(reward);
            Debug.Log(json);
        }

        [Button]
        private void TestDeserialize()
        {
            string json =
                "{\"item\":[{\"type\":0,\"id\":0,\"number\":500,\"itemType\":-1},{\"type\":0,\"id\":0,\"number\":1000,\"itemType\":-1}]}";
            RewardContainer rewards = JsonConvert.DeserializeObject<RewardContainer>(json);
            Debug.LogAssertion(rewards.items.Length);
        }

        private void HandleSpecialGiftCode()
        {
            var currCode = this.inputGiftCode.text.ToLower();

            Debug.LogWarning($"[Gift Code] {currCode}");

            var gameLaunch = FindObjectOfType<GameLaunch>();

            if (gameLaunch == null) return;

            if (currCode.Equals(IGNORE_IAP_CODE))
            {
                var iapManager = gameLaunch.Context.GetContainer().Resolve(nameof(IapManager));
                ((IapManager)iapManager)?.SetIsTestIAP(true);
            }
            else if (currCode.Equals(TEST_IAP_CODE))
            {
                var iapManager = gameLaunch.Context.GetContainer().Resolve(nameof(IapManager));
                ((IapManager)iapManager)?.SetIsTester(true);
            }
            else if (currCode.Equals(ENABLE_LOG))
            {
                gameLaunch.SetReporter(true);
            }
            else if (currCode.Equals(ENABLE_CHEAT))
            {
                gameLaunch.SetCheat(true);
                LoadSceneUtils.LoadScene(SceneName.Start);
            }
        }
    }
}