using System.Collections;
using System.Collections.Generic;
using EW2.Tutorial.Map;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep7 : FocusType
    {

        private Map0Tutorial map0Tutorial;
        public TutorialStep7()
        {
            tutorialId = AnyTutorialConstants.FOCUS_BUILD_THIRD_TOWER;
        }

        public override void Execute()
        {
            map0Tutorial = FindingObjects.CalculateMap0TutorialInspector();
            base.Execute();
            map0Tutorial.ShowThirdTowerPoint();
            FindingObjects.CalculateCameraController().DisableCanClick();
        }
        
        public override void Complete()
        {
            base.Complete();
            HidingFocus?.Invoke();
            ExecuteNextStepTutorial();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus build the third tower");
        }

        protected override Vector3 CalculateFocusPosition() => map0Tutorial.CalculateThirdTowerPointPosition();

    }
}