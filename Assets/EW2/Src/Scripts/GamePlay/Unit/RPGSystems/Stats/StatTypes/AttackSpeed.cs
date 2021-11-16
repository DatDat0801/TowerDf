using UnityEngine;

namespace EW2
{
    public class AttackSpeed : RPGAttribute
    {
        public AttackSpeed()
        {
            // do something
        }

        public float TimeTriggerAttack()
        {
            return 1.0f / StatValue;
        }
    }
}