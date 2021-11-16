using System.Collections.Generic;
using EW2.Spell;
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
    public enum SummonType
    {
        Normal = 0,
        Premium = 1
    }

    public class SpellContainer : TabContainer
    {
        private const int SUMMON_ONE = 1;
        private const int SUMMON_TEN = 10;
        private const int INDEX_ADS_DATA = 5;

        #region Normal

        [Header("Normal Spell")]
        [SerializeField]
        private Text titleNormal;

        [SerializeField] private TimeCountDownUi timeCountdown;
        [SerializeField] private ButtonSummonSpell btnSummonOneNormal;
        [SerializeField] private ButtonSummonSpell btnSummonTenNormal;
        [SerializeField] private Button btnHelpNormal;
        [SerializeField] private ParticleSystem fxSummonOneNormal, fxSummonTenNormal;

        private GachaSpellNormal _spellNormalData;
        private GachaSpellCopyRate _gachaSpellCopyRate;
        private GachaSpellPremium _spellPremiumData;
        private GachaSpellRateProtection _spellRateProtect;

        #endregion

        #region Premium

        [Header("Premium Spell")]
        [SerializeField]
        private Text titlePremium;

        [SerializeField] private TimeCountDownUi timeCountdownPremium;
        [SerializeField] private ButtonSummonSpell btnSummonOnePremium;
        [SerializeField] private ButtonSummonSpell btnSummonTenPremium;
        [SerializeField] private ParticleSystem fxSummonOnePremium, fxSummonTenPremium;
        [SerializeField] private Button btnHelpPremium;
        [SerializeField] private Text txtBtnWatchAds;
        [SerializeField] private Button btnWatchAds;

        #endregion

        [SerializeField] private GameObject blockPanel;

        private void OnEnable()
        {
            VideoAdPlayer.Instance.OnRewarded += WatchAdsSuccess;
        }

        private void OnDisable()
        {
            if (VideoAdPlayer.Instance != null)
                VideoAdPlayer.Instance.OnRewarded -= WatchAdsSuccess;
        }

        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.OnRefreshSpellGacha, RefreshUi);
            btnHelpNormal.onClick.AddListener(HelpNormalClick);
            btnHelpPremium.onClick.AddListener(HelpPremiumClick);
            this.btnWatchAds.onClick.AddListener(WatchAdsOnClick);

        }

        private void RefreshUi()
        {
            ShowInfoSpellNormal();
            ShowInfoSpellPremium();
        }

        private void GetData()
        {
            this._spellNormalData = GameContainer.Instance.Get<GachaDataBase>().Get<GachaSpellNormal>();
            this._spellPremiumData = GameContainer.Instance.Get<GachaDataBase>().Get<GachaSpellPremium>();
            this._gachaSpellCopyRate = GameContainer.Instance.Get<GachaDataBase>().Get<GachaSpellCopyRate>();
            this._spellRateProtect = GameContainer.Instance.Get<GachaDataBase>().Get<GachaSpellRateProtection>();
        }

        private void ShowInfoSpellNormal()
        {
            titleNormal.text = L.gameplay.normal_spell_name;
            var isFree = UserData.Instance.OtherUserData.GetTimeRemainNormalSpell() <= 0;
            timeCountdown.gameObject.SetActive(!isFree);
            if (!isFree)
            {
                timeCountdown.SetTitle(L.popup.free_in);
                timeCountdown.SetData(UserData.Instance.OtherUserData.GetTimeRemainNormalSpell(), default, (() => {
                    RefreshUi();
                    EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
                }));
            }

            this.btnSummonOneNormal.InitButton(isFree, this._spellNormalData.gachaSpellDataBases[0], SummonType.Normal,
                SummonSpell);
            this.btnSummonTenNormal.InitButton(false, this._spellNormalData.gachaSpellDataBases[1], SummonType.Normal,
                SummonSpell);
        }

        private void ShowInfoSpellPremium()
        {
            titlePremium.text = L.gameplay.premium_spell_name;
            txtBtnWatchAds.text = L.button.reward_video_claim;
            var isFree = UserData.Instance.OtherUserData.GetTimeRemainPremiumSpell() <= 0;
            timeCountdownPremium.gameObject.SetActive(!isFree);
            if (!isFree)
            {
                timeCountdownPremium.SetTitle(L.popup.free_in);
                timeCountdownPremium.SetData(UserData.Instance.OtherUserData.GetTimeRemainPremiumSpell(), default,
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
                btnSummonOnePremium.InitButton(isFree, this._spellPremiumData.gachaSpellDataBases[0],
                    SummonType.Premium, SummonSpell);
            }

            btnSummonTenPremium.InitButton(false, this._spellPremiumData.gachaSpellDataBases[1], SummonType.Premium,
                SummonSpell);
        }

        public override void ShowContainer()
        {
            blockPanel.SetActive(false);
            GetData();
            ShowInfoSpellNormal();
            ShowInfoSpellPremium();
            gameObject.SetActive(true);
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void SummonSpell(int numberSpell, SummonType summonType)
        {
            EventManager.EmitEventData(GamePlayEvent.OnSummon, new int[] { InventoryType.Spell, (int)summonType });

            //Reward[] arrRewards = new Reward[numberSpell];
            var arrRewards = new List<Reward>();

            var listRate = new List<float>();

            if (summonType == SummonType.Normal)
            {
                foreach (var rarity in this._spellNormalData.gachaSpellRateDatas)
                {
                    listRate.Add(rarity.rate);
                }
            }
            else
            {
                foreach (var rarity in this._spellPremiumData.gachaSpellRateDatas)
                {
                    listRate.Add(rarity.rate);
                }
            }

            var resourceId = summonType == SummonType.Normal
                ? AnalyticsConstants.SourceSpellGachaNormal
                : AnalyticsConstants.SourceSpellGachaPremium;

            for (int i = 0; i < numberSpell; i++)
            {
                // bảo hiểm 
                //int indexResult = (i == 0 && numberSpell == SUMMON_TEN && summonType == SummonType.Premium)
                //    ? GetRarityProtect()
                //    : RandomFromDistribution.RandomChoiceFollowingDistribution(listRate);
                int indexResult = RandomFromDistribution.RandomChoiceFollowingDistribution(listRate);

                var reward = GetRewardSpells(indexResult);
                if (reward != null)
                {
                    Reward.AddToUserData(new Reward[] { reward }, resourceId,
                        numberSpell.ToString());
                    //arrRewards[i] = reward;
                    arrRewards.Add(reward);
                }
            }

            LoadSaveUtilities.AutoSave(false);
            blockPanel.SetActive(true);
            ShowEffect(numberSpell, summonType);
            InvokeProxy.Iinvoke.Invoke(this, () => ShowReward(arrRewards.ToArray()), 1f);

            EventManager.EmitEventData(GamePlayEvent.OnGachaSpell, numberSpell);
        }

        private Reward GetRewardSpells(int indexRarity)
        {
            var spellDataBases = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>();
            var dictSpellRandom = spellDataBases.GetDictSpellByRarity();
            if (dictSpellRandom != null)
            {
                var listSpells = dictSpellRandom[indexRarity];
                var idSpellResult = listSpells[Random.Range(0, listSpells.Count)];
                var dataSpellResult = UserData.Instance.GetInventory(InventoryType.Spell, idSpellResult);
                if (dataSpellResult == null)
                {
                    var spell = Reward.CreateInventory(ResourceType.Inventory, InventoryType.Spell, idSpellResult, 1);
                    return spell;
                }
                else
                {
                    var spellData = (SpellItem)dataSpellResult;

                    var levelMax = spellDataBases.GetSpellLevelMax(idSpellResult);

                    var fragmentUsed = spellDataBases.GetTotalFragmentUsed(spellData.ItemId, spellData.Level);

                    var fragmentToMax = spellDataBases.GetTotalFragmentToLevelMax(spellData.ItemId) - fragmentUsed;

                    if (spellData.Level >= levelMax || spellData.GetFragments() >= fragmentToMax)
                    {
                        // chuyen thanh fragment chung
                        var number = this._gachaSpellCopyRate.GetRateConvertCopy(idSpellResult);
                        var spellFragment = Reward.CreateInventory(ResourceType.Inventory, InventoryType.SpellFragment,
                            indexRarity, number);
                        return spellFragment;
                    }
                    else
                    {
                        // them fragment
                        var number = 1;
                        if (UserData.Instance.GetInventory(InventoryType.Spell, idSpellResult) != null)
                            number = this._gachaSpellCopyRate.GetRateConvertCopy(idSpellResult);

                        var spell = Reward.CreateInventory(ResourceType.Inventory, InventoryType.Spell, idSpellResult,
                            number);
                        return spell;
                    }
                }
            }

            return null;
        }

        private void HelpNormalClick()
        {
            var info = new SpellGachaInfoProperty(SummonType.Normal);
            UIFrame.Instance.OpenWindow(ScreenIds.spell_gacha_info, info);
        }

        private void HelpPremiumClick()
        {
            var info = new SpellGachaInfoProperty(SummonType.Premium);
            UIFrame.Instance.OpenWindow(ScreenIds.spell_gacha_info, info);
        }

        private void ShowEffect(int numberSummon, SummonType typeSummon)
        {
            if (typeSummon == SummonType.Normal)
            {
                fxSummonOneNormal.gameObject.SetActive(false);
                fxSummonTenNormal.gameObject.SetActive(false);
                fxSummonOneNormal.Clear();
                fxSummonTenNormal.Clear();
                if (numberSummon == 1)
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
            else
            {
                this.fxSummonOnePremium.gameObject.SetActive(false);
                this.fxSummonTenPremium.gameObject.SetActive(false);
                this.fxSummonOnePremium.Clear();
                this.fxSummonTenPremium.Clear();
                if (numberSummon == 1)
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
            if (UserData.Instance.OtherUserData.UnlockSpellData.isFirstTimeGaCha)
            {
                callBack = UserData.Instance.OtherUserData.UnlockSpellData.UnlockFlowStep2;
            }
            PopupUtils.ShowMultipleReward(arrRewards, callBack);
            InvokeProxy.Iinvoke.Invoke(this, () => {
                blockPanel.SetActive(false);
                HideAllEffect();
            }, 1f);
        }
        private void HideAllEffect()
        {
            this.fxSummonOneNormal.gameObject.SetActive(false);
            this.fxSummonTenNormal.gameObject.SetActive(false);
            this.fxSummonOnePremium.gameObject.SetActive(false);
            this.fxSummonTenPremium.gameObject.SetActive(false);
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
            var dataBase = this._spellPremiumData.gachaSpellDataBases[0];
            UserData.Instance.OtherUserData.timeCountdownPremiumSpell =
                TimeManager.NowInSeconds + (long)(3600 * dataBase.timeCountdownFree);

            SummonSpell(SUMMON_ONE, SummonType.Premium);
            RefreshUi();
            EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
        }

        private int GetRarityProtect()
        {
            var listRate = new List<float>();
            foreach (var rarity in this._spellRateProtect.spellRateProtections)
            {
                listRate.Add(rarity.rate);
            }

            var indexRandom = RandomFromDistribution.RandomChoiceFollowingDistribution(listRate);
            return this._spellRateProtect.spellRateProtections[indexRandom].rarity;
        }


    }
}
