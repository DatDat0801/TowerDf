using System;
using UnityEngine;

namespace EW2
{
    public class SingleMove
    {
        private readonly Unit owner;

        private MoveSpeed moveSpeed;

        private float speed;

        public float Speed
        {
            get { return speed; }

            set { speed = value; }
        }

        private Vector3 target;

        private Transform targetTransform;

        public Action onFinish = delegate { };

        public SingleMove(Unit owner)
        {
            this.owner = owner;

            InitSpeed();
        }

        private void InitSpeed()
        {
            moveSpeed = owner.Stats.GetStat<MoveSpeed>(RPGStatType.MoveSpeed);

            moveSpeed.PropertyChanged += (sender, args) => { speed = moveSpeed.StatValue; };

            speed = moveSpeed.StatValue;
        }

        public void SetTarget(Vector3 positionTarget)
        {
            if (owner.UnitState.IsLockState)
            {
                return;
            }

            this.target = positionTarget;

            owner.Flip(positionTarget.x);

            owner.UnitSpine.Move();
        }

        public void SetTarget(Transform transform)
        {
            this.targetTransform = transform;

            SetTarget(transform.position);
        }

        public void Execute()
        {
            if (targetTransform != null)
            {
                target = targetTransform.position;
            }

            float step = speed * Time.deltaTime; // calculate distance to move

            float distance = Vector2.Distance(owner.Transform.position, target);

            if (distance > step)
            {
                var position = owner.Transform.position;

                owner.Transform.position = Vector2.MoveTowards(position, target, step);
            }
            else
            {
                owner.Transform.position = target;
                targetTransform = null;
                onFinish?.Invoke();
            }

            var currentPosition = owner.Transform.position;
            var currentZCoordinatePosition = UnitPositionUtils.ComputeZCoordinateFollowYCoordinate(currentPosition.y);
            owner.transform.position = new Vector3(currentPosition.x, currentPosition.y, currentPosition.y / 10);
        }
    }
}