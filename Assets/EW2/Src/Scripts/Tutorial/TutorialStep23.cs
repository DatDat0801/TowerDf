using System.Collections;
using System.Collections.Generic;
using EW2.Tutorial.Map;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep23 : FocusType
    {

        private Map0Tutorial map0Tutorial;

        public TutorialStep23()
        {
            tutorialId = AnyTutorialConstants.FOCUS_1002_HERO_SkILL_POSITION;
        }

        public override void Execute()
        {
            map0Tutorial = FindingObjects.CalculateMap0TutorialInspector();
            base.Execute();
            FindingObjects.CalculateCameraController().DisableCanActiveUI();
            map0Tutorial.HeroSkillPositionFocusBtn.gameObject.SetActive(true);
            map0Tutorial.HeroSkillPositionFocusBtn.transform.position = map0Tutorial.HeroSkillTransform.position;
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus 1002 hero skill position");
        }

        public override void Complete()
        {
            base.Complete();
            
            GamePlayController.Instance.ResumeGame();
            
            map0Tutorial.HeroSkillPositionFocusBtn.gameObject.SetActive(false);
            var heroSkillBtn = FindingObjects.CalculateGamePlayWindowController().GetFirstHeroSkillBtn();
            heroSkillBtn.ActiveSelectedHeroSkillToTarget(CalculateFocusPosition());
            FindingObjects.CalculateCameraController().EnableCanClick();
            FindingObjects.CalculateCameraController().EnableCanActiveUI();
            
            HidingFocus?.Invoke();
            ComputingNextStep?.Invoke();
            
        }
        

        protected override Vector3 CalculateFocusPosition()
        {
            return map0Tutorial.HeroSkillTransform.position;
        }
        
    }
}