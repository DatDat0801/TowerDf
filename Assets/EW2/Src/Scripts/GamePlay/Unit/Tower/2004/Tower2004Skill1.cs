using System.Collections;
using UnityEngine;

namespace EW2
{
    public class Tower2004Skill1 : TowerSkill1
    {
        private Tower2004 control;
        private TowerData2004 dataTower;
        private TowerData2004.Skill1 dataSkill;

        public override void Init(Unit owner, BranchType branchType, SkillType skillType, TowerData data)
        {
            this.owner = owner;
            this.branchType = branchType;
            this.skillType = skillType;
            this.dataTower = data as TowerData2004;
            this.control = owner as Tower2004;
        }

        protected override void OnRaiseSuccess()
        {
            CoroutineUtils.Instance.StartCoroutine(SetOnRaiseToSoldier());
        }

        public override void UpdateData()
        {
            dataSkill = dataTower.GetDataSkill1ByLevel(Level);
        }

        private IEnumerator SetOnRaiseToSoldier()
        {
            for (int i = 0; i < control.Soldiers.Count; i++)
            {
                var soldier = control.Soldiers[i] as Soldier2004;
                if (soldier != null)
                {
                    soldier.OnRaiseSkillCounter(dataSkill);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}