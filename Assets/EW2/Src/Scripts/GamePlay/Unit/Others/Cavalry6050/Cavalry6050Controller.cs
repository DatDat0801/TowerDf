using Hellmade.Sound;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class Cavalry6050Controller : Dummy
    {
        private UnitState cavalryState;
        public override UnitState UnitState => cavalryState ?? (cavalryState = new DummyState(this));

        private Cavalry6050Spine cavalrySpine;
        public override UnitSpine UnitSpine => cavalrySpine ?? (cavalrySpine = new Cavalry6050Spine(this));

        public override bool IsAlive => true;

        private Idle idle;

        private CavalryReadySpecialAction cavalryReadySpecial;

        private bool isReady;

        private int cavalryId;

        private GameObject moveSmoke;
        

        public void InitCavalry(int id)
        {
            this.cavalryId = id;

            InitAction();

            SetIdle();
        }

        public void SetReady()
        {
            if (cavalryId == 0)
            {
                cavalryReadySpecial.ResetTime(cavalrySpine.ready2Name);

                cavalryReadySpecial.onComplete += currNameAnim =>
                {
                    if (cavalrySpine.ready2Name.Equals(currNameAnim))
                        cavalryReadySpecial.ResetTime(cavalrySpine.ready1Name);
                    else
                        cavalryReadySpecial.ResetTime(cavalrySpine.ready2Name);
                };
                
                isReady = true;

            }
            else
            {
                cavalrySpine.ReadyNormal();
            }
        }

        public void SetIdle()
        {
            isReady = false;

            if (moveSmoke)
                LeanPool.Despawn(moveSmoke);

            idle.Execute();
        }

        public void SetMove()
        {
            isReady = false;

            moveSmoke = ResourceUtils.GetVfx("Assets", "6050_move", Vector3.zero, Quaternion.identity, this.transform);
            cavalrySpine.Move();
            
        }

        public override void OnUpdate(float deltaTime)
        {
            if (isReady)
            {
                cavalryReadySpecial.Execute(deltaTime);
            }
        }

        public override void Remove()
        {
        }

        protected override void InitAction()
        {
            idle = new Idle(this);

            cavalryReadySpecial = new CavalryReadySpecialAction(this);
        }

        public override void AttackMelee()
        {
        }

        public override void AttackRange()
        {
        }
    }
}