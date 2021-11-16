using UnityEngine;

namespace EW2
{
    public class UnitFlipByLocalScale : MonoBehaviour, IUnitFlipByLocalScale
    {
        public Vector3 LocalScale
        {
            get => transform.localScale;
            set => this.transform.localScale = value;
        }
    }
}