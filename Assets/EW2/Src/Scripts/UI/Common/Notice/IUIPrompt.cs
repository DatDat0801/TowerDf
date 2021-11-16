namespace EW2
{
    public interface IUIPrompt
    {
        bool Status { get; set; }
        void Notice();
    }
}