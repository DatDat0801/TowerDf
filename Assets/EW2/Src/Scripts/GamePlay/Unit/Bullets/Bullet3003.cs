using System;
using Constants;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class Bullet3003 : BulletMovingCurve
    {
        private RotatingObject rotatingObject;

        private void Awake()
        {
            rotatingObject = GetComponentInChildren<RotatingObject>();
        }

        public override void Init(Unit shooter, Action<Unit> onFlyFinish = null)
        {
            base.Init(shooter, onFlyFinish);
            rotatingObject.IsRotate = true;
        }

        protected override void StopMotion()
        {
            base.StopMotion();
            rotatingObject.ResetAndStopRotate();
        }
    }
}