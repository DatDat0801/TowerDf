using System;
using Cysharp.Threading.Tasks;
using EW2.Tutorial.General;
using TigerForge;
using UnityEngine;
using Zitga.UIFramework;

namespace EW2
{
    public class TowerPointController : MonoBehaviour
    {
        public Action PressingTowerPoint { get; set; }

        public Building myTower;
        public SpriteRenderer myRenderer;
        public SpriteRenderer raiseIcon;
        public Transform pointRallyDefault;

        public ActionState CurrentState { get; private set; }

        void Awake()
        {
            Vector3 pos = transform.position;
            pos.z = pos.y / 10f;
            transform.position = pos;
            myRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            //EventManager.StartListening(GamePlayEvent.OnAddMoney(ResourceType.MoneyInGame, MoneyInGameType.Gold), CheckRaise);
            EventManager.StartListening(GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.Gold),
                CheckRaise);
            EventManager.StartListening(GamePlayEvent.OnAddMoney(ResourceType.MoneyInGame, MoneyInGameType.Gold),
                CheckRaise);
        }


        private void Start()
        {
            CheckRaise();
        }

        private void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.Gold),
                CheckRaise);
            EventManager.StopListening(GamePlayEvent.OnAddMoney(ResourceType.MoneyInGame, MoneyInGameType.Gold),
                CheckRaise);
        }

        public void CheckRaise()
        {
            raiseIcon.enabled = CanRaise();
        }

        private bool CanRaiseSkill(BranchType branchType, long gold)
        {
            bool isLockedSkill = true;

            switch (branchType)
            {
                case BranchType.Skill1:
                    if (!myTower.IsLock(1))
                    {
                        isLockedSkill = false;
                    }

                    break;
                case BranchType.Skill2:
                    if (!myTower.IsLock(2))
                    {
                        isLockedSkill = false;
                    }

                    break;
            }

            if (isLockedSkill) return false;

            if (myTower.CheckUpgradeMaxLevelSkill(branchType))
            {
                return false;
            }

            var cost = myTower.GetPriceRaiseSkill(branchType);

            return cost <= gold;
        }

        bool IsLock()
        {
            //Debug.LogAssertion(myTower.IsLock(0)+"  "+myTower.IsLock(1)+"  "+myTower.IsLock(2)+"  "+TutorialManager.Instance.IsLockUpgradeTower);
            if (TutorialManager.Instance.IsLockUpgradeTower) return true;
            var currentUserStage = UserData.Instance.CampaignData.GetHighestPlayedStage();
            var unlockData = GameContainer.Instance.GetTowerUnlocks();
            TowerUnlock towerUnlockBranch = null; //branch 0
            for (var i = 0; i < unlockData.Length; i++)
            {
                if (unlockData[i].stage >= (currentUserStage - 1))
                {
                    towerUnlockBranch = unlockData[i];
                    break;
                }
            }

            if (towerUnlockBranch == null) return false;
            if (myTower.IsLock(towerUnlockBranch.branch))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CanRaise()
        {
            var gold = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold);

            if (myTower == null)
            {
                var towers = GameContainer.Instance.Get<UnitDataBase>().Get<TowerWorldData>().GetCurrentTowerWorld()
                    .towers;

                foreach (var t in towers)
                {
                    var cost = GameContainer.Instance.GetTowerCost(t).BuildCost;
                    if (cost <= gold)
                    {
                        raiseIcon.enabled = true;
                        return true;
                    }
                }
            }
            else
            {
                if (myTower.CheckUpgradeMaxLevel())
                {
                    return (CanRaiseSkill(BranchType.Skill1, gold) || CanRaiseSkill(BranchType.Skill2, gold));
                }

                //Debug.LogAssertion("ISLOCK: "+IsLock());
                if (IsLock())
                {
                    raiseIcon.enabled = false;
                    return false;
                }

                if (myTower.GetPriceRaise() <= gold)
                {
                    raiseIcon.enabled = true;
                    return true;
                }
            }

            raiseIcon.enabled = false;
            return false;
        }

        public void OnPressed()
        {
            if (CurrentState == ActionState.Stun)
            {
                return;
            }

            TowerOption.Instance.TowerPointSelected = this;
            DeselectAllHeroAndItsSkill();
            GamePlayUIManager.Instance.TryOpenUI(TowerOption.Instance);
            PressingTowerPoint?.Invoke();
        }

        void DeselectAllHeroAndItsSkill()
        {
            var gamePlayWindow = UIFrame.Instance.FindWindow(ScreenIds.game_play) as GamePlayWindowController;
            if (gamePlayWindow != null)
                gamePlayWindow.DeselectAllButtonSkills();
        }

        public async void SetStunPowerPoint(double second)
        {
            if (CurrentState != ActionState.Stun)
            {
                CurrentState = ActionState.Stun;
                await UniTask.Delay(TimeSpan.FromSeconds(second));

                CurrentState = ActionState.None;
            }
        }

        public void Build(Building building)
        {
            myTower = building;

            myRenderer.enabled = false;
            CheckRaise();
        }

        public void Sell()
        {
            myTower.Remove();

            myTower = null;

            myRenderer.enabled = true;
        }

        /// <summary>
        /// upgrade branch, 0 is center, 1 is left, 2 is right
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        public bool IsLock(int branch, Building tower)
        {
            var unlockData = GameContainer.Instance.GetTowerUnlocks();
            var currentUserStage = UserData.Instance.CampaignData.GetHighestPlayedStage();
            if (branch == 0)
            {
                TowerUnlock towerUnlockBranch0 = null; //branch 0
                for (var i = 0; i < unlockData.Length; i++)
                {
                    if (unlockData[i].stage >= currentUserStage)
                    {
                        towerUnlockBranch0 = unlockData[i];
                        break;
                    }
                }

                if (towerUnlockBranch0 == null)
                {
                    return false;
                }

                if (towerUnlockBranch0.type == TowerUnlock.LevelType.Level)
                {
                    return tower.Level >= towerUnlockBranch0.level;
                }
            }

            if (branch == 1)
            {
                TowerUnlock towerUnlockBranch1 = null; //branch 0
                for (var i = 0; i < unlockData.Length; i++)
                {
                    if (unlockData[i].branch == 1)
                    {
                        towerUnlockBranch1 = unlockData[i];
                        break;
                    }
                }

                var skill = tower.GetSkill(BranchType.Skill1);
                if (skill != null)
                {
                    //return skill.Level >= towerUnlockBranch1.level && towerUnlockBranch1.stage >= currentUserStage;
                    if (towerUnlockBranch1.stage > currentUserStage)
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
                TowerUnlock towerUnlockBranch2 = null; //branch 0
                for (var i = 0; i < unlockData.Length; i++)
                {
                    if (unlockData[i].branch == 2)
                    {
                        towerUnlockBranch2 = unlockData[i];
                        break;
                    }
                }

                var skill = tower.GetSkill(BranchType.Skill2);
                if (skill != null)
                {
                    if (towerUnlockBranch2.stage > currentUserStage)
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
    }
}