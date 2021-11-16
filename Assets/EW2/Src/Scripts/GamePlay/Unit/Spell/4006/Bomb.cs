namespace EW2.Spell
{
    //make enemy dies by Bomb unit, not by spell 4006 Unit
    public class Bomb : Unit
    {
        private UnitState unitState;
        public override UnitState UnitState => unitState ?? (unitState = new DummyState(this));
        public override void OnUpdate(float deltaTime)
        {
            
        }

        public override void Remove()
        {
            
        }

        protected override void InitAction()
        {
            
        }
    }
}