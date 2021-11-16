using System.Collections.Generic;
using System.Collections.Specialized;
using EW2.Constants;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class Tower2001 : Building, RangeTower
    {
        public SpriteRenderer imgTower;
        public Transform[] pointSpawnSoldier;
        private const int NumberSoldierSpawn = 2;
        public TowerData2001 towerData { get; private set; }
        private Soldier2001 soldierSelected;
        public Soldier2001 SoldierSelected => soldierSelected;
        //private EnemyBase enemyTarget;
        private List<EnemyBase> enemyTargets;

        private TowerRangeTargetCollection searchTarget;

        public TowerRangeTargetCollection SearchTarget
        {
            get
            {
                if (searchTarget == null)
                {
                    searchTarget = GetComponentInChildren<TowerRangeTargetCollection>();
                }

                return searchTarget;
            }
        }

        public Tower2001BuffAtkSpeed searchTargetBuffAtkSpeed;

        public override RPGStatCollection Stats
        {
            get
            {
                if (stats == null)
                {
                    stats = new Tower2001Stats(this, towerData.GetDataStatBaseByLevel(Level));
                }

                return stats;
            }
        }

        public override TowerType TowerType
        {
            get => TowerType.None;
        }

        private TowerAttackRange towerAttackRange;

        public void SetRangeDetect()
        {
            var rangeDetect = Stats.GetStat<RangeDetect>(RPGStatType.RangeDetect);
            SearchTarget.transform.localScale =
                Vector3.one * rangeDetect.StatValue / GameConfig.RatioConvertSizeRangeDetect;
        }

        public void OnTargetChange(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (enemyTargets == null || enemyTargets.Count <= towerData.BonusStat.targets)
                    {
                        enemyTargets =
                            SearchTarget.SelectTargets(towerData.BonusStat.targets == 0
                                ? 1
                                : towerData.BonusStat.targets);
                        //enemyTarget = SearchTarget.SelectTarget();
                        if (UnitState.Current != ActionState.UseSkill)
                            UnitState.Set(ActionState.AttackRange);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (enemyTargets.Count > 0 || enemyTargets.Count > towerData.BonusStat.targets)
                    {
                        foreach (var target in args.OldItems)
                        {
                            foreach (var newTarget in enemyTargets)
                            {
                                if (newTarget == (Unit)target)
                                {
                                    enemyTargets =
                                        SearchTarget.SelectTargets(towerData.BonusStat.targets == 0 ? 1 : towerData.BonusStat.targets);
                                    //enemyTarget = SearchTarget.SelectTarget();
                                    if (UnitState.Current != ActionState.UseSkill)
                                        UnitState.Set(ActionState.AttackRange);
                                    break;
                                }
                            }
                        }
                    }

                    break;
            }

            if (enemyTargets.Count > 0)
            {
                //print($"[Tower] {this.name} target: " + enemyTarget.name);
                foreach (var enemyBase in enemyTargets)
                {
                    print($"[Tower] {this.name} target: " + enemyBase.name);
                }
            }
            else
            {
                UnitState.Set(ActionState.None);

                ResetAttack();

                var skill1 = skillController.GetSkill(BranchType.Skill1);

                if (skill1 != null && skill1.CheckUnlocked() && skill1.IsReady)
                    skill1.StartCooldown();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            SearchTarget.Targets.CollectionChanged += OnTargetChange;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            SearchTarget.Targets.CollectionChanged -= OnTargetChange;
            if (searchTargetBuffAtkSpeed)
            {
                if (searchTargetBuffAtkSpeed != null)
                    searchTargetBuffAtkSpeed.DeactiveSkill();
            }
        }

        protected override void InitSkill()
        {
            Tower2001Skill1 skill1 = new Tower2001Skill1();
            skill1.Init(this, BranchType.Skill1, SkillType.ActiveTarget, this.towerData);
            skillController.AddSkill(skill1);

            Tower2001Skill2 skill2 = new Tower2001Skill2();
            skill2.Init(this, BranchType.Skill2, SkillType.Passive, this.towerData);
            skillController.AddSkill(skill2);
        }

        public override void ShowPreview(int towerId, TowerPointController point)
        {
            SearchTarget.gameObject.SetActive(false);

            base.InitTower(towerId, point);

            towerData = TowerData as TowerData2001;

            DamageType = towerData.GetDataStatBaseByLevel(Level).damageType;

            LoadSpriteTower(Level);

            InitSoldier();
            SetSortLayerForTutorial();
        }

        public override void HidePreview()
        {
            foreach (var soldierBase in Soldiers)
            {
                LeanPool.Despawn(soldierBase.gameObject);
            }

            Soldiers.Clear();
        }

        public override void SetSortLayerForTutorial()
        {
            SetSortLayerSoilders(2, SortingLayerConstants.TUTORIAL);
            SetLayerTower(2, SortingLayerConstants.TUTORIAL);
        }

        public override void SetSortLayerForGamePlay()
        {
            SetSortLayerSoilders(1, SortingLayerConstants.UNIT);
            SetLayerTower(1, SortingLayerConstants.UNIT);

        }

        private void SetSortLayerSoilders(int startOrder, string layer)
        {
            for (int i = 0; i < Soldiers.Count; i++)
            {
                MeshRenderer soilderRenderer = Soldiers[i].GetComponent<MeshRenderer>();
                soilderRenderer.sortingLayerName = layer;
                soilderRenderer.sortingOrder = startOrder;
            }
        }

        private void SetLayerTower(int startOrder, string layer)
        {
            SpriteRenderer towerRenderer = this.GetComponent<SpriteRenderer>();
            towerRenderer.sortingLayerName = layer;
            towerRenderer.sortingOrder = startOrder;
        }
        public override void InitTower(int towerId, TowerPointController point)
        {
            InitAction();

            InitSkill();

            searchTarget.SetTargetType(towerData.GetDataStatBaseByLevel(Level).searchTarget);

            SearchTarget.gameObject.SetActive(true);

            SetRangeDetect();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (UnitState.Current == ActionState.AttackRange || UnitState.Current == ActionState.UseSkill)
            {
                towerAttackRange.Execute(deltaTime);
            }
        }

        protected override void InitAction()
        {
            towerAttackRange = new TowerAttackRange(this, PrepareAttackState);
        }

        protected override void InitSoldier()
        {
            if (Soldiers.Count == NumberSoldierSpawn) return;

            for (int i = 0; i < NumberSoldierSpawn; i++)
            {
                GameObject goSoldier =
                    ResourceUtils.GetUnit("soldier_2001", pointSpawnSoldier[i].position, Quaternion.identity);
                if (goSoldier != null)
                {
                    goSoldier.name = $"soldier_{i}";
                    goSoldier.transform.parent = transform;
                    var soldier = goSoldier.GetComponent<Soldier2001>();
                    soldier.InitDataSoldier(i, this);
                    soldier.SetSkin(Level);
                    Soldiers.Add(soldier);
                }
            }

            SelectSoldierAttack();
        }

        #region Action

        public void PrepareAttackState()
        {
            if (enemyTargets.Count > 0)
            {
                if (skillController.CheckSkillActiveTarget())
                {
                    UnitState.Set(ActionState.UseSkill);

                    skillController.ActiveSkillTarget(BranchType.Skill1, () => {
                        if (enemyTargets.Count > 0)
                        {
                            ResetAttack();
                            SwitchSoldierAttack();
                            UnitState.Set(ActionState.AttackRange);
                        }
                    });
                }
                else
                {
                    AttackTarget();
                }
            }
        }

        public void AttackTarget()
        {
            soldierSelected.AttackTarget(enemyTargets[0], enemyTargets, () => { });
            SwitchSoldierAttack();
        }

        private void SwitchSoldierAttack()
        {
            if (enemyTargets.Count <= 0) return;
            SelectSoldierAttack();
        }


        private void SelectSoldierAttack()
        {
            if (soldierSelected == null)
            {
                soldierSelected = (Soldier2001)Soldiers[Soldiers.Count - 1];
                return;
            }

            if (soldierSelected.IdSoldier == 0)
            {
                soldierSelected = (Soldier2001)Soldiers[Soldiers.Count - 1];
            }
            else
            {
                soldierSelected = (Soldier2001)Soldiers[0];
            }
        }

        private void ResetAttack()
        {
            towerAttackRange?.ResetTime();
            for (int i = 0; i < Soldiers.Count; i++)
            {
                Soldier2001 soldier = Soldiers[i] as Soldier2001;
                if (soldier != null)
                {
                    soldier.UpdateTarget(enemyTargets);
                    soldier.Idle();
                }
            }
        }

        #endregion

        #region Raise

        public override void OnLevelChange(int level)
        {
            ((TowerStats)Stats).UpdateStats(towerData.GetDataStatBaseByLevel(level));
            base.OnLevelChange(level);
            SetRangeDetect();
        }

        #endregion

        protected override void LoadSpriteTower(int level)
        {
            if (imgTower == null)
                imgTower = GetComponent<SpriteRenderer>();
            imgTower.sprite = ResourceUtils.GetSpriteAtlas("tower", $"tower_2001_lv{level}");
        }
    }
}
