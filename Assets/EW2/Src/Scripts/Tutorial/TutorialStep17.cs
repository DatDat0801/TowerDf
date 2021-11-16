using System.Collections;
using System.Collections.Generic;
using EW2.Tutorial.Map;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep17 : FocusType
    {

        private Map0Tutorial map0Tutorial;

        public TutorialStep17()
        {
            tutorialId = AnyTutorialConstants.FOCUS_1002_HERO_MOVE_POSITION;
        }

        public override void Execute()
        {
            map0Tutorial = FindingObjects.CalculateMap0TutorialInspector();
            base.Execute();
            FindingObjects.CalculateCameraController().DisableCanClick();
            map0Tutorial.Hero1002MovePositionFocusBtn.gameObject.SetActive(true);
            map0Tutorial.Hero1002MovePositionFocusBtn.transform.position = map0Tutorial.Hero1002MoveTransform.position;

            
        }

        public override void Complete()
        {
            base.Complete();
            HidingFocus?.Invoke();
            GamePlayController.Instance.ResumeGame();
            FindingObjects.CalculateCameraController().EnableCanClick();
            map0Tutorial.Hero1002MovePositionFocusBtn.gameObject.SetActive(false);
            var heroBtn = FindingObjects.CalculateGamePlayWindowController().GetSecondHeroBtn();
            heroBtn.MoveSelectedHeroToPosition(CalculateFocusPosition());
            TutorialManager.Instance.StartCoroutine(CoExecuteNextStepTutorial());
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus 1002 hero move position");
        }
        

        protected override Vector3 CalculateFocusPosition()
        {
            return map0Tutorial.Hero1002MoveTransform.position;
        }

        private IEnumerator CoExecuteNextStepTutorial()
        {
            var secondDelayExecuteNextStep = 3;
            yield return new WaitForSeconds(secondDelayExecuteNextStep);
            ExecuteNextStepTutorial();
            
            
        }
    }
}