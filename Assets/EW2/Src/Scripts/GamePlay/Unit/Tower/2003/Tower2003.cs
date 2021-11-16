using System.Collections.Specialized;
using UnityEngine;

namespace EW2
{
    public class Tower2003 : Building, RangeTower
    {
        private const int NumberSoldierSpawn = 1;

        public SpriteRenderer imgLeg;

        public GameObject goLegFront;

        public Soldier2003 soldier;

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

        public TowerData2003 towerData { get; private set; }

        private TowerAttackRange towerAttackRange;

        private EnemyBase enemyTarget;

        public override RPGStatCollection Stats
        {
            get
            {
                if (stats == null)
                {
                    stats = new Tower2003Stats(this, towerData.GetDataStatBaseByLevel(Level));
                }

                return stats;
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
        }

        public override void InitTower(int towerId, TowerPointController point)
        {
            base.InitTower(towerId, point);

            towerData = TowerData as TowerData2003;

            LoadSpriteTower(Level);

            InitAction();

            InitSoldier();

            InitSkill();

            searchTarget.SetTargetType(towerData.GetDataStatBaseByLevel(Level).searchTarget);

            SearchTarget.gameObject.SetActive(true);

            SetRangeDetect();
        }

        protected override void InitAction()
        {
            towerAttackRange = new TowerAttackRange(this, PrepareAttackState);
        }

        protected override void InitSoldier()
        {
            if (Soldiers.Count == NumberSoldierSpawn) return;

            if (soldier != null)
            {
                soldier.InitDataSoldier(0, this);
                soldier.SetSkin(Level);
                Soldiers.Add(soldier);
            }
        }

        protected override void RemoveSoldiers()
        {
            Soldiers.Clear();
        }

        protected override void InitSkill()
        {
            Tower2003Skill1 skill1 = new Tower2003Skill1();
            skill1.Init(this, BranchType.Skill1, SkillType.ActiveTarget, this.towerData);
            skillController.AddSkill(skill1);

            Tower2003Skill2 skill2 = new Tower2003Skill2();
            skill2.Init(this, BranchType.Skill2, SkillType.ActiveTarget, this.towerData);
            skillController.AddSkill(skill2);
        }

        public override void ShowPreview(int towerId, TowerPointController point)
        {
            SearchTarget.gameObject.SetActive(false);

            base.InitTower(towerId, point);

            towerData = TowerData as TowerData2003;

            DamageType = towerData.GetDataStatBaseByLevel(Level).damageType;

            LoadSpriteTower(Level);

            InitSoldier();
        }

        public override void HidePreview()
        {
            Soldiers.Clear();
        }
        public override void SetSortLayerForTutorial()
        {

        }

        public override void SetSortLayerForGamePlay()
        {

        }
        protected override void LoadSpriteTower(int levelTower)
        {
            if (imgLeg == null)
                return;
            imgLeg.sprite = ResourceUtils.GetSpriteAtlas("tower", $"leg_tower_lv_{levelTower}");
            if (levelTower == 2)
                goLegFront.SetActive(true);
            else
                goLegFront.SetActive(false);
        }

        public override TowerType TowerType
        {
            get => TowerType.None;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (UnitState.Current == ActionState.AttackRange)
            {
                towerAttackRange.Execute(deltaTime);
            }
        }

        #region Action

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
                    if (enemyTarget == null)
                    {
                        enemyTarget = SearchTarget.SelectTarget();
                        if (UnitState.Current != ActionState.SkillPassive1 &&
                            UnitState.Current != ActionState.SkillPassive2)
                            UnitState.Set(ActionState.AttackRange);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (enemyTarget != null)
                    {
                        foreach (var target in args.OldItems)
                        {
                            if (enemyTarget == (Unit)target)
                            {
                                enemyTarget = SearchTarget.SelectTarget();

                                if (UnitState.Current == ActionState.SkillPassive1 ||
                                    UnitState.Current == ActionState.SkillPassive2) return;

                                if (UnitState.Current != ActionState.SkillPassive1 &&
                                    UnitState.Current != ActionState.SkillPassive2)
                                    UnitState.Set(ActionState.AttackRange);
                                break;
                            }
                        }
                    }

                    break;
            }

            if (enemyTarget == null)
            {
                UnitState.Set(ActionState.None);
                ResetAttack();
            }
        }

        public void PrepareAttackState()
        {
            if (enemyTarget != null)
            {
                if (skillController.CheckSkillActiveTarget())
                {
                    var skillActive = skillController.GetSkillActive();
                    if (skillActive != null)
                    {
                        Debug.LogWarning($"Active skill {skillActive.BranchType}");
                        if (skillActive.BranchType == BranchType.Skill1)
                            UnitState.Set(ActionState.SkillPassive1);
                        else
                            UnitState.Set(ActionState.SkillPassive2);

                        skillActive.ActiveSkill(() => {
                            UnitState.Set(ActionState.None);
                            ResetAttack();
                            if (enemyTarget != null)
                                UnitState.Set(ActionState.AttackRange);
                        });
                    }
                }
                else
                {
                    AttackTarget();
                }
            }
        }

        public void AttackTarget()
        {
            soldier.AttackTarget(enemyTarget, null);
        }

        private void ResetAttack()
        {
            towerAttackRange?.ResetTime();
            for (int i = 0; i < Soldiers.Count; i++)
            {
                var soldier = Soldiers[i] as Soldier2003;
                if (soldier != null)
                {
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
    }
}
