using System;
using EW2.Spell;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class ItemInfoWindowProperties : WindowProperties
    {
        public ResourceType resourceType;
        public int itemId;
        public int itemType;
        public bool isShard;
        public int numberItem;

        public ItemInfoWindowProperties(ResourceType resourceType, int id, int itemType, int number, bool shard = false)
        {
            this.resourceType = resourceType;
            this.itemId = id;
            this.itemType = itemType;
            this.isShard = shard;
            this.numberItem = number;
        }
    }

    public class ItemInfoWindownController : AWindowController<ItemInfoWindowProperties>
    {
        [SerializeField] private Text txtName;

        [SerializeField] private Text txtType;

        [SerializeField] private Text txtDesc;

        [SerializeField] private Transform rootReward;

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            ShowUi();
        }

        private void ShowUi()
        {
            ShowItem();
            txtName.text = GetName();
            txtName.color = Ultilities.GetColorRarity(GetRarityItem());
            var desc = GetDesc();
            txtDesc.gameObject.SetActive(desc.Length > 0);
            txtDesc.text = desc;
            txtType.text = GetTypeResource();
        }

        private string GetTypeResource()
        {
            return Localization.Current.Get("currency_type", $"name_type_{(int)Properties.resourceType}");
        }

        private int GetRarityItem()
        {
            if (Properties.resourceType == ResourceType.Money)
            {
                return Ultilities.GetRarityMoney(Properties.itemId);
            }
            else if (Properties.resourceType == ResourceType.Inventory)
            {
                if (Properties.itemType == InventoryType.Spell)
                {
                    return GetRaritySpell(Properties.itemId);
                }
                else if (Properties.itemType == InventoryType.SpellFragment)
                {
                    return Properties.itemId;
                }
                else if (Properties.itemType == InventoryType.Rune)
                {
                    var dataCompare = InventoryDataBase.GetRuneId(Properties.itemId);
                    return dataCompare.Item2;
                }
                else if (Properties.itemType == InventoryType.RandomRune0 ||
                         Properties.itemType == InventoryType.RandomRune1 ||
                         Properties.itemType == InventoryType.RandomRune2 ||
                         Properties.itemType == InventoryType.RandomRune3 ||
                         Properties.itemType == InventoryType.RandomRune4)
                {
                    switch (Properties.itemType)
                    {
                        case InventoryType.RandomRune0:
                            return 0;
                        case InventoryType.RandomRune1:
                            return 1;
                        case InventoryType.RandomRune2:
                            return 2;
                        case InventoryType.RandomRune3:
                            return 3;
                        case InventoryType.RandomRune4:
                            return 4;
                        default:
                            return 0;
                    }
                }
                else if (Properties.itemType == InventoryType.SpellSpecial)
                {
                    return 2;
                }
                else if (Properties.itemType == InventoryType.RandomSpell0 ||
                         Properties.itemType == InventoryType.RandomSpell1 ||
                         Properties.itemType == InventoryType.RandomSpell2 ||
                         Properties.itemType == InventoryType.RandomSpell3 ||
                         Properties.itemType == InventoryType.RandomSpell4)
                {
                    switch (Properties.itemType)
                    {
                        case InventoryType.RandomSpell0:
                            return 0;
                        case InventoryType.RandomSpell1:
                            return 1;
                        case InventoryType.RandomSpell2:
                            return 2;
                        case InventoryType.RandomSpell3:
                            return 3;
                        case InventoryType.RandomSpell4:
                            return 4;
                        default:
                            return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private void ShowItem()
        {
            if (Properties.resourceType == ResourceType.Money || Properties.resourceType == ResourceType.Hero)
            {
                var reward = Reward.Create(Properties.resourceType, Properties.itemId, 0);
                SpawnReward(reward);
            }
            else if (Properties.resourceType == ResourceType.Inventory)
            {
                var reward = Reward.CreateInventory(Properties.resourceType, Properties.itemType, Properties.itemId, 0);
                SpawnReward(reward);
            }
        }

        private void SpawnReward(Reward reward)
        {
            var rewardUi = ResourceUtils.GetRewardUi(reward.type);

            rewardUi.SetData(reward);

            rewardUi.SetParent(rootReward);
        }

        private string GetName()
        {
            if (Properties.resourceType == ResourceType.Money)
            {
                return Localization.Current.Get("currency_type", $"currency_{Properties.itemId}");
            }
            else if (Properties.resourceType == ResourceType.Inventory)
            {
                if (Properties.itemType == InventoryType.Spell)
                {
                    if (!Properties.isShard)
                    {
                        return Ultilities.GetNameSpell(Properties.itemId);
                    }
                    else
                    {
                        var nameShard = string.Format(L.gameplay.specific_shard_name,
                            Ultilities.GetNameSpell(Properties.itemId));
                        return nameShard;
                    }
                }
                else if (Properties.itemType == InventoryType.SpellFragment)
                {
                    var nameShard = string.Format(L.gameplay.specific_shard_name,
                        Localization.Current.Get("gameplay", $"rarity_{Properties.itemId}"));
                    return nameShard;
                }
                else if (Properties.itemType == InventoryType.Rune)
                {
                    return Ultilities.GetNameRune(Properties.itemId);
                }
                else if (Properties.itemType == InventoryType.RandomRune0 ||
                         Properties.itemType == InventoryType.RandomRune1 ||
                         Properties.itemType == InventoryType.RandomRune2 ||
                         Properties.itemType == InventoryType.RandomRune3 ||
                         Properties.itemType == InventoryType.RandomRune4)
                {
                    var rarity = GetRarityItem();
                    var nameRarity = Localization.Current.Get("gameplay", $"rarity_{rarity}");
                    return string.Format(L.currency_type.inventory_random_rune, nameRarity);
                }
                else if (Properties.itemType == InventoryType.SpellSpecial ||
                         Properties.itemType == InventoryType.RandomSpell0 ||
                         Properties.itemType == InventoryType.RandomSpell1 ||
                         Properties.itemType == InventoryType.RandomSpell2 ||
                         Properties.itemType == InventoryType.RandomSpell3 ||
                         Properties.itemType == InventoryType.RandomSpell4)
                {
                    return L.currency_type.inventory_spell_pack_random;
                }
            }
            else if (Properties.resourceType == ResourceType.Hero)
            {
                return Ultilities.GetNameHero(Properties.itemId);
            }

            return "";
        }

        private string GetDesc()
        {
            if (Properties.resourceType == ResourceType.Money)
            {
                return Localization.Current.Get("currency_type", $"currency_{Properties.itemId}_des");
            }
            else if (Properties.resourceType == ResourceType.Inventory)
            {
                if (Properties.itemType == InventoryType.Spell)
                {
                    if (!Properties.isShard)
                    {
                        var spellId = 0;
                        if (Properties.itemId < 40000)
                        {
                            spellId = Properties.itemId;
                        }
                        else
                        {
                            var tailNumb = Properties.itemId - 40000;
                            spellId = 4000 + tailNumb / 10;
                        }

                        return Localization.Current.Get("spell", $"spell_default_description_{spellId}");
                    }
                    else
                    {
                        var nameSpell = Ultilities.GetNameSpell(Properties.itemId);
                        var desc = string.Format(L.currency_type.inventory_unique_spell_fragment_des, nameSpell);
                        return desc;
                    }
                }
                else if (Properties.itemType == InventoryType.SpellFragment)
                {
                    return Localization.Current.Get("currency_type", "inventory_spell_fragment_des");
                }
                else if (Properties.itemType == InventoryType.Rune)
                {
                    return Localization.Current.Get("rune",
                        $"rune_detail_des_{InventoryDataBase.GetRuneId(Properties.itemId).Item1}");
                }
                else if (Properties.itemType == InventoryType.RandomRune0 ||
                         Properties.itemType == InventoryType.RandomRune1 ||
                         Properties.itemType == InventoryType.RandomRune2 ||
                         Properties.itemType == InventoryType.RandomRune3 ||
                         Properties.itemType == InventoryType.RandomRune4)
                {
                    var rarity = GetRarityItem();
                    var nameRarity = Localization.Current.Get("gameplay", $"rarity_{rarity}");
                    var descLocalization = L.currency_type.inventory_rune_pack_random_des;
                    var desc = string.Format(descLocalization, Properties.numberItem, nameRarity);
                    return desc;
                }
                else if (Properties.itemType == InventoryType.SpellSpecial)
                {
                    return string.Format(L.currency_type.inventory_spell_pack_random_des, "3");
                }
                else if (Properties.itemType == InventoryType.RandomSpell0 ||
                         Properties.itemType == InventoryType.RandomSpell1 ||
                         Properties.itemType == InventoryType.RandomSpell2 ||
                         Properties.itemType == InventoryType.RandomSpell3 ||
                         Properties.itemType == InventoryType.RandomSpell4)
                {
                    var rarity = GetRarityItem();
                    var nameRarity = Localization.Current.Get("gameplay", $"rarity_{rarity}");
                    var descLocalization = L.currency_type.inventory_spell_pack_random_des_1;
                    var desc = string.Format(descLocalization, Properties.numberItem, nameRarity);
                    return desc;
                }
                else
                {
                    return "";
                }
            }

            return "";
        }

        private int GetRaritySpell(int idSpell)
        {
            var db = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>().GetSpellDataBase(idSpell);
            if (db != null)
                return db.spellStatDatas.rarity;
            return 0;
        }
    }
}