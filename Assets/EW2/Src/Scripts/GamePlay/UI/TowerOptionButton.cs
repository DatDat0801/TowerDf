using System;
using EW2.Tutorial.General;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Observables;

namespace EW2
{
    public class TowerOptionButton : MonoBehaviour
    {
        [SerializeField] protected GameObject goBgLight, goBgDark;
        [SerializeField] protected GameObject goConfirm;
        [SerializeField] protected GameObject goIcon;
        [SerializeField] protected Text lbPrice, txtDisable;
        [SerializeField] protected Image lockIcon, currencyIcon;

        public TowerRaiseCost RaiseCost { get; protected set; }

        public Building Tower { get; private set; }

        public int Id { get; private set; }

        public int CurrentBranch { get; private set; }

        public delegate void TowerButtonEvent(TowerOptionButton t, TowerOption.Action action);

        public static TowerButtonEvent OnPressed;
        public static TowerButtonEvent OnPressedSuccess;
        public static TowerButtonEvent OnConfirmed;

        public Action BuildingSuccess { get; set; }

        protected TowerOption.Action myAction;

        protected bool isConfirm;

        protected int goldNeed;

        protected bool lockButton;

        private Button optionBtn;

        private void Awake()
        {
            OnPressedSuccess += HandleOnPressed;
            optionBtn = GetComponent<Button>();
        }

        private void OnEnable()
        {
            EventManager.StartListening(GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.Gold),
                CheckBg);
        }

        private void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.Gold),
                CheckBg);
        }

        private void Start()
        {
            CheckBg();
        }

        private void CheckBg()
        {
            var gold = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold);

            UpdateBg(gold >= goldNeed);
            
        }

        void HandleOnPressed(TowerOptionButton t, TowerOption.Action action)
        {
            if (t == null || RaiseCost == null)
            {
                return;
            }

            if (action != TowerOption.Action.Rally)
            {
                if (isConfirm)
                {
                    isConfirm = false;
                    UpdateUI();
                }
            }
        }

        public void Init(int id, TowerOption.Action action)
        {
            Id = id;

            RaiseCost = GameContainer.Instance.GetTowerCost(id);

            myAction = action;

            isConfirm = false;

            goldNeed = RaiseCost.BuildCost;

            lbPrice.text = goldNeed.ToString();
            lbPrice.transform.parent.gameObject.SetActive(true);

            goConfirm.SetActive(false);
            goIcon.SetActive(true);

            var gold = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold);

            UpdateBg(gold >= goldNeed);

            lockButton = false;
        }

        public void Init(Building building, TowerOption.Action action)
        {
            this.Tower = building;
            this.RaiseCost = building.RaiseCost;
            myAction = action;
            isConfirm = false;
            goConfirm.SetActive(false);
            goIcon.SetActive(true);

            if (action == TowerOption.Action.Sell)
            {
                SetUISell();
            }
            else if (action == TowerOption.Action.Raise)
            {
                SetUIUpgrade();
            }
            else if (action == TowerOption.Action.RaiseSkill1 || action == TowerOption.Action.RaiseSkill2)
            {
                SetUIUpgrade();
            }
            else if (action == TowerOption.Action.Rally)
            {
                lbPrice.transform.parent.gameObject.SetActive(false);
            }
        }

        public virtual void LockBtn()
        {
            if (optionBtn)
            {
                goIcon.SetActive(false);
                goBgDark.SetActive(true);
                goBgLight.SetActive(false);
                lbPrice.gameObject.SetActive(false);
                txtDisable.gameObject.SetActive(true);
                lockIcon.gameObject.SetActive(true);
                currencyIcon.gameObject.SetActive(false);
            }
        }

        public virtual void UnlockBtn()
        {
            if (optionBtn)
            {
                var gold = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold);
                if (gold >= goldNeed)
                {
                    goBgDark.SetActive(false);
                    goBgLight.SetActive(true);    
                }
                else
                {
                    goBgDark.SetActive(true);
                    goBgLight.SetActive(false);    
                }
                
                goIcon.SetActive(true);
                
                lockIcon.gameObject.SetActive(false);
                lbPrice.gameObject.SetActive(true);
                txtDisable.gameObject.SetActive(false);
                currencyIcon.gameObject.SetActive(true);
            }
        }

        private void SetUISell()
        {
            lbPrice.text = this.Tower.GetSellCost().ToString();
            lbPrice.transform.parent.gameObject.SetActive(true);
            lockButton = false;
        }

        protected virtual void SetUIUpgrade()
        {
            if (Tower.CheckUpgradeMaxLevel())
                return;

            goldNeed = Tower.GetPriceRaise();

            lbPrice.text = goldNeed.ToString();

            var gold = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold);

            UpdateBg(gold >= goldNeed);

            lbPrice.transform.parent.gameObject.SetActive(true);
            lockButton = false;
        }


        public void OnClick()
        {
            if (lockButton)
            {
                return;
            }

            var actionCanLock = myAction == TowerOption.Action.Raise || myAction == TowerOption.Action.RaiseSkill1 ||
                                myAction == TowerOption.Action.RaiseSkill2;


            if ((IsLock(CurrentBranch) || TutorialManager.Instance.IsLockUpgradeTower) && Tower != null &&
                actionCanLock)
            {
                OnPressed?.Invoke(this, TowerOption.Action.Lock);
                return;
            }
            else if (TutorialManager.Instance.IsLockUpgradeTower && myAction == TowerOption.Action.Sell)
            {
                OnPressed?.Invoke(this, TowerOption.Action.LockSell);
                return;
            }

            if (CheckActionSpendCurrency())
            {
                if (!isConfirm)
                {
                    OnPressed?.Invoke(this, myAction);
                }

                if (CheckEnoughMoney())
                {
                    if (!isConfirm)
                    {
                        OnPressedSuccess?.Invoke(this, myAction);
                    }
                    else
                    {
                        BuildingSuccess?.Invoke();
                        OnConfirmed?.Invoke(this, myAction);
                    }

                    isConfirm = !isConfirm;
                    UpdateUI();
                }
                else
                {
                    Debug.LogWarning("****Not Enough Gold*****");
                }
            }
            else if (myAction == TowerOption.Action.Rally)
            {
                isConfirm = true;
                OnConfirmed(this, myAction);
            }
            else // Test
            {
                if (!isConfirm)
                {
                    OnPressedSuccess?.Invoke(this, myAction);
                }
                else
                {
                    OnConfirmed?.Invoke(this, myAction);
                }

                isConfirm = !isConfirm;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (myAction != TowerOption.Action.Lock)
            {
                goConfirm.SetActive(isConfirm);
                goIcon.SetActive(!isConfirm);
            }
        }

        //this work with gold quantity
        protected void UpdateBg(bool canAction)
        {
            // goBgLight.SetActive(canAction);
            // goBgDark.SetActive(!canAction);
            lbPrice.color = !canAction ? new Color(1f, 0.3490196f, 0.1921569f, 1f) : Color.white;
        }

        private bool CheckEnoughMoney()
        {
            if (myAction == TowerOption.Action.Build || myAction == TowerOption.Action.Raise ||
                myAction == TowerOption.Action.RaiseSkill1 || myAction == TowerOption.Action.RaiseSkill2)
            {
                var price = long.Parse(lbPrice.text);
                if (GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold) >= price)
                    return true;
            }
            else if (myAction == TowerOption.Action.Sell)
            {
                return true;
            }

            return false;
        }

        private bool CheckActionSpendCurrency()
        {
            if (myAction == TowerOption.Action.Build || myAction == TowerOption.Action.Sell ||
                myAction == TowerOption.Action.Raise || myAction == TowerOption.Action.RaiseSkill1 ||
                myAction == TowerOption.Action.RaiseSkill2)
                return true;
            return false;
        }

        public bool IsLock(int branch)
        {
            if (Tower == null) return false;
            CurrentBranch = branch;
            return Tower.IsLock(branch);
        }
    }
}