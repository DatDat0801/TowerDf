using System.Collections.Generic;
using Hellmade.Sound;
using Lean.Pool;
using Spine;
using Spine.Unity;
using UnityEngine;
using Event = Spine.Event;


namespace EW2
{
    [System.Serializable]
    public class Skill2Enemy3015 : SkillEnemy
    {
        [SerializeField] [SpineSkin()] private string normalSkin;
        [SerializeField] [SpineSkin()] private string rageSkin;

        private EnemyData3015.EnemyData3015Skill2 skill2Data;
        private bool checkAppliedRage;
        private Enemy3015 enemy;

        public override void CastSkill()
        {
            if (CanRage())
            {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.ENEMY_3015_RAGE_ACTIVATE);
                EazySoundManager.PlaySound(audioClip);
                ApplyEffectRage();
            }
        }

        public float GetPercentHpCanApply() => skill2Data.percentHp;

        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);
            checkAppliedRage = false;
            skill2Data = skill2Data ?? GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3015>()
                .GetSkill2(enemyBase.Level);
            enemy = (Enemy3015) enemyBase;
            ChangeNormalSkin();
        }


        private bool CanRage()
        {
            return enemy.IsPhase1() && !checkAppliedRage && enemy.IsAlive && enemy.StatusController.CanUseSkill();
        }

        private void ApplyEffectRage()
        {
            ChangeRageSkin();
            var attackSpeedAttribute = enemyBase.Stats.GetStat<RPGAttribute>(RPGStatType.AttackSpeed);
            var damageSpeedAttribute = enemyBase.Stats.GetStat<RPGAttribute>(RPGStatType.Damage);
            var statusOverTimeConfig = new StatusOverTimeConfig()
            {
                creator = enemy,
                owner = enemy,
                lifeTime = skill2Data.secondApply,
                intervalTime = skill2Data.secondApply
            };
            var modifierAttackSpeed = new RPGStatModifier(attackSpeedAttribute, skill2Data.modifierTypeAttackSpeed,
                skill2Data.percentIncreaseAttackSpeed, false, enemy, enemy);
            var modifierAttackSpeedOverTime = new ModifierStatOverTime(statusOverTimeConfig, modifierAttackSpeed);
            var modifierDamage = new RPGStatModifier(damageSpeedAttribute, skill2Data.modifierTypeDamage,
                skill2Data.percentIncreaseDamage, false, enemy, enemy);
            var modifierDamageOverTime = new ModifierStatOverTime(statusOverTimeConfig, modifierDamage);
            modifierDamageOverTime.OnCompleted += CompleteRage;
            enemy.StatusController.AddStatus(modifierAttackSpeedOverTime);
            enemy.StatusController.AddStatus(modifierDamageOverTime);
            checkAppliedRage = true;
        }

        private void ChangeRageSkin()
        {
            enemy.UnitSpine.SetSkinSpine(rageSkin);
        }

        private void ChangeNormalSkin()
        {
            enemy.UnitSpine.SetSkinSpine(normalSkin);
        }

        private void CompleteRage()
        {
            ChangeNormalSkin();
        }
    }
}