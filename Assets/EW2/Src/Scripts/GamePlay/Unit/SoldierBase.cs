using UnityEngine;

namespace EW2
{
    public abstract class SoldierBase : Dummy
    {
        protected Idle idle;
        protected Building owner;
        protected int idSoldier;

        public int IdSoldier
        {
            get => idSoldier;
        }

        public virtual GameObject Aura { get; }
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

        protected override void Awake()
        {
            base.Awake();

            UnitType = UnitType.Hero;
        }

        protected override void InitAction()
        {
            idle = new Idle(this);
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        #region Action

        public override void Remove()
        {
            StatusController.RemoveAll();

            Stats.ClearStatModifiers();
        }

        public override void AttackMelee()
        {
        }

        public override void AttackRange()
        {
        }

        public virtual void SetSkin(int levelTower)
        {
            var nameSkin = $"{owner.Id}_lv_0" + levelTower;
            UnitSpine.SetSkinSpine(nameSkin);
        }

        #endregion

        public override void Flip(float positionX)
        {
            var flip = Transform.position.x > positionX;
            if ((Transform.localScale.x > 0 && flip) || (Transform.localScale.x < 0 && flip == false))
            {
                var localScale = Transform.localScale;
                localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
                Transform.localScale = localScale;
            }

            FlipComponent();
        }
    }
}