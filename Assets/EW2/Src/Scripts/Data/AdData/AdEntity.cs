using System;
namespace EW2
{
    [Serializable]
    public class AdEntity
    {
        public int adId;
        public string placementName;
        public string location;
        public Reward[] rewards;
        public int adQuantity;
    }
}