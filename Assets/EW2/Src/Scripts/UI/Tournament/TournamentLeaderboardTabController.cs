using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class TournamentLeaderboardTabController : TabButton
    {
        private const float POS_X_ON = -795f;
        private const float POS_X_OFF = -758f;

        [SerializeField] private Image imgSymbol;

        public override void SetTabActiveChangeImg(bool isActive)
        {
            imgSymbol.sprite = isActive
                ? ResourceUtils.GetSpriteAtlas("tab_image", $"icon_tab_arena_{index}_on")
                : ResourceUtils.GetSpriteAtlas("tab_image", $"icon_tab_arena_{index}_off");

            imgSymbol.SetNativeSize();

            button.image.sprite = isActive
                ? ResourceUtils.GetSpriteAtlas("tab_image", $"bg_tab_hero_room_select")
                : ResourceUtils.GetSpriteAtlas("tab_image", $"bg_tab_hero_room_unselect");
            button.image.SetNativeSize();

            if (isActive) transform.SetAsLastSibling();
            else transform.SetAsFirstSibling();

            var rectTransform = GetComponent<RectTransform>();
            var rectPos = rectTransform.anchoredPosition;
            if (isActive) rectPos.x = POS_X_ON;
            else rectPos.x = POS_X_OFF;
            rectTransform.anchoredPosition = rectPos;

            base.SetTabActiveChangeImg(isActive);
        }
    }
}