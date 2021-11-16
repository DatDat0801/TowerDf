using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class TroopSearchTarget : AllySearchTarget
    {
        protected override void OnAddEnemyToBlockList(EnemyBase enemy)
        {
            if (target.Value == null || !allyBlockEnemy.BlockList.Contains(target.Value) || !target.Value.IsAlive)
            {
                Debug.LogWarning(Owner.name + $"====> Set Enemy {enemy.Id}");
                (target.Value, targetAttackType.Value) = SelectTarget();
            }
        }

        protected override void OnRemoveEnemyFromBlockList(EnemyBase enemy)
        {
            if (IsEnable && (target.Value == null || !target.Value.IsAlive))
            {
                (target.Value, targetAttackType.Value) = SelectTarget();
            }
        }
    }
}