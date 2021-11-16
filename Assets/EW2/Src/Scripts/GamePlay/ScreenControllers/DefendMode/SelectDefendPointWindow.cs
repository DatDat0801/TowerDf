using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.UIFramework;

namespace EW2
{
    public class SelectDefendPointWindow : AWindowController
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtNameDfp;
        [SerializeField] private Text txtDescDfp;
        [SerializeField] private Text txtTrialLeft;
        [SerializeField] private Image iconDFP;
        [SerializeField] private DefensivePointBar defensivePointBar;
        [SerializeField] private Button btnConfirm;
        [SerializeField] private Button btnClose;

        private List<DefensivePointSelectedData> _listDefensivePoint = new List<DefensivePointSelectedData>();
        private int _DFPSelected;

        protected override void Awake()
        {
            base.Awake();
            this.btnClose.onClick.AddListener(() => {
                UIFrame.Instance.CloseWindow(ScreenIds.popup_select_defend_point);
            });
            this.btnConfirm.onClick.AddListener(ConfirmClick);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            GetData();
            this.defensivePointBar.SetData(this._listDefensivePoint);
            ShowInfoDfp();
        }

        private void GetData()
        {
            txtTitle.text = L.playable_mode.select_base_txt;
            this.btnConfirm.GetComponentInChildren<Text>().text = L.button.btn_confirm;
            this._DFPSelected = UserData.Instance.UserHeroDefenseData.defensePointId;
            var shopDfp = GameContainer.Instance.Get<ShopDataBase>().Get<ShopDFSPData>();
            var listDfp = shopDfp.exchangePrices;
            if (listDfp == null) return;

            this._listDefensivePoint.Clear();
            foreach (var dfp in listDfp)
            {
                var dfpSelect = new DefensivePointSelectedData();
                dfpSelect.SetData(dfp, this._DFPSelected == dfp.defensivePointId);
                this._listDefensivePoint.Add(dfpSelect);
            }
        }

        private void ConfirmClick()
        {
            UserData.Instance.UserHeroDefenseData.defensePointId = this._DFPSelected;
            EventManager.EmitEvent(GamePlayEvent.OnRefreshDefensePoint);
            UIFrame.Instance.CloseWindow(ScreenIds.popup_select_defend_point);
        }

        public void UpdateDfpSelect(int dfpId)
        {
            this._DFPSelected = dfpId;
            ShowInfoDfp();
        }

        private void ShowInfoDfp()
        {
            this.iconDFP.sprite = ResourceUtils.GetSpriteAtlas("defensive_point", $"icon_defensive_point_{this._DFPSelected}");
            
            this.txtNameDfp.text = Localization.Current.Get("playable_mode", $"defensive_point_name_{_DFPSelected}");

            this.txtDescDfp.text = Localization.Current.Get("playable_mode", $"defensive_point_skill_{_DFPSelected}");

            var numberTrial = UserData.Instance.UserHeroDefenseData.numberTrial;
            if (numberTrial > 0)
            {
                this.txtTrialLeft.text = string.Format(L.playable_mode.trial_left_txt, $"<color='#BBFD1D'>{numberTrial}</color>");
            }
            else
            {
                this.txtTrialLeft.text = string.Format(L.playable_mode.trial_left_txt, $"<color='{GameConfig.TextColorRed}'>{numberTrial}</color>");
            }
        }
    }
}