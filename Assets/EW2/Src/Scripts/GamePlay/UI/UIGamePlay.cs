namespace EW2
{
    public enum UI_STATE
    {
        Free,
        Soft,
        Normal,
        SelectHero,
        Rally,
        ActiveSkill,
        ActiveSpell
    }

    public interface UIGameplay
    {
        void Open();

        void Close();

        UI_STATE GetUIType();
    }
}