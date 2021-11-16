using System;
using System.Collections.Generic;
using Invoke;
using Lean.Pool;
using UnityEngine;
using Zitga.ContextSystem;
using Zitga.Update;

namespace EW2
{
    public class CoinController : MonoBehaviour, IUpdateSystem
    {
        private Action<Vector3> onFlyFinish;
        
        private bool isFly;

        private Vector3 target;

        public virtual void OnEnable()
        {
            if (Context.Current.GetService<GlobalUpdateSystem>() != null)
            {
                Context.Current.GetService<GlobalUpdateSystem>().Add(this);
            }
        }

        public virtual void OnDisable()
        {
            if (Context.Current != null)
                Context.Current.GetService<GlobalUpdateSystem>().Remove(this);
        }
        
        public void InitCoint(Vector3 target, Action<Vector3> onFlyFinish = null)
        {
            this.target = new Vector3(target.x + 0.3f,target.y,0);

            this.onFlyFinish = onFlyFinish;

            isFly = true;
        }
        
        public void OnUpdate(float deltaTime)
        {
            if (isFly)
            {
                float step = 15f * Time.deltaTime; // calculate distance to move

                float distance = Vector3.Distance(transform.position, target);

                if (distance > step)
                {
                    var position = transform.position;

                    transform.position = Vector3.MoveTowards(position, target, step);
                }
                else
                {
                    isFly = false;

                    transform.position = target;

                    this.onFlyFinish?.Invoke(transform.position);
                    
                    LeanPool.Despawn(gameObject);
                }
            }        }
    }
}