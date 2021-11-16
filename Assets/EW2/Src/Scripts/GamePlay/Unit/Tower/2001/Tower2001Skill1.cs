using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class Tower2001Skill1 : TowerSkill1
    {
        private Tower2001 control;

        private TowerData2001 dataTower2001;

        private TowerData2001.Skill1 dataSkill;

        private Action callbackActiveDone;

        private Coroutine coroutine;

        public override void Init(Unit owner, BranchType branchType, SkillType skillType, TowerData data)
        {
            this.owner = owner;
            this.control = owner as Tower2001;
            this.branchType = branchType;
            this.skillType = skillType;
            this.dataTower2001 = data as TowerData2001;
        }

        public override void UpdateData()
        {
            dataSkill = dataTower2001.GetDataSkill1ByLevel(Level);
            if (dataSkill != null)
                timeCooldown = dataSkill.cooldown;
        }

        public override void ActiveSkill(Action callback)
        {
            if (!CheckUnlocked()) return;

            callbackActiveDone = callback;

            AttackTarget();
        }

        private void AttackTarget()
        {
            var targetAttack = control.SearchTarget.SelectTargetHpHighest();

            if (targetAttack != null)
            {
                control.SoldierSelected.AttackSkill(targetAttack, dataSkill, () =>
                {
                    callbackActiveDone?.Invoke();

                    StartCooldown();
                });
            }
        }
    }
}