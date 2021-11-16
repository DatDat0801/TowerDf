using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ItemTowerAvatarController : MonoBehaviour
    {
        [SerializeField] private Image avatarTower;

        public void InitItem(int towerId)
        {
            this.avatarTower.sprite = ResourceUtils.GetSpriteAtlas("avatar_tower", $"tower_{towerId}");
        }
    }
}