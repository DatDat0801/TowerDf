using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class TauntStatusOverTime : StatusOverTime
    {
        public TauntStatusOverTime(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.Taunt;

            Stacks = false;
        }

        public override void UpdateValue()
        {
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            if (this.config.owner.StatusController.CanDoActionCCStatus(StatusType.Taunt))
            {
                EnemyBase enemy = (EnemyBase)config.owner;
                enemy.blockCollider.SetBlock((HeroBase)config.creator);
            }
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
            if(enemy == null) return;
            enemy.blockCollider.RemoveBlock();
        }
    }
}