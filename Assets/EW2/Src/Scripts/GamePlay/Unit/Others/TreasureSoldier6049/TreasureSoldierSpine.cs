using Spine;

namespace EW2
{
    public class TreasureSoldierSpine : DummySpine
    {
        private string chestOpenName, chestCloseName;

        public TreasureSoldierSpine(Unit owner) : base(owner)
        {

            chestOpenName = "chest_open";

            chestCloseName = "chest_close";
        }
        
        public TrackEntry ChestClose()
        {
            return SetAnimation(1, chestCloseName, true);
        }

        public TrackEntry ChestOpen()
        {
            return SetAnimation(1, chestOpenName, false);
        }
        
    }
}