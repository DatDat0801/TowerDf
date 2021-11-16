using System;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class BulletSkill1Enemy3017 : MonoBehaviour
    {
        public Action FinishMove { get; set; }
        public Vector3 TargetPosition { get; set; }
        
        [SerializeField] private float speed;


        private void Update()
        {
            MoveToTarget();
        }


        private void MoveToTarget()
        {
            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, step);
            if (CheckFinishMove())
            {
                FinishMove?.Invoke();
                LeanPool.Despawn(this);
            }
        }

        private bool CheckFinishMove() => (transform.position - TargetPosition).sqrMagnitude <= 0.1f;
    }
}