using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using EW2.Constants;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class Tower2002 : Building, RangeTower
    {
        private const int NumberSoldierSpawn = 1;

        [SerializeField] private GameObject[] towerLevels;

        [SerializeField] private Transform[] towerSpawnPoints;

        public TowerData2002 towerData { get; private set; }

        private EnemyBase enemyTarget;

        public Soldier2002 Soldier { get; private set; }



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

        public override RPGStatCollection Stats
        {
            get
            {
                if (stats == null)
                {
                    stats = new Tower2002Stats(this, towerData.GetDataStatBaseByLevel(Level));
                }

                return stats;
            }
        }

        private TowerAttackRange towerAttackRange;

        public override void InitTower(int towerId, TowerPointController point)
        {
            base.InitTower(towerId, point);

            towerData = TowerData as TowerData2002;

            InitSoldier();

            InitAction();

            InitSkill();

            searchTarget.SetTargetType(towerData.GetDataStatBaseByLevel(Level).searchTarget);

            SearchTarget.gameObject.SetActive(true);

            RallyHole(point.pointRallyDefault.position);

            SetRangeDetect();
        }

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
                        if (UnitState.Current != ActionState.UseSkill)
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
                                if (UnitState.Current != ActionState.UseSkill)
                                    UnitState.Set(ActionState.AttackRange);
                                break;
                            }
                        }
                    }

                    break;
            }

            if (enemyTarget != null)
            {
                //Debug.Log($"[Tower] {this.name} target: " + enemyTarget.name);
            }
            else
            {
                UnitState.Set(ActionState.None);
                ResetAttack();

                var skill = skillController.GetSkill(BranchType.Skill1);
                if (skill != null && skill.CheckUnlocked() && skill.IsReady)
                {
                    skill.StartCooldown();
                }
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

        protected override void InitSkill()
        {
            Tower2002Skill1 skill1 = new Tower2002Skill1();
            skill1.Init(this, BranchType.Skill1, SkillType.ActiveTarget, this.towerData);
            skillController.AddSkill(skill1);

            Tower2002Skill2 skill2 = new Tower2002Skill2();
            skill2.Init(this, BranchType.Skill2, SkillType.ActiveTarget, this.towerData);
            skillController.AddSkill(skill2);
        }

        public override void ShowPreview(int towerId, TowerPointController point)
        {
            base.InitTower(towerId, point);

            SearchTarget.gameObject.SetActive(false);

            towerData = TowerData as TowerData2002;

            DamageType = towerData.GetDataStatBaseByLevel(Level).damageType;

            InitSoldier();

            LoadSpriteTower(Level);
            SetSortLayerForTutorial();
        }

        public override void HidePreview()
        {
            if (Soldiers.Count > 0)
            {
                LeanPool.Despawn(Soldier.gameObject);

                Soldier = null;

                Soldiers.Clear();
            }
        }

        public override void SetSortLayerForTutorial()
        {
            int srartOrder = SetLayerTower(2, SortingLayerConstants.TUTORIAL);
            SetSortLayerSoilders(srartOrder, SortingLayerConstants.TUTORIAL);
        }
        public override void SetSortLayerForGamePlay()
        {

            int srartOrder = SetLayerTower(1, SortingLayerConstants.UNIT);
            SetSortLayerSoilders(srartOrder, SortingLayerConstants.UNIT);
        }

        private void SetSortLayerSoilders(int startOrder, string layer)
        {
            for (int i = 0; i < Soldiers.Count; i++)
            {
                MeshRenderer soilderRenderer = Soldiers[i].GetComponent<MeshRenderer>();
                soilderRenderer.sortingLayerName = layer;
                soilderRenderer.sortingOrder = startOrder + 1;
            }
        }

        private int SetLayerTower(int startOrder, string layer)
        {
            List<SpriteRenderer> towerRenderers = new List<SpriteRenderer>();
            towerRenderers.Add(this.towerLevels[0].GetComponent<SpriteRenderer>());
            towerRenderers.Add(this.towerLevels[0].transform.GetChild(0).GetComponent<SpriteRenderer>());
            for (int i = 0; i < towerRenderers.Count; i++)
            {
                towerRenderers[i].sortingLayerName = layer;
                towerRenderers[i].sortingOrder = startOrder + i;
            }
            return towerRenderers.Count;
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

        protected override void InitAction()
        {
            towerAttackRange = new TowerAttackRange(this, PrepareAttackState);
        }

        protected override void InitSoldier()
        {
            if (Soldiers.Count == NumberSoldierSpawn) return;

            if (TowerPointController)
            {
                for (int i = 0; i < NumberSoldierSpawn; i++)
                {
                    GameObject goSoldier =
                        ResourceUtils.GetUnit("soldier_2002", towerSpawnPoints[i].position, Quaternion.identity);
                    if (goSoldier != null)
                    {
                        goSoldier.name = $"soldier_{i}";
                        goSoldier.transform.parent = transform;

                        Soldier = goSoldier.GetComponent<Soldier2002>();
                        Soldier.InitDataSoldier(i, this, TowerPointController.pointRallyDefault.position);
                        Soldier.SetSkin(Level);
                        Soldiers.Add(Soldier);
                    }
                }
            }
        }

        #region Action

        public void PrepareAttackState()
        {
            if (enemyTarget != null)
            {
                var skillData = skillController.GetSkillActive();
                if (skillData != null &&
                    (enemyTarget.MoveType != MoveType.Fly || skillData.BranchType != BranchType.Skill2))
                {
                    //Debug.LogWarning("Active skill");

                    UnitState.Set(ActionState.UseSkill);

                    skillController.ActiveSkillTarget(skillData.BranchType, () => {
                        if (enemyTarget != null)
                        {
                            ResetAttack();

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
            Soldier.AttackTarget(enemyTarget, null);
        }

        private void ResetAttack()
        {
            towerAttackRange?.ResetTime();

            Soldier.Idle();
        }

        public void RallyHole(Vector3 pointRally)
        {
            Soldier.RallyHole(pointRally);
        }

        #endregion

        #region Raise

        public override void OnLevelChange(int level)
        {
            ((TowerStats)Stats).UpdateStats(towerData.GetDataStatBaseByLevel(level));
            base.OnLevelChange(level);
            SetRangeDetect();
        }

        public bool CheckUnlockSkill2()
        {
            return skillController.GetSkill(BranchType.Skill2).Level > 0;
        }

        #endregion

        protected override void LoadSpriteTower(int level)
        {
            foreach (var towerLevel in towerLevels)
            {
                towerLevel.SetActive(false);
            }

            if (towerLevels.Length < level)
            {
                throw new Exception($"Level is over: {level}");
            }

            Transform soldierTransform;

            (soldierTransform = Soldier.transform).SetParent(towerSpawnPoints[level - 1]);

            soldierTransform.localPosition = Vector3.zero;

            towerLevels[level - 1].SetActive(true);
        }
    }
}
