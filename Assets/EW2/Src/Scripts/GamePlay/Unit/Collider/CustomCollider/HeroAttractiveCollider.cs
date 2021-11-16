using UnityEngine;

namespace EW2
{
    /// <summary>
    /// All the Units trigger with this collider will change its target
    /// </summary>
    public class HeroAttractiveCollider : ColliderTrigger<HeroBase>, IAttractive
    {
        public void SetRadius(float radius)
        {
            var col = GetComponent<CircleCollider2D>();
            col.radius = radius;
        }
        public virtual void ChangeTarget(Unit unit)
        {
            var enemy = (EnemyBase)unit;
            if (enemy.MoveType == MoveType.Fly)
            {
                return;
            }
            enemy.blockCollider.SetBlock(owner);
            
            
            //Debug.LogAssertion($"Enemy: {enemy.name} is changed target to {owner.name}");
        }
        
    }
}