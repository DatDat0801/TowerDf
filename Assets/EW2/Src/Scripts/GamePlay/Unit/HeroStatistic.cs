namespace EW2
{
    public class HeroStatistic
    {
        public int skills = 0;
        public int revive = 0;
        public int move = 0;

        public void UseSkill()
        {
            skills += 1;
        }
        
        public void Revive()
        {
            revive += 1;
        }
        
        public void Move()
        {
            move += 1;
        }
    }
}