using System;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class TargetLimitedTimeModifier
    {
        private readonly Dummy target;
        private readonly EnemyBase owner;
        private readonly float duration;

        private Coroutine coroutine;

        public TargetLimitedTimeModifier(EnemyBase owner, Dummy target, float duration)
        {
            this.target = target;
            this.owner = owner;
            this.duration = duration;
        }

        public void SetTarget()
        {
            //Stop();
            DoExecute();
            //coroutine = CoroutineUtils.Instance.StartCoroutine(DoExecute());
        }

        private async void DoExecute()
        {
            //yield return new WaitForSeconds(duration);
            //spawn 
            var healthBar = owner.Transform.Find("health_bar_frame");

            var posFx = healthBar == null ? Vector3.zero : healthBar.localPosition;

            var go = ResourceUtils.GetVfx("Effects", "fx_status_taunt", posFx, Quaternion.identity, owner.transform);
            var sizeEffect = UnitSizeConstants.GetUnitSize(owner.MySize);
            go.transform.localScale = new Vector3(sizeEffect, sizeEffect, sizeEffect);
            
            owner.blockCollider.SetBlock(target);
            owner.StatusController.AddStatus(new SilentStatus(new StatusOverTimeConfig(){lifeTime = duration, owner = owner, creator = target, statusType = StatusType.Silent}));
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            owner.blockCollider.RemoveBlock();
            //despawn
            LeanPool.Despawn(go);
        }

        // public void Stop()
        // {
        //     if (coroutine != null)
        //     {
        //         CoroutineUtils.Instance.StopCoroutine(coroutine);
        //     }
        // }
    }
}