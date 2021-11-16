using UnityEngine;

namespace EW2
{
    public class PointCallSpawn : MonoBehaviour
    {
        private const float Spacing = 0.08f;
        public Location location;
        public int laneId;

        public Vector3 GetPointSpawnButton()
        {
            var pos = transform.position;

            if (location == Location.Top)
            {
                pos.x -= Spacing;
                pos.y -= Spacing;
            }
            else if (location == Location.Bottom)
            {
                pos.x += Spacing;
                pos.y += Spacing;
            }
            else if (location == Location.Left)
            {
                pos.x += Spacing;
            }
            else if (location == Location.Right)
            {
                pos.x -= Spacing;
            }

            return pos;
        }
    }
}