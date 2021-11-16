using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ShopTabController : TabButton
    {
        [SerializeField] private int typeCurrency;
        [SerializeField] private Image imgSymbol;

        private Color colorActive = new Color(1f, 1f, 1f, 1f);

        private Color colorDeactive = new Color(1f, 1f, 1f, 0f);

        public override void SetTabActiveChangeImgAndLabel(bool isActive, Color colorActiveText,
            Color colorInactiveText)
        {
            if (this.typeCurrency != -1)
                lbButton.text = Ultilities.GetNameCurrency(typeCurrency);

            button.image.color =
                isActive ? colorActive : colorDeactive;

            imgSymbol.sprite = isActive
                ? ResourceUtils.GetSpriteAtlas("shop_icons", $"icon_tab_{index}_on")
                : ResourceUtils.GetSpriteAtlas("shop_icons", $"icon_tab_{index}_off");

            imgSymbol.SetNativeSize();

            lbButton.color = isActive ? colorActiveText : colorInactiveText;

            base.SetTabActiveChangeImgAndLabel(isActive, colorActiveText, colorInactiveText);
        }
    }
}