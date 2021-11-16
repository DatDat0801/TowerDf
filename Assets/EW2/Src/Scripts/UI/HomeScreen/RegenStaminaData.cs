namespace EW2
{
    public class RegenStaminaData
    {
        public long timeStart;
        
        public long nextTimeRegenSeconds;
        
        public long timeRegenFullSeconds;

        public void ResetRegen()
        {
            timeStart = 0;
            
            nextTimeRegenSeconds = 0;

            timeRegenFullSeconds = 0;
        }
    }
}