using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EW2.Tutorial.General;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep2 : DialogType
    {
        public TutorialStep2()
        {
            tutorialId = AnyTutorialConstants.DIALOG_1001_2;
        }
        
        public override void Complete()
        {
            base.Complete();
            ExecuteNextStepTutorial();
            HidingDialog?.Invoke();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "show dialog 1001_2");
        }
    }
    
    
}