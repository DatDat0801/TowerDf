using System;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class DefensivePointSelectedData
    {
        public int defensivePointId;

        public bool isSelected;

        public void SetData(ShopDFSPItemData data, bool isItemSelected)
        {
            this.defensivePointId = data.defensivePointId;
            this.isSelected = isItemSelected;
        }
    }

    public class ItemDefensivePointController : EnhancedScrollerCellView
    {
        [SerializeField] private Image iconDFP;

        [SerializeField] private Text txtTrial;

        [SerializeField] private GameObject goTrial;

        [SerializeField] private GameObject iconTick;

        [SerializeField] private GameObject iconLock;

        [SerializeField] private Button buttonDFP;

        private Action<int> _onClickCb;
        private DefensivePointSelectedData _dFpData;

        private void Awake()
        {
            this.buttonDFP.onClick.AddListener(ItemClick);
        }

        private void ItemClick()
        {
            if (this.iconTick.activeSelf) return;

            if (this.iconLock.activeSelf)
            {
                var data = new UnlockDefendPointProperty(this._dFpData.defensivePointId);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_unlock_defend_point, data);
            }
            else
            {
                this._onClickCb?.Invoke(this._dFpData.defensivePointId);
            }
        }

        public void SetData(DefensivePointSelectedData data, Action<int> itemClickCb)
        {
            this._dFpData = data;
            this._onClickCb = itemClickCb;
            ShowUi();
        }

        private void ShowUi()
        {
            var userData = UserData.Instance.UserHeroDefenseData;
            this.iconDFP.sprite = ResourceUtils.GetSpriteAtlas("defensive_point", $"icon_defensive_point_{this._dFpData.defensivePointId}");
            this.iconTick.SetActive(this._dFpData.isSelected);
            this.iconLock.SetActive(false);
            this.goTrial.SetActive(false);
            if (!userData.CheckDefensePointUnlocked(this._dFpData.defensivePointId))
            {
                if (userData.numberTrial > 0)
                {
                    this.txtTrial.text = L.playable_mode.trial_txt;
                    this.goTrial.SetActive(true);
                    this.iconLock.SetActive(false);
                }
                else
                {
                    this.goTrial.SetActive(false);
                    this.iconLock.SetActive(true);
                }
            }
        }
    }
}
