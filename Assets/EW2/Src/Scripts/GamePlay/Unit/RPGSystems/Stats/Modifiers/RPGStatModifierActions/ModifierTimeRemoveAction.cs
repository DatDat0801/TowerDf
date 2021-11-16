using System.Collections;
using UnityEngine;

namespace EW2
{
    public class ModifierTimeRemoveAction : RPGStatModifierAction
    {
        private readonly float value;

        private Coroutine coroutine;

        public ModifierTimeRemoveAction(RPGStatModifier owner, float value) : base(owner)
        {
            this.value = value;
        }

        public override void Execute()
        {
            Stop();

            coroutine = CoroutineUtils.Instance.StartCoroutine(IExecute());
        }

        protected IEnumerator IExecute()
        {
            yield return new WaitForSeconds(value);
                
            owner.RemoveAction(this);
                
            owner.Remove();
        }

        public override void Stop()
        {
            if (coroutine != null)
            {
                CoroutineUtils.Instance.StopCoroutine(coroutine);
            }
        }
    }
}