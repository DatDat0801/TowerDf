using System;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class UnlockDefendPointProperty : WindowProperties
    {
        public int dfpUnlock;

        public UnlockDefendPointProperty(int dfpId)
        {
            this.dfpUnlock = dfpId;
        }
    }

    public class UnlockDefendPointWindow : AWindowController<UnlockDefendPointProperty>
    {
        [SerializeField] private Button btnUnlock;
        [SerializeField] private Button btnClose;
        [SerializeField] private Image iconDFP;
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtNameDfp;
        [SerializeField] private Text txtDescDfp;

        protected override void Awake()
        {
            base.Awake();
            this.btnClose.onClick.AddListener(CloseClick);
            this.btnUnlock.onClick.AddListener(UnlockClick);
        }

        private void UnlockClick()
        {
            UIFrame.Instance.CloseAllPopup(false);
            ShopWindowProperties shopProperties = new ShopWindowProperties(ShopTabId.DefensivePoint);
            UIFrame.Instance.OpenWindow(ScreenIds.shop_scene, shopProperties);
        }

        private void CloseClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_unlock_defend_point);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            this.txtTitle.text = L.playable_mode.unlock_base_txt;
            this.btnUnlock.GetComponentInChildren<Text>().text = L.button.btn_unlock;
            this.iconDFP.sprite =
                ResourceUtils.GetSpriteAtlas("defensive_point", $"icon_defensive_point_{Properties.dfpUnlock}");
            this.txtNameDfp.text =
                Localization.Current.Get("playable_mode", $"defensive_point_name_{Properties.dfpUnlock}");
            this.txtDescDfp.text =
                Localization.Current.Get("playable_mode", $"defensive_point_skill_{Properties.dfpUnlock}");
        }
    }
}
