using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EW2
{
    public class Ship6045TargetCollection : TargetCollection<Dummy>
    {
        protected override Dummy GetTarget(Collider2D other)
        {
            if (other.CompareTag(TagName.Tower) || other.CompareTag(TagName.Enemy))
                return null;

            var unitCollider = other.GetComponent<BodyCollider>();

            if (unitCollider == null)
                throw new Exception("unit is null");

            return unitCollider.Owner;
        }

        protected override void FilterTarget(Dummy target)
        {
            if (target == null)
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
                Targets.Add(target);
            }
        }

        public override Dummy SelectTarget()
        {
            return null;
        }

        public override void RemoveTarget(Dummy target)
        {
            Targets.Remove(target);
        }

        public List<Dummy> GetListTargets()
        {
            var listHeroes = Targets.ToList().FindAll((x => x is HeroBase));

            var listSoldier = Targets.ToList().FindAll((x => x is SoldierBase));

            var listResult = new List<Dummy>();

            if (listHeroes.Count > 0)
            {
                listResult.AddRange(listHeroes);
            }

            if (listResult.Count < 3)
            {
                var numberTarget = 3 - listResult.Count;

                for (int i = 0; i < numberTarget; i++)
                {
                    if (listSoldier.Count > 0)
                    {
                        listResult.Add(listSoldier[Random.Range(0, listSoldier.Count)]);
                    }
                    else if (listHeroes.Count > 0)
                    {
                        listResult.Add(listHeroes[Random.Range(0, listHeroes.Count)]);
                    }
                }
            }

            return listResult;
        }
    }
}