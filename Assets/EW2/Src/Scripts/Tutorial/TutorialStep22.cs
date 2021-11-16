using System.Collections;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep22 : FocusType
    {
        private SkillButton heroSkillButton;

        public TutorialStep22()
        {
            tutorialId = AnyTutorialConstants.FOCUS_1002_SKILL_ICON;
        }

        public override void Execute()
        {
            heroSkillButton = FindingObjects.CalculateGamePlayWindowController().GetSecondHeroSkillBtn();
            GamePlayController.Instance.ResumeGame();
            FindingObjects.CalculateMap0TutorialInspector().ResetCameraDefault();
            TutorialManager.Instance.StartCoroutine(CoExecute());
            //testing
            FindingObjects.CalculateCameraController().DisableCanClick();
        }

        public override void Complete()
        {
            heroSkillButton.SelectingSkillHeroBtn -= Complete;
            base.Complete();
            HidingFocus?.Invoke();
            ExecuteNextStepTutorial();
            DecreaseLayerFocusPoint(heroSkillButton.gameObject);
            FindingObjects.CalculateCameraController().EnableCanClick();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus 1002 skill icon");
        }

        protected override Vector3 CalculateFocusPosition() => heroSkillButton.transform.position;
        
        private IEnumerator CoExecute()
        {
            var secondDelay = 1;
            yield return new WaitForSeconds(secondDelay);
            GamePlayController.Instance.PauseGame();
            base.Execute();
            heroSkillButton.SelectingSkillHeroBtn += Complete;
            IncreaseLayerFocusPoint(heroSkillButton.gameObject);
           
            
        }

    }
}