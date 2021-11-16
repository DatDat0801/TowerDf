using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class HeroRoomTabController : TabButton
    {
        private const float PosXOn = 412f;
        private const float PosXOff = 383f;
        private const float PosSymbolXOn = 0;
        private const float PosSymbolXOff = 17f;

        [SerializeField] private Image imgSymbol;

        public override void SetTabActiveChangeImg(bool isActive)
        {
            imgSymbol.sprite = isActive
                ? ResourceUtils.GetSpriteAtlas("hero_room", $"tab_{index}_on")
                : ResourceUtils.GetSpriteAtlas("hero_room", $"tab_{index}_off");

            imgSymbol.SetNativeSize();

            button.image.sprite = isActive
                ? ResourceUtils.GetSpriteAtlas("tab_image", $"bg_tab_hero_room_select")
                : ResourceUtils.GetSpriteAtlas("tab_image", $"bg_tab_hero_room_unselect");
            button.image.SetNativeSize();

            if (isActive) transform.SetAsLastSibling();
            else transform.SetAsFirstSibling();

            var rectTransform = GetComponent<RectTransform>();
            var rectPos = rectTransform.anchoredPosition;
            if (isActive) rectPos.x = PosXOn;
            else rectPos.x = PosXOff;
            rectTransform.anchoredPosition = rectPos;

            var rectTransformSymbol = imgSymbol.GetComponent<RectTransform>();
            var rectPosSymbol = rectTransformSymbol.anchoredPosition;
            if (isActive) rectPosSymbol.x = PosSymbolXOn;
            else rectPosSymbol.x = PosSymbolXOff;
            rectTransformSymbol.anchoredPosition = rectPosSymbol;

            base.SetTabActiveChangeImg(isActive);
        }
    }
}