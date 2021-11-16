using UnityEngine;

namespace EW2
{
    public class BodyCollider : MonoBehaviour
    {
        public Dummy Owner;
        
        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (Owner.IsAlive == false)
            {
                //Debug.Log($"{Owner.name} was died: ");
                return;
            }

            if (!Owner.CanChooseIsTarget)
            {
                return;
            }

            if (CompareTag(other.tag))
            {
                var objEffect = other.GetComponent<IAddEffect>();
                objEffect?.AddEffect(Owner);
            }
            else
            {
                var objDamage = other.GetComponent<IGetDamage>();

                var damageInfo = objDamage?.GetDamage(Owner);
                
                if (damageInfo != null)
                    TakeDamage(damageInfo);

                var attractive = other.GetComponent<IAttractive>();
                //Debug.LogAssertion($"other {other.transform} and parent {other.transform.parent} collided with this {transform.name} and parent {transform.parent.name}");
                attractive?.ChangeTarget(Owner);
            }
           
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            // Debug.Log($"Get more damage from {damageInfo.creator.name} to {Owner.Transform.name}");

            Owner.GetHurt(damageInfo);
        }
    }
}