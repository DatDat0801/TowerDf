using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ItemInfoWaveController : MonoBehaviour
    {
        public Image enemyAvatar;

        public Text lbNumber;

        public void ShowInfo(int enemyId, int number)
        {
            enemyAvatar.sprite = ResourceUtils.GetSpriteAtlas("enemies_icons", $"enemy_{enemyId}");

            lbNumber.text = $"X{number}";
        }
    }
}