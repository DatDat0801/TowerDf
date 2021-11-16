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
    public class TutorialStep21 : FocusType
    {
        private ButtonSkillUpgradeController skillUpgradeBtn;

        public TutorialStep21()
        {
            tutorialId = AnyTutorialConstants.FOCUS_UPGRADE_SKILL_HERO;
        }

        public override void Execute()
        {
            skillUpgradeBtn = FindingObjects.CalculateHeroSkillContentController().GetPassive3SkillUpgradeBtn();
            TutorialManager.Instance.StartCoroutine(CoExecute());
        }

        public override void Complete()
        {
            base.Complete();
            HidingFocus?.Invoke();
            if (skillUpgradeBtn)
                DecreaseLayerFocusPoint(skillUpgradeBtn.gameObject);
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus upgrade skill hero");
            UIFrame.Instance.EnableCanKeyBack(true);
        }


        protected override Vector3 CalculateFocusPosition()
        {
            return skillUpgradeBtn.Focus.transform.position;
        }

        private IEnumerator CoExecute()
        {
            var secondDelay = 0.2f;
            yield return new WaitForSecondsRealtime(secondDelay);
            base.Execute();
            IncreaseLayerFocusPoint(skillUpgradeBtn.gameObject);
        }
    }
}