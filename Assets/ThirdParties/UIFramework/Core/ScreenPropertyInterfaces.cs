namespace Zitga.UIFramework
{
    /// <summary>
    /// Base interface for all the screen properties
    /// </summary>
    public interface IScreenProperties
    {
    }

    /// <summary>
    /// Base interface for all Panel properties
    /// </summary>
    public interface IPanelProperties : IScreenProperties
    {
        PanelPriority Priority { get; set; }
    }

    /// <summary>
    /// Base interface for Window properties.
    /// </summary>
    public interface IWindowProperties : IScreenProperties
    {
        WindowPriority WindowQueuePriority { get; set; }
        bool HideOnForegroundLost { get; set; }
        bool IsPopup { get; set; }
        bool IsBgClose { get; set; }
        bool SuppressPrefabProperties { get; set; }
        bool CanNotKeyBack { get; set; }
    }
}