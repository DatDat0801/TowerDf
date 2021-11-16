namespace EW2
{
    public abstract class TowerSkill2 : BaseTowerSkill
    {
        protected TowerSkill2()
        {
            MaxLevel = 1;
        }

        protected override void OnRaiseSuccess()
        {
            if (timeCooldown <= 0)
                ActiveSkill();
            else
                ResetCooldown();
        }
    }
}