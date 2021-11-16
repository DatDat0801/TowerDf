using System.Collections;
using System.Collections.Generic;
using EW2.Tutorial.Map;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep19 : FocusType
    {

        private Map0Tutorial map0Tutorial;

        public TutorialStep19()
        {
            tutorialId = AnyTutorialConstants.FOCUS_1001_HERO_SkILL_POSITION;
        }

        public override void Execute()
        {
            map0Tutorial = FindingObjects.CalculateMap0TutorialInspector();
            base.Execute();
            FindingObjects.CalculateCameraController().DisableCanClick();
            map0Tutorial.HeroSkillPositionFocusBtn.gameObject.SetActive(true);
            map0Tutorial.HeroSkillPositionFocusBtn.transform.position = map0Tutorial.HeroSkillTransform.position;
            

        }

        public override void Complete()
        {
            base.Complete();
            HidingFocus?.Invoke();
            GamePlayController.Instance.ResumeGame();
            FindingObjects.CalculateCameraController().EnableCanClick();
            map0Tutorial.HeroSkillPositionFocusBtn.gameObject.SetActive(false);
            var heroSkillBtn = FindingObjects.CalculateGamePlayWindowController().GetFirstHeroSkillBtn();
            heroSkillBtn.ActiveSelectedHeroSkillToTarget(CalculateFocusPosition());
            ComputingNextStep?.Invoke();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus 1001 skill hero position");
        }
        

        protected override Vector3 CalculateFocusPosition()
        {
            return map0Tutorial.HeroSkillTransform.position;
        }
        
    }
}