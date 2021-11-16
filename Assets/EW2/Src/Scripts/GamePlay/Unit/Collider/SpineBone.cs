using Spine.Unity;
using UnityEngine;

namespace EW2
{
    [RequireComponent(typeof(SkeletonUtilityBone))]
    public class SpineBone<T> : MonoBehaviour where T : Unit
    {
        private SkeletonUtilityBone skeleton;

        private T owner;
        public T Owner {
            get
            {
                if (owner == null)
                {
                    if (skeleton == null)
                    {
                        skeleton = GetComponent<SkeletonUtilityBone>();
                    }
                    
                    owner = skeleton.hierarchy.GetComponent<T>();
                }

                return owner;
            }
        }
    }
}