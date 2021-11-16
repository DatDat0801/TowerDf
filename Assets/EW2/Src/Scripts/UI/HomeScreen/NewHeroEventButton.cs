using EW2;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

public class NewHeroEventButton : SalebundleButtonUi
{
    private void OnEnable()
    {
        title.text = L.game_event.nhero_event_name_txt.ToUpper();
        var data = UserData.Instance.UserEventData.NewHeroEventUserData;
        timeRemain.SetTimeRemain(data.TimeRemain());
    }

    public override void ButtonOnClick()
    {
        UIFrame.Instance.OpenWindow(ScreenIds.new_hero_event);
        FirebaseLogic.Instance.ButtonClick("main_menu", "new_hero_event", 0);
    }
}