using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using Zitga.TrackingFirebase;

namespace EW2
{
    [System.Serializable]
    public class HeroItem
    {
        public int heroId;

        public int level;

        public int exp;

        public int skillPoint;

        public bool unlock;

        public int[] levelSkills;

        public int spellId;

        public int[] runeEquips;

        public Dictionary<int, RuneSetItem> dictRuneSets;

        public Dictionary<int, int> countSetRune;

        public HeroItem(int id)
        {
            heroId = id;

            level = 1;

            exp = 0;

            skillPoint = 0;

            levelSkills = new[] {1, 0, 0, 0};

            unlock = false;

            spellId = -1;

            runeEquips = new[] {-1, -1, -1, -1, -1, -1};

            dictRuneSets = new Dictionary<int, RuneSetItem>();

            countSetRune = new Dictionary<int, int>();
        }

        public void SetLevel(int level)
        {
            this.level = Mathf.Clamp(level, 1, GameConfig.HeroLevelMax);
        }

        public void SetExp(int exp)
        {
            var heroDataBase = GameContainer.Instance.GetHeroData(heroId);
            int currentLevel = level;

            while (currentLevel < GameConfig.HeroLevelMax)
            {
                var expMax = (int) heroDataBase.stats[currentLevel].maxExp;
                if (exp >= expMax)
                {
                    exp = exp - expMax;
                    skillPoint++;
                    currentLevel++;
                    FirebaseLogic.Instance.LogHeroStatChange(heroId);
                }
                else
                {
                    this.exp = exp;
                    break;
                }
            }

            if (currentLevel > level)
            {
                SetLevel(currentLevel);
                EventManager.EmitEventData(GamePlayEvent.OnHeroLevelUp, heroId);
            }
        }

        public void SetLevelSkill(int skillIndex, int level)
        {
            this.levelSkills[skillIndex] = level;
        }

        public void SetSkillPoint(int skillPoint)
        {
            this.skillPoint = skillPoint;
        }

        public bool IsCanResetSkill()
        {
            var check = false;

            for (int i = 0; i < levelSkills.Length; i++)
            {
                if (i == 0)
                {
                    if (levelSkills[i] > 1) check = true;
                }
                else
                {
                    if (levelSkills[i] > 0) check = true;
                }
            }

            return check;
        }

        public bool IsLevelMax()
        {
            return level >= GameConfig.HeroLevelMax;
        }

        public void ResetAllSkill()
        {
            var numberSkillPoint = 0;

            for (int i = 0; i < levelSkills.Length; i++)
            {
                numberSkillPoint += GetSkilPointRevert(levelSkills[i]);

                if (i == 0)
                    levelSkills[i] = 1;
                else
                    levelSkills[i] = 0;
            }

            skillPoint += (numberSkillPoint - 1);
        }

        private int GetSkilPointRevert(int leveSkill)
        {
            var totalSkillPoint = 0;

            var dataSkill = GameContainer.Instance.GetHeroSkillUpgrade();

            for (int i = 0; i < leveSkill; i++)
            {
                totalSkillPoint += dataSkill.skillUpgradeDatas[i].cost;
            }

            return totalSkillPoint;
        }

        #region Spell

        public void EquipSpell(int spellId)
        {
            this.spellId = spellId;
        }

        public void UnequipSpell()
        {
            this.spellId = -1;
        }

        public int GetSpellUsed()
        {
            return spellId;
        }

        public bool CheckSpellUsed(int spellId)
        {
            return this.spellId == spellId;
        }

        #endregion

        #region Rune

        public void EquipRune(RuneItem runeItem)
        {
            var dataCompare = InventoryDataBase.GetRuneId(runeItem.RuneIdConvert);
            if (countSetRune.ContainsKey(dataCompare.Item1))
                countSetRune[dataCompare.Item1]++;
            else
                countSetRune[dataCompare.Item1] = 1;

            for (int i = 0; i < runeEquips.Length; i++)
            {
                if (runeEquips[i] < 0)
                {
                    runeEquips[i] = runeItem.ItemId;
                    break;
                }
            }

            CheckSetRune();
        }

        public void UnequipRune(RuneItem runeItem)
        {
            var dataCompare = InventoryDataBase.GetRuneId(runeItem.RuneIdConvert);
            if (countSetRune.ContainsKey(dataCompare.Item1))
            {
                countSetRune[dataCompare.Item1]--;

                if (countSetRune[dataCompare.Item1] <= 0)
                    countSetRune.Remove(dataCompare.Item1);
            }

            for (int i = 0; i < runeEquips.Length; i++)
            {
                if (runeEquips[i] == runeItem.ItemId)
                {
                    runeEquips[i] = -1;
                }
            }

            CheckSetRune();
        }

        public int GetRuneBySlot(int slotId)
        {
            if (slotId < runeEquips.Length)
                return runeEquips[slotId];
            return -1;
        }

        public bool CanEquipRune()
        {
            var check = false;
            foreach (var runeEquip in runeEquips)
            {
                if (runeEquip < 0)
                {
                    check = true;
                    break;
                }
            }

            return check;
        }

        private void CheckSetRune()
        {
            foreach (var setRune in countSetRune)
            {
                if (setRune.Value >= (int) RuneSet.RuneSet2 && setRune.Value < (int) RuneSet.RuneSet4)
                {
                    var runeSetItem = new RuneSetItem(setRune.Key, (int) RuneSet.RuneSet2);
                    AddRuneSetItem(runeSetItem);
                }
                else if (setRune.Value >= (int) RuneSet.RuneSet4 && setRune.Value < (int) RuneSet.RuneSet6)
                {
                    var runeSetItem = new RuneSetItem(setRune.Key, (int) RuneSet.RuneSet4);
                    AddRuneSetItem(runeSetItem);
                }
                else if (setRune.Value >= (int) RuneSet.RuneSet6)
                {
                    var runeSetItem = new RuneSetItem(setRune.Key, (int) RuneSet.RuneSet6);
                    AddRuneSetItem(runeSetItem);
                }
                else
                {
                    if (dictRuneSets.ContainsKey(setRune.Key))
                        dictRuneSets.Remove(setRune.Key);
                }
            }
        }

        private void AddRuneSetItem(RuneSetItem runeSetItem)
        {
            if (dictRuneSets.ContainsKey(runeSetItem.Id))
                dictRuneSets[runeSetItem.Id] = runeSetItem;
            else
                dictRuneSets.Add(runeSetItem.Id, runeSetItem);
        }

        public int GetRuneSetType(RuneItem runeItem)
        {
            var dataCompare = InventoryDataBase.GetRuneId(runeItem.RuneIdConvert);
            if (dictRuneSets.ContainsKey(dataCompare.Item1))
                return dictRuneSets[dataCompare.Item1].RuneSet;

            return 0;
        }

        #endregion
    }
}