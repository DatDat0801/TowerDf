using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2.Tutorial.Step
{
    public class TutorialStep14 : DialogType
    {
        private PopupNoticeWindowController popup;

        public TutorialStep14()
        {
            tutorialId = AnyTutorialConstants.DIALOG_1002_14;
        }

        public override void Execute()
        {
            FindingObjects.CalculateMap0TutorialInspector().ExecuteFocusHero1002();
            // FindingObjects.CalculateTutorialUI().InactiveBlockRegion();
            TutorialManager.Instance.StartCoroutine(CoExecute());
            UIFrame.Instance.HideAllPanels(true);
            UIFrame.Instance.CloseAllPopup(true);
        }

        public override void Complete()
        {
            base.Complete();
            ExecuteNextStepTutorial();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Show dialog 1002");
        }

        private void SpawnHero()
        {
            var heroId = 1002;
            FindingObjects.CalculateGamePlayWindowController().SpawnHero(heroId);
            //add hero in tutorial
            GamePlayController.heroList.Add(heroId);
        }

        private IEnumerator CoExecute()
        {
            var secondDelayExecute = 5f;
            yield return new WaitForSecondsRealtime(secondDelayExecute);
            base.Execute();
            SpawnHero();
            GamePlayController.Instance.PauseGame();
        }
    }
}