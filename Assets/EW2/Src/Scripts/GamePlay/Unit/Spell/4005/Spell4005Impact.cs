using Invoke;
using Lean.Pool;
using UnityEngine;

namespace EW2.Spell
{
    public class Spell4005Impact : MonoBehaviour
    {
        //[SerializeField] protected Collider2D damageBoxAOE;

        private SpellStatBase _spellStat;
        private RegenHpInArea _regenHpInArea;
        private Dummy _triggerAlly;
        private Spell4005SkillData _skillData4005;
        public void Initialize(SpellStatBase statBase, Spell4005SkillData skillData, Unit spell, HeroBase hero)
        {
            owner = spell;
            this._skillData4005 = skillData;
            this._spellStat = statBase;
            transform.localScale = new Vector2(statBase.range / 1.5f, statBase.range / 1.5f);
            //Trigger(0.2f, 0.15f);
            InvokeProxy.Iinvoke.Invoke(this, RemoveFx, this._spellStat.duration);
        }

        // 1 - 1.5
        //  Y - X
        protected Unit owner;

        protected void RemoveFx()
        {
            LeanPool.Despawn(this);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(tag))
            {
                var body = other.GetComponent<BodyCollider>();
                if (body != null)
                {
                    this._triggerAlly = body.Owner;
                    this._regenHpInArea = new RegenHpInArea(new StatusOverTimeConfig()
                    {
                        owner = this._triggerAlly,
                        creator = owner,
                        lifeTime = this._spellStat.duration,
                        intervalTime = GameConfig.IntervalRegeneration,
                        delayTime = 0.25f,
                        baseValue = this._skillData4005.healActiveHp
                    })
                    {
                        Stacks = true,
                    };
                    this._regenHpInArea.Execute(delegate
                    {
                        var spell4005 = (Spell4005) owner;
                        spell4005.vfx.DoPlayEffect(0.6f, 0f);
                    });
                    //vfx
                    // body.Owner.StatusController.AddStatus(
                    //     new HealOverTime(new StatusOverTimeConfig() {lifeTime = 2f, owner = body.Owner}));
                    Debug.Log($"<color=green>{this.owner.Id.ToString()} Start regen HP for ally {body.Owner.Id.ToString()}, Added {_regenHpInArea.Value.ToString()}</color>");
                }
            }
        }


        protected void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(tag))
            {
                var ally = other.GetComponent<BodyCollider>();
                this._triggerAlly = null;
                if (ally != null)
                {
                    Debug.LogAssertion($"Ally {ally.Owner.Id} Exit");
                    this._regenHpInArea.Stop();
                }
            }
        }
    }
}