using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EW2.Tutorial.General;
using EW2.Tutorial.Map;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2.Tutorial.Step
{
    public class TutorialStep10 : FocusType
    {
        private GamePlayController gamePlayController;
        private Map0Tutorial map0Tutorial;

        public TutorialStep10()
        {
            tutorialId = AnyTutorialConstants.FOCUS_SPAWN_WAVE;
        }

        public override void Execute()
        {
            gamePlayController = FindingObjects.CalculateGamePlayController();
            map0Tutorial = FindingObjects.CalculateMap0TutorialInspector();
            map0Tutorial.ExecuteFocusStartWave();
            TutorialManager.Instance.StartCoroutine(CoExecute());
            //Close other opening window
            UIFrame.Instance.HideAllPanels(true);
            UIFrame.Instance.CloseAllPopup(true);
            
        }

        public override void Complete()
        {
            GamePlayController.Instance.ResumeGame();
            HidingFocus?.Invoke();
            TutorialManager.Instance.StartCoroutine(CoComplete());
            SpawnHero();
            map0Tutorial.ShowAllTowerPoints();
            map0Tutorial.ResetCameraDefault();
            //Debug.LogAssertion("RESET");
            map0Tutorial.ResetLayerFocusPoint();
          //  FindingObjects.CalculateTutorialUI().ActiveBlockRegion();
          FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus spawn wave button");
        }

        protected override Vector3 CalculateFocusPosition()
            => gamePlayController.GetStartWaveBtnController().position;

        private IEnumerator CoComplete()
        {
            var secondDelayComplete = 0.5f;
            yield return new WaitForSeconds(secondDelayComplete);
            base.Complete(); 
            DecreaseLayerFocusPoint(gamePlayController.GetStartWaveBtnController().gameObject);
            FindingObjects.CalculateCameraController().EnableCanClick();
            ComputingNextStep?.Invoke();
        }

        private IEnumerator CoExecute()
        {
            var secondDelayExecute = 1;
            yield return new WaitForSeconds(secondDelayExecute);
            base.Execute();
            GamePlayController.Instance.PauseGame();
            gamePlayController.ShowStartWaveBtn();
            IncreaseLayerFocusPoint(gamePlayController.GetStartWaveBtnController().gameObject);
        }

        private void SpawnHero()
        {
            var heroId = 1001;
            FindingObjects.CalculateGamePlayWindowController().SpawnHero(heroId);
            GamePlayController.heroList.Add(heroId);
        }
    }
}