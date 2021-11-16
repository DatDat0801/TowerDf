using System.Collections;
using System.Collections.Generic;
using EW2.Tutorial.Map;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep16 : FocusType
    {
        private HeroButton heroButton;

        public TutorialStep16()
        {
            tutorialId = AnyTutorialConstants.FOCUS_1002_HERO_ICON;
        }

        public override void Execute()
        {
            heroButton = FindingObjects.CalculateGamePlayWindowController().GetSecondHeroBtn();
            GamePlayController.Instance.ResumeGame();
            FindingObjects.CalculateMap0TutorialInspector().ResetCameraDefault();
            TutorialManager.Instance.StartCoroutine(CoExecute());
            
        }

        public override void Complete()
        {
            base.Complete();
            heroButton.SelectingHeroBtn -= Complete;
            HidingFocus?.Invoke();
            ExecuteNextStepTutorial();
            DecreaseLayerFocusPoint(heroButton.gameObject);
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus hero 1002 hero icon");
        }
        

        protected override Vector3 CalculateFocusPosition() => heroButton.transform.position;

        private IEnumerator CoExecute()
        {
            var secondDelay = 1;
            yield return new WaitForSeconds(secondDelay);
            GamePlayController.Instance.PauseGame();
            base.Execute();
            heroButton.SelectingHeroBtn += Complete;
            IncreaseLayerFocusPoint(heroButton.gameObject);
        }
    }
}