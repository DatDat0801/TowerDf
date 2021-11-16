using System.Collections.Specialized;

namespace EW2
{
    public interface RangeTower
    {
        TowerRangeTargetCollection SearchTarget { get; }
        void SetRangeDetect();
        void OnTargetChange(object sender, NotifyCollectionChangedEventArgs args);
        void PrepareAttackState();
        void AttackTarget();
    }
}