using System.Collections;
using System.Collections.Generic;
using EW2.Tutorial.Map;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep18 : FocusType
    {
        private SkillButton heroSkillButton;

        public TutorialStep18()
        {
            tutorialId = AnyTutorialConstants.FOCUS_1001_SKILL_ICON;
        }

        public override void Execute()
        {
            heroSkillButton = FindingObjects.CalculateGamePlayWindowController().GetFirstHeroSkillBtn();
            base.Execute();
            heroSkillButton.SelectingSkillHeroBtn += Complete;
            IncreaseLayerFocusPoint(heroSkillButton.gameObject);
            
            GamePlayController.Instance.PauseGame();
        }

        public override void Complete()
        {
            heroSkillButton.SelectingSkillHeroBtn -= Complete;
            base.Complete();
            HidingFocus?.Invoke();
            ExecuteNextStepTutorial();
            DecreaseLayerFocusPoint(heroSkillButton.gameObject);
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus 1001 skill icon");
        }

        protected override Vector3 CalculateFocusPosition() => heroSkillButton.transform.position;

    }
}