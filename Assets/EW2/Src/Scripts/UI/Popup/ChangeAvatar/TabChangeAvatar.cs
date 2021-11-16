using System;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class TabChangeAvatar : TabButton
    {
        private const string TabActive = "bg_tap_button_select";

        private const string TabInactive = "bg_tap_button_unselect";

        protected override void Awake()
        {
            base.Awake();

            if (index == 0)
            {
                lbButton.text = L.popup.image_txt;
            }
            else
            {
                lbButton.text = L.popup.country_txt;
            }

            EventManager.StartListening(GamePlayEvent.OnChangeLanggueSuccess, OnUpdateLanggue);
        }

        private void OnDestroy()
        {
            EventManager.StopListening(GamePlayEvent.OnChangeLanggueSuccess, OnUpdateLanggue);
        }

        public override void SetTabActiveChangeImgAndLabel(bool isActive, Color colorActiveText,
            Color colorInactiveText)
        {
            button.image.sprite =
                isActive ? ResourceUtils.GetSpriteTab(TabActive) : ResourceUtils.GetSpriteTab(TabInactive);

            lbButton.color = isActive ? colorActiveText : colorInactiveText;

            base.SetTabActiveChangeImgAndLabel(isActive, colorActiveText, colorInactiveText);
        }

        private void OnUpdateLanggue()
        {
            if (index == 0)
            {
                lbButton.text = L.popup.image_txt;
            }
            else
            {
                lbButton.text = L.popup.country_txt;
            }
        }
    }
}