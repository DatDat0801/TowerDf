using Hellmade.Sound;
using Spine.Unity;
using UnityEngine;


namespace EW2
{
    [System.Serializable]
    public class Skill1Enemy3005 : SkillEnemy
    {
        [SerializeField][SpineSkin()] private string normalSkin;
        [SerializeField][SpineSkin()] private string rageSkin;
        
        private EnemyData3005.EnemyData3005Skill1 skill1Data;
        private bool checkAppliedRage;
        private Enemy3005 enemy;

        public override void CastSkill()
        {
            if (CanRage())
            {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.ENEMY_3005_RAGE_ACTIVATE);
                EazySoundManager.PlaySound(audioClip);

                ApplyEffectRage();
            }
        }

        public float GetPercentHpCanApply() => skill1Data.percentHp;

        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);
            checkAppliedRage = false;
            skill1Data = skill1Data ?? GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3005>().GetSkill1(enemyBase.Level);
            enemy = (Enemy3005)enemyBase;
            ChangeNormalSkin();
        }
        

        private bool CanRage()
        {
            return !enemy.IsPhase1() && !checkAppliedRage && enemy.IsAlive && enemy.StatusController.CanUseSkill();
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
                lifeTime = skill1Data.secondApply,
                intervalTime = skill1Data.secondApply
            };
            var modifierAttackSpeed = new RPGStatModifier(attackSpeedAttribute, skill1Data.modifierTypeAttackSpeed,
                skill1Data.percentIncreaseAttackSpeed, false, enemy, enemy);
            var modifierAttackSpeedOverTime = new ModifierStatOverTime(statusOverTimeConfig,modifierAttackSpeed);
            var modifierDamage = new RPGStatModifier(damageSpeedAttribute, skill1Data.modifierTypeDamage,
                skill1Data.percentIncreaseDamage, false, enemy, enemy);
            var modifierDamageOverTime = new ModifierStatOverTime(statusOverTimeConfig,modifierDamage);
            modifierDamageOverTime.OnCompleted = CompleteRage;
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