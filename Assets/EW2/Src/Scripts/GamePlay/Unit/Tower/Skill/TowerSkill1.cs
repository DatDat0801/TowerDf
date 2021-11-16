namespace EW2
{
    public abstract class TowerSkill1 : BaseTowerSkill
    {
        protected TowerSkill1()
        {
            MaxLevel = 3;
        }

        protected override void OnRaiseSuccess()
        {
            ResetCooldown();
        }
    }
}