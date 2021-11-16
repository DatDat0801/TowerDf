using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public static class SortTarget
    {
        public static List<T> NearestTarget<T>(this List<T> targets, Unit unit) where T : Unit
        {
            targets.Sort((a, b) =>
            {
                var position = unit.Transform.position;

                var aDistance = Vector2.Distance(a.Transform.position, position);

                var bDistance = Vector2.Distance(b.Transform.position, position);

                return (aDistance < bDistance) ? -1 : 1;
            });

            return targets;
        }

        public static List<T> NearestEndPoint<T>(this List<T> targets) where T : EnemyBase
        {
            targets.Sort((a, b) =>
            {
                var aDistance = a.DistanceToEndPoint();

                var bDistance = b.DistanceToEndPoint();

                return (aDistance < bDistance) ? -1 : 1;
            });

            return targets;
        }
    }
}