using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class DFSPItemUi : RewardUI
    {
        [SerializeField] private Image icon;
        private Reward _dfspData;
       
        public override void SetData<T>(T data)
        {
            this._dfspData = data;
            UpdateUi();
        }

        protected override void UpdateUi()
        {
            icon.sprite = ResourceUtils.GetSpriteAtlas("dfsp_icons",$"icon_defense_point_{this._dfspData.id.ToString()}");
        }

        protected override void ItemClick()
        {
        }
    }
}