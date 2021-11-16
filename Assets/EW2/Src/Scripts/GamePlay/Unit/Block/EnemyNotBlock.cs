namespace EW2
{
    public class EnemyNotBlockCollider : SpineBone<EnemyBase>
    {

        public bool CanTakeBlock(Dummy creator)
        {
            return false;
        }
    }
}