using System;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.ContextSystem;
using Zitga.Update;

namespace EW2
{
    public class SkillButton : MonoBehaviour, IUpdateSystem, UIGameplay
    {
        public Action SelectingSkillHeroBtn { get; set; }

        [SerializeField] private Image imgCooldown;

        [SerializeField] private Button button;

        [SerializeField] private Image iconSkill;

        [SerializeField] private GameObject goFocus;

        [SerializeField] private GameObject goStack;

        [SerializeField] private Text lbStack;

        private HeroBase _hero;

        private bool _isSkillDone;

        private bool _isActive;

        private float _countTime;

        private float _cooldownTime;

        private bool _isSelected;

        private bool _quickActive;

        private bool _stackSkill;

        private int _numberStack;

        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        public void InitData(HeroBase heroBase, bool isQuickActive = false, bool isStack = false, int stackNumber = 0)
        {
            this._hero = heroBase;

            this._cooldownTime = this._hero.GetCoolDownTime();

            this._countTime = this._cooldownTime;

            this._quickActive = isQuickActive;

            this._stackSkill = isStack;

            this._numberStack = stackNumber;

            InitUi();
        }

        private void InitUi()
        {
            goFocus.SetActive(false);

            goStack.SetActive(false);

            lbStack.text = this._numberStack.ToString();

            imgCooldown.gameObject.SetActive(false);

            imgCooldown.fillAmount = 1;

            iconSkill.sprite = ResourceUtils.GetSpriteHeroIcon($"hero_{this._hero.HeroStatBase.id}_skill_0");
        }

        public void OnEnable()
        {
            this._isActive = true;
            UpdateCooldown();
            Context.Current.GetService<GlobalUpdateSystem>().Add(this);
        }

        public void OnDisable()
        {
            if (Context.Current != null)
                Context.Current.GetService<GlobalUpdateSystem>().Remove(this);
        }

        private void OnDestroy()
        {
            if (this._hero != null)
                EventManager.StopListening(GamePlayEvent.SelectingHero + $"{this._hero.name}", Close);

            this._hero = null;

            EventManager.StopListening(GamePlayEvent.OnTargetDone, TargetDone);

            EventManager.StopListening(GamePlayEvent.UIStateChanged, UiStateChanged);
        }

        #region UI Gameplay

        public void Open()
        {
            this._hero.Aura.SetActive(true);

            goFocus.SetActive(true);

            if (this._stackSkill)
            {
                lbStack.text = this._numberStack.ToString();

                goStack.SetActive(true);
            }

            this._isSelected = true;
            EventManager.StartListening(GamePlayEvent.SelectingHero + $"{this._hero.name}", Close);

            EventManager.StartListening(GamePlayEvent.OnTargetDone, TargetDone);

            EventManager.StartListening(GamePlayEvent.UIStateChanged, UiStateChanged);
        }

        public void Close()
        {
            if (this._hero)
                this._hero.Aura.SetActive(false);
            if (this.goFocus)
                goFocus.SetActive(false);
            if(goStack)
                goStack.SetActive(false);
            if (this._isSkillDone)
            {
                EventManager.StopListening(GamePlayEvent.SelectingHero + $"{this._hero.name}", Close);
            }

            EventManager.StopListening(GamePlayEvent.OnTargetDone, TargetDone);

            EventManager.StopListening(GamePlayEvent.UIStateChanged, UiStateChanged);

            this._isSelected = false;
        }

        public UI_STATE GetUIType()
        {
            return UI_STATE.ActiveSkill;
        }

        #endregion

        private void UseActiveSkill()
        {
            if (this._isActive)
            {
                this._hero.UseSkillActive();

                this._isActive = false;

                this._countTime = this._hero.GetCoolDownTime();

                EventManager.EmitEventData(GamePlayEvent.OnHeroUseActiveSkill, this._hero.Id);
            }
        }

        public void Deactive()
        {
            this._cooldownTime = this._hero.GetReviveTime();

            this._countTime = this._cooldownTime;

            imgCooldown.gameObject.SetActive(true);

            this._isActive = false;

            goFocus.SetActive(false);

            GamePlayUIManager.Instance.CloseCurrentUI(true);
        }

        public void OnUpdate(float deltaTime)
        {
            if (this._isActive)
            {
                return;
            }

            this._countTime -= deltaTime;

            if (this._countTime <= 0)
            {
                this._isActive = true;

                this._isSkillDone = false;

                imgCooldown.fillAmount = 1;

                imgCooldown.gameObject.SetActive(false);
            }

            UpdateCooldown();
        }

        private void UpdateCooldown()
        {
            imgCooldown.fillAmount = Mathf.Clamp01(this._countTime / this._cooldownTime);
        }

        #region GameEvent

        void TargetDone()
        {
            Vector3 position = EventManager.GetData<Vector3>(GamePlayEvent.OnTargetDone);
            //Select pos active skill
            ActiveSelectedHeroSkillToTarget(position);
            GamePlayUIManager.Instance.CloseCurrentUI(true);
            if (!this._quickActive && !this._isSkillDone)
            {
                goFocus.SetActive(true);
                this._isSelected = true;
            }
        }

        public void ActiveSelectedHeroSkillToTarget(Vector3 position)
        {
            if (this._isSelected)
            {
                this._hero.ActiveSkillToTarget(position, OnSkillDone);
            }
        }

        private void OnSkillDone()
        {
            this._isSkillDone = true;
            if (!this._stackSkill)
            {
                GamePlayUIManager.Instance.CloseCurrentUI(true);
                Close();
                this._countTime = this._cooldownTime;

                imgCooldown.gameObject.SetActive(true);

                this._isActive = false;

                EventManager.EmitEventData(GamePlayEvent.OnHeroUseActiveSkill, this._hero.Id);
                // Debug.Log($"{hero.HeroStatBase.id} fired the skill");
            }
            else // skill stack
            {
            }
        }

        void UiStateChanged()
        {
            GamePlayUIManager.Instance.CloseCurrentUI(true);
        }

        #endregion


        public void OnClick()
        {
            if (!this._isActive) return;

            this._cooldownTime = this._hero.GetCoolDownTime();

            if (this._quickActive)
            {
                if (!this._hero.CanControl) return;
                UseActiveSkill();
                imgCooldown.gameObject.SetActive(true);
            }
            else
            {
                if (this._isSelected)
                {
                    this._hero.CancelActiveSkill();
                    Close();
                    GamePlayUIManager.Instance.CloseCurrentUI(true);
                }
                else
                {
                    SelectingSkillHeroBtn?.Invoke();
                    GamePlayUIManager.Instance.TryOpenUI(this);
                }
            }
        }

        public void DecreaseCooldown(float amount)
        {
            this._countTime -= amount;
        }
        
    }
}