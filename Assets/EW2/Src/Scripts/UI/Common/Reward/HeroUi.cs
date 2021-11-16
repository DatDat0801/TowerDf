using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class HeroUi : RewardUI
    {
        private Reward data;
        [SerializeField] private Image icon;

        [SerializeField] private Text txtValue;

        public override void SetData<T>(T data)
        {
            this.data = data;

            UpdateUi();
        }

        protected override void UpdateUi()
        {
            icon.sprite = ResourceUtils.GetSpriteHeroIcon($"hero_icon_{data.id}");
            if (this.data.number > 0)
                txtValue.text = this.data.number.ToString();
            else
                this.txtValue.text = "";
        }

        protected override void ItemClick()
        {
            var data = new ItemInfoWindowProperties(this.data.type, this.data.id, this.data.itemType, this.data.number);
            UIFrame.Instance.OpenWindow(ScreenIds.item_info, data);
        }
    }
}