using System;
using Invoke;
using UnityEngine;
using Zitga.ContextSystem;
using Zitga.Update;

namespace EW2
{
    public abstract class Unit : MonoBehaviour, IUpdateSystem
    {
        public int Id { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public UnitType UnitType { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public PriorityTargetType PriorityTargetType { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public UnitClassType UnitClassType { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public DamageType DamageType { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public Transform Transform => transform;

        public bool Immutable { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual UnitState UnitState { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual UnitSpine UnitSpine { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        protected RPGStatCollection stats;

        public virtual RPGStatCollection Stats => stats;

        protected StatusOverTimeController statusController;

        private IUnitFlipByLocalPosition[] _componentsToBeFlipped;

        private IUnitFlipByLocalScale[] _componentsToBeFlipScale;

        public virtual StatusOverTimeController StatusController =>
            statusController ?? (statusController = new StatusOverTimeController(this));

        public virtual void OnEnable()
        {
            UnitState.Set(ActionState.None);

            if (Context.Current.GetService<GlobalUpdateSystem>() != null)
            {
                Context.Current.GetService<GlobalUpdateSystem>().Add(this);
            }
        }

        public virtual void OnDisable()
        {
            UnitState.Set(ActionState.None);

            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);

            if (Context.Current != null)
                Context.Current.GetService<GlobalUpdateSystem>().Remove(this);
        }

        protected virtual void Awake()
        {
            _componentsToBeFlipped = GetComponentsInChildren<IUnitFlipByLocalPosition>();
            _componentsToBeFlipScale = GetComponentsInChildren<IUnitFlipByLocalScale>();
        }

        public abstract void OnUpdate(float deltaTime);

        public abstract void Remove();

        protected abstract void InitAction();

        public virtual bool IsAlive => true;

        public virtual void Flip(float positionX)
        {
            var flip = Transform.position.x > positionX;
            // if ((Transform.localScale.x > 0 && flip) || (Transform.localScale.x < 0 && flip == false))
            // {
            //     var localScale = Transform.localScale;
            //     localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            //     Transform.localScale = localScale;
            // }

            if ((UnitSpine.Skeleton.ScaleX > 0 && flip) || (UnitSpine.Skeleton.ScaleX < 0 && flip == false))
            {
                var scaleX = UnitSpine.Skeleton.ScaleX;
                UnitSpine.Skeleton.ScaleX = -scaleX;

                //var damageBoxes = GetComponentsInChildren<IUnitFlipByLocalPosition>();
                for (var i = 0; i < _componentsToBeFlipped.Length; i++)
                {
                    var localPosition = _componentsToBeFlipped[i].LocalPosition;
                    localPosition = new Vector3(-localPosition.x, localPosition.y, localPosition.z);
                    _componentsToBeFlipped[i].LocalPosition = localPosition;
                }

                for (var i = 0; i < this._componentsToBeFlipScale.Length; i++)
                {
                    var localScale = _componentsToBeFlipScale[i].LocalScale;
                    localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
                    _componentsToBeFlipScale[i].LocalScale = localScale;
                }
            }
        }

        public void SetId(int id)
        {
            this.Id = id;
        }
    }
}