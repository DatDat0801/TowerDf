using System;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ButtonSummonUi : MonoBehaviour
    {
        [SerializeField] protected Text txtLabelButton;
        [SerializeField] protected Text txtLabelFree;
        [SerializeField] protected Text txtLabelPrice;
        [SerializeField] protected Image imgMoneyType;
        [SerializeField] protected Button btn;
        [SerializeField] protected GameObject panelPrice;

        protected int moneyType;
        protected SummonType summonType;
        protected Action<int, SummonType> summonCallback;
        protected long gemReq;

        private void Awake()
        {
            btn.onClick.AddListener(SummonOnClick);
        }

        public virtual void InitButton(string labelButton, bool isFree, SummonType summonType, Action<int, SummonType> summonCb)
        {
            this.summonType = summonType;
            summonCallback = summonCb;
            txtLabelButton.text = labelButton;
            if (txtLabelFree)
            {
                txtLabelFree.text = L.button.free_name;
                txtLabelFree.gameObject.SetActive(isFree);
            }

            panelPrice.SetActive(!isFree);
            ShowUiPrice();
        }

        public virtual void ShowUiPrice()
        {
        }

        protected virtual void SummonOnClick()
        {
        }
    }
}