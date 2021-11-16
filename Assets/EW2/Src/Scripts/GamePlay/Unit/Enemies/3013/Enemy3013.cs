using Lean.Pool;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class Enemy3013 : EnemyBase
    {
        [SerializeField] private Transform vfxDieSpawnTransform;
        
        [SerializeField] private string vfxDieName;
        
        
        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3013>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3013Spine(this));

        public override void Remove()
        {
            // EventManager.EmitEventData(GamePlayEvent.OnEnemyDie, this.Id);
            SpawnControllerBase.spawnedEnemies.Remove(Id);
            //base.Remove();
            StatusController.RemoveAll();

            Stats.ClearStatModifiers();

            blockCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

            GetComponentInChildren<BodyCollider>().gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

            SetBlockCollider(false);
            
            if (!IsCompleteEndPoint)
            {
                //die.onComplete = () => { effectDieCoroutine = CoroutineUtils.Instance.StartCoroutine(EffectDie()); };
                effectDieCoroutine = CoroutineUtils.Instance.StartCoroutine(EffectDie());
                //die.Execute();
            }
            else
            {
                blockCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

                BodyCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

                //idle.Execute();
                
            }
            LeanPool.Despawn(gameObject,0.5f);
            SpawnDeadVfx();

        }

        protected void SpawnDeadVfx()
        {
            ResourceUtils.GetVfx(EffectType.Enemy.ToString(), vfxDieName, vfxDieSpawnTransform.transform.position, Quaternion.identity);
        }
    }
}