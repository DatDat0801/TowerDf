using System;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class BuyHeroButton : MonoBehaviour
    {
        [SerializeField] private Button buyButton;
        [SerializeField] private Button eventButton;
        [SerializeField] private Button buyButtonDisable;
        private IBuyHeroButton _strategy;
        private int _heroId;

        private void Start()
        {
            this.buyButton.onClick.AddListener(OnBuyClick);
            this.eventButton.onClick.AddListener(OnOpenEvent);
            this.buyButtonDisable.onClick.AddListener(OnBuyDisableClick);
        }

        void OnOpenEvent()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.hero_room_scene);
            UIFrame.Instance.OpenWindow(ScreenIds.new_hero_event);
        }

        void OnBuyClick()
        {
            this._strategy.ButtonClick();
        }

        void OnBuyDisableClick()
        {
            this._strategy.ButtonDisableClick();
        }

        public void SetHeroId(int id)
        {
            this._heroId = id;
            SetStrategy();
            if (this.buyButtonDisable)
            {
                buyButtonDisable.gameObject.SetActive(false);
            }

            if (this._strategy != null)
                this._strategy.ChangeBehavior();
        }

        private void SetStrategy()
        {
            switch (this._heroId)
            {
                case 1004:
                    this._strategy = new BuyHero1004Button() {
                        HeroId = 1004, EventBtn = this.eventButton.gameObject, BuyBtn = this.buyButton.gameObject
                    };
                    break;
                case 1003:
                    this._strategy =
                        new BuyHeroGenericButton() {
                            HeroId = 1003,
                            EventBtn = this.eventButton.gameObject,
                            BuyBtn = this.buyButton.gameObject,
                            BuyDisableBtn = this.buyButtonDisable.gameObject
                        };
                    break;
                case 1005:
                    this._strategy = new BuyNewHeroButton() {
                        HeroId = 1005, BuyBtn = this.buyButton.gameObject, EventBtn = this.eventButton.gameObject
                    };
                    break;
                default:
                    Debug.Log($"Not supported key {this._heroId.ToString()}");
                    break;
            }
        }
    }
}