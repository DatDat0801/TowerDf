using System.Collections;
using System.Collections.Generic;
using EW2.Tutorial.Map;
using UnityEngine;
using EW2.Tutorial.General;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2.Tutorial.Step
{
    public class TutorialStep20 : FocusType
    {

        private Button levelUpBtn;

        public TutorialStep20()
        {
            tutorialId = AnyTutorialConstants.FOCUS_UPGRADE_LEVEL_HERO;
        }

        public override void Execute()
        {
            UIFrame.Instance.EnableCanKeyBack(false);
            TutorialManager.Instance.StartCoroutine(CoExecute());
            
        }

        public override void Complete()
        {
            base.Complete();
            HidingFocus?.Invoke();
            DecreaseLayerFocusPoint(levelUpBtn.gameObject);
            ExecuteNextStepTutorial();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus upgrade level hero");
        }
        

        protected override Vector3 CalculateFocusPosition()
        {
            return levelUpBtn.transform.position;
        }

        private IEnumerator CoExecute()
        {
            var secondDelay = 0.3f;
            yield return  new WaitForSecondsRealtime(secondDelay);
            levelUpBtn = FindingObjects.CalculateHeroSkillContentController().BtnLevelUp;
            if(levelUpBtn == null) yield return null;
            base.Execute();
            IncreaseLayerFocusPoint(levelUpBtn.gameObject);
            
        }
        
    }
}