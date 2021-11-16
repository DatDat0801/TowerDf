using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class AttractiveOverTime : StatusOverTime
    {
        public AttractiveOverTime(StatusOverTimeConfig config) : base(config)
        {
            Stacks = false;
        }

        public override void UpdateValue()
        {
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            EnemyBase enemy = (EnemyBase)config.owner;
            enemy.blockCollider.SetBlock((Dummy)config.creator);
        }

        protected override IEnumerator IExecute(UnityAction callback)
        {
            isExecuting = true;
            float runTime = 0;

            Prepare();
            EnemyBase enemy = (EnemyBase)config.owner;
            yield return new WaitForSeconds(config.delayTime);

            if (config.owner.IsAlive)
            {
                DoStatus(callback);

                while (runTime < TimeLife && config.creator.UnitState.Current != ActionState.Move)
                {
                    yield return new WaitForSeconds(1);
                    runTime += 1;
                }

                //yield return new WaitForSeconds(TimeLife);
            }

            Complete();

            Remove();
        }

        public override void Prepare()
        {
        }

        public override void Complete()
        {
            EnemyBase enemy = (EnemyBase)config.owner;
            enemy.blockCollider.RemoveBlock();
        }
    }
}