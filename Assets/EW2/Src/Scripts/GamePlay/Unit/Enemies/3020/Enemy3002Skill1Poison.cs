using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class Enemy3002Skill1Poison : TargetCollection<Dummy>
    {
        private Dictionary<int, PoisonStatus> listPoisons = new Dictionary<int, PoisonStatus>();
        private Unit owner;
        private EnemyData3020.EnemyData3020Skill1 dataSkill;

        public void InitPoison(Enemy3020 enemy3020, EnemyData3020.EnemyData3020Skill1 data3020Skill1)
        {
            this.owner = enemy3020;
            this.dataSkill = data3020Skill1;
        }

        protected override Dummy GetTarget(Collider2D other)
        {
            if (other.CompareTag(TagName.Hero) == false)
                return null;

            var unitCollider = other.GetComponentInParent<Dummy>();

            // if (unitCollider == null)
            //     throw new Exception("unit is null");

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
                // Debug.Log("Add target: " + target.name);
                var damagePoison = target.Stats.GetStat(RPGStatType.Health).StatValue * dataSkill.ratioPoisonPerSecond;

                var statusOverTime = new StatusOverTimeConfig()
                {
                    creator = this.owner,
                    owner = target,
                    baseValue = damagePoison,
                    lifeTime = dataSkill.timeLifePoison,
                    intervalTime = dataSkill.intervalTimePoison,
                    damageType = owner.DamageType
                };

                var poison = new PoisonStatus(statusOverTime);

                target.StatusController.AddStatus(poison);

                AddPoison(target, poison);

                Targets.Add(target);
            }
        }

        public override Dummy SelectTarget()
        {
            return default;
        }

        public override void RemoveTarget(Dummy target)
        {
            RemovePoison(target);
            Targets.Remove(target);
        }

        private void AddPoison(Dummy target, PoisonStatus poison)
        {
            if (!listPoisons.ContainsKey(target.GetInstanceID()))
                listPoisons.Add(target.GetInstanceID(), poison);
        }


        private void RemovePoison(Dummy target)
        {
            if (target == null) return;

            if (target.IsAlive)
            {
                foreach (var poison in listPoisons)
                {
                    if (poison.Key == target.GetInstanceID())
                    {
                        poison.Value.Remove();
                        listPoisons.Remove(poison.Key);
                        break;
                    }
                }
            }
            else
            {
                foreach (var poison in listPoisons)
                {
                    if (poison.Key == target.GetInstanceID())
                    {
                        listPoisons.Remove(poison.Key);
                        break;
                    }
                }
            }
        }
    }
}