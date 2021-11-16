using System;
using System.Collections;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class Tower2002Skill2 : TowerSkill2
    {
        private Tower2002 control;
        private TowerData2002 dataTower;
        private TowerData2002.Skill2 dataSkill;
        private Action callbackActiveDone;
        private Coroutine coroutine;

        public override void Init(Unit owner, BranchType branchType, SkillType skillType, TowerData data)
        {
            this.owner = owner;
            this.control = owner as Tower2002;
            this.branchType = branchType;
            this.skillType = skillType;
            this.dataTower = data as TowerData2002;
        }

        public override void UpdateData()
        {
            dataSkill = dataTower.GetDataSkill2ByLevel(Level);
            if (dataSkill != null)
                timeCooldown = dataSkill.cooldown;
        }

        public override void ActiveSkill(Action callback)
        {
            if (!CheckUnlocked()) return;

            callbackActiveDone = callback;

            control.Soldier.UsePassive2();

            control.Soldier.SpawnHolePassive2(dataSkill.numberMaxTarget, dataSkill.timeLife);

            if (coroutine != null)
                CoroutineUtils.Instance.StopCoroutine(coroutine);

            coroutine = CoroutineUtils.Instance.StartCoroutine(IAttackTarget());
        }

        private IEnumerator IAttackTarget()
        {
            yield return new WaitForSeconds(1);

            callbackActiveDone?.Invoke();

            StartCooldown();
        }
    }
}