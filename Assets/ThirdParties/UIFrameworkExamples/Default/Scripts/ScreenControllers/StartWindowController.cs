namespace Zitga.UIFramework.Examples
{
    public class StartWindowController : AWindowController
    {
        public void UI_Start() {
            UIFrame.Instance.ShowPanel(ScreenIds.NavigationPanel);
            UIFrame.Instance.ShowPanel(ScreenIds.ToastPanel);
        }
    }
}
