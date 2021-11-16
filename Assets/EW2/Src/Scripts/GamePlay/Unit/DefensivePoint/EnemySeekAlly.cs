using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zitga.Observables;

namespace EW2
{
    public class EnemySeekAlly : ColliderTrigger<EnemyBase>
    {
        //private List<EnemyBase> _enemiesInRage;
        private ObservableList<EnemyBase> _targets;

        public ObservableList<EnemyBase> Targets => this._targets ?? (this._targets = new ObservableList<EnemyBase>());

        // public List<EnemyBase> EnemiesInRange
        // {
        //     get
        //     {
        //         if (this._enemiesInRage == null)
        //         {
        //             this._enemiesInRage = new List<EnemyBase>();
        //             return this._enemiesInRage;
        //         }
        //
        //         return this._enemiesInRage;
        //     }
        // }
        public void SetCollider(Collider2D c)
        {
            this.isAoe = true;
            this.collider2D = c;
        }

        public Collider2D GetCollider()
        {
            return this.collider2D;
        }

        protected override void OnEnable()
        {
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = GetTarget(other);
            FilterTarget(enemy);
        }


        private EnemyBase GetTarget(Collider2D other)
        {
            if (other.CompareTag(TagName.Enemy))
            {
                var body = other.GetComponent<BodyCollider>();
                if (body != null)
                {
                    var enemy = (EnemyBase)body.Owner;
                    //Targets.Add(enemy);
                    return enemy;
                }
            }

            return null;
        }

        private void FilterTarget(EnemyBase target)
        {
            if (target == null || target.IsAlive == false)
            {
                Debug.Log("Target is null");
                return;
            }

            if (Targets.Contains(target) || target == this.owner)
            {
                // do nothing
                Debug.Log("Exist unit: " + target.Transform.name);
            }
            else
            {
                // Debug.LogAssertion(
                //     $"Add target: {target.name},{target.GetInstanceID()} by {this.owner.name},{this.owner.GetInstanceID()}");
                Targets.Add(target);
            }
        }

        public EnemyBase GetNearestEnemy(HashSet<EnemyBase> excepts)
        {
            foreach (EnemyBase enemy in excepts)
            {
                if (Targets.Contains(enemy)) Targets.Remove(enemy);
            }

            if (Targets.Count == 0)
                return null;
            if (Targets.Count == 1)
                return Targets[0];
            return Targets.ToList().FilterByNearestDistance(this.owner.transform);
        }

        public void ClearTargets()
        {
            Targets.Clear();
        }

        public int GetTargetCount()
        {
            return Targets.Count;
        }
    }
}