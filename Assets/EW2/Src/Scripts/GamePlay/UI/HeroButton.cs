using System;
using Lean.Pool;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.ContextSystem;
using Zitga.UIFramework;
using Zitga.Update;

namespace EW2
{
    public class HeroButton : MonoBehaviour, UIGameplay, IUpdateSystem
    {
        public Action SelectingHeroBtn { get; set; }

        [SerializeField] private Text lbLevel;

        [SerializeField] private Button button;

        [SerializeField] private Image heroAvatar;

        [SerializeField] private Image health;

        [SerializeField] private GameObject focus;

        [SerializeField] private Material matGreyscale;
        private HealthPoint healthPoint;

        private HeroBase hero;

        private bool isActive;

        private float countTime;

        private float cooldownTime;

        private bool isSelected;

        private GameObject rangerCircle;

        private HeroInfoController heroInfo;

        private void Awake()
        {
            isActive = true;

            button.onClick.AddListener(OnClick);
        }

        private void Start()
        {
            if (lbLevel)
                lbLevel.text = (hero.Level).ToString();
        }

        private void OnDestroy()
        {
            hero = null;
            EventManager.StopListening(GamePlayEvent.OnTargetDone, TargetDone);
            EventManager.StopListening(GamePlayEvent.UIStateChanged, UiStateChanged);
        }

        public void OnEnable()
        {
            UpdateText();
            Context.Current.GetService<GlobalUpdateSystem>().Add(this);
        }

        public void OnDisable()
        {
            if (Context.Current != null)
                Context.Current.GetService<GlobalUpdateSystem>().Remove(this);
        }

        public void Open()
        {
            isSelected = true;

            ShowInfoHero();

            ShowEffect();

            EventManager.StartListening(GamePlayEvent.OnTargetDone, TargetDone);

            EventManager.StartListening(GamePlayEvent.UIStateChanged, UiStateChanged);
        }

        public void Close()
        {
            EventManager.StopListening(GamePlayEvent.OnTargetDone, TargetDone);

            EventManager.StopListening(GamePlayEvent.UIStateChanged, UiStateChanged);

            isSelected = false;

            HideEffect();

            if (heroInfo)
                heroInfo.HideInfoPopup();
        }

        public UI_STATE GetUIType()
        {
            return UI_STATE.SelectHero;
        }


        #region GameEvent

        void TargetDone()
        {
            var position = EventManager.GetData<Vector3>(GamePlayEvent.OnTargetDone);
            //Hero move
            hero.TouchMove(position);
        }

        public void MoveSelectedHeroToPosition(Vector3 position)
        {
            hero.TouchMove(position);
        }


        void UiStateChanged()
        {
            GamePlayUIManager.Instance.CloseUI(this);
        }

        #endregion

        public void InitData(HeroBase heroBase)
        {
            this.hero = heroBase;

            this.cooldownTime = hero.GetReviveTime();

            this.countTime = this.cooldownTime;

            UpdateUiWhenHeroRevive();

            InitHealthUi();
        }

        public void OnUpdate(float deltaTime)
        {
            if (isActive)
            {
                return;
            }

            countTime += deltaTime;

            if (countTime >= cooldownTime)
            {
                isActive = true;

                countTime = cooldownTime;

                hero.Revive();

                UpdateUiWhenHeroRevive();
            }

            UpdateText();
        }

        public void OnClick()
        {
            if (!isActive) return;

            if (isSelected)
            {
                GamePlayUIManager.Instance.CloseUI(this);
            }
            else
            {
                SelectingHeroBtn?.Invoke();
                EventManager.EmitEvent(GamePlayEvent.SelectingHero + $"{hero.name}");
                GamePlayUIManager.Instance.TryOpenMultiUI(this);
            }
        }

        public void SingleSelect()
        {
            GamePlayUIManager.Instance.CloseAllUI();

            GamePlayUIManager.Instance.TryOpenMultiUI(this);
        }

        private void UpdateText()
        {
            // heroAvatar.fillAmount = Mathf.Clamp01(countTime / cooldownTime);

            lbLevel.text =
                isActive ? (hero.Level).ToString() : Mathf.CeilToInt(cooldownTime - countTime).ToString();
        }

        public void Deactive()
        {
            isActive = false;

            countTime = 0;

            UpdateUiWhenHeroDie();
        }

        private void InitUi()
        {
            isActive = true;

            focus.SetActive(false);

            health.fillAmount = 1f;

            heroAvatar.sprite = ResourceUtils.GetSpriteHeroIcon($"hero_icon_{hero.HeroStatBase.id}");
        }

        private void UpdateUiWhenHeroDie()
        {
            var listImage = GetComponentsInChildren<Image>();

            foreach (var img in listImage)
            {
                img.material = matGreyscale;
            }

            button.enabled = false;

            HideEffect();

            if (heroInfo)
                heroInfo.HideInfoPopup();
        }

        private void UpdateUiWhenHeroRevive()
        {
            InitUi();

            var listImage = GetComponentsInChildren<Image>();

            foreach (var img in listImage)
            {
                img.material = null;
            }

            button.enabled = true;
        }

        private void InitHealthUi()
        {
            healthPoint = hero.Stats.GetStat<HealthPoint>(RPGStatType.Health);

            healthPoint.PropertyChanged += (sender, args) => UpdateHealthUi(healthPoint.CurrentValue);
        }

        private void UpdateHealthUi(float hp)
        {
            health.fillAmount = hp / healthPoint.StatValue;
        }

        private void ShowEffect()
        {
            if (focus)
                focus.SetActive(true);

            if (hero)
            {
                hero.SelectCircle.ShowSelectCircle();
            }

            if (hero.HeroStatBase.searchTarget == MoveType.All)
            {
                if (rangerCircle == null)
                    rangerCircle = ResourceUtils.GetUnitOther("ranger_attack_circle");

                if (rangerCircle)
                {
                    rangerCircle.GetComponent<RangerAttackCircleController>().SetTarget(hero);

                    rangerCircle.SetActive(true);
                }
            }
        }

        private void HideEffect()
        {
            if (focus)
                focus.SetActive(false);

            if (hero)
            {
                hero.SelectCircle.HideSelectCircle();
            }

            if (rangerCircle)
            {
                LeanPool.Despawn(rangerCircle);
            }
        }

        private void ShowInfoHero()
        {
            heroInfo = FindObjectOfType<HeroInfoController>();

            if (heroInfo == null)
            {
                var go = ResourceUtils.GetUnitOther("hero_information", UIFrame.Instance.MainCanvas.transform);
                if (go)
                {
                    heroInfo = go.GetComponent<HeroInfoController>();
                }
            }

            heroInfo.transform.SetParent(UIFrame.Instance.MainCanvas.transform);

            heroInfo.ShowInfo(hero);
        }
        /// <summary>
        /// Deselect this hero
        /// </summary>
        public void Deselect()
        {
            isActive = false;
            Close();
        }
    }
}
