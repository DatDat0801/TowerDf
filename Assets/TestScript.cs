using System.Collections.Generic;
using EW2;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public List<Vector3> listPoints;
    [Button]
    private Vector3 GetBackwardPosition(Vector3 enemyPosition, float backwardUnit)
    {
        //find the 2 nearest knot 
        var knots = CalculateTwoPointNearTarget(enemyPosition);
        var d1 = Vector3.Distance(listPoints[knots.startPointIndex], enemyPosition);
        if (d1 >= backwardUnit)
        {
            GameObject go = new GameObject("result");
            go.transform.position = listPoints[knots.startPointIndex];
            return listPoints[knots.startPointIndex];
        }

        float backwardDistance = d1;

        for (int i = knots.startPointIndex - 1; i > 0; i--)
        {
            var nextSegment = Vector3.Distance(listPoints[i], listPoints[i - 1]);
            backwardDistance += nextSegment;
            if (backwardDistance >= backwardUnit)
            {
                var ratio = (backwardDistance - backwardUnit)/nextSegment;
                var resultPos = Vector3.Lerp(listPoints[i], listPoints[i - 1], 1 - ratio);
                GameObject go = new GameObject("result");
                go.transform.position = resultPos;
                return resultPos;
            }
        }
        return Vector3.negativeInfinity;
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
