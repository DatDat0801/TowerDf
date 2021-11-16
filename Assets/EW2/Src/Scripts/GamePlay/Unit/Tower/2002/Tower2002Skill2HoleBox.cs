using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using Invoke;
using Lean.Pool;
using System;
using UnityEngine;

namespace EW2
{
    public class Tower2002Skill2HoleBox : MonoBehaviour
    {
        private const int TowerId = 2002;

        [SerializeField] private Collider2D collider;

        [SerializeField] private int delayspawn = 1000;

        private int maxTeleport;

        private int numberTeleport;

        private bool CanTeleport => numberTeleport < maxTeleport;

        private void OnEnable()
        {
            collider.enabled = false;
        }

        private void OnDisable()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this, Despawn);
        }

        protected EnemyBase GetTarget(Collider2D other)
        {
            if (other.CompareTag(TagName.Enemy) == false)
                return null;

            var unitCollider = other.GetComponent<EnemyBlockCollider>();

            if (unitCollider == null)
                throw new Exception("unit is null");
            try
            {
                return (EnemyBase)unitCollider.Owner;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void Init(int maxTeleport, float lifeTime)
        {
            numberTeleport = 0;

            this.maxTeleport = maxTeleport;

            InvokeProxy.Iinvoke.Invoke(this, Despawn, lifeTime);
        }

        public void Trigger(float timeDelay)
        {
            collider.enabled = false;

            CoroutineUtils.Instance.DelayTriggerCollider(collider, float.MaxValue, timeDelay);
            //CoroutineUtils.Instance.DelayTriggerColliderContinuous(collider, 1000, timeDelay);
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!CanTeleport)
            {
                return;
            }

            var target = GetTarget(other);
            if (target.MySize == UnitSize.Boss || target.MySize == UnitSize.MiniBoss || target.MoveType == MoveType.Fly)
            {
                return;
            }

            if (target.MoveType != MoveType.Fly)
            {
                Teleport(target);

                numberTeleport++;

                if (!CanTeleport)
                {
                    Despawn();
                }
            }
        }

        private void Teleport(EnemyBase target)
        {
            Debug.Log("Teleport: " + target.name);

            EazySoundManager.PlaySound(
                ResourceSoundManager.GetSoundTower(TowerId, SoundConstant.Tower2002ImpactTeleport),
                EazySoundManager.GlobalSoundsVolume);

            SpawnVfxIn(target.transform.position);
            //StartCoroutine(ResetMovePathCorontine(target));
            ResetMovePath(target);
        }

        private void SpawnVfxIn(Vector3 position)
        {
            var go = ResourceUtils.GetVfxTower("2002_skill_2_impact_teleport_in", position, Quaternion.identity);
            LeanPool.Despawn(go, 1.5f);
            //go.AddComponent<DestroyMe>().deathtimer = 1.5f;
        }

        private void Despawn()
        {
            LeanPool.Despawn(gameObject);
        }
        public async void ResetMovePath(EnemyBase target)
        {
            var towerStat = UserData.Instance.UserTowerData.GetTowerStat(2002);
            Vector3 backPos;
            if (towerStat != null)
            {
                int upgradeLevel = towerStat.towerLevel;
                Tower2002BonusStat tower2002BonusStat =
                    GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2002(upgradeLevel);
                backPos = target.LineController.GetBackwardPosition(transform.position, tower2002BonusStat.distanceUnit);
                if (upgradeLevel >= 3)
                {
                    target.ResetMove();
                }
                else
                {
                    DisapearTarget(ref target, backPos);
                    await UniTask.Delay(delayspawn);
                    SpawnTarget(ref target, backPos);
                    //Debug.LogAssertion($"Teleport {Id} to position {backPos}");
                }
            }
            else
            {
                //null user data
                Tower2002BonusStat tower2002BonusStat = GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2002(1);
                backPos = target.LineController.GetBackwardPosition(transform.position, tower2002BonusStat.distanceUnit);
                DisapearTarget(ref target, backPos);
                await UniTask.Delay(delayspawn);
                SpawnTarget(ref target, backPos);
                //Debug.LogAssertion($"Tower stat null, teleport {Id} to position {backPos}");
            }

            target.ResetMove();
        }

        private void DisapearTarget(ref EnemyBase target, Vector3 backPos)
        {
            var vfx = ResourceUtils.GetVfxTower("2002_skill_2_impact_teleport_out", backPos, Quaternion.identity);
            LeanPool.Despawn(vfx, 2.5f);
            target.UnitState.Set(ActionState.None);
            //gameObject.SetActive(false);
            target.Renderer.enabled = false;
            target.BodyCollider.enabled = false;
        }

        private void SpawnTarget(ref EnemyBase target, Vector3 backPos)
        {
            target.Renderer.enabled = true;
            target.BodyCollider.enabled = true;
            var points = target.LineController.CalculateRemainPathWayPoints(backPos);
            if (!points.Contains(backPos))
            {
                points.Insert(0, backPos);
                target.MoveToEndPoint(points);
            }
            else
            {
                target.MoveToEndPoint(points);
            }
        }

    }
}
