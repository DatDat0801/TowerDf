using System;
using EW2.Tutorial.General;
using Zitga.UIFramework;

namespace EW2.Tutorial.Step
{
    public class DialogType : TutorialBase
    {
        public static Action<int, string, bool> ShowingDialog { get; set; }
        public static Action HidingDialog { get; set; }

        public override void Execute()
        {
            base.Execute();
            FindingObjects.CalculateCameraController().DisableCanClick();
            var stepTutorialData = GameContainer.Instance.GetTutorialData().GetStepTutorialData(tutorialId);
            ShowingDialog?.Invoke(stepTutorialData.speakerId, stepTutorialData.dialog, false);
            UIFrame.Instance.HideAllPanels(true);
            UIFrame.Instance.CloseAllPopup();
        }
    }
}