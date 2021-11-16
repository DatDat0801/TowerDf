using UnityEngine;

namespace EW2
{
    public interface IUnitFlipByLocalPosition
    {
        Vector3 LocalPosition { get; set; }
        string Name { get; }
    }
}