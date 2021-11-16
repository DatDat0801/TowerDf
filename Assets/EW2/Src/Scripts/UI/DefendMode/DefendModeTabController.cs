using UnityEngine;
using UnityEngine.UI;

namespace EW2.DefendMode
{
    public class DefendModeTabController : TabButton
    {
        [SerializeField] private Image imgSymbol;

        private Color colorActive = new Color(1f, 1f, 1f, 1f);

        private Color colorDeactive = new Color(1f, 1f, 1f, 0f);

        public override void SetTabActiveChangeImgAndLabel(bool isActive, Color colorActiveText,
            Color colorInactiveText)
        {
            if (index == (int)DefendModeTabId.Lobby)
            {
                lbButton.text = L.playable_mode.lobby_txt;
            }
            else if (index == (int)DefendModeTabId.Rewards)
            {
                lbButton.text = L.gameplay.rewards;
            }
            else
            {
                lbButton.text = L.playable_mode.leaderboard_txt;
            }

            button.image.color = isActive ? colorActive : colorDeactive;

            imgSymbol.sprite = isActive
                ? ResourceUtils.GetSpriteAtlas("tab_image", $"icon_tab_dfm_{index}_on")
                : ResourceUtils.GetSpriteAtlas("tab_image", $"icon_tab_dfm_{index}_off");

            imgSymbol.SetNativeSize();

            lbButton.color = isActive ? colorActiveText : colorInactiveText;

            base.SetTabActiveChangeImgAndLabel(isActive, colorActiveText, colorInactiveText);
        }
    }
}