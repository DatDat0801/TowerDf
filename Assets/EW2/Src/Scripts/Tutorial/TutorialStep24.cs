using EW2.Tutorial.General;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2.Tutorial.Step
{
    public class TutorialStep24 : DialogType
    {
        public TutorialStep24()
        {
            tutorialId = AnyTutorialConstants.TRIAL_HERO_DESC;
        }

        public override void Execute()
        {
            FindingObjects.CalculateCameraController().DisableCanClick();
            var stepTutorialData = GameContainer.Instance.GetTutorialData().GetStepTutorialData(tutorialId);
            ShowingDialog?.Invoke(stepTutorialData.speakerId, L.popup.hero_pre_trial_dialog_txt, true);
            UIFrame.Instance.HideAllPanels(true);
            UIFrame.Instance.CloseAllPopup();
        }

        public override void Complete()
        {
            base.Complete();
            UserData.Instance.AccountData.isCompleteTutTrial = true;
            UserData.Instance.Save();
            GamePlayController.Instance.GetCameraController().EnableCanClick();
            HidingDialog?.Invoke();
        }
    }
}