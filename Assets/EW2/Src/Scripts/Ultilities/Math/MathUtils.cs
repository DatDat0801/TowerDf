using UnityEngine;

public static class MathUtils
{
    private const float DOUBLE_PI = Mathf.PI * 2;
    private const float HALF_PI = Mathf.PI / 2;

    public static bool IsSmallerThanRange(Vector3 position1, Vector3 position2, float range)
    {
        if ((position1 - position2).sqrMagnitude < (range * range))
        {
            return true;
        }

        return false;
    }

    public static bool IsBiggerThanRange(Vector3 position1, Vector3 position2, float range)
    {
        if ((position1 - position2).sqrMagnitude > range)
        {
            return true;
        }

        return false;
    }

    public static bool IsInCircleRange(Vector3 positionCurrent, Vector3 position, float range)
    {
        position.z = positionCurrent.z = 0;
        if ((positionCurrent - position).sqrMagnitude < range)
            return true;
        return false;
    }

    public static float FastSin(float angle)
    {
        if (angle < -Mathf.PI)
        {
            angle += DOUBLE_PI;
        }
        else if (angle > Mathf.PI)
        {
            angle -= DOUBLE_PI;
        }

        float sin;
        if (angle < 0)
        {
            sin = angle * (1.27323954f + 0.405284735f * angle);
            if (sin < 0)
            {
                sin = 0.225f * (sin * -sin - sin) + sin;
            }
            else
            {
                sin = 0.225f * (sin * sin - sin) + sin;
            }
        }
        else
        {
            sin = angle * (1.27323954f - 0.405284735f * angle);
            if (sin < 0)
            {
                sin = 0.225f * (sin * -sin - sin) + sin;
            }
            else
            {
                sin = 0.225f * (sin * sin - sin) + sin;
            }
        }

        return sin;
    }

    public static float FastCos(float angle)
    {
        if (angle < -Mathf.PI)
        {
            angle += DOUBLE_PI;
        }
        else if (angle > Mathf.PI)
        {
            angle -= DOUBLE_PI;
        }

        angle += HALF_PI;
        if (angle > Mathf.PI)
        {
            angle -= DOUBLE_PI;
        }

        float cos;
        if (angle < 0)
        {
            cos = angle * (1.27323954f + 0.405284735f * angle);
            if (cos < 0)
            {
                cos = 0.225f * (cos * -cos - cos) + cos;
            }
            else
            {
                cos = 0.225f * (cos * cos - cos) + cos;
            }
        }
        else
        {
            cos = angle * (1.27323954f - 0.405284735f * angle);
            if (cos < 0)
            {
                cos = 0.225f * (cos * -cos - cos) + cos;
            }
            else
            {
                cos = 0.225f * (cos * cos - cos) + cos;
            }
        }

        return cos;
    }

    public static bool CheckLimitPositionMap(Vector3 position, float limitWidth, float limitHeight)
    {
        if (limitWidth == 0 || limitHeight == 0)
        {
            return false;
        }

        if (Mathf.Abs(position.x) > limitWidth || Mathf.Abs(position.y) > limitHeight)
        {
            return true;
        }

        return false;
    }

    public static Vector3 GetPositionToMeleeAttack(Vector3 owner, Vector3 target, float offset)
    {
        Vector3 position = target;
        if (owner.x <= target.x)
        {
            position.x = target.x - offset;
        }
        else
        {
            position.x = target.x + offset;
        }

        return position;
    }
}