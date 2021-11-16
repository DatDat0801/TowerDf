using UnityEngine;

namespace EW2.Spell
{
    public class TotemPassiveImpact : ColliderTrigger<HeroBase>
    {
        private Spell4011PassiveData _passiveData;
        private HeroBase _hero;
        private TerrifyStatus _terrifyStatus;

        public void Initialize(Spell4011PassiveData passiveData, HeroBase heroBase)
        {
            this._passiveData = passiveData;
            this._hero = heroBase;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(tag))
            {
                var body = other.GetComponent<BodyCollider>();
                if (body != null)
                {
                    if (((EnemyBase) body.Owner).MoveType == MoveType.Fly)
                    {
                        return;
                    }
                    
                    this._terrifyStatus = new TerrifyStatus(new StatusOverTimeConfig() {
                        creator = this._hero,
                        owner = body.Owner,
                        lifeTime = this._passiveData.duration,
                        statusType = StatusType.Terrify
                    });
                    body.Owner.StatusController.AddStatus(this._terrifyStatus);
                }
            }
        }
    }
}