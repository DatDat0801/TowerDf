using System.Collections;
using EW2;
using UnityEngine;

public class EnemyBlockCollider : SpineBone<EnemyBase>
{
    private Dummy target;

    public virtual bool CanTakeBlock(Dummy creator = null)
    {
        return target == null;
    }

    public void SetBlock(Dummy hero)
    {
        if (Owner.TraitType == TraitEnemyType.Trickster)
        {
            return;
        }

        target = hero;

        Owner.target = target;

        MoveToBlockTarget();

        //Debug.Log($"{Owner.name} was blocked by {hero.name}");
    }

    public void MoveToBlockTarget()
    {
        if (target && Owner.MoveType != MoveType.None)
        {
            Owner.MoveToBlockTarget(target.transform);
        }
    }

    public void RemoveBlock()
    {
        if (Owner.TraitType == TraitEnemyType.Trickster)
        {
            return;
        }

        target = null;

        Owner.target = null;

        if (Owner.IsAlive && Owner.gameObject.activeSelf && !Owner.UnitState.IsStatusCC())
        {
            Owner.MoveToEndPoint();

            CoroutineUtils.Instance.StartCoroutine(DelayBlockCollider());
        }
    }

    private IEnumerator DelayBlockCollider()
    {
        Owner.SetBlockCollider(false);

        yield return new WaitForFixedUpdate();

        Owner.SetBlockCollider(Owner.IsAlive);
    }

    public float Distance()
    {
        if (target)
        {
            return Vector2.Distance(Owner.transform.position, target.transform.position);
        }

        return float.MaxValue;
        //throw new Exception("target is null");
    }
}