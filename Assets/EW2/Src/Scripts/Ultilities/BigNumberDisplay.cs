namespace EW2
{
    public class BigNumberDisplay
    {
        public int Value { get; set; }

        public string ConvertToString()
        {
            if (Value < 1000) return Value.ToString();
            if (Value >= 1000 && Value < 1000000)
            {
                int div = (int)(Value / 1000);
                return $"{div.ToString()}K";
            }
            else
            {
                int div = (int)(Value / 1000000);
                return $"{div.ToString()}M";
            }
        }
    }
}