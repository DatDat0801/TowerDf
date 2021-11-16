using System;
using Coffee.UIExtensions;
using EW2.Tools;
using TigerForge;
using UnityEngine;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class DefensiveBuffWindowProperties : WindowProperties
    {
        public bool isInitialized;

        public DefensiveBuffWindowProperties(bool isInitialized)
        {
            this.isInitialized = isInitialized;
        }
    }

    public class DefensiveBuffWindowController : AWindowController<DefensiveBuffWindowProperties>
    {
        [SerializeField] private RectTransform container;
        [SerializeField] private DefensiveBuffItemUI itemPrefab;

        private DefensiveBuffManager _manager;

        private void OnDisable()
        {
            GamePlayController.Instance?.ResumeGame();
            var eventName = GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.Gold);
            EventManager.StopListening(eventName, Refresh);
        }

        private void OnEnable()
        {
            var eventName = GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.Gold);
            EventManager.StartListening(eventName, Refresh);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            if (!Properties.isInitialized)
            {
                this._manager = new DefensiveBuffManager();
            }

            Initialize();
        }

        private void Initialize()
        {
            GamePlayController.Instance.PauseGame();
            DeviceUtils.ClearGarbage();
            this.container.DestroyAllChildren();
            int buffId = 7000;

            for (int i = 0; i < 6; i++)
            {
                buffId++;
                var item = Instantiate(this.itemPrefab, this.container);
                item.Repaint(this._manager.ConstructBuff(new BuffItem(), buffId));
                item.OnBuyItem = OnBuyBuff;
            }

            Refresh();
        }

        public void Refresh()
        {
            var gold = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold);
            foreach (Transform o in this.container)
            {
                var buffItem = o.GetComponent<DefensiveBuffItemUI>();
                buffItem.DisableBuy(buffItem.BuffItem.Price <= gold);
            }
        }

        private void OnBuyBuff(DefensiveBuffItemUI defensiveBuffItemUI)
        {
            BuffItem buffItem = defensiveBuffItemUI.BuffItem;
            var gold = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold);
            if (gold >= buffItem.Price)
            {
                GamePlayData.Instance.SubMoneyInGame(MoneyInGameType.Gold, buffItem.Price);
                _manager.buffHandler.Buff(buffItem);
                this._manager.AddBuffToUserData(buffItem.BuffData.buffId);

                defensiveBuffItemUI.Repaint(this._manager.ConstructBuff(new BuffItem(),
                    int.Parse(buffItem.BuffData.buffId)));

#if TRACKING_FIREBASE
                var battleId = UserData.Instance.UserHeroDefenseData.battleId;
                var waveCurr = CallWave.Instance.CurrWave;

                FirebaseLogic.Instance.DefenseModeBuffEarn(battleId, waveCurr, buffItem.BuffData.buffId, 1);
#endif
            }
            else
            {
                Debug.LogWarning("Not enough gold");
            }
        }
    }
}