using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class UnitViewUI : MonoBehaviour
    {
        [SerializeField] private Image icon;

        public void ShowUi(OptionShowInfoTournament option, int unitId)
        {
            if (option == OptionShowInfoTournament.Buff || option == OptionShowInfoTournament.Nerf)
            {
                var avatarId = (unitId % 1000) - 1;
                icon.sprite = ResourceUtils.GetSpriteAvatar(avatarId);
            }
            else if (option == OptionShowInfoTournament.Ban)
            {
                icon.sprite = ResourceUtils.GetSpriteAtlas("spell", $"{unitId}_0");
            }
        }
    }
}