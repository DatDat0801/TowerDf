using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class GraphicTab : TabButton
    {
        private const int LowGraphic = 0;

        private const int MediumGraphic = 1;

        private const int HighGraphic = 2;

        private const string TabActive = "bg_button_blue";

        private const string TabInactive = "bg_button_blue_unselect";

        protected override void Awake()
        {
            base.Awake();

            OnUpdateLanggue();

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

            lbButton.GetComponent<Outline>().enabled = isActive;
            
            base.SetTabActiveChangeImgAndLabel(isActive, colorActiveText, colorInactiveText);
        }

        private void OnUpdateLanggue()
        {
            if (index == LowGraphic)
            {
                lbButton.text = L.button.low_setting_btn;
            }
            else if (index == MediumGraphic)
            {
                lbButton.text = L.button.medium_setting_btn;
            }
            else
            {
                lbButton.text = L.button.high_setting_btn;
            }
        }
    }
}