using System;
using Cysharp.Threading.Tasks;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class FirstPurchaseButton: MonoBehaviour
    {
        [SerializeField]
        private Button firstPurchaseButton;

        #region Monobehaviour

        private void Awake()
        {
            firstPurchaseButton.onClick.AddListener(OnFirstPurchaseClick);
        }

        private void OnEnable()
        {
            EventManager.StartListening(GamePlayEvent.FIRST_PURCHASE_CLAIMED, OnFirstPurchaseClaimed);
            CheckFirstPurchase();
        }

        private void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.FIRST_PURCHASE_CLAIMED, OnFirstPurchaseClaimed);
        }

        #endregion

        void CheckFirstPurchase()
        {
            var firstPurchase = UserData.Instance.UserEventData.FirstPurchase;
            gameObject.SetActive(!firstPurchase.IsClaimed);
        }
        void OnFirstPurchaseClaimed()
        {
            gameObject.SetActive(false);
        }
        
        void OnFirstPurchaseClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.first_purchase_popup);
        }

    }
}