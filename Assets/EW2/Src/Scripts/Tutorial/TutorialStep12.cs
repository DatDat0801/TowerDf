using EW2.Tutorial.Map;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep12 : FocusType
    {

        private Map0Tutorial map0Tutorial;
        public TutorialStep12()
        {
            tutorialId = AnyTutorialConstants.FOCUS_MAGE_BUILED_TOWER;
        }

        public override void Execute()
        {
            map0Tutorial = FindingObjects.CalculateMap0TutorialInspector();
            base.Execute();
            map0Tutorial.ShowSecondTowerPoint();
            map0Tutorial.TrackCompleteFocusMageBuildedTowerTutorial();
            FindingObjects.CalculateCameraController().DisableCanClick();
            
        }
        
        public override void Complete()
        {
            base.Complete();
            HidingFocus?.Invoke();
            ExecuteNextStepTutorial();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus mage tower");
        }

        protected override Vector3 CalculateFocusPosition() => map0Tutorial.CalculateSecondTowerPointPosition();

    }
}