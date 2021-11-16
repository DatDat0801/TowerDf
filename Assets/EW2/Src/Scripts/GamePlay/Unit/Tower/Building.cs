using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using Zitga.Observables;

namespace EW2
{
    public abstract class Building : Unit
    {
        public ObservableProperty<int> Level { get; private set; }

        public TowerRaiseCost RaiseCost { get; protected set; }

        public TowerData TowerData { get; protected set; }


        protected readonly List<SoldierBase> soldiers = new List<SoldierBase>();

        public List<SoldierBase> Soldiers => soldiers;
        public abstract TowerType TowerType { get; }

        public TowerPointController TowerPointController { get; protected set; }

        private UnitState _buildingState;

        public override UnitState UnitState => this._buildingState ?? (this._buildingState = new BuildingState(this));

        private BuildingSpine _buildingSpine;
        public override UnitSpine UnitSpine => this._buildingSpine ?? (this._buildingSpine = new BuildingSpine(this));

        protected readonly TowerSkillController skillController = new TowerSkillController();

        private GameObject _effectBuffAtkSpeed;

        [ShowInInspector]
        private Dictionary<object, RPGStatModifier> _dictBuffAtkSpeeds = new Dictionary<object, RPGStatModifier>();

        private List<object> _currentBuffOwner = new List<object>();

        private int _countBuffAtkSpeed;


        protected override void Awake()
        {
            UnitType = UnitType.Tower;
        }

        protected abstract void InitSoldier();

        protected virtual void InitSkill()
        {
        }

        public abstract void ShowPreview(int towerId, TowerPointController point);
        public abstract void HidePreview();

        public virtual void SetSortLayerForTutorial()
        {

        }
        public virtual void SetSortLayerForGamePlay()
        {

        }
        public virtual void InitTower(int towerId, TowerPointController point)
        {
            Level = 1;

            Id = towerId;

            TowerPointController = point;

            TowerData = GameContainer.Instance.GetTowerData(towerId);

            RaiseCost = GameContainer.Instance.Get<UnitDataBase>().Get<TowerCost>().raiseCosts[towerId];
        }

        protected abstract void LoadSpriteTower(int levelTower);

        public virtual void OnRaiseLevel()
        {
            if (Level < RaiseCost.raiseLevelCost.Length)
            {
                GamePlayData.Instance.SubMoneyInGame(MoneyInGameType.Gold, GetPriceRaise());

                Level++;
                OnLevelChange(Level);
                LoadSpriteTower(Level);
            }

            RaiseSoldiers();
        }

        protected virtual void RaiseSoldiers()
        {
            foreach (var soldier in Soldiers)
            {
                if (soldier != null)
                {
                    soldier.SetSkin(Level);
                }
            }
        }

        public virtual void OnLevelChange(int level)
        {
            if (this._currentBuffOwner.Count > 0)
            {
                int count = this._currentBuffOwner.Count;
                for (int i = 0; i < count; i++)
                {
                    this.Stats.AddStatModifier(RPGStatType.AttackSpeed, this._dictBuffAtkSpeeds[this._currentBuffOwner[i]]);
                }
            }
        }

        public void OnRaiseSkill(BranchType branchType)
        {
            var cost = GetPriceRaiseSkill(branchType);
            if (skillController.OnRaiseSkill(branchType))
            {
                GamePlayData.Instance.SubMoneyInGame(MoneyInGameType.Gold, cost);
            }
        }

        public BaseTowerSkill GetSkill(BranchType branchType)
        {
            return skillController.GetSkill(branchType);
        }

        public bool CheckUpgradeMaxLevel()
        {
            return Level >= RaiseCost.raiseLevelCost.Length;
        }

        public bool CheckUpgradeMaxLevelSkill(BranchType branchType)
        {
            return skillController.GetSkill(branchType).CheckMaxLevelSkill();
        }

        public int GetPriceRaiseSkill(BranchType branchType)
        {
            return RaiseCost.GetSkillCost(branchType, skillController.GetSkillLevel(branchType));
        }

        public int GetPriceRaise()
        {
            return RaiseCost.raiseLevelCost[Level];
        }

        public int GetSellCost()
        {
            return RaiseCost.GetSellCost(
                Level,
                skillController.GetSkillLevel(BranchType.Skill1),
                skillController.GetSkillLevel(BranchType.Skill2)
            );
        }

        public override void Remove()
        {
            RemoveSoldiers();

            skillController.Reset();

            LeanPool.Despawn(gameObject);
        }

        protected virtual void RemoveSoldiers()
        {
            foreach (var soldierBase in Soldiers)
            {
                LeanPool.Despawn(soldierBase.gameObject);
            }

            Soldiers.Clear();
        }

        #region Handle Buff Link Tower

        public void AddBuffAtkSpeed(object creator, RPGStatModifier atkSpeed, bool isOwner)
        {
            this._dictBuffAtkSpeeds.Add(creator, atkSpeed);
            if (!CheckDuplicateBuff(creator))
            {
                GameObject effect = ResourceUtils.GetVfx("Status", "fx_status_buff_atk_speed", transform.position,
                    Quaternion.identity,null,0,false);

                if (isOwner)
                {
                    effect.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                }
                else
                {
                    effect.transform.localScale = Vector3.one;
                }

                // Debug.LogWarning($"[Befor Add] {this.name} speed: " + Stats.GetStat(RPGStatType.AttackSpeed).StatValue);
                this._effectBuffAtkSpeed = effect;
                this.Stats.AddStatModifier(RPGStatType.AttackSpeed, atkSpeed);
                this._currentBuffOwner.Add(creator);
                // Debug.LogWarning($"[After Add] {this.name} speed: " + Stats.GetStat(RPGStatType.AttackSpeed).StatValue);
            }
        }
        private bool CheckDuplicateBuff(object creator)
        {
            string creatorName = creator.GetType().Name;
            for (int i = 0; i < this._currentBuffOwner.Count; i++)
            {
                if (this._currentBuffOwner[i].GetType().Name.Equals(creatorName))
                {
                    return true;
                }
            }
            return false;
        }
        public void DebuffAtkSpeed(object creator)
        {
            if (this._dictBuffAtkSpeeds.ContainsKey(creator))
            {
                if (this._currentBuffOwner.Contains(creator))
                {
                    this.Stats.RemoveStatModifier(RPGStatType.AttackSpeed, this._dictBuffAtkSpeeds[creator]);

                    this._currentBuffOwner.Remove(creator);
                }

                this._dictBuffAtkSpeeds.Remove(creator);

                if (this._dictBuffAtkSpeeds.Count <= 0)
                {
                    if (this._effectBuffAtkSpeed != null)
                        LeanPool.Despawn(this._effectBuffAtkSpeed);

                    this._effectBuffAtkSpeed = null;
                }
                else
                {

                    foreach (var item in this._dictBuffAtkSpeeds)
                    {
                        if (CheckDuplicateBuff(item.Key))
                        {
                            continue;
                        }
                        this.Stats.AddStatModifier(RPGStatType.AttackSpeed, item.Value);

                        this._currentBuffOwner.Add(item.Key);

                        var ownerBuff = (Tower2001BuffAtkSpeed)this._currentBuffOwner[0];

                        if (ownerBuff)
                        {
                            if (ownerBuff.Owner.gameObject.GetInstanceID() == gameObject.GetInstanceID())
                            {
                                this._effectBuffAtkSpeed.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                            }
                            else
                            {
                                this._effectBuffAtkSpeed.transform.localScale = Vector3.one;
                            }
                        }

                        break;
                    }

                }
            }
        }

        public virtual async void SetStunInSecond(double second)
        {
            var currentState = UnitState.Current;
            UnitState.Set(ActionState.Stun);
            UnitState.IsLockState = true;
            await UniTask.Delay(TimeSpan.FromSeconds(second));
            UnitState.IsLockState = false;
            UnitState.Set(currentState);
        }


        /// <summary>
        /// upgrade branch, 0 is center, 1 is left, 2 is right
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        public bool IsLock(int branch)
        {
            TowerUnlock[] unlocks = GameContainer.Instance.GetTowerUnlocks();
            int currentUserStage = UserData.Instance.CampaignData.GetHighestPlayedStage();

            if (branch == 0)
            {
                TowerUnlock towerUnlockBranch0 = null; //branch 0
                for (int i = 0; i < unlocks.Length; i++)
                {
                    if (unlocks[i].stage > currentUserStage && unlocks[i].branch == branch)
                    {
                        towerUnlockBranch0 = unlocks[i];
                        break;
                    }
                }

                if (towerUnlockBranch0 == null)
                {
                    return false;
                }

                if (towerUnlockBranch0.type == TowerUnlock.LevelType.Level)
                {
                    //Debug.LogAssertion(Level.Value+ "&" + towerUnlockBranch0.level);
                    return Level.Value >= towerUnlockBranch0.level;
                }
            }

            if (branch == 1)
            {
                TowerUnlock towerUnlockBranch1 = null; //branch 1

                for (var i = 0; i < unlocks.Length; i++)
                {
                    if (unlocks[i].stage >= currentUserStage && unlocks[i].branch == branch)
                    {
                        towerUnlockBranch1 = unlocks[i];
                        break;
                    }
                }

                var skill = GetSkill(BranchType.Skill1);
                if (skill != null)
                {
                    //return skill.Level >= towerUnlockBranch1.level && towerUnlockBranch1.stage >= currentUserStage;
                    if (towerUnlockBranch1 == null) return false;
                    if (towerUnlockBranch1.stage >= currentUserStage + 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if (branch == 2)
            {
                TowerUnlock towerUnlockBranch2 = null; //branch 2
                for (var i = 0; i < unlocks.Length; i++)
                {
                    if (unlocks[i].stage >= currentUserStage && unlocks[i].branch == branch)
                    {
                        towerUnlockBranch2 = unlocks[i];
                        break;
                    }
                }

                var skill = GetSkill(BranchType.Skill2);
                if (skill != null)
                {
                    if (towerUnlockBranch2 == null) return false;
                    if (towerUnlockBranch2.stage >= currentUserStage + 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }


        #endregion

        public override void OnDisable()
        {
            base.OnDisable();

            stats = null;
        }
    }
}
