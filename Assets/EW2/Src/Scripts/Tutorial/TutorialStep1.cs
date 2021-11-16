using System.Collections;
using System.Collections.Generic;
using EW2.Tutorial.General;
using UnityEngine;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep1 : DialogType
    {
        public TutorialStep1()
        {
            tutorialId = AnyTutorialConstants.DIALOG_1001_1;
        }

        public override void Execute()
        {
            base.Execute();
            HideObjects();

        }

        public override void Complete()
        {
            base.Complete();
            ExecuteNextStepTutorial();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "show dialog 1001_1");
        }

        private void HideObjects()
        {
            FindingObjects.CalculateGamePlayController().HideStartWaveBtn();
            FindingObjects.CalculateMap0TutorialInspector().HideAllTowerPoints();
        }
    }
}