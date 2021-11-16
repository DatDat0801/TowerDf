using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EW2.Tutorial.General;
using EW2.Tutorial.Map;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2.Tutorial.Step
{
    public class TutorialStep11 : DialogType
    {
        private Map0Tutorial map0Tutorial;
        
        public TutorialStep11()
        {
            tutorialId = AnyTutorialConstants.DIALOG_1001_11;
        }

        public override void Execute()
        {
            base.Execute();
            map0Tutorial = FindingObjects.CalculateMap0TutorialInspector();
            GamePlayController.Instance.PauseGame();
        }

        public override void Complete()
        {
            base.Complete();
            HidingDialog?.Invoke();
            GamePlayController.Instance.ResumeGame();
            map0Tutorial.ResetCameraDefault();
            TutorialManager.Instance.StartCoroutine(CoExecuteNextStepTutorial());
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Show dialog of hero id 1001");
        }

        private IEnumerator CoExecuteNextStepTutorial()
        {
            var secondDelay = map0Tutorial.CalculateSecondDelayCameraFocus() + 0.8f;
            yield return new WaitForSecondsRealtime(secondDelay);
            GamePlayController.Instance.PauseGame();
            ExecuteNextStepTutorial();
        }
    }
    
    
}