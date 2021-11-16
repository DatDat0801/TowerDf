using System.Collections;
using EW2.Constants;
using Hellmade.Sound;
using UnityEngine;

namespace EW2
{
    public class Tower2004 : Building
    {
        public SpriteRenderer imgTower;
        private TowerData2004 towerData;
        private int numberSoldierSpawn;

        public override RPGStatCollection Stats
        {
            get
            {
                if (stats == null)
                {
                    stats = new Soldier2004Stats(this, towerData.GetDataSoldierByLevel(Level));
                }

                return stats;
            }
        }
        public Tower2004BonusStat BonusStat
        {
            get
            {
                var towerStat = UserData.Instance.UserTowerData.GetTowerStat(2004);
                if (towerStat == null)
                {
                    return GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2004(1);
                }

                var upgradeLevel = towerStat.towerLevel;
                return GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2004(upgradeLevel);
            }
        }

        public override void InitTower(int towerId, TowerPointController point)
        {
            base.InitTower(towerId, point);
            towerData = TowerData as TowerData2004;
            //add stats by Tower Upgrade System
            if (BonusStat.level < 3)
            {
                numberSoldierSpawn = towerData.GetStatBarrackByLevel(Level).soldierNumber;
            }
            else
            {
                numberSoldierSpawn = towerData.GetStatBarrackByLevel(Level).soldierNumber + BonusStat.troopNumber;
            }

            DamageType = towerData.GetDataSoldierByLevel(Level).damageType;
            LoadSpriteTower(Level);
            InitSoldier();
            InitSkill();
        }

        protected override void InitSoldier()
        {
            if (Soldiers.Count == numberSoldierSpawn) return;

            if (TowerPointController)
            {
                for (int i = 0; i < numberSoldierSpawn; i++)
                {
                    GameObject goSoldier =
                        ResourceUtils.GetUnit("soldier_2004", transform.position, Quaternion.identity);
                    if (goSoldier != null)
                    {
                        goSoldier.name = $"soldier_{i}";
                        goSoldier.transform.parent = transform;
                        var soldier = goSoldier.GetComponent<Soldier2004>();
                        var soldier2004Data = towerData.GetDataSoldierByLevel(Level);

                        soldier.InitDataSoldier(i, this, TowerPointController.pointRallyDefault.position,
                            soldier2004Data);
                        soldier.SetSkin(Level);

                        Soldiers.Add(soldier);
                    }
                }
            }
        }

        public void RallySoldier(Vector3 pointRally)
        {
            for (int i = 0; i < Soldiers.Count; i++)
            {
                Soldier2004 soldier2004 = Soldiers[i] as Soldier2004;
                if (soldier2004 != null && soldier2004.IsAlive)
                {
                    soldier2004.RallyController.Rally(pointRally);
                    soldier2004.Rally();
                }
            }
        }

        protected override void LoadSpriteTower(int level)
        {
            if (imgTower == null)
                imgTower = GetComponent<SpriteRenderer>();
            imgTower.sprite = ResourceUtils.GetSpriteAtlas("tower", $"tower_2004_lv{level}");
        }

        protected override void InitSkill()
        {
            Tower2004Skill1 skillCounter = new Tower2004Skill1();
            skillCounter.Init(this, BranchType.Skill1, SkillType.Passive, this.towerData);
            skillController.AddSkill(skillCounter);

            Tower2004Skill2 skillBuff = new Tower2004Skill2();
            skillBuff.Init(this, BranchType.Skill2, SkillType.Passive, this.towerData);
            skillController.AddSkill(skillBuff);
        }

        public override void ShowPreview(int towerId, TowerPointController point)
        {
            LoadSpriteTower(1);
            SetSortLayerForTutorial();
        }

        public override void HidePreview()
        {
        }
        public override void SetSortLayerForTutorial()
        {
            SetLayerTower(2, SortingLayerConstants.TUTORIAL);
        }
        public override void SetSortLayerForGamePlay()
        {
            SetLayerTower(1, SortingLayerConstants.UNIT);
        }

        private void SetLayerTower(int startOrder, string layer)
        {
            SpriteRenderer towerRenderer = this.GetComponent<SpriteRenderer>();
            towerRenderer.sortingLayerName = layer;
            towerRenderer.sortingOrder = startOrder;
        }
        #region Raise

        protected override void RaiseSoldiers()
        {
            CoroutineUtils.Instance.StartCoroutine(SetRaiseToSoldier());
        }

        private IEnumerator SetRaiseToSoldier()
        {
            if (Level >= 3)
            {
                EazySoundManager.PlaySound(ResourceSoundManager.GetSoundTower(Id, SoundConstant.Tower2004UpgradeLvl3), EazySoundManager.GlobalSoundsVolume);
            }
            foreach (var soldierBase in Soldiers)
            {
                var soldier = soldierBase as Soldier2004;

                if (soldier != null)
                {
                    soldier.OnRaise(Level, towerData.GetDataSoldierByLevel(Level));
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        public override void OnLevelChange(int level)
        {
            ((Soldier2004Stats)Stats).UpdateStats(towerData.GetDataSoldierByLevel(level));
            base.OnLevelChange(level);
        }

        #endregion

        public override TowerType TowerType
        {
            get => TowerType.Barrack;
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        protected override void InitAction()
        {
        }

        protected override void RemoveSoldiers()
        {
            foreach (var soldierBase in Soldiers)
            {
                soldierBase.Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue = soldierBase.Stats.GetStat<HealthPoint>(RPGStatType.Health).StatBaseValue;
                soldierBase.StatusController.RemoveAll();
                soldierBase.HealthBar.ResetHealthBar();
                soldierBase.Stats.GetStat<HealthPoint>(RPGStatType.Health).RemoveShieldPoint();
                var shield = soldierBase.GetComponentInChildren<ShieldBarController>();
                if (shield != null)
                {
                    shield.gameObject.SetActive(false);
                }
                //LeanPool.Despawn(soldierBase.gameObject);
                Destroy(soldierBase.gameObject);
            }

            Soldiers.Clear();
        }


    }
}
