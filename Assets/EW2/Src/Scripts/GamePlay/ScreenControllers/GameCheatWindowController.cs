using System;
using Firebase.Crashlytics;
using TigerForge;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;
using Zitga.UIFramework;
using Random = UnityEngine.Random;

namespace EW2
{
    public class GameCheatWindowController : AWindowController
    {
        [SerializeField] private InputField fieldMoneyType;

        [SerializeField] private InputField fieldMoney;

        [SerializeField] private InputField fieldInventoryType;

        [SerializeField] private InputField fieldInventoryId;

        [SerializeField] private InputField fieldRarity;

        [SerializeField] private InputField fieldNumber;

        [SerializeField] private InputField fieldLevel;

        [SerializeField] private InputField fieldAchievementId;

        [SerializeField] private InputField fieldCount;

        [SerializeField] private InputField fieldNumberDFP;

        [SerializeField] private Button btnClose;

        [SerializeField] private Button btnCheatMoney;

        [SerializeField] private Button btnCheatInventory;

        [SerializeField] private Button btnCheatUnlockFullCampaign;
        [SerializeField] private InputField campaignMode;

        [SerializeField] private Button btnUnlockNightmareRandom;

        [SerializeField] private Button btnCheatCrash;

        [SerializeField] private Button btnClearData;

        [SerializeField] private Button btnCheatAchievement;

        [SerializeField] private Button btnSetTicketDFP;

        protected override void Awake()
        {
            base.Awake();

            btnClose.onClick.AddListener(() => { UIFrame.Instance.CloseWindow(ScreenIds.popup_cheat); });

            btnCheatMoney.onClick.AddListener(CheatMoney);

            btnCheatInventory.onClick.AddListener(CheatInventory);

            btnCheatUnlockFullCampaign.onClick.AddListener(CheatUnlockCampaign);

            btnUnlockNightmareRandom.onClick.AddListener(CheatNightmareRandom);

            btnCheatCrash.onClick.AddListener(CrashClick);

            btnClearData.onClick.AddListener(ClearDataClick);

            btnCheatAchievement.onClick.AddListener(CheatAchievement);

            this.btnSetTicketDFP.onClick.AddListener(SetTicketDFP);
        }

        private void SetTicketDFP()
        {
            var number = this.fieldNumberDFP.text;
            UserData.Instance.UserHeroDefenseData.numberTicket = int.Parse(number);
            UserData.Instance.Save();
            ToastCheatDone();
        }

        private void CheatNightmareRandom()
        {
            var numbStar = 0;

            for (int i = 0; i < 20; i++)
            {
                if (!UserData.Instance.CampaignData.IsUnlockedHardStage(0, i)) continue;
                var campaignId = MapCampaignInfo.GetCampaignId(0, i, 1);
                var starRandom = Random.Range(1, 4);
                numbStar += starRandom;
                UserData.Instance.SetCampaignStar(campaignId, starRandom);
                UserData.Instance.Save();
            }

            var currStar = UserData.Instance.GetMoney(MoneyType.GoldStar);
            UserData.Instance.SubMoney(MoneyType.GoldStar, currStar, "cheat", "9", false);
            UserData.Instance.AddMoney(MoneyType.GoldStar, numbStar, "cheat", "9", false);
            btnUnlockNightmareRandom.enabled = false;
            ToastCheatDone();
        }

        private void CheatInventory()
        {
            var inventoryType = int.Parse(fieldInventoryType.text);

            var inventoryId = int.Parse(fieldInventoryId.text);

            var number = int.Parse(fieldNumber.text);

            var level = int.Parse(fieldLevel.text);

            var rarity = int.Parse(fieldRarity.text);

            if (inventoryType == InventoryType.Spell || inventoryType == InventoryType.SpellFragment)
            {
                var spellItem = new SpellItem(inventoryId, number, inventoryType, level);
                UserData.Instance.AddInventory(spellItem, "cheat", "", false);
            }
            else if (inventoryType == InventoryType.Rune)
            {
                var idConvert = InventoryDataBase.GetRuneIdConvert(inventoryId, rarity);
                var runeItem = new RuneItem(idConvert, 1, inventoryType);
                UserData.Instance.AddInventory(runeItem, "cheat", "", false);
            }

            ToastCheatDone();
        }

        private void ClearDataClick()
        {
            var myFile = new EasyFileSave();
            myFile.Delete();
            ToastCheatDone();
        }

        private void CrashClick()
        {
#if TRACKING_FIREBASE
            Crashlytics.LogException(new Exception("Crash Game Test"));
            Utils.ForceCrash(ForcedCrashCategory.Abort);
#endif
        }

        private void CheatMoney()
        {
            var moneyType = int.Parse(fieldMoneyType.text);

            var money = int.Parse(fieldMoney.text);

            if (money > 0)
            {
                UserData.Instance.AddMoney(moneyType, money, "Cheat", "", false);
            }
            else
            {
                UserData.Instance.SubMoney(moneyType, Mathf.Abs(money), "Cheat", "", false);
            }

            ToastCheatDone();
        }

        private void CheatUnlockCampaign()
        {
            int modeId = 0;
            try
            {
                modeId = int.Parse(campaignMode.text);
            }
            catch (Exception e)
            {
                Debug.LogAssertion(e);
                //throw;
            }


            bool isUnlocked = true;

            for (int i = 0; i < 20; i++)
            {
                var campaignId = MapCampaignInfo.GetCampaignId(0, i, modeId);
                if (UserData.Instance.CampaignData.GetStar(campaignId) < 3)
                {
                    isUnlocked = false;
                    break;
                }
            }

            if (isUnlocked) return;

            for (int i = 0; i < 20; i++)
            {
                var campaignId = MapCampaignInfo.GetCampaignId(0, i, modeId);
                UserData.Instance.SetCampaignStar(campaignId, 3);
                UserData.Instance.Save();
            }

            if (modeId == 0)
            {
                var currentSilverStars = UserData.Instance.GetMoney(MoneyType.SliverStar);
                int newSilverStars = UserData.Instance.CampaignData.GetTotalStars(modeId);
                UserData.Instance.SubMoney(MoneyType.SliverStar, currentSilverStars, "cheat", "8", false);
                UserData.Instance.AddMoney(MoneyType.SliverStar, newSilverStars, "cheat", "8", false);
            }
            else
            {
                var currentGoldenStars = UserData.Instance.GetMoney(MoneyType.GoldStar);
                int newGoldenStars = UserData.Instance.CampaignData.GetTotalStars(modeId);
                UserData.Instance.SubMoney(MoneyType.GoldStar, currentGoldenStars, "cheat", "9", false);
                UserData.Instance.AddMoney(MoneyType.GoldStar, newGoldenStars, "cheat", "9", false);
            }

            ToastCheatDone();
        }

        private void CheatAchievement()
        {
            var id = int.Parse(fieldAchievementId.text);

            var count = int.Parse(fieldCount.text);

            if (count > 0)
            {
                var achievementQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<AchievementQuest>();
                var achievement = achievementQuest.GetQuest(id);
                if (achievement != null)
                {
                    achievement.Count = count;
                    UserData.Instance.Save();
                    ToastCheatDone();
                }
            }
        }

        private void ToastCheatDone()
        {
            EventManager.EmitEventData(GamePlayEvent.ShortNoti, "Saved!");
        }
    }
}