using System.Linq;
using Hellmade.Sound;
using Invoke;
using Lean.Pool;
using Spine;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class Crocodile6047Controller : Dummy
    {
        [SerializeField] private Collider2D boxTrigger;

        protected UnitState crocodileState;
        public override UnitState UnitState => crocodileState ?? (crocodileState = new DummyState(this));

        protected Crocodile6047Spine crocodileSpine;
        public override UnitSpine UnitSpine => crocodileSpine ?? (crocodileSpine = new Crocodile6047Spine(this));

        private Idle idle;

        private AttackMelee attackMelee;

        private Crocodile6047Data.DataCrocodile crocodileData;

        private Dummy targetAttack = null;

        private bool isReadyAttack;

        private GameObject warning;

        public override bool IsAlive
        {
            get { return true; }
        }

        private void Start()
        {
            boxTrigger.enabled = false;

            GetData();

            InitAction();

            EventManager.StartListening(GamePlayEvent.OnStartGame, Cooldown);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            EventManager.StopListening(GamePlayEvent.OnStartGame, Cooldown);
        }

        private void GetData()
        {
            var corcodile6047Data = GameContainer.Instance.Get<UnitDataBase>().Get<Crocodile6047Data>();

            crocodileData = corcodile6047Data.GetDataCorcodile();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (UnitState.Current == ActionState.AttackMelee)
            {
                attackMelee.Execute(deltaTime);
            }
            else if (isReadyAttack)
            {
                if (targetAttack != null)
                {
                    isReadyAttack = false;

                    if (warning)
                        LeanPool.Despawn(warning);

                    AttackMelee();
                }
            }
        }

        protected override void InitAction()
        {
            UnitState.Set(ActionState.None);

            idle = new Idle(this);

            attackMelee = new AttackMelee(this);

            attackMelee.onComplete = () => {
                targetAttack = null;

                UnitState.Set(ActionState.None);

                crocodileSpine.Dive();

                Cooldown();
            };

            crocodileSpine.Dive();
        }

        public override void Remove()
        {
        }

        private void Cooldown()
        {
            InvokeProxy.Iinvoke.CancelInvoke(this, CrocodileFloating);

            InvokeProxy.Iinvoke.Invoke(this, CrocodileFloating, crocodileData.timeCooldown / 2);

            InvokeProxy.Iinvoke.CancelInvoke(this, ReadyToAttack);

            InvokeProxy.Iinvoke.Invoke(this, ReadyToAttack, crocodileData.timeCooldown);
        }

        private void ReadyToAttack()
        {
            boxTrigger.enabled = true;

            warning = ResourceUtils.GetVfx("Assets", "6047_warning", Vector3.zero, Quaternion.identity, transform);

            isReadyAttack = true;
        }

        private void CrocodileFloating()
        {
            var track = crocodileSpine.Floating();

            track.Complete += CrocodileIdle;
        }

        private void CrocodileIdle(TrackEntry trackentry)
        {
            idle.Execute();

            trackentry.Complete -= CrocodileIdle;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag.Equals(TagName.Enemy))
            {
                var enemyBase = other.GetComponent<BodyCollider>().Owner as EnemyBase;

                if (enemyBase)
                {
                    if (enemyBase.MoveType == MoveType.Fly) return;

                    if (!crocodileData.listUnitIgnor.Contains(enemyBase.EnemyData.id) && targetAttack == null)
                    {
                        Debug.LogWarning("Add " + enemyBase.name);
                        targetAttack = enemyBase;
                    }
                }
            }
            else if (other.tag.Equals(TagName.Hero))
            {
                var heroBase = other.GetComponent<BodyCollider>().Owner as HeroBase;

                if (heroBase && targetAttack == null)
                {
                    Debug.LogWarning("Add " + heroBase.name);

                    targetAttack = heroBase;
                }
            }
        }

        public override void AttackMelee()
        {
            boxTrigger.enabled = false;
            //Sfx
            var audioClip = ResourceUtils.LoadSound(SoundConstant.ALLIGATOR_BIT);
            EazySoundManager.PlaySound(audioClip);

            InvokeProxy.Iinvoke.CancelInvoke(this, KillTarget);

            InvokeProxy.Iinvoke.Invoke(this, KillTarget, 0.5f);

            attackMelee.ResetTime();

            UnitState.Set(ActionState.AttackMelee);
        }

        public override void AttackRange()
        {
        }

        private void KillTarget()
        {
            if (targetAttack)
            {
                targetAttack.UnitState.Set(ActionState.None);

                targetAttack.Remove();

                if (targetAttack is HeroBase)
                {
                    ((HeroBase)targetAttack).OnDie();
                }
                else
                {
                    EventManager.EmitEventData(GamePlayEvent.OnEnemyDie, targetAttack.Id);
                    LeanPool.Despawn(targetAttack.gameObject);
                }
            }
        }
    }
}