using UnityEngine;

namespace EW2
{
    public class UnitFlipByLocalPosition : MonoBehaviour, IUnitFlipByLocalPosition
    {
        public Vector3 LocalPosition
        {
            get => transform.localPosition;
            set => this.transform.localPosition = value;
        }

        public string Name { get => name; }
    }
}