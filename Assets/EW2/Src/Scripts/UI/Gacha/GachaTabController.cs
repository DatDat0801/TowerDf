using System.Collections;
using System.Collections.Generic;
using EW2;
using UnityEngine;
using UnityEngine.UI;

public class GachaTabController : TabButton
{
    [SerializeField] private Image imgSymbol;

    private Color colorActive = new Color(1f, 1f, 1f, 1f);

    private Color colorDeactive = new Color(1f, 1f, 1f, 0f);

    public override void SetTabActiveChangeImgAndLabel(bool isActive, Color colorActiveText, Color colorInactiveText)
    {
        if (index == (int) GachaTabId.Spell)
            lbButton.text = L.gameplay.name_tab_spell.ToUpper();
        else
            lbButton.text = L.gameplay.name_tab_rune.ToUpper();

        button.image.color =
            isActive ? colorActive : colorDeactive;

        imgSymbol.sprite = isActive
            ? ResourceUtils.GetSpriteAtlas("tab_image", $"icon_tab_gacha_{index}_on")
            : ResourceUtils.GetSpriteAtlas("tab_image", $"icon_tab_gacha_{index}_off");
        
        imgSymbol.SetNativeSize();

        lbButton.color = isActive ? colorActiveText : colorInactiveText;

        base.SetTabActiveChangeImgAndLabel(isActive, colorActiveText, colorInactiveText);
    }
}