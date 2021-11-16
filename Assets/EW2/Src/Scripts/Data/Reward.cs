using System;
using System.Collections.Generic;
using System.Linq;
using EW2.Spell;
using TigerForge;
using Random = UnityEngine.Random;

namespace EW2
{
    public class RewardContainer
    {
        public Reward[] items;
    }

    [System.Serializable]
    public class Reward : IEquatable<Reward>
    {
        public ResourceType type;
        public int id;
        public int number;
        public int itemType;

        public virtual void AddToUserData(bool isSave = true, string source = "", string sourceId = "",
            bool isPurchase = false)
        {
            if (number <= 0)
                return;

            switch (type)
            {
                case ResourceType.Money:
                    UserData.Instance.AddMoney(id, number, source, sourceId, isPurchase, isSave);
                    break;
                case ResourceType.Hero:
                    if (id == MoneyType.SkillPoint)
                    {
                        if (UserData.Instance.UserHeroData.CheckHeroUnlocked(this.itemType))
                        {
                            var heroData = UserData.Instance.UserHeroData.GetHeroById(this.itemType);
                            heroData.SetSkillPoint(this.number);
                            UserData.Instance.Save();
                        }
                    }
                    else
                    {
                        UserData.Instance.UserHeroData.AddHero(id, number);
                    }

                    break;
                case ResourceType.Inventory:
                    if (itemType == InventoryType.Spell)
                    {
                        var spellItem = new SpellItem(id, number, itemType);
                        UserData.Instance.AddInventory(spellItem, source, sourceId, isPurchase);
                    }
                    else if (itemType == InventoryType.SpellFragment)
                    {
                        var spellFragment = new SpellFragmentItem(id, number, itemType);
                        UserData.Instance.AddInventory(spellFragment, source, sourceId, isPurchase);
                    }
                    else if (itemType == InventoryType.Rune)
                    {
                        var runeItem = new RuneItem(id, number, itemType);
                        UserData.Instance.AddInventory(runeItem, source, sourceId, isPurchase);
                    }

                    break;
            }
        }

        public static Reward Create(ResourceType type, int id, int number)
        {
            return new Reward {type = type, id = id, number = number};
        }

        public static Reward CreateInventory(ResourceType type, int itemType, int id, int number)
        {
            return new Reward {type = type, id = id, number = number, itemType = itemType};
        }

        public static void AddToUserData(Reward[] rewards, string source = "", string sourceId = "",
            bool isPurchase = false)
        {
            foreach (var reward in rewards)
            {
                reward.AddToUserData(false, source, sourceId, isPurchase);
            }

            UserData.Instance.Save();
        }

        public static Reward[] MergeRewards(params Reward[][] a)
        {
            var newArray = a.SelectMany(y => y);

            var results = newArray.GroupBy(p => new Reward() {id = p.id, type = p.type, itemType = p.itemType})
                .Select(g => new Reward() {
                    id = g.Key.id, type = g.Key.type, itemType = g.Key.itemType, number = g.Sum(p => p.number)
                }).ToArray();
            return results;
        }

        public static string GetCategory(int moneyType)
        {
            switch (moneyType)
            {
                case MoneyType.Diamond:
                    return "diamond";
                case MoneyType.Crystal:
                    return "crystal";
                case MoneyType.Exp:
                    return "exp";
                case MoneyType.Stamina:
                    return "stamina";
                case MoneyType.SliverStar:
                    return "sliver_star";
                case MoneyType.GoldStar:
                    return "gold_star";
                case MoneyType.KeyRuneBasic:
                    return "key_rune_basic";
                case MoneyType.KeyRunePremium:
                    return "key_rune_premium";
                case MoneyType.KeySpellBasic:
                    return "key_spell_basic";
                case MoneyType.KeySpellPremium:
                    return "key_spell_premium";
                case MoneyType.ExpRune:
                    return "exp_rune";
                case MoneyType.QuestActivityPoint:
                    return "quest_activity_point";
                case MoneyType.GloryRoadPoint:
                    return "glory_road_point";
                case MoneyType.TournamentTicket:
                    return "tournament_ticket";

                default: return "none";
            }
        }

        public static string GetCategoryInventory(int inventoryType)
        {
            switch (inventoryType)
            {
                case InventoryType.Rune:
                    return "rune";
                case InventoryType.Spell:
                    return "spell";
                case InventoryType.SpellFragment:
                    return "spell_fragment";
                default: return "none";
            }
        }

        public static string GetItemCategory(int type)
        {
            switch (type)
            {
                case (int)ResourceType.Money:
                    return "money";
                case (int)ResourceType.MoneyInGame:
                    return "money_ingame";
                case (int)ResourceType.Hero:
                    return "hero";
                case (int)ResourceType.Inventory:
                    return "inventory";

                default: return "none";
            }
        }

        public static Reward GetRuneRandom(int rarity)
        {
            var runeIdRandom = Random.Range(0, 10);
            while (runeIdRandom == (int)RuneId.LifeRune || runeIdRandom == (int)RuneId.ArgonyRune ||
                   runeIdRandom == (int)RuneId.MiseryRune)
            {
                runeIdRandom = Random.Range(0, 10);
            }

            var runeIdConvert = InventoryDataBase.GetRuneIdConvert(runeIdRandom, rarity);
            var rune = CreateInventory(ResourceType.Inventory, InventoryType.Rune, runeIdConvert, 1);
            return rune;
        }

        public static Reward[] GenRewards(Reward[] rewards)
        {
            var listReward = new List<Reward>();

            foreach (var reward in rewards)
            {
                if (reward.type == ResourceType.Inventory)
                {
                    if (reward.itemType == InventoryType.RandomRune0 || reward.itemType == InventoryType.RandomRune1 ||
                        reward.itemType == InventoryType.RandomRune2 || reward.itemType == InventoryType.RandomRune3 ||
                        reward.itemType == InventoryType.RandomRune4)
                    {
                        var rarity = 0;

                        switch (reward.itemType)
                        {
                            case InventoryType.RandomRune0:
                                rarity = 0;
                                break;
                            case InventoryType.RandomRune1:
                                rarity = 1;
                                break;
                            case InventoryType.RandomRune2:
                                rarity = 2;
                                break;
                            case InventoryType.RandomRune3:
                                rarity = 3;
                                break;
                            case InventoryType.RandomRune4:
                                rarity = 4;
                                break;
                        }

                        for (int i = 0; i < reward.number; i++)
                        {
                            var runeItem = Reward.GetRuneRandom(rarity);
                            listReward.Add(runeItem);
                        }
                    }
                    else if (reward.itemType == InventoryType.SpellSpecial)
                    {
                        var spell3003 = CreateSpellReward(4003);
                        listReward.Add(spell3003);
                        var spell3005 = CreateSpellReward(4005);
                        listReward.Add(spell3005);
                        var spell3006 = CreateSpellReward(4006);
                        listReward.Add(spell3006);
                    }
                    else if (reward.itemType == InventoryType.Spell)
                    {
                        var spell = CreateSpellReward(reward.id);
                        listReward.Add(spell);
                    }
                    else if (reward.itemType == InventoryType.SpellFragment)
                    {
                        var fragment = CreateInventory(ResourceType.Inventory, InventoryType.SpellFragment, reward.id,
                            reward.number);
                        listReward.Add(fragment);
                    }
                    else if (reward.itemType == InventoryType.Rune)
                    {
                        listReward.Add(reward);
                    }
                    else if (reward.itemType == InventoryType.RandomSpell0 ||
                             reward.itemType == InventoryType.RandomSpell1 ||
                             reward.itemType == InventoryType.RandomSpell2 ||
                             reward.itemType == InventoryType.RandomSpell3 ||
                             reward.itemType == InventoryType.RandomSpell4)
                    {
                        var rarity = 0;

                        switch (reward.itemType)
                        {
                            case InventoryType.RandomSpell0:
                                rarity = 0;
                                break;
                            case InventoryType.RandomSpell1:
                                rarity = 1;
                                break;
                            case InventoryType.RandomSpell2:
                                rarity = 2;
                                break;
                            case InventoryType.RandomSpell3:
                                rarity = 3;
                                break;
                            case InventoryType.RandomSpell4:
                                rarity = 4;
                                break;
                        }

                        for (int i = 0; i < reward.number; i++)
                        {
                            var runeItem = GetRewardSpells(rarity, listReward);
                            listReward.Add(runeItem);
                        }
                    }
                    else if (reward.itemType == InventoryType.RandomSpecialRune)
                    {
                        var dataDropRate = GameContainer.Instance.Get<InventoryDataBase>().Get<SpecialRuneDrop>()
                            .specialRuneDrops;
                        var listRate = new List<float>();

                        listRate.Add(dataDropRate[0].rarity2Rate);
                        listRate.Add(dataDropRate[1].rarity2Rate);
                        listRate.Add(dataDropRate[2].rarity2Rate);

                        for (int i = 0; i < reward.number; i++)
                        {
                            var rarity = RandomFromDistribution.RandomChoiceFollowingDistribution(listRate);
                            var runeItem = GetRuneRandom(rarity);
                            listReward.Add(runeItem);
                        }
                    }
                    else if (reward.itemType == InventoryType.RandomSpecialSpell)
                    {
                        var dataDropRate = GameContainer.Instance.Get<InventoryDataBase>().Get<SpecialSpellDrop>()
                            .specialSpellDrops;
                        var listRate = new List<float>();

                        listRate.Add(dataDropRate[0].rarity2Rate);
                        listRate.Add(dataDropRate[1].rarity2Rate);
                        listRate.Add(dataDropRate[2].rarity2Rate);

                        for (int i = 0; i < reward.number; i++)
                        {
                            var rarity = RandomFromDistribution.RandomChoiceFollowingDistribution(listRate);
                            var runeItem = GetRewardSpells(rarity, listReward);
                            listReward.Add(runeItem);
                        }
                    }
                }
                else if (reward.type == ResourceType.GiftCode)
                {
                    var results = HandleRewardGiftCode(reward.id);
                    if (results != null)
                    {
                        listReward.AddRange(results);
                    }
                }
                else
                {
                    listReward.Add(reward);
                }
            }

            return listReward.ToArray();
        }

        public static Reward CreateSpellReward(int spellId)
        {
            var spellDataBases = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>();
            var spellUserData = UserData.Instance.GetInventory(InventoryType.Spell, spellId);
            var gachaSpellCopyRate = GameContainer.Instance.Get<GachaDataBase>().Get<GachaSpellCopyRate>();

            if (spellUserData == null)
            {
                var spell = CreateInventory(ResourceType.Inventory, InventoryType.Spell, spellId, 1);
                return spell;
            }
            else
            {
                var spellData = (SpellItem)spellUserData;

                var levelMax = spellDataBases.GetSpellLevelMax(spellId);

                var fragmentUsed = spellDataBases.GetTotalFragmentUsed(spellData.ItemId, spellData.Level);

                var fragmentToMax = spellDataBases.GetTotalFragmentToLevelMax(spellData.ItemId) - fragmentUsed;

                if (spellData.Level >= levelMax || spellData.GetFragments() >= fragmentToMax)
                {
                    // chuyen thanh fragment chung
                    var number = gachaSpellCopyRate.GetRateConvertCopy(spellId);
                    var spellFragment = CreateInventory(ResourceType.Inventory, InventoryType.SpellFragment, spellId,
                        number);
                    return spellFragment;
                }
                else
                {
                    // them fragment
                    var number = 1;
                    if (UserData.Instance.GetInventory(InventoryType.Spell, spellId) != null)
                        number = gachaSpellCopyRate.GetRateConvertCopy(spellId);

                    var spell = CreateInventory(ResourceType.Inventory, InventoryType.Spell, spellId,
                        number);
                    return spell;
                }
            }
        }

        public bool Equals(Reward other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.type == other.type && this.id == other.id && this.itemType == other.itemType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Reward)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)this.type;
                hashCode = (hashCode * 397) ^ this.id;
                hashCode = (hashCode * 397) ^ this.itemType;
                return hashCode;
            }
        }

        public static Reward[] HandleRewardGiftCode(int giftCodeType)
        {
            switch (giftCodeType)
            {
                case GiftCodeType.UnlockGloryRoad:
                    var userData = UserData.Instance.UserEventData.GloryRoadUser;
                    if (!userData.IsUnlockPremium())
                    {
                        var premiumKey = UserData.Instance.AccountData.tokenId;
                        userData.SetPremiumKey($"{premiumKey}{ShortId.Generate(10)}");
                        UserData.Instance.Save();
                        Ultilities.ShowToastNoti(L.game_event.already_unlock_premium_txt);
                    }

                    return null;
                case GiftCodeType.ReceiveBuyNow:
                    var packData = GameContainer.Instance.Get<ShopDataBase>().Get<BuyNowData>();
                    var shopItemData = packData.shopItemDatas[0];
                    if (!UserData.Instance.UserShopData.CheckPackNonconsumePurchased(shopItemData.productId))
                    {
                        UserData.Instance.UserShopData.AddProductIdNonconsumePurchased(shopItemData.productId);
                    }

                    UserData.Instance.UserEventData.BuyNowUserData.isOpen = false;
                    EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
                    return GenRewards(shopItemData.rewards);
                default: return null;
            }
        }

        public static Reward GetRewardSpells(int indexRarity, List<Reward> listRewards)
        {
            var spellDataBases = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>();
            var dictSpellRandom = spellDataBases.GetDictSpellByRarity();
            var gachaSpellCopyRate = GameContainer.Instance.Get<GachaDataBase>().Get<GachaSpellCopyRate>();
            if (dictSpellRandom != null)
            {
                var isCheckSpellExist = false;
                var listSpells = dictSpellRandom[indexRarity];
                var idSpellResult = listSpells[Random.Range(0, listSpells.Count)];
                var dataSpellResult = UserData.Instance.GetInventory(InventoryType.Spell, idSpellResult);

                if (dataSpellResult != null)
                {
                    isCheckSpellExist = true;
                }

                if (!isCheckSpellExist)
                {
                    foreach (var spell in listRewards)
                    {
                        if (spell.id == idSpellResult)
                        {
                            isCheckSpellExist = true;
                            break;
                        }
                    }
                }

                if (!isCheckSpellExist)
                {
                    var spell = CreateInventory(ResourceType.Inventory, InventoryType.Spell, idSpellResult, 1);
                    return spell;
                }
                else
                {
                    var spellData = dataSpellResult != null
                        ? (SpellItem)dataSpellResult
                        : new SpellItem(idSpellResult, 0, InventoryType.Spell);

                    var levelMax = spellDataBases.GetSpellLevelMax(idSpellResult);

                    var fragmentUsed = spellDataBases.GetTotalFragmentUsed(spellData.ItemId, spellData.Level);

                    var fragmentToMax = spellDataBases.GetTotalFragmentToLevelMax(spellData.ItemId) - fragmentUsed;

                    if (spellData.Level >= levelMax || spellData.GetFragments() >= fragmentToMax)
                    {
                        // chuyen thanh fragment chung
                        var number = gachaSpellCopyRate.GetRateConvertCopy(idSpellResult);
                        var spellFragment = CreateInventory(ResourceType.Inventory, InventoryType.SpellFragment,
                            indexRarity, number);
                        return spellFragment;
                    }
                    else
                    {
                        // them fragment
                        var number = gachaSpellCopyRate.GetRateConvertCopy(idSpellResult);

                        var spell = CreateInventory(ResourceType.Inventory, InventoryType.Spell, idSpellResult,
                            number);
                        return spell;
                    }
                }
            }

            return null;
        }
    }
}