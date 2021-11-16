using System.Collections.Generic;
using Hellmade.Sound;
using UnityEngine;

namespace EW2
{
    public class Hero1004ActiveImpact : TargetCollection<Dummy>
    {
        private Dictionary<int, ModifierStatOverTime> listSpeedOverTimes = new Dictionary<int, ModifierStatOverTime>();
        private Pet1004 _owner;
        private float _timeLifeStatus;
        private float _damageStatus;
        private Hero1004PassiveSkill1 _passiveSkill1;

        public void InitImpact(Pet1004 hero1004, HeroData1004.ActiveSkill skillData)
        {
            this._owner = hero1004;
            this._timeLifeStatus = skillData.timeLife;
            this._damageStatus = skillData.damagePerSecond;
            this._passiveSkill1 = (Hero1004PassiveSkill1)hero1004.SkillController.GetSkillPassive(0);
            var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_active_skill_pool");
            EazySoundManager.PlaySound(audioClip);
        }

        protected override Dummy GetTarget(Collider2D other)
        {
            if (!other.CompareTag(TagName.Enemy))
                return null;

            var unitCollider = other.GetComponentInParent<Dummy>();

            return unitCollider;
        }

        protected override void FilterTarget(Dummy target)
        {
            if (target == null || target.IsAlive == false)
            {
                Debug.Log("Target is null");
                return;
            }

            if (Targets.Contains(target))
            {
                // do nothing
                Debug.Log("Exist unit: " + target.Transform.name);
            }
            else
            {
                var poison = new PoisonStatus(new StatusOverTimeConfig() {
                    creator = this._owner,
                    owner = target,
                    lifeTime = this._timeLifeStatus,
                    intervalTime = 1,
                    baseValue = this._damageStatus,
                    damageType = this._owner.DamageType,
                    statusType = StatusType.Poison
                });
                poison.Stacks = false;

                target.StatusController.AddStatus(poison);

                // add slow status
                if (this._passiveSkill1 != null && this._passiveSkill1.IsActive())
                {
                    var moveSpeedAttribute = target.Stats.GetStat<RPGAttribute>(RPGStatType.MoveSpeed);

                    var statusOverTimeConfig = new StatusOverTimeConfig() {
                        creator = this._owner,
                        owner = target,
                        lifeTime = this._timeLifeStatus,
                        intervalTime = 0,
                        statusType = StatusType.Slow
                    };

                    var modifierMoveSpeed = new RPGStatModifier(moveSpeedAttribute,
                        this._passiveSkill1.GetDataPassive().modifierType,
                        -this._passiveSkill1.GetDataPassive().ratioReductionSpeed, false, this._owner, target);
                    var modifierMoveSpeedOverTime = new ModifierStatOverTime(statusOverTimeConfig, modifierMoveSpeed);

                    target.StatusController.AddStatus(modifierMoveSpeedOverTime);

                    AddSlowStatus(target, modifierMoveSpeedOverTime);
                }

                Targets.Add(target);
            }
        }

        public override Dummy SelectTarget()
        {
            return default;
        }

        public override void RemoveTarget(Dummy target)
        {
            RemoveSlow(target);
            Targets.Remove(target);
        }

        private void AddSlowStatus(Dummy target, ModifierStatOverTime damageOverTime)
        {
            if (!listSpeedOverTimes.ContainsKey(target.GetInstanceID()))
                listSpeedOverTimes.Add(target.GetInstanceID(), damageOverTime);
        }

        private void RemoveSlow(Dummy target)
        {
            if (target == null) return;
            int targetId = target.GetInstanceID();
            if (listSpeedOverTimes.ContainsKey(targetId))
            {
                var slow = listSpeedOverTimes[targetId];
                if (target.IsAlive)
                {
                    target.StatusController.CompleteStatus(slow);
                }
                listSpeedOverTimes.Remove(targetId);
            }
        }
    }
}
