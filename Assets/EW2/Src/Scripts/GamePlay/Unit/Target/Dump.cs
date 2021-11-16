using System;
using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class DataDump
    {
        public float damage;
        public float interval;
        public DamageType damageType;
        public float duration;
        public EffectOnType[] effectOnType;
    }
    
    public class Dump<T> : TargetCollection<Dummy> where T: StatusOverTime 
    {
        private Dictionary<int, T> dictStatusOverTimes = new Dictionary<int, T>();
        
        private DataDump dataDump;

        [SerializeField] protected Unit creator;

        public void InitDump(DataDump dataDump)
        {
            this.dataDump = dataDump;
        }
        
        protected override Dummy GetTarget(Collider2D other)
        {
            
            var unitCollider = other.GetComponentInParent<Dummy>();
            
            if (unitCollider == null)
            {
                return null;
            }
            
            foreach (var type in dataDump.effectOnType)
            {
                switch (type)
                {
                    case EffectOnType.All:
                        return unitCollider;
                    case EffectOnType.None:
                        return null;
                    case EffectOnType.Hero:
                        if (other.CompareTag(TagName.Hero))
                            return unitCollider;
                        break;
                    case EffectOnType.FlyEnemy:
                        if (other.CompareTag(TagName.Enemy))
                        {
                            if (((EnemyBase) unitCollider).MoveType == MoveType.Fly ||
                                ((EnemyBase) unitCollider).MoveType == MoveType.All)
                            {
                                return unitCollider;
                            }
                        }
                        break;
                    case EffectOnType.GroundEnemy:
                        if (other.CompareTag(TagName.Enemy))
                        {
                            if (((EnemyBase) unitCollider).MoveType == MoveType.Ground ||
                                ((EnemyBase) unitCollider).MoveType == MoveType.All)
                            {
                                return unitCollider;
                            }
                        }
                        break;
                }
            }

            return null;
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
                RemoveStatus(target);
                Targets.Remove(target);
            }

            var statusOverTime = new StatusOverTimeConfig()
            {
                creator = creator,
                owner = target,
                baseValue = dataDump.damage,
                lifeTime = Single.PositiveInfinity,
                intervalTime = dataDump.interval,
                damageType = dataDump.damageType
            };

            var status = (T)Activator.CreateInstance(typeof(T), statusOverTime);

            target.StatusController.AddStatus(status);

            AddStatusList(target, status);

            Targets.Add(target);
            
        }

        public override Dummy SelectTarget()
        {
            return default;
        }

        public override void RemoveTarget(Dummy target)
        {
            RemoveStatus(target);
            Targets.Remove(target);
            
            if (target == null || target.IsAlive == false)
            {
                Debug.Log("Target is null");
                return;
            }
            
            var statusOverTime = new StatusOverTimeConfig()
            {
                creator = creator,
                owner = target,
                baseValue = dataDump.damage,
                lifeTime = dataDump.duration,
                intervalTime = dataDump.interval,
                damageType = dataDump.damageType
            };

            var poison = new PoisonStatus(statusOverTime);
            target.StatusController.AddStatus(poison);
            
        }
        
        private void AddStatusList(Dummy target, T status)
        {
            if (!dictStatusOverTimes.ContainsKey(target.GetInstanceID()))
                dictStatusOverTimes.Add(target.GetInstanceID(), status);
        }
        
        private void RemoveStatus(Dummy target)
        {
            if (target == null) return;

            if (target.IsAlive)
            {
                foreach (var poison in dictStatusOverTimes)
                {
                    if (poison.Key == target.GetInstanceID())
                    {
                        poison.Value.Remove();
                        dictStatusOverTimes.Remove(poison.Key);
                        break;
                    }
                }
            }
            else
            {
                foreach (var status in dictStatusOverTimes)
                {
                    if (status.Key == target.GetInstanceID())
                    {
                        dictStatusOverTimes.Remove(status.Key);
                        break;
                    }
                }
            }
        }
    }

}
