using System;
using EW2.CampaignInfo.HeroSelect;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ItemHeroSelectedDefense : HeroSelectedView
    {
        [SerializeField] private Button itemButton;

        private Action _itemOnClick;

        public void SetItemClickCb(Action onClick)
        {
            this._itemOnClick = onClick;
        }

        private void Awake()
        {
            if (this.itemButton)
            {
                this.itemButton.onClick.AddListener(ItemClick);
            }
        }

        private void ItemClick()
        {
            this._itemOnClick?.Invoke();
        }
    }
}