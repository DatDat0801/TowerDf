using UnityEngine;
using UnityEngine.Assertions;

namespace EW2
{
    public class TowerTargetCollection3016 : TowerTargetCollection
    {

        public void SetRadius(float radius)
        {
            var collider2d = GetComponent<CircleCollider2D>();
            collider2d.radius = radius;
            Radius = radius;
        }
        protected override Building GetTarget(Collider2D other)
        {
            if (other.CompareTag(TagName.Tower) == false)
                return null;
            var target = other.GetComponent<Building>();
            Assert.IsNotNull(target);
            if (target.TowerType == TowerType.Barrack) return null;
            return target;
        }
    }
}