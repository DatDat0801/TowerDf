using System;
using Invoke;
using Spine;
using UnityEngine;

namespace EW2
{
    public class Enemy3018 : EnemyBase
    {
        public EnemyNormalDamageBox normalAttackBox;

        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3018>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3018Spine(this));

        public override void OnEnable()
        {
            base.OnEnable();
            UnitState.IsLockState = true;
        }

        protected void Start()
        {
            UnitState.Set(ActionState.Appear);
            var track = UnitSpine.Appear();
            track.Complete += UnlockState;
        }

        private void UnlockState(TrackEntry trackentry)
        {
            SetMoveState();
            trackentry.Complete -= UnlockState;
        }

        protected override void InitCollider(float timeDelay)
        {
            SetBlockCollider(false);
            BodyCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

            InvokeProxy.Iinvoke.Invoke(this,
                () => {
                    SetBlockCollider(true);
                    BodyCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.EnemyBodyBox);
                }, 6.6f);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!IsAlive || UnitState.IsLockState) return;
            base.OnUpdate(deltaTime);
        }

        protected override void ExecuteNoneState()
        {
        }

        public void SetMoveState()
        {
            UnitState.IsLockState = false;
            UnitState.Set(ActionState.Move);
            UnitSpine.Move();
        }
    }
}