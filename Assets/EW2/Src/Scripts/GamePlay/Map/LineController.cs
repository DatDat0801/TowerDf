using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Map
{
    public class LineController : MonoBehaviour
    {
        private DOTweenPath path;

        private List<Vector3> listPoints = new List<Vector3>();

        private void Start()
        {
            if (path == null)
            {
                path = GetComponent<DOTweenPath>();
            }

            if (path != null)
            {
                foreach (Vector2 point in path.path.wps)
                {
                    listPoints.Add(point);
                }
            }
        }

        /// <summary>
        /// back enemy a unit
        /// </summary>
        /// <param name="enemyPosition"></param>
        /// <param name="backwardUnit"></param>
        /// <returns></returns>
        public Vector3 GetBackwardPosition(Vector3 enemyPosition, float backwardUnit)
        {
            //find the 2 nearest knot 
            var knots = CalculateTwoPointNearTarget(enemyPosition);
            var d1 = Vector3.Distance(listPoints[knots.startPointIndex], enemyPosition);
            if (d1 >= backwardUnit)
            {
                return listPoints[knots.startPointIndex];
            }

            float backwardDistance = d1;

            for (int i = knots.startPointIndex - 1; i > 0; i--)
            {
                var nextSegment = Vector3.Distance(listPoints[i], listPoints[i - 1]);
                backwardDistance += nextSegment;
                if (backwardDistance >= backwardUnit)
                {
                    var ratio = (backwardDistance - backwardUnit) / nextSegment;
                    var resultPos = Vector3.Lerp(listPoints[i], listPoints[i - 1], 1 - ratio);
                    return resultPos;
                    // if (ratio >= 0.5)
                    // {
                    //     return listPoints[i - 1];
                    // }
                    // else
                    // {
                    //     var resultPos = listPoints[i];
                    //     return resultPos;
                    // }
                }
            }

            return listPoints[0];
        }

        public List<Vector3> GetPathWaypoints()
        {
            return listPoints;
        }

        public Vector3 GetSpawnPoint()
        {
            return transform.position;
        }

        public Vector3? GetEndPoint()
        {
            if (listPoints.Count > 0)
                return listPoints[listPoints.Count - 1];

            return null;
        }

        public List<Vector3> CalculateRemainPathWayPoints(Vector3 position)
        {
            var calculatedRemainPathWayPoints = new List<Vector3>();
            var nearestEndPointIndex = CalculateTwoPointNearTarget(position).endPointIndex;
            for (int i = 0, length = listPoints.Count; i < length; i++)
            {
                if (i >= nearestEndPointIndex)
                {
                    calculatedRemainPathWayPoints.Add(listPoints[i]);
                }
            }

            return calculatedRemainPathWayPoints;
        }

        public List<Vector3> CalculateGoBackWayPoints(Vector3 position)
        {
            var calculatedRemainPathWayPoints = new List<Vector3>();
            var nearestEndPointIndex = CalculateTwoPointNearTarget(position).startPointIndex;
            for (int i = listPoints.Count - 1; i >= 0; i--)
            {
                if (i <= nearestEndPointIndex)
                {
                    calculatedRemainPathWayPoints.Add(listPoints[i]);
                }
            }

            return calculatedRemainPathWayPoints;
        }

        public bool IsFirstSpawnPoint(Vector3 point)
        {
            if (listPoints == null) return false;
            return listPoints[0] == point;
        }

        public List<Vector3> CalculateRemainPathWayPointsTest(Vector3 position)
        {
            var calculatedRemainPathWayPoints = new List<Vector3>();
            var nearestEndPointIndex = CalculateTwoPointNearTarget(position).startPointIndex;
            for (int i = 0, length = listPoints.Count; i < length; i++)
            {
                if (i >= nearestEndPointIndex)
                {
                    calculatedRemainPathWayPoints.Add(listPoints[i]);
                }
            }

            return calculatedRemainPathWayPoints;
        }

        private (int startPointIndex, int endPointIndex) CalculateTwoPointNearTarget(Vector3 position)
        {
            var calculatedTwoPointNearTarget = (-1, -1);
            var minDistanceToTarget = float.MaxValue;
            for (int i = 0, length = listPoints.Count - 1; i < length; i++)
            {
                var currentDistanceToTarget = (listPoints[i] - position).sqrMagnitude +
                                              (listPoints[i + 1] - position).sqrMagnitude;
                if (minDistanceToTarget > currentDistanceToTarget)
                {
                    minDistanceToTarget = currentDistanceToTarget;
                    calculatedTwoPointNearTarget = (i, i + 1);
                }
            }

            return calculatedTwoPointNearTarget;
        }
    }
}