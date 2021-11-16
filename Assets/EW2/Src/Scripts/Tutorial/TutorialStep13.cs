using EW2.Tutorial.General;
using EW2.Tutorial.Map;
using System.Collections;
using UnityEngine;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2.Tutorial.Step
{
    public class TutorialStep13 : FocusType
    {
        private TowerOptionButton towerUpgradeBtn;
        private Map0Tutorial map0Tutorial;
        public TutorialStep13()
        {
            tutorialId = AnyTutorialConstants.FOCUS_UPGRADE_MAGE_TOWER;
        }

        public override void Execute()
        {
            towerUpgradeBtn = FindingObjects.CalculateTowerOption().GetTowerUpgradeBtn();
            map0Tutorial = FindingObjects.CalculateMap0TutorialInspector();
            towerUpgradeBtn.BuildingSuccess += CompleteBuilding;
            TutorialManager.Instance.StartCoroutine(CoExecute());
            UIFrame.Instance.HideAllPanels(true);
            UIFrame.Instance.CloseAllPopup(true);

        }

        public void CompleteBuilding()
        {
            //TutorialManager.Instance.StartCoroutine(CoComplete());
            CoComplete();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus mage tower to guild user upgrading");
        }

        protected override Vector3 CalculateFocusPosition()
            => towerUpgradeBtn.transform.position;

        private void CoComplete()
        {
            //var secondDelayComplete = 0.5f;
            towerUpgradeBtn.BuildingSuccess -= CompleteBuilding;
            //yield return new WaitForSecondsRealtime(secondDelayComplete);
            base.Complete();
            map0Tutorial.SetLayerTowerGameplay(1);
            HidingFocus?.Invoke();
            DecreaseLayerFocusPoint(towerUpgradeBtn.gameObject);
            GamePlayController.Instance.ResumeGame();

            ComputingNextStep?.Invoke();
            FindingObjects.CalculateCameraController().EnableCanClick();

            //ExecuteNextStepTutorial();
        }

        private IEnumerator CoExecute()
        {
            var secondDelayExecute = 0.3f;
            yield return new WaitForSecondsRealtime(secondDelayExecute);
            base.Execute();
            IncreaseLayerFocusPoint(towerUpgradeBtn.gameObject);
        }

    }
}
