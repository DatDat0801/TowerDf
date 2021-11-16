using DG.Tweening;
using EW2.Tutorial.General;
using Hellmade.Sound;
using System;
using UnityEngine;
using Zitga.Localization;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class TowerOption : MonoBehaviour, UIGameplay
    {
        public enum Action
        {
            Build,
            Raise,
            Sell,
            Rally,
            RaiseSkill1,
            RaiseSkill2,
            Lock,
            LockSell
        }

        public enum SlotOption : int
        {
            Archer = 0,
            Mage = 1,
            Golem = 2,
            Barrack = 3,
            Upgrade = 4,
            Sell = 5,
            Skill1 = 6,
            Skill2 = 7,
            Rally = 8
        }

        private static TowerOption _instance;

        public static TowerOption Instance
        {
            get
            {
                if (_instance == null)
                {
                    Transform parent = UIFrame.Instance.MainCanvas.transform;
                    GameObject obj = ResourceUtils.GetUnitOther("tower_option", parent);
                    if (obj != null)
                    {
                        obj.SetActive(false);
                        _instance = obj.GetComponent<TowerOption>();
                    }
                }

                return _instance;
            }
        }

        private Building towerPreview;

        [SerializeField] private BuildingInfoPopup buildingInfoPopup;

        [SerializeField] private TowerOptionButton[] listOption;

        public TowerPointController TowerPointSelected { get; set; }

        private RectTransform rectTransform => GetComponent<RectTransform>();

        private void OnEnable()
        {
            TowerOptionButton.OnPressed += HandleOnPressed;
            TowerOptionButton.OnPressedSuccess += HandleOnPressedSuccess;
            TowerOptionButton.OnConfirmed += HandleOnConfirmed;

            SetTowerButtonState();
        }

        private void OnDisable()
        {
            TowerOptionButton.OnPressedSuccess -= HandleOnPressedSuccess;
            TowerOptionButton.OnConfirmed -= HandleOnConfirmed;
            TowerOptionButton.OnPressed -= HandleOnPressed;
        }

        #region UIGameplay implementation

        void SetTowerButtonState()
        {
            if (TutorialManager.Instance.IsLockUpgradeTower)
            {
                GetTowerSellBtn().LockBtn();
            }
            else
            {
                GetTowerSellBtn().UnlockBtn();
            }

            //lock upgrade button if level >=2 and tutorial condition
            var upgradeBtn = GetTowerUpgradeBtn();
            var currentTower = upgradeBtn.Tower;
            if (currentTower == null) return;
            var lockBranch1 = upgradeBtn.IsLock(0);
            if (lockBranch1 || TutorialManager.Instance.IsLockUpgradeTower)
            {
                upgradeBtn.LockBtn();
            }
            else
            {
                upgradeBtn.UnlockBtn();
            }

            var skill1Btn = GetTowerSkill1Btn();
            if (skill1Btn.IsLock(1) || TutorialManager.Instance.IsLockUpgradeTower)
            {
                skill1Btn.LockBtn();
            }
            else
            {
                skill1Btn.UnlockBtn();
            }

            var skill2Btn = GetTowerSkill2Btn();
            if (skill2Btn.IsLock(2) || TutorialManager.Instance.IsLockUpgradeTower)
            {
                skill2Btn.LockBtn();
            }
            else
            {
                skill2Btn.UnlockBtn();
            }
        }

        public void Open()
        {
            transform.SetAsFirstSibling();
            CallOptionAtPoint();

            if (TowerPointSelected.myTower != null)
            {
                TowerPointSelected.myTower.Level.ValueChanged += OnTowerLevelChanged;

                var towerData = GameContainer.Instance.GetTowerData(TowerPointSelected.myTower.Id);
                var statBase = towerData.GetDataStatFinalByLevel(TowerPointSelected.myTower.Level);
                //Debug.LogAssertion($"Tower: {TowerPointSelected.myTower.Id} range: {statBase.detectRangeAttack}");
            }

            var audioClip1 = ResourceUtils.LoadSound(SoundConstant.BUTTON_CLICK);
            EazySoundManager.PlaySound(audioClip1);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            if (towerPreview != null)
            {
                towerPreview.HidePreview();
                towerPreview.Remove();
            }

            RangeCircleVisual.Instance.Hide();

            if (TowerPointSelected.myTower != null)
                TowerPointSelected.myTower.Level.ValueChanged -= OnTowerLevelChanged;
        }

        /// <summary>
        /// update green arrow when level changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTowerLevelChanged(object sender, EventArgs e)
        {
            TowerPointSelected.CheckRaise();
        }

        public UI_STATE GetUIType()
        {
            return UI_STATE.Soft;
        }

        #endregion

        #region Delegate

        void HandleOnPressed(TowerOptionButton t, Action action)
        {
            if (t == null)
            {
                return;
            }

            switch (action)
            {
                case Action.Build:
                    ShowBuildingInfoPopup(true);

                    UpdateRaiseBuildingInfoPopup(t.Id, 0);
                    break;
                case Action.Raise:
                    ShowBuildingInfoPopup(true);

                    UpdateRaiseBuildingInfoPopup(t.Tower.Id, t.Tower.Level);
                    //preview range
                    RangeCircleVisual.Instance.ShowRangeBuildingByLevel(t.Tower, t.Tower.Level + 1);
                    break;

                case Action.RaiseSkill1:
                    ShowBuildingInfoPopup(true);

                    UpdateRaiseSkillInfoPopup(t.Tower.Id, 0, t.Tower.GetSkill(BranchType.Skill1).Level);
                    break;
                case Action.RaiseSkill2:
                    ShowBuildingInfoPopup(true);

                    UpdateRaiseSkillInfoPopup(t.Tower.Id, 1, t.Tower.GetSkill(BranchType.Skill2).Level);
                    break;

                case Action.Lock:
                    ShowBuildingInfoPopup(true);
                    UpdateLockInfo(t.Tower.Id, t.Tower.Level, t.CurrentBranch);
                    break;

                case Action.LockSell:
                    break;
            }
        }

        void HandleOnPressedSuccess(TowerOptionButton t, Action action)
        {
            if (t == null)
            {
                return;
            }

            switch (action)
            {
                case Action.Build:
                    LoadSpritePreview(t.RaiseCost.id);
                    //show range circle visual
                    var towerData = GameContainer.Instance.GetTowerData(t.Id);
                    RangeCircleVisual.Instance.ShowRangePreviewTower(towerData, towerPreview.transform.position);
                    break;
            }
        }

        /// <summary>
        /// play
        /// </summary>
        void PlayBuildSuccessSound()
        {
            var audioClip1 = ResourceUtils.LoadSound(SoundConstant.TOWER_UPGRADE);
            EazySoundManager.PlaySound(audioClip1);
        }

        void HandleOnConfirmed(TowerOptionButton t, Action action)
        {
            switch (action)
            {
                case Action.Build:
                    GamePlayData.Instance.SubMoneyInGame(MoneyInGameType.Gold, t.RaiseCost.BuildCost);

                    ResourceUtils.GetEffectBuildTower(TowerPointSelected.transform);

                    InstantTower(t.RaiseCost.id);

                    GamePlayUIManager.Instance.CloseCurrentUI(true);

                    ShowBuildingInfoPopup(false);

                    PlayBuildSuccessSound();
                    break;
                case Action.Sell:
                    // firebase tracking
                    if (StartWaveButtonController.Instance.GameStarted)
                    {
                        FirebaseLogic.Instance.StageTowerSell(GamePlayController.Instance.MapId,
                            CallWave.Instance.CurrWave, TowerPointSelected.myTower.Id, TowerPointSelected.myTower.Level,
                            TowerPointSelected.myTower.GetSkill(BranchType.Skill1).Level,
                            TowerPointSelected.myTower.GetSkill(BranchType.Skill2).Level);
                    }

                    GamePlayController.Instance.RemoveBuilding(TowerPointSelected.myTower);

                    var valueRefund = TowerPointSelected.myTower.GetSellCost();
                    TowerPointSelected.Sell();
                    GamePlayData.Instance.AddMoneyInGame(MoneyInGameType.Gold, valueRefund);
                    GamePlayUIManager.Instance.CloseCurrentUI(true);

                    var audioClip1 = ResourceUtils.LoadSound(SoundConstant.TOWER_SELL);
                    EazySoundManager.PlaySound(audioClip1);


                    break;
                case Action.Rally:
                    ShowRangeRally(TowerPointSelected.myTower, TowerPointSelected.transform.position);
                    break;
                case Action.Raise:
                    ResourceUtils.GetEffectBuildTower(TowerPointSelected.transform);
                    TowerPointSelected.myTower.OnRaiseLevel();
                    GamePlayUIManager.Instance.CloseCurrentUI(false);

                    ShowBuildingInfoPopup(false);
                    //Datnd ensure the green arrow notify updated when upgrade success 
                    TowerPointSelected.CheckRaise();

                    PlayBuildSuccessSound();
                    break;
                case Action.RaiseSkill1:
                case Action.RaiseSkill2:
                    ResourceUtils.GetEffectBuildTower(TowerPointSelected.transform);
                    TowerPointSelected.myTower.OnRaiseSkill(action == Action.RaiseSkill1
                        ? BranchType.Skill1
                        : BranchType.Skill2);
                    GamePlayUIManager.Instance.CloseCurrentUI(false);

                    ShowBuildingInfoPopup(false);
                    TowerPointSelected.CheckRaise();
                    PlayBuildSuccessSound();
                    break;
            }
        }

        #endregion

        public TowerOptionButton GetArcherTowerOptionBtn() => listOption[(int)SlotOption.Archer];

        public TowerOptionButton GetMageTowerOptionBtn() => listOption[(int)SlotOption.Mage];
        public TowerOptionButton GetGolemTowerOptionBtn() => listOption[(int)SlotOption.Golem];
        public TowerOptionButton GetBarrackTowerOptionBtn() => listOption[(int)SlotOption.Barrack];

        public TowerOptionButton GetTowerUpgradeBtn() => listOption[(int)SlotOption.Upgrade];
        public TowerOptionButton GetTowerSkill1Btn() => listOption[(int)SlotOption.Skill1];
        public TowerOptionButton GetTowerSkill2Btn() => listOption[(int)SlotOption.Skill2];
        public TowerOptionButton GetTowerSellBtn() => listOption[(int)SlotOption.Sell];

        private bool IsRallyBuilding(int id)
        {
            if (id == (int)BuildingId.Barrack)
            {
                return true;
            }
            else if (id == (int)BuildingId.Mage)
            {
                var tower2002 = TowerPointSelected.myTower as Tower2002;
                return tower2002.CheckUnlockSkill2();
            }

            return false;
        }

        private void ShowBuildTower()
        {
            var towers = GameContainer.Instance.Get<UnitDataBase>().Get<TowerWorldData>().GetCurrentTowerWorld().towers;

            for (int i = 0; i < towers.Length; i++)
            {
                listOption[i].Init(towers[i], Action.Build);
                listOption[i].gameObject.SetActive(true);
            }
        }

        private void ShowRally()
        {
            var dataTower = TowerPointSelected.myTower.TowerData;

            if (!IsRallyBuilding(dataTower.id))
            {
                return;
            }

            listOption[(int)SlotOption.Rally]
                .Init(TowerPointSelected.myTower, Action.Rally);
            listOption[(int)SlotOption.Rally].gameObject.SetActive(true);
        }

        private void ShowRaiseLevel()
        {
            if (TowerPointSelected.myTower.CheckUpgradeMaxLevel())
                return;

            listOption[(int)SlotOption.Upgrade]
                .Init(TowerPointSelected.myTower, Action.Raise);
            listOption[(int)SlotOption.Upgrade].gameObject.SetActive(true);
            //RangeCircleVisual.Instance.ShowRangeBuildingByLevel(TowerPointSelected.myTower, TowerPointSelected.myTower.Level);
            RangeCircleVisual.Instance.ShowRangeBuildingByCurrentStat(TowerPointSelected.myTower);
        }

        private void ShowRaiseSkill()
        {
            if (!TowerPointSelected.myTower.CheckUpgradeMaxLevel())
            {
                return;
            }

            listOption[(int)SlotOption.Skill1]
                .Init(TowerPointSelected.myTower, Action.RaiseSkill1);
            listOption[(int)SlotOption.Skill1].gameObject.SetActive(true);

            listOption[(int)SlotOption.Skill2]
                .Init(TowerPointSelected.myTower, Action.RaiseSkill2);
            listOption[(int)SlotOption.Skill2].gameObject.SetActive(true);
            //RangeCircleVisual.Instance.ShowRangeBuildingByLevel(TowerPointSelected.myTower, TowerPointSelected.myTower.Level);
            RangeCircleVisual.Instance.ShowRangeBuildingByCurrentStat(TowerPointSelected.myTower);
        }

        private void ShowSell()
        {
            listOption[(int)SlotOption.Sell].Init(TowerPointSelected.myTower, Action.Sell);
            listOption[(int)SlotOption.Sell].gameObject.SetActive(true);
        }

        private void CallOptionAtPoint()
        {
            //disable if Stun
            if (TowerPointSelected.myTower != null)
            {
                if (TowerPointSelected.myTower.UnitState.Current == ActionState.Stun)
                {
                    return;
                }
            }

            ShowBuildingInfoPopup(false);

            foreach (var child in listOption)
            {
                child.gameObject.SetActive(false);
            }

            transform.position = TowerPointSelected.transform.position;
            if (TowerPointSelected.myTower == null)
            {
                ShowBuildTower();
            }
            else
            {
                //offset the option by y +33
                var anchoredPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y + 33);
                ShowRally();

                ShowRaiseSkill();

                ShowRaiseLevel();

                ShowSell();
            }

            ShowAnim();
        }


        public void ShowAnim()
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            transform.DOKill();
            transform.DOScale(Vector3.one, 0.2f).SetUpdate(true);
        }

        #region Load Resource

        private void InstantTower(int idTower)
        {
            if (towerPreview != null)
            {
                towerPreview.SetSortLayerForGamePlay();
                towerPreview.InitTower(idTower, TowerPointSelected);
                TowerPointSelected.Build(towerPreview);
                GamePlayController.Instance.AddBuilding(towerPreview);
                towerPreview.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Tower);
                towerPreview = null;
            }
        }

        private void LoadSpritePreview(int towerId)
        {
            if (towerPreview != null)
            {
                towerPreview.HidePreview();
                towerPreview.Remove();
            }


            towerPreview =
                GamePlayController.Instance.SpawnController.SpawnTower(towerId, TowerPointSelected.transform.position);

            if (towerPreview != null)
            {
                towerPreview.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);
                towerPreview.ShowPreview(towerId, TowerPointSelected);
            }
        }

        private void ShowRangeRally(Building towerMelee, Vector3 pointSpawn)
        {
            GamePlayUIManager.Instance.TryOpenUI(RangeRallyController.Instance);
            RangeRallyController.Instance.SetDataRange(towerMelee);
        }

        private void UpdateRaiseSkillInfoPopup(int towerId, int skillId, int level)
        {
            var towerData = GameContainer.Instance.GetTowerData(towerId);

            var buildInfo = new BuildingInfoContent()
            {
                title = Localization.Current.Get("tower", $"tower_skill_{towerId}_{skillId}"),
                content = towerData.GetInfoSkill(skillId, level,
                    Localization.Current.Get("tower", $"tower_skill_des_{towerId}_{skillId}")),
                damage = "",
                atkSpeed = "",
                atkType = ""
            };
            buildingInfoPopup.SetInfo(buildInfo);
        }

        private void UpdateRaiseBuildingInfoPopup(int towerId, int level)
        {
            var towerData = GameContainer.Instance.GetTowerData(towerId);

            var info = towerData.GetInfo(level + 1);

            var buildInfo = new BuildingInfoContent()
            {
                title = Localization.Current.Get("tower", $"tower_name_{towerId.ToString()}_{level.ToString()}"),
                content = Localization.Current.Get("tower", $"tower_des_{towerId.ToString()}_{level.ToString()}"),
                damage = info.damage,
                atkSpeed = info.atkSpeed,
                atkType = info.atkType
            };
            buildingInfoPopup.SetInfo(buildInfo);
        }

        void UpdateLockInfo(int towerId, int level, int branch)
        {
            var unlocks = GameContainer.Instance.GetTowerUnlocks();
            var currentUserStage = UserData.Instance.CampaignData.GetHighestPlayedStage();
            int index = -1; // unlockData.FirstOrDefault(o => o.stage >= currentUserStage);
            int unlockValue = -1;
            // if (branch == 0)
            // {
            //     unlockValue = currentUserStage == 1 ? -1 : currentUserStage;
            // }
            //
            if (branch == 2)
            {
                unlockValue = currentUserStage == 2 ? 1 : currentUserStage;
            }
            else
            {
                unlockValue = currentUserStage;
            }

            for (var i = 0; i < unlocks.Length; i++)
            {
                if (unlocks[i].stage > unlockValue && unlocks[i].branch == branch)
                {
                    index = i;
                    break;
                }
            }

            // Debug.LogAssertion("branch " + unlocks[index].branch + " level" + unlocks[index].level + "  stage" +
            //                    unlocks[index].stage + 1);

            var text = String.Empty;
            if (index != -1)
            {
                text = string.Format(L.popup.unlock_upgrade_tower,
                    currentUserStage <= 0 && level == 2 ? 2.ToString() : (unlocks[index].stage + 1).ToString());
            }
            else
            {
                text = string.Format(L.popup.unlock_upgrade_tower, 2.ToString());
            }

            var title = "";
            if (branch == 0)
            {
                title = Localization.Current.Get("tower", $"tower_name_{towerId.ToString()}_{(level - 1).ToString()}");
            }
            else
            {
                title = Localization.Current.Get("tower", $"tower_skill_{towerId}_{branch - 1}");
            }

            var buildInfo = new BuildingInfoContent()
            {
                title = title,
                content = text //Localization.Current.Get("", L.popup.unlock_upgrade_tower),
            };
            buildingInfoPopup.SetInfo(buildInfo);
        }

        private void ShowBuildingInfoPopup(bool isShow)
        {
            buildingInfoPopup.gameObject.SetActive(isShow);

            if (isShow)
            {
                buildingInfoPopup.UpdatePosition();
            }
        }

        #endregion
    }
}
