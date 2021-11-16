using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EW2
{
    public class CoroutineUtils : Singleton<CoroutineUtils>
    {
        public Coroutine DelayTriggerCollider(Collider2D collider2D, float time, float delayTime = 0)
        {
            return StartCoroutine(IDelayTriggerCollider(collider2D, time, delayTime));
        }
        public Coroutine DelayTriggerColliderContinuous(Collider2D collider2D, float time, float delayTime = 0)
        {
            return StartCoroutine(IDelayTriggerColliderContinuous(collider2D, time, delayTime));
        }
        private IEnumerator IDelayTriggerCollider(Collider2D collider2D, float time, float delayTime)
        {
            if (delayTime > 0)
            {
                yield return new WaitForSeconds(delayTime);
            }

            if (collider2D == null) yield break;
            collider2D.enabled = true;

            if (time <= 0)
            {
                yield return new WaitForFixedUpdate();
            }
            else
            {
                yield return new WaitForSeconds(time);
            }
            if (collider2D == null) yield break;
            collider2D.enabled = false;
        }
        private IEnumerator IDelayTriggerColliderContinuous(Collider2D collider2D, float time, float delayTime)
        {
            float runtime = 0;
            if (delayTime > 0)
            {
                yield return new WaitForSeconds(delayTime);
            }

            if (collider2D == null) yield break;
            collider2D.enabled = true;
            while (runtime < time)
            {
                collider2D.enabled = true;
                
                yield return new WaitForSeconds(0.1f);
                runtime += 0.1f;
                collider2D.enabled = false;
            }
            if (time <= 0)
            {
                yield return new WaitForFixedUpdate();
            }

            if (collider2D == null) yield break;
            collider2D.enabled = false;
        }
 
    }
}