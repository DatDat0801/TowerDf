using System;
using Hellmade.Sound;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EW2
{
    public class Soldier2002 : SoldierBase
    {
        public Transform pointSpawnBullet;

        public Tower2002 Tower { get; private set; }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Soldier2002Spine(this));

        public override RPGStatCollection Stats
        {
            get
            {
                var towerData2002 = Tower.TowerData as TowerData2002;
                if (towerData2002 != null)
                    stats = new Tower2002Stats(this, towerData2002.GetDataStatBaseByLevel(Tower.Level));

                var attackSpeed = stats.GetStat<AttackSpeed>(RPGStatType.AttackSpeed);
                attackSpeed.StatBaseValue =
                    SpineUtils.GetAnimationTime(this.UnitSpine.AnimationState, this.UnitSpine.attackRangeName);

                return stats;
            }
        }

        private TowerData2002 towerData;

        /// <summary>
        /// 
        /// </summary>
        private AttackRange attackRange;

        /// <summary>
        /// 
        /// </summary>
        protected UnitAction attackPassive1;

        /// <summary>
        /// 
        /// </summary>
        protected UnitAction attackPassive2;

        private Unit targetEnemy;

        private Action callbackAtkComplete;

        private Vector3 holePosition;

        private GameObject goHoleBlack;

        public override void OnUpdate(float deltaTime)
        {
            switch (UnitState.Current)
            {
                case ActionState.None:
                    idle.Execute();
                    break;
                case ActionState.AttackRange:
                    if (targetEnemy == null) return;
                    Flip(targetEnemy.transform.position.x);
                    attackRange.Execute(deltaTime);
                    break;
            }
        }

        public void InitDataSoldier(int soldierId, Tower2002 owner, Vector3 holePosition)
        {
            this.owner = this.Tower = owner;

            towerData = Tower.TowerData as TowerData2002;

            DamageType = owner.DamageType;

            this.idSoldier = soldierId;

            UnitSpine.Skeleton.ScaleX = -1;

            SetId(owner.Id);

            InitAction();

            RallyHole(holePosition);

            Tower.Level.ValueChanged += OnTowerLevelUp;
        }

        private void OnTowerLevelUp(object sender, EventArgs e)
        {
            ((TowerStats) Stats).UpdateStats(((TowerData2002) Tower.TowerData).GetDataStatBaseByLevel(Tower.Level));
        }

        protected override void InitAction()
        {
            base.InitAction();

            this.attackRange = new AttackRange(this);

            this.attackPassive1 = new SkillPassive1(this);

            this.attackPassive2 = new SkillPassive2(this);
        }

        #region Action

        public override bool IsAlive => true;

        public override void OnDisable()
        {
            if (goHoleBlack != null)
                LeanPool.Despawn(goHoleBlack);
            base.OnDisable();
        }

        public override void Remove()
        {
        }

        public override void AttackRange()
        {
            var idSoundRandom = Random.Range(1, 4);

            EazySoundManager.PlaySound(
                ResourceSoundManager.GetSoundTower(this.Tower.Id,
                    string.Format(SoundConstant.Tower2002Cast, idSoundRandom)),
                EazySoundManager.GlobalSoundsVolume);

            attackRange.onComplete = () =>
            {
                UnitState.Set(ActionState.None);
                attackRange.ResetTime();
                callbackAtkComplete?.Invoke();
            };
            UnitState.Set(ActionState.AttackRange);
        }


        public void Idle()
        {
            print("Idle");
            this.targetEnemy = null;
            attackRange.ResetTime();
            UnitState.Set(ActionState.None);
        }

        public void AttackTarget(Unit target, Action completeAtk)
        {
            this.targetEnemy = target;
            if (this.targetEnemy == null)
            {
                UnitState.Set(ActionState.None);
                return;
            }

            callbackAtkComplete = completeAtk;
            AttackRange();
        }

        #endregion

        public override void SetSkin(int levelTower)
        {
            var nameSkin = $"lv_" + levelTower;
            UnitSpine.SetSkinSpine(nameSkin);
        }

        public void SpawnBullet()
        {
            if (targetEnemy != null)
            {
                GameObject bullet;

                bullet = ResourceUtils.GetUnit("bullet_2002", pointSpawnBullet.position, Quaternion.identity);

                if (bullet != null)
                {
                    var angle = CaculateAngle(targetEnemy.transform.position);

                    bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

                    Bullet2002 control = bullet.GetComponent<Bullet2002>();

                    var bulletData = ((Tower2002TowerStatBase) towerData.GetDataStatBaseByLevel(Tower.Level))
                        .bulletData;

                    control.Init(this, targetEnemy, bulletData);
                }
            }
        }

        public void SpawnBulletPassive1()
        {
            ResourceUtils.GetVfxTower("2002_skill_1.1_projectile", pointSpawnBullet.position, Quaternion.identity);
        }

        public void SpawnHolePassive2(int numberTeleport, float lifeTime)
        {
            EazySoundManager.PlaySound(
                ResourceSoundManager.GetSoundTower(this.owner.Id, SoundConstant.Tower2002CastSkill2),
                EazySoundManager.GlobalSoundsVolume);

            var hole = ResourceUtils.GetVfxTower("2002_skill_2.1_black_hole", holePosition, Quaternion.identity);
            var holeScript = hole.GetComponent<Tower2002Skill2HoleBox>();
            holeScript.Init(numberTeleport, lifeTime);
            holeScript.Trigger(0.5f);
            goHoleBlack = hole;
        }

        public void UsePassive1()
        {
            attackPassive1.Execute();
        }

        public void UsePassive2()
        {
            attackPassive2.Execute();

            ResourceUtils.GetVfxTower("2002_skill_2.1_cast", Vector3.zero,
                Quaternion.identity, pointSpawnBullet);
        }

        public void RallyHole(Vector3 pointRally)
        {
            holePosition = pointRally;
        }

        private float CaculateAngle(Vector3 targetPos)
        {
            var angle = 30f;

            var dir = transform.position - targetPos;

            angle = (float) (Math.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            // if (angle < 0)
            // {
            //     angle = 180 + angle;
            // }
            // else if (angle > 0)
            // {
            //     angle = -180 + angle;
            // }

            return angle;
        }

        public override void Flip(float positionX)
        {
            var flip = Transform.position.x < positionX;
            // if ((Transform.localScale.x > 0 && flip) || (Transform.localScale.x < 0 && flip == false))
            // {
            //     var localScale = Transform.localScale;
            //     localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            //     Transform.localScale = localScale;
            // }

            if ((UnitSpine.Skeleton.ScaleX > 0 && flip) || (UnitSpine.Skeleton.ScaleX < 0 && flip == false))
            {
                var scaleX = UnitSpine.Skeleton.ScaleX;
                UnitSpine.Skeleton.ScaleX = -scaleX;
            }
        }
    }
}