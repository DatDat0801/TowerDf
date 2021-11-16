
namespace EW2
{
    public class PoisonDumpUnit : Unit
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
