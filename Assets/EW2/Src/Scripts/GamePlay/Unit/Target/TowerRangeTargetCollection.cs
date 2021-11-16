using System.Collections.Generic;
using System.Linq;

namespace EW2
{
    public class TowerRangeTargetCollection : AllyTargetCollection
    {
        public override EnemyBase SelectTarget()
        {
            var target = Targets.ToList().FilterByType(UnitType.Enemy).FilterByNearestDistance(transform);
            return target;
        }

        public EnemyBase SelectTargetHpHighest()
        {
            var target = Targets.ToList().FilterByHpHighest();
            return target;
        }

        public EnemyBase SelectTargetDifferent(List<EnemyBase> targetList)
        {
            var listEnemy = Targets.ToList().FilterByType(UnitType.Enemy);
            for (int i = 0; i < listEnemy.Count; i++)
            {
                if (targetList.Contains(listEnemy[i]) == false)
                    return listEnemy[i];
            }

            if (listEnemy.Count > 0)
            {
                return listEnemy[UnityEngine.Random.Range(0, listEnemy.Count)];
            }

            return null;
        }

        public List<EnemyBase> SelectTargets(int quantity)
        {
            var targets = Targets.ToList().FilterByType(UnitType.Enemy).Take(quantity);
            return targets.ToList();
        }
    }
}