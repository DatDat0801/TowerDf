namespace EW2
{
    public interface IBuyHeroButton
    {
        int HeroId { get; set; }
        void ButtonClick();
        void ChangeBehavior();
        void ButtonDisableClick();
    }
}