using Spine.Unity;
using UnityEngine;

namespace EW2
{
    public class SpineMaterialChanger : MonoBehaviour
    {
        [SerializeField] private Material originalMaterial;
        [SerializeField] private Material replacementMaterial;

        private SkeletonRenderer _skeletonRenderer;

        public void SetCustomMaterialOverrides()
        {
            if (this._skeletonRenderer == null)
            {
                this._skeletonRenderer = GetComponent<SkeletonRenderer>();
                this._skeletonRenderer.Initialize(false);
            }
            
            this._skeletonRenderer.CustomMaterialOverride[this.originalMaterial] = this.replacementMaterial;
        }

        public void RemoveCustomMaterialOverrides()
        {
            if (this._skeletonRenderer == null)
            {
                this._skeletonRenderer = GetComponent<SkeletonRenderer>();
                this._skeletonRenderer.Initialize(false);
            }

            this._skeletonRenderer.CustomMaterialOverride.Remove(this.originalMaterial);
        }
    }
}