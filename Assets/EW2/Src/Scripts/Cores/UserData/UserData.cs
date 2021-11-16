using System;
using System.Collections.Generic;
using System.Linq;
using EW2.Spell;
using Newtonsoft.Json;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class UserData
    {
        private const string Key = "user_data";

        private const string Password = "zitga6789@";

        private static UserData _instance;

        private static EasyFileSave _myFile;

        private static JsonSerializerSettings _jsonSerializerSettings;

        public static UserData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _jsonSerializerSettings = new JsonSerializerSettings()
                        {TypeNameHandling = TypeNameHandling.Objects};

                    _myFile = new EasyFileSave();

                    if (_myFile.Load(Password) && _myFile.KeyExists(Key))
                    {
                        Load();
                        _instance.BackUpStars();
                    }
                    else
                    {
                        _instance = new UserData();
                    }
                }

                return _instance;
            }
            private set { _instance = value; }
        }

        private UserData()
        {
            GlobalResources = new ResourceController();

            CampaignData = new UserCampaignData();

            RegenStamina = new RegenStaminaData();

            AccountData = new AccountData();

            SettingData = new SettingData();

            UserHeroData = new UserHeroData();

            BackUpData = new BackUpData();

            OtherUserData = new OtherUserData();

            UserTowerData = new UserTowerData();

            UserShopData = new UserShopData();

            UserDailyCheckin = new UserDailyCheckin();

            UserDailyQuest = new UserDailyQuest();

            UserAchievementQuest = new UserAchievementQuest();

            UserAdDataWrapper = new UserAdDataWrapper();
            
            UserEventData = new UserEventData();
            
            UserHeroDefenseData = new UserHeroDefenseData();

            TournamentData = new UserTournamentData();
        }

        public UserTournamentData TournamentData { get; }
        public ResourceController GlobalResources { get; }

        public UserCampaignData CampaignData { get; }

        public RegenStaminaData RegenStamina { get; }

        public AccountData AccountData { get; }

        public SettingData SettingData { get; }

        public UserHeroData UserHeroData { get; }

        public BackUpData BackUpData { get; }

        public OtherUserData OtherUserData { get; }
        public UserTowerData UserTowerData { get; }

        public UserShopData UserShopData { get; }

        public UserDailyCheckin UserDailyCheckin { get; }

        public UserDailyQuest UserDailyQuest { get; }

        public UserAchievementQuest UserAchievementQuest { get; }

        public UserAdDataWrapper UserAdDataWrapper { get; }
        
        public UserEventData UserEventData { get; }
        
        public UserHeroDefenseData UserHeroDefenseData { get; }

        public long GetMoney(int moneyType)
        {
            return GlobalResources.GetMoney(moneyType);
        }

        public void AddMoney(int moneyType, long value, string source, string sourceId, bool isPurchase = false,
            bool isSave = true)
        {
            GlobalResources.Add(ResourceType.Money, moneyType, value);

            try
            {
                // tracking
                if (moneyType == MoneyType.Crystal)
                {
                    FirebaseLogic.Instance.SetRemainingCrystal(GetMoney(MoneyType.Crystal).ToString());
                }
                else if (moneyType == MoneyType.Diamond)
                {
                    FirebaseLogic.Instance.SetRemainingGem(GetMoney(MoneyType.Diamond).ToString());
                }

                FirebaseLogic.Instance.LogResource("money", Reward.GetCategory(moneyType), moneyType.ToString(),
                    Reward.GetCategory(moneyType), value, GetMoney(moneyType), source, sourceId, isPurchase);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }


            if (isSave)
                Save();
        }

        public void SubMoney(int moneyType, long value, string source, string sourceId, bool isPurchase = false,
            bool isSave = true)
        {
            GlobalResources.Sub(ResourceType.Money, moneyType, value);

            EventManager.EmitEventData(GamePlayEvent.OnSpendResource, new int[] {moneyType, (int) value});

            if (isSave)
                Save();

            try
            {
                // tracking
                if (moneyType == MoneyType.Crystal)
                {
                    FirebaseLogic.Instance.SetRemainingCrystal(GetMoney(MoneyType.Crystal).ToString());
                }
                else if (moneyType == MoneyType.Diamond)
                {
                    FirebaseLogic.Instance.SetRemainingGem(GetMoney(MoneyType.Diamond).ToString());
                }

                FirebaseLogic.Instance.LogResource("money", Reward.GetCategory(moneyType), moneyType.ToString(),
                    Reward.GetCategory(moneyType), -value, GetMoney(moneyType), source, sourceId, isPurchase);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public void AddInventory(ItemInventoryBase inventoryData, string source, string sourceId, bool isPurchase,
            bool isSave = true)
        {
            if (inventoryData.InventoryType == InventoryType.Spell)
            {
                var spell = GetInventory(inventoryData.InventoryType, inventoryData.ItemId);
                if (spell == null)
                {
                    BadgeUI.openNewSpell = true;
                    EventManager.EmitEvent(GamePlayEvent.OnEarnSpell);
                }
                else
                {
                    var spellUpgradeData = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>()
                        .GetSpellDataUpgrade(spell.ItemId, spell.Level + 1);
                    if (((SpellItem) spell).GetFragments() >= spellUpgradeData.reqFragment)
                        OtherUserData.AddSpellCanUpgrade(spell.ItemId);
                }
            }

            GlobalResources.Add(ResourceType.Inventory, inventoryData);

            if (isSave)
                Save();

            try
            {
                // tracking
                var itemId = inventoryData.ItemId;

                if (inventoryData.InventoryType == InventoryType.Rune)
                    itemId = ((RuneItem) inventoryData).RuneIdConvert;

                FirebaseLogic.Instance.LogResource("inventory",
                    Reward.GetCategoryInventory(inventoryData.InventoryType), itemId.ToString(),
                    Reward.GetCategoryInventory(inventoryData.InventoryType), inventoryData.Quantity,
                    GetInventory(inventoryData.InventoryType, inventoryData.ItemId).Quantity, source, sourceId,
                    isPurchase);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public void UpdateInventory(int inventoryType, ItemInventoryBase inventoryData, string source, bool isPurchase,
            bool isSave = true)
        {
            GlobalResources.UpdateInventoryData(inventoryType, inventoryData);

            if (isSave)
                Save();

            try
            {
                // tracking
                var itemId = inventoryData.ItemId;

                if (inventoryData.InventoryType == InventoryType.Rune)
                    itemId = ((RuneItem) inventoryData).RuneIdConvert;
                
                FirebaseLogic.Instance.LogResource("inventory",
                    Reward.GetCategoryInventory(inventoryData.InventoryType), itemId.ToString(),
                    Reward.GetCategoryInventory(inventoryData.InventoryType), inventoryData.Quantity,
                    GetInventory(inventoryData.InventoryType, inventoryData.ItemId).Quantity, source, "", isPurchase);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public ItemInventoryBase GetInventory(int inventoryType, int itemId)
        {
            return GlobalResources.GetItemInventory(inventoryType, itemId);
        }

        public List<SpellItem> GetListSpell()
        {
            var resource = GlobalResources.GetListSpell();
            if (resource.Count > 0)
            {
                var listConvert = new List<SpellItem>();
                foreach (var spell in resource)
                {
                    listConvert.Add((SpellItem) spell);
                }

                return listConvert;
            }

            return new List<SpellItem>();
        }

        public List<RuneItem> GetListRune()
        {
            var resource = GlobalResources.GetListRune();
            if (resource.Count > 0)
            {
                var listConvert = new List<RuneItem>();
                foreach (var spell in resource)
                {
                    listConvert.Add((RuneItem) spell);
                }

                return listConvert;
            }

            return new List<RuneItem>();
        }

        public void RemoveRune(ItemInventoryBase runeItem, string source, string sourceId, bool isSave = true)
        {
            try
            {
                FirebaseLogic.Instance.LogResource("inventory",
                    Reward.GetCategoryInventory(runeItem.InventoryType), runeItem.ItemId.ToString(),
                    Reward.GetCategoryInventory(runeItem.InventoryType), runeItem.Quantity,
                    GetInventory(runeItem.InventoryType, runeItem.ItemId).Quantity, source, "", false);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                //throw;
            }

            GlobalResources.Sub(ResourceType.Inventory, runeItem);

            if (isSave)
                Save();
        }

        public void RemoveRunes(List<ItemInventoryBase> runeItems, string source, string sourceId, bool isSave = true)
        {
            foreach (var runeItem in runeItems)
            {
                RemoveRune(runeItem, source, sourceId);
            }
        }

        public void SetCampaignStar(int campaignId, int star)
        {
            CampaignData.SetStar(campaignId, star);

            Save();
        }

        public void SetTimeStartRegenStamina(long timeSecond)
        {
            RegenStamina.timeStart = timeSecond;

            Save();
        }

        public void UpdateTimeFullRegenStamina(long timeFull)
        {
            RegenStamina.timeRegenFullSeconds = timeFull;

            Save();
        }

        public void UpdateNextTimeRegenStamina(long timeNext)
        {
            RegenStamina.nextTimeRegenSeconds = timeNext;

            Save();
        }

        public void StopRegenStamina()
        {
            RegenStamina.ResetRegen();

            Save();
        }

        public void BackUpStars()
        {
            if (!BackUpData.isBackedUpStar)
            {
                BackUpData.isBackedUpStar = true;

                var currentStar = GlobalResources.GetMoney(MoneyType.SliverStar);
                var stars = CampaignData.CampaignDict.Values.Sum();
                var addStars = currentStar > stars ? stars : stars - currentStar;
                AddMoney(MoneyType.SliverStar, addStars, AnalyticsConstants.SourceCampaign, "", false);
            }
        }

        public void Save()
        {
            var data = JsonConvert.SerializeObject(this, _jsonSerializerSettings);

            _myFile.Add(Key, data);

            _myFile.Save(Password);
        }

        private static void Load()
        {
            var data = _myFile.GetString(Key, string.Empty);
            if (data.Length > 0)
            {
                _instance = JsonConvert.DeserializeObject<UserData>(data, _jsonSerializerSettings);
            }
        }

        public void SetData(UserData userData)
        {
            Instance = userData;
            Save();
        }

        /// <summary>
        /// set a stage
        /// </summary>
        public void SetAsPlayed(int stage)
        {
            CampaignData.SetAsPlayed(stage);
            Save();
        }

        public void UpgradeTower(int towerId)
        {
            UserTowerData.Upgrade(towerId);
            Debug.Log(UserTowerData.towerStats.Count);
            Save();
        }

        public bool ResetUserTowerData(int towerCost)
        {
            var currentMoney = GlobalResources.GetMoney(MoneyType.Diamond);
            if (currentMoney < towerCost)
            {
                return false;
            }

            SubMoney(MoneyType.Diamond, towerCost, AnalyticsConstants.SourceTowerUpgrade, "", false);
            UserTowerData.ResetUserTowerData();
            Save();
            return true;
        }

        public void SetFirstPurchase(UnityAction callback)
        {
            var success =  UserEventData.FirstPurchase.SetFirstPurchase(TimeManager.NowUtc);
            if (success)
            {
                Save();
                callback?.Invoke();
            }
        }

        public void SetClaimedFirstPurchase()
        {
            UserEventData.FirstPurchase.SetClaimed();
            Save();
        }
        
    }
}