using System;
using Cysharp.Threading.Tasks;
using Invoke;
using Spine.Unity;
using UnityEngine;

namespace EW2.Spell
{
    public class TriggerBombCollider : ColliderTrigger<Bomb>
    {
        public Spell4006ExplosionImpact explosionImpact;
        [SerializeField] private SkeletonAnimation skeletonAnimation;

        private Vector3 _spawnPosition;

        private const string APPEAR_ANIMATION = "appear";
        private const string IDLE_ANIMATION = "loop";
        private const string SPAWN_ANIMATION = "start";

        private bool _triggered;

        private SpellStatBase _spellData;
        // protected override void OnEnable()
        // {
        //     triggered = false;

        // }

        public async void Initialize(SpellUnitBase spell, Vector3 pos, float startDelay)
        {
            this._triggered = false;
            this.collider2D.enabled = false;
            this._spellData = spell.SpellStatBase;
            explosionImpact.SpellData(this._spellData, spell);
            // var circleCollider = (CircleCollider2D) damageBoxAOE;
            // circleCollider.radius = statBase.range;
            var spawnEntry = skeletonAnimation.AnimationState.SetAnimation(0, SPAWN_ANIMATION, true);
            await UniTask.Delay(TimeSpan.FromSeconds(startDelay));
            var appearEntry = skeletonAnimation.AnimationState.SetAnimation(0, APPEAR_ANIMATION, false);
            appearEntry.Complete += entry => {
                this.collider2D.enabled = true;
                skeletonAnimation.AnimationState.SetAnimation(0, IDLE_ANIMATION, true);
                Trigger(int.MaxValue);
            };
            // appearEntry.Complete += entry => 
            // {
            //
            // };
            this._spawnPosition = pos;
        }

        public void InitBombTrigger()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //Debug.LogAssertion($"Triggered bomb= {this._triggered}");
            if (!other.CompareTag(tag) && this._triggered == false)
            {
                var body = other.GetComponent<BodyCollider>();
                var enemy = (EnemyBase)body.Owner;
                if (body != null && enemy.MoveType == MoveType.Ground)
                {
                    //Explosion();
                    this._triggered = true;
                    explosionImpact.Explosion(this._spawnPosition, this._spellData.range);
                    //Debug.LogAssertion($"Explosion affected on {body.Owner.Id} Bomb ID {this.GetHashCode()}");
                }
            }
        }
    }
}