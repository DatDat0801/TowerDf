using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EW2
{
    public static class SearchTarget
    {
        public static bool IsUnitType<T>(this T unit, UnitType unitType) where T : Unit
        {
            return unit.UnitType == unitType;
        }

        public static bool IsSearchTargetType<T>(this T unit, PriorityTargetType priorityTargetType) where T : Unit
        {
            return unit.PriorityTargetType == priorityTargetType;
        }

        public static bool IsClassType<T>(this T unit, UnitClassType classType) where T : Unit
        {
            return unit.UnitClassType == classType;
        }

        public static List<T> FilterByType<T>(this List<T> targets, UnitType unitType) where T : Unit
        {
            return targets.FindAll((x => x.IsUnitType<T>(unitType)));
        }

        public static List<T> FilterByType<T>(this List<T> targets, PriorityTargetType priorityTargetType)
            where T : Unit
        {
            return targets.FindAll((x => x.IsSearchTargetType(priorityTargetType)));
        }

        public static List<T> FilterByType<T>(this List<T> targets, UnitClassType classType) where T : Unit
        {
            return targets.FindAll((x => x.IsClassType(classType)));
        }

        public static T FilterByNearestDistance<T>(this List<T> targets, Transform owner) where T : Unit
        {
            T target = null;
            float currDistance = 0;
            foreach (var unit in targets)
            {
                if (target == null)
                {
                    currDistance = Vector2.Distance(owner.position, unit.Transform.position);
                    target = unit;
                }
                else
                {
                    float distance = Vector2.Distance(owner.position, unit.Transform.position);
                    if (distance < currDistance)
                        target = unit;
                }
            }

            return target;
        }

        public static T FilterByHpHighest<T>(this List<T> targets) where T : Unit
        {
            targets = targets.OrderByDescending(unit => unit.Stats.GetStat(RPGStatType.Health).StatValue).ToList();

            return targets[0];
        }
    }
}