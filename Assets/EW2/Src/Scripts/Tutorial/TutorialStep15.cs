using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep15 : DialogType
    {
        public TutorialStep15()
        {
            tutorialId = AnyTutorialConstants.DIALOG_1001_15;
        }
        
        public override void Complete()
        {
            base.Complete();
            HidingDialog?.Invoke();
            ExecuteNextStepTutorial();
            FindingObjects.CalculateMap0TutorialInspector().ResetCameraDefault();
            //  GamePlayController.Instance.ResumeGame();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Show dialog 1001");
        }
    }
    
    
}