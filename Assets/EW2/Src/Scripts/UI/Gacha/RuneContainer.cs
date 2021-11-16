using System.Collections.Generic;
using Invoke;
using SocialTD2;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;
using Random = UnityEngine.Random;

namespace EW2
{
    public class RuneContainer : TabContainer
    {
        private const int SUMMON_ONE = 1;
        private const int SUMMON_TEN = 10;
        private const int INDEX_ADS_DATA = 4;

        #region Normal

        [Header("Normal Spell")]
        [SerializeField]
        private Text titleNormal;

        [SerializeField] private TimeCountDownUi timeCountdownNormal;
        [SerializeField] private ButtonSummonRune btnSummonOneNormal;
        [SerializeField] private ButtonSummonRune btnSummonTenNormal;
        [SerializeField] private Button btnHelpNormal;
        [SerializeField] private ParticleSystem fxSummonOneNormal, fxSummonTenNormal;

        #endregion

        #region Premium

        [Header("Premium Spell")]
        [SerializeField]
        private Text titlePremium;

        [SerializeField] private TimeCountDownUi timeCountdownPremium;
        [SerializeField] private ButtonSummonRune btnSummonOnePremium;
        [SerializeField] private ButtonSummonRune btnSummonTenPremium;
        [SerializeField] private ParticleSystem fxSummonOnePremium, fxSummonTenPremium;
        [SerializeField] private Button btnHelpPremium;
        [SerializeField] private Text txtBtnWatchAds;
        [SerializeField] private Button btnWatchAds;

        #endregion

        [SerializeField] private GameObject blockPanel;

        private GachaRuneData gachaRuneData;
        private GachaRuneRateProtection runeRateProtect;

        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.OnRefreshRuneGacha, RefreshUi);
            btnHelpNormal.onClick.AddListener(HelpNormalClick);
            btnHelpPremium.onClick.AddListener(HelpPremiumClick);
            btnWatchAds.onClick.AddListener(WatchAdsOnClick);
        }

        private void OnEnable()
        {
            VideoAdPlayer.Instance.OnRewarded += WatchAdsSuccess;
        }

        private void OnDisable()
        {
            if (VideoAdPlayer.Instance != null)
                VideoAdPlayer.Instance.OnRewarded -= WatchAdsSuccess;
        }

        private void RefreshUi()
        {
            ShowInfoRuneNormal();
            ShowInfoRunePremium();
        }

        private void GetData()
        {
            gachaRuneData = GameContainer.Instance.Get<GachaDataBase>().Get<GachaRuneData>();
            runeRateProtect = GameContainer.Instance.Get<GachaDataBase>().Get<GachaRuneRateProtection>();
        }

        private void ShowInfoRuneNormal()
        {
            titleNormal.text = L.gameplay.normal_rune_name;
            var isFree = UserData.Instance.OtherUserData.GetTimeRemainNormalRune() <= 0;
            timeCountdownNormal.gameObject.SetActive(!isFree);
            if (!isFree)
            {
                timeCountdownNormal.SetTitle(L.popup.free_in);
                timeCountdownNormal.SetData(UserData.Instance.OtherUserData.GetTimeRemainNormalRune(), default, (() => {
                    RefreshUi();
                    EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
                }));
            }

            btnSummonOneNormal.InitButton(isFree, gachaRuneData.GetDataGacha(SUMMON_ONE, SummonType.Normal),
                SummonType.Normal,
                SummonGacha);
            btnSummonTenNormal.InitButton(false, gachaRuneData.GetDataGacha(SUMMON_TEN, SummonType.Normal),
                SummonType.Normal,
                SummonGacha);
        }

        private void ShowInfoRunePremium()
        {
            titlePremium.text = L.gameplay.premium_rune_name;
            txtBtnWatchAds.text = L.button.reward_video_claim;
            var isFree = UserData.Instance.OtherUserData.GetTimeRemainPremiumRune() <= 0;
            timeCountdownPremium.gameObject.SetActive(!isFree);
            if (!isFree)
            {
                timeCountdownPremium.SetTitle(L.popup.free_in);
                timeCountdownPremium.SetData(UserData.Instance.OtherUserData.GetTimeRemainPremiumRune(), default,
                    (() => {
                        RefreshUi();
                        EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
                    }));
            }

            if (isFree)
            {
                btnWatchAds.gameObject.SetActive(true);
                btnSummonOnePremium.gameObject.SetActive(false);
            }
            else
            {
                btnWatchAds.gameObject.SetActive(false);
                btnSummonOnePremium.gameObject.SetActive(true);
                btnSummonOnePremium.InitButton(isFree, gachaRuneData.GetDataGacha(SUMMON_ONE, SummonType.Premium),
                    SummonType.Premium, SummonGacha);
            }

            btnSummonTenPremium.InitButton(false, gachaRuneData.GetDataGacha(SUMMON_TEN, SummonType.Premium),
                SummonType.Premium,
                SummonGacha);
        }

        public override void ShowContainer()
        {
            blockPanel.SetActive(false);
            GetData();
            ShowInfoRuneNormal();
            ShowInfoRunePremium();
            gameObject.SetActive(true);
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void SummonGacha(int numberRune, SummonType summonType)
        {
            EventManager.EmitEventData(GamePlayEvent.OnSummon, new int[] { InventoryType.Rune, (int)summonType });

            var arrRewards = new Reward[numberRune + 1];

            var listRate = new List<float>();
            foreach (var rarity in gachaRuneData.GetDataGachaRate(summonType))
            {
                listRate.Add(rarity.rate);
            }

            var resourceId = summonType == SummonType.Normal
                ? AnalyticsConstants.SourceRuneGachaNormal
                : AnalyticsConstants.SourceRuneGachaPremium;
            var itemId = numberRune.ToString();

            for (int i = 0; i < numberRune; i++)
            {
                int indexResult = (i == 0 && numberRune == SUMMON_TEN && summonType == SummonType.Premium)
                    ? GetRarityProtect()
                    : RandomFromDistribution.RandomChoiceFollowingDistribution(listRate);
                var reward = GetRewardRune(indexResult);
                if (reward != null)
                {
                    Reward.AddToUserData(new Reward[] { reward }, resourceId, itemId);
                    arrRewards[i] = reward;
                }
            }

            // add exp rune
            var expRune = gachaRuneData.GetDataGacha(numberRune, summonType).expPerOnce * numberRune;
            var expReward = Reward.CreateInventory(ResourceType.Money, InventoryType.None, MoneyType.ExpRune, expRune);
            Reward.AddToUserData(new Reward[] { expReward }, resourceId, itemId);
            arrRewards[arrRewards.Length - 1] = expReward;
            //

            LoadSaveUtilities.AutoSave(false);
            blockPanel.SetActive(true);
            ShowEffect(numberRune, summonType);
            InvokeProxy.Iinvoke.Invoke(this, () => ShowReward(arrRewards), 1f);

            EventManager.EmitEventData(GamePlayEvent.OnGachaRune, numberRune);
        }

        private int GetRarityProtect()
        {
            var listRate = new List<float>();
            foreach (var rarity in runeRateProtect.runeRateProtections)
            {
                listRate.Add(rarity.rate);
            }

            var indexRandom = RandomFromDistribution.RandomChoiceFollowingDistribution(listRate);
            return runeRateProtect.runeRateProtections[indexRandom].rarity;
        }

        private Reward GetRewardRune(int indexRarity)
        {
            var runeIdRandom = Random.Range(0, 10);
            while (runeIdRandom == (int)RuneId.LifeRune || runeIdRandom == (int)RuneId.ArgonyRune ||
                   runeIdRandom == (int)RuneId.MiseryRune)
            {
                runeIdRandom = Random.Range(0, 10);
            }

            var runeIdConvert = InventoryDataBase.GetRuneIdConvert(runeIdRandom, indexRarity);
            var rune = Reward.CreateInventory(ResourceType.Inventory, InventoryType.Rune, runeIdConvert, 1);
            return rune;
        }

        private void HelpNormalClick()
        {
            var info = new RuneGachaInfoProperty(SummonType.Normal);
            UIFrame.Instance.OpenWindow(ScreenIds.rune_gacha_info, info);
        }

        private void HelpPremiumClick()
        {
            var info = new RuneGachaInfoProperty(SummonType.Premium);
            UIFrame.Instance.OpenWindow(ScreenIds.rune_gacha_info, info);
        }

        private void ShowEffect(int numberSummon, SummonType typeSummon)
        {
            if (typeSummon == SummonType.Normal)
            {
                fxSummonOneNormal.gameObject.SetActive(false);
                fxSummonTenNormal.gameObject.SetActive(false);
                fxSummonOneNormal.Clear();
                fxSummonTenNormal.Clear();
                if (numberSummon == SUMMON_ONE)
                {
                    fxSummonOneNormal.gameObject.SetActive(true);
                    fxSummonOneNormal.Play();
                }
                else
                {
                    fxSummonTenNormal.gameObject.SetActive(true);
                    fxSummonTenNormal.Play();
                }
            }
            else if (typeSummon == SummonType.Premium)
            {
                fxSummonOnePremium.gameObject.SetActive(false);
                fxSummonTenPremium.gameObject.SetActive(false);
                fxSummonOnePremium.Clear();
                fxSummonTenPremium.Clear();
                if (numberSummon == SUMMON_ONE)
                {
                    fxSummonOnePremium.gameObject.SetActive(true);
                    fxSummonOnePremium.Play();
                }
                else
                {
                    fxSummonTenPremium.gameObject.SetActive(true);
                    fxSummonTenPremium.Play();
                }
            }
        }

        private void ShowReward(Reward[] arrRewards)
        {
            UnityAction callBack = null;
            if (UserData.Instance.OtherUserData.UnlockRuneData.isFirstTimeGaCha)
            {
                callBack = UserData.Instance.OtherUserData.UnlockRuneData.UnlockFlowStep2;
            }
            PopupUtils.ShowReward(arrRewards, callBack);
            InvokeProxy.Iinvoke.Invoke(this, () => {
                blockPanel.SetActive(false);
                HideAllEffect();
            }, 1.5f);
        }

        private void HideAllEffect()
        {
            fxSummonOneNormal.gameObject.SetActive(false);
            fxSummonTenNormal.gameObject.SetActive(false);
            fxSummonOnePremium.gameObject.SetActive(false);
            fxSummonTenPremium.gameObject.SetActive(false);
        }

        private void WatchAdsOnClick()
        {
            var adRewardData = GameContainer.Instance.GetAdRewardDatabase();
            var addData = adRewardData.ads[INDEX_ADS_DATA];
            if (addData != null)
            {
                VideoAdPlayer.Instance.PlayAdClick(addData.placementName);
            }
        }

        private void WatchAdsSuccess(string arg0)
        {
            var dataBase = gachaRuneData.GetDataGacha(SUMMON_ONE, SummonType.Premium);
            UserData.Instance.OtherUserData.timeCountdownPremiumRune =
                TimeManager.NowInSeconds + (long)(3600 * dataBase.timeCountdownFree);

            SummonGacha(SUMMON_ONE, SummonType.Premium);
            RefreshUi();
            EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
        }
    }
}
