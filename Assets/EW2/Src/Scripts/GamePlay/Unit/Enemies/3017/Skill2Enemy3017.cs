using Cysharp.Threading.Tasks;
using Lean.Pool;
using Map;
using Spine;
using Spine.Unity;
using TigerForge;
using UnityEngine;
using Event = Spine.Event;


namespace EW2
{
    [System.Serializable]
    public class Skill2Enemy3017 : SkillEnemy
    {
        [System.NonSerialized] public EnemyData3017.EnemyData3017Skill2 skill2Data;

        [SerializeField] [SpineAnimation()] private string skill2AnimationName;
        [SerializeField] private int firstMiniBossId;
        [SerializeField] private int secondMiniBossId;
        [SerializeField] private int firstMiniBossSpawnLineId;
        [SerializeField] private int secondMiniBossSpawnLineId;
        [SerializeField] private Transform firstMiniBossSpawnTransform;
        [SerializeField] private Transform secondMiniBossSpawnTransform;

        private AnimationEventUnit animationEventUnit = new AnimationEventUnit();
        private Enemy3017 enemy;

        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);
            enemy = (Enemy3017)enemyBase;
            animationEventUnit.CompleteAnimation = CompleteAnimation;
            animationEventUnit.InitAnimationEvents(enemy.UnitSpine.SkeletonAnimation);
            skill2Data = skill2Data ?? GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3017>()
                .GetSkill2(enemyBase.Level);
        }

        public override void CastSkill()
        {
            enemy.UnitState.Set(ActionState.Skill2);
            ExecuteSkillAnimation();
        }

        private void ExecuteSkillAnimation()
        {
            enemy.enemySpine.AnimationState.SetAnimation(0, skill2AnimationName, false);
            AppearMiniBoss();
        }

        private void AppearMiniBoss()
        {
            var firstMiniBossLineController =
                GamePlayController.Instance.SpawnController.MapController.lines[firstMiniBossSpawnLineId];
            var secondMiniBossLineController =
                GamePlayController.Instance.SpawnController.MapController.lines[secondMiniBossSpawnLineId];
            SpawnMiniBoss(firstMiniBossId, firstMiniBossSpawnTransform, Vector3.one, firstMiniBossLineController);
            SpawnMiniBoss(secondMiniBossId, secondMiniBossSpawnTransform, Vector3.one, secondMiniBossLineController);
        }

        private void SpawnMiniBoss(int idMiniBoss, Transform spawnTransform, Vector3 localScale,
            LineController lineController)
        {
            if (!enemy.StatusController.CanUseSkill())
            {
                return;
            }

            var position = spawnTransform.position;
            var enemyControl = GamePlayController.Instance.SpawnController.SpawnEnemy(idMiniBoss, position,
                lineController.GetPathWaypoints());
            enemyControl.transform.localScale = localScale;
        }

        private void CompleteAnimation(TrackEntry trackEntry)
        {
            var animationName = trackEntry.Animation.Name;
            if (animationName == skill2AnimationName)
            {
                EventManager.EmitEventData(GamePlayEvent.OnEnemyDie, this.enemy.Id);
                LeanPool.Despawn(enemy);
            }
        }
    }
}