using TigerForge;
using UnityEngine;
using UnityEngine.EventSystems;
using Zitga.UIFramework;


namespace EW2
{
    public class DefensivePointBase : Dummy, IPointerClickHandler
    {
        [SerializeField] protected SpriteRenderer destructBaseSprite;

        // private HealthBarController _healthBar;
        //
        // public HealthBarController HealthBar
        // {
        //     get
        //     {
        //         if (this._healthBar == null)
        //             this._healthBar = GetComponentInChildren<HealthBarController>(true);
        //
        //         return this._healthBar;
        //     }
        // }

        private UnitState _unitState;
        public override UnitState UnitState => this._unitState ?? (this._unitState = new DummyState(this));

        protected AllySearchTarget searchTarget;

        public AllySearchTarget SearchTarget
        {
            get
            {
                if (searchTarget == null)
                {
                    searchTarget = GetComponentInChildren<AllySearchTarget>();
                }

                return searchTarget;
            }
        }

        private DefensivePointSpine _defensivePointSpine;

        public override UnitSpine UnitSpine =>
            this._defensivePointSpine ?? (this._defensivePointSpine = new DefensivePointSpine(this));

        protected DefensivePointData defensivePointData;
        private bool _initialized;

        #region ActionProperties

        protected UnitAction die;
        protected UnitAction getHurt;
        protected UnitAction idle;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            InitAction();
        }


        public override void OnUpdate(float deltaTime)
        {
        }

        public async override void Remove()
        {
            this.die.Execute();
            SetActiveSearchTarget(false);
            ResourceUtils.GetVfx("Defensive_point", "8005_destruct", transform.position, Quaternion.identity);
            this.destructBaseSprite.enabled = true;
            //sfx
            // var audioClip = ResourceUtils.LoadSound(SoundConstant.SPELL_4011_DISAPPEAR);
            // EazySoundManager.PlaySound(audioClip);
            //await UniTask.Delay(TimeSpan.FromSeconds(3.5f));
            //LeanPool.Despawn(gameObject);
        }

        protected void SetActiveSearchTarget(bool isActive)
        {
            searchTarget.gameObject.SetActive(isActive);
        }

        protected override void InitAction()
        {
            SearchTarget.SetBlockNumber(int.MaxValue);
            this.die = new Die(this);
            this.getHurt = new GetHurt(this);
            this.idle = new Idle(this);
            var healthPoint = Stats.GetStat<HealthPoint>(RPGStatType.Health);
            if (HealthBar)
            {
                HealthBar.SetHealthBar(healthPoint);
            }

            healthPoint.CurrentValue = healthPoint.StatValue;
            UnitSpine.Idle();
            this.destructBaseSprite.enabled = false;
        }

        protected int getHurtCount = 1;

        public override void GetHurt(DamageInfo damageInfo)
        {
            base.GetHurt(damageInfo);
            var healthPoint = Stats.GetStat<HealthPoint>(RPGStatType.Health);
            EventManager.EmitEventData(GamePlayEvent.OnHealthPointDFPUpdate, (int)(healthPoint.CurrentValue));
            
            if (IsAlive == false) return;
            int x = (int)((1 - healthPoint.CalculateCurrentPercent()) * 100);
            if (x / 25 >= this.getHurtCount)
            {
                this.getHurt.Execute();
                ResourceUtils.GetVfx("Defensive_point", "8005_get_hurt", transform.position, Quaternion.identity);
                this.getHurtCount++;
            }
        }

        #region Not implement

        public override void AttackMelee()
        {
            throw new System.NotImplementedException();
        }

        public override void AttackRange()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public void OnPointerClick(PointerEventData eventData)
        {
            var properties = new DefensiveBuffWindowProperties(this._initialized);
            if (this._initialized == false)
            {
                this._initialized = true;
            }
            UIFrame.Instance.OpenWindow(ScreenIds.popup_buff_dsp, properties);
        }
    }
}