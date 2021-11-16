using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep9 : DialogType
    {
        public TutorialStep9()
        {
            tutorialId = AnyTutorialConstants.DIALOG_1001_9;
        }
        
        public override void Complete()
        {
            base.Complete();
            HidingDialog?.Invoke();
            ExecuteNextStepTutorial();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Show dialog of hero id 1001");
        }
    }
    
    
}