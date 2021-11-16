using EW2.Tutorial.Map;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep5 : FocusType
    {

        private Map0Tutorial map0Tutorial;
        public TutorialStep5()
        {
            tutorialId = AnyTutorialConstants.FOCUS_BUILD_SECOND_TOWER;
        }

        public override void Execute()
        {
            map0Tutorial = FindingObjects.CalculateMap0TutorialInspector();
            base.Execute();
            map0Tutorial.ShowSecondTowerPoint();
            //show it brighter only step 5, not at step 12
            map0Tutorial.IncreaseMageTowerPoint();
            FindingObjects.CalculateCameraController().DisableCanClick();
            
            //IncreaseLayerFocusPoint(towerOptionBtn.gameObject);
        }
        
        public override void Complete()
        {
            base.Complete();
            HidingFocus?.Invoke();
            ExecuteNextStepTutorial();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus build the second tower");
        }

        protected override Vector3 CalculateFocusPosition() => map0Tutorial.CalculateSecondTowerPointPosition();
        

    }
}