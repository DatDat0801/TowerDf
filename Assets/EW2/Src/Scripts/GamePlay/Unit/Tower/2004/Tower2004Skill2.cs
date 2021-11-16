using System;
using System.Collections;
using UnityEngine;

namespace EW2
{
    public class Tower2004Skill2 : TowerSkill2
    {
        private Tower2004 control;
        private TowerData2004 dataTower2004;
        private TowerData2004.Skill2 data2004Skill2;

        public override void Init(Unit owner, BranchType branchType, SkillType skillType, TowerData data)
        {
            this.owner = owner;
            this.branchType = branchType;
            this.skillType = skillType;
            this.dataTower2004 = data as TowerData2004;
            this.control = owner as Tower2004;
        }

        public override void UpdateData()
        {
            data2004Skill2 = dataTower2004.GetDataSkill2ByLevel(Level);
        }

        public override void ActiveSkill(Action callback = null)
        {
            var totalArmor = 0f;
            var totalMagicResistance = 0f;
            //Add more by TowerUpgradeSystem
            if (CheckUnlocked())
            {
                totalArmor = data2004Skill2.armorBonus + control.BonusStat.level5Stat.skill2Armor;
                totalMagicResistance = data2004Skill2.magicResBonus + control.BonusStat.level5Stat.skill2MagicResistance;
            }
            else
            {
                totalArmor = data2004Skill2.armorBonus;
                totalMagicResistance = data2004Skill2.magicResBonus;
            }

            for (int i = 0; i < control.Soldiers.Count; i++)
            {
                var soldier = control.Soldiers[i] as Soldier2004;
                if (soldier != null)
                {
                    RPGStatModifier armorPhysical =
                        new RPGStatModifier(new ArmorPhysical(), data2004Skill2.modifierTypeArmor,
                            totalArmor, false,
                            this.owner, soldier);
                    soldier.Stats.AddStatModifier(RPGStatType.Armor, armorPhysical, true);
                    RPGStatModifier armorMagic =
                        new RPGStatModifier(new ArmorMagical(), data2004Skill2.modifierTypeMagicRes,totalMagicResistance
                            ,
                            false,
                            this.owner, soldier);
                    soldier.Stats.AddStatModifier(RPGStatType.Resistance, armorMagic, true);
                }
            }
        }

        protected override void OnRaiseSuccess()
        {
            base.OnRaiseSuccess();

            CoroutineUtils.Instance.StartCoroutine(SetOnRaiseToSoldier());
        }

        private IEnumerator SetOnRaiseToSoldier()
        {
            for (int i = 0; i < control.Soldiers.Count; i++)
            {
                var soldier = control.Soldiers[i] as Soldier2004;
                if (soldier != null)
                {
                    soldier.OnRaiseSkill2();
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}