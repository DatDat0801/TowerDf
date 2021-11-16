using System;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class VfxOverTime
    {
        public string vfxName { get; set; }
        public Unit owner { get; set; }
        
        public async void DoPlayEffect(float time, float delayTime)
        {
            if (delayTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
            }

            var go = ResourceUtils.GetVfx("Status", vfxName, Vector3.zero, Quaternion.identity,
                owner.Transform);

            if (time <= 0)
            {
                await UniTask.WaitForFixedUpdate();
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(time));
            }

            LeanPool.Despawn(go);
        }
    }
}