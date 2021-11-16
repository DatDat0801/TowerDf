using System.Collections;
using System.Collections.Generic;
using EW2.Tutorial.Map;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep3 : FocusType
    {

        private Map0Tutorial map0Tutorial;
        public TutorialStep3()
        {
            tutorialId = AnyTutorialConstants.FOCUS_BUILD_FIRST_TOWER;
        }

        public override void Execute()
        {
            map0Tutorial = FindingObjects.CalculateMap0TutorialInspector();
            map0Tutorial.ExecuteFocusTowerBuilding();
            TutorialManager.Instance.StartCoroutine(CoExecute());
        }
        
        public override void Complete()
        {
            base.Complete();
            HidingFocus?.Invoke();
            ExecuteNextStepTutorial();
            //map0Tutorial.ResetLayerFocusPoint();
        }

        protected override Vector3 CalculateFocusPosition() => map0Tutorial.CalculateFirstTowerPointPosition();

        private IEnumerator CoExecute()
        {
            yield return new WaitForSecondsRealtime(map0Tutorial.CalculateSecondDelayCameraFocus());
            base.Execute();
            map0Tutorial.ShowFirstTowerPoint();
            FindingObjects.CalculateCameraController().DisableCanClick();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus build the first tower");
        }

    }
}