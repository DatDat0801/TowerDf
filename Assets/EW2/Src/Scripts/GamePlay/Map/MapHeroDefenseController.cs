using EW2;
using UnityEngine;

namespace Map
{
    public class MapHeroDefenseController : MapController
    {
        [SerializeField] private Transform pointDefensivePoint;

        private void Start()
        {
            var dfpId = UserData.Instance.UserHeroDefenseData.defensePointId;
            var pointSpawn = new Vector3(this.pointDefensivePoint.position.x, this.pointDefensivePoint.position.y, - this.pointDefensivePoint.position.y / 10f);
            var goDefensivePoint = ResourceUtils.GetUnit(dfpId.ToString(), pointSpawn, Quaternion.identity);
            if (goDefensivePoint != null)
            {
                goDefensivePoint.transform.SetParent(this.transform);
            }
        }
    }
}