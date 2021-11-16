using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class Tower2002Skill1 : TowerSkill1
    {
        private Tower2002 control;
        private TowerData2002 dataTower;
        private TowerData2002.Skill1 dataSkill;
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
            dataSkill = dataTower.GetDataSkill1ByLevel(Level);
            if (dataSkill != null)
                timeCooldown = dataSkill.cooldown;
        }

        public override void ActiveSkill(Action callback)
        {
            if (!CheckUnlocked()) return;

            callbackActiveDone = callback;

            control.Soldier.UsePassive1();

            if (coroutine != null)
                CoroutineUtils.Instance.StopCoroutine(coroutine);

            coroutine = CoroutineUtils.Instance.StartCoroutine(IAttackTarget());
        }

        private IEnumerator IAttackTarget()
        {
            yield return new WaitForSeconds(1);

            var targets = new List<EnemyBase>();

            int magicBall = 0;
            if (control.towerData.BonusStat.level < 5)
            {
                magicBall = dataSkill.numberMagicBall;
            }
            else
            {
                magicBall = dataSkill.numberMagicBall + control.towerData.BonusStat.upgradedBall;
            }
            
            for (int i = 0; i < magicBall; i++)
            {
                var targetAttack = control.SearchTarget.SelectTargetDifferent(targets);

                if (targetAttack)
                {
                    targets.Add(targetAttack);

                    SpawnBall(targetAttack, dataSkill.damagePerShot);
                }

                yield return new WaitForSeconds(dataSkill.delayAttack);
            }

            callbackActiveDone?.Invoke();

            StartCooldown();
        }

        private void SpawnBall(Unit target, float damage)
        {
            var ball = ResourceUtils.GetVfxTower("2002_skill_1.1_impact", target.transform.position,
                Quaternion.identity);

            var damageBox = ball.GetComponent<Tower2002Skill1DamageBox>();

            damageBox.InitDamage(damage, control, target);
        }
    }
}