using UnityEngine;
using UnityEngine.UI;

namespace EW2.Quest
{
    public class QuestTabController : TabButton
    {
        [SerializeField] private Image imgSymbol;

        private Color colorActive = new Color(1f, 1f, 1f, 1f);

        private Color colorDeactive = new Color(1f, 1f, 1f, 0f);

        public override void SetTabActiveChangeImgAndLabel(bool isActive, Color colorActiveText,
            Color colorInactiveText)
        {
            if (index == 0)
                lbButton.text = L.popup.daily_quest_txt.ToUpper();
            else
                lbButton.text = L.popup.achievement_txt.ToUpper();

            button.image.color =
                isActive ? colorActive : colorDeactive;

            imgSymbol.sprite = isActive
                ? ResourceUtils.GetSpriteAtlas("tab_image", $"icon_tab_{index}_on")
                : ResourceUtils.GetSpriteAtlas("tab_image", $"icon_tab_{index}_off");

            imgSymbol.SetNativeSize();

            lbButton.color = isActive ? colorActiveText : colorInactiveText;

            base.SetTabActiveChangeImgAndLabel(isActive, colorActiveText, colorInactiveText);
        }
    }
}