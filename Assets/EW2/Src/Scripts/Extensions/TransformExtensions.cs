using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

namespace EW2.Tools
{
    /// <summary>
    /// Transform extensions
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Destroys a transform's children
        /// </summary>
        /// <param name="transform"></param>
        public static void DestroyAllChildren(this Transform transform)
        {
            for (int t = transform.childCount - 1; t >= 0; t--)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(transform.GetChild(t).gameObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(transform.GetChild(t).gameObject);
                }
            }
        }
        /// <summary>
        /// Despawn all children using Lean pool
        /// </summary>
        /// <param name="transform"></param>
        public static void DespawnAllChildren(this Transform transform)
        {
            for (int t = transform.childCount - 1; t >= 0; t--)
            {
                LeanPool.Despawn(transform.GetChild(t).gameObject);
            }
        }

        /// <summary>
        /// Finds children by name, breadth first
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="transformName"></param>
        /// <returns></returns>
        public static Transform FindDeepChildBreadthFirst(this Transform parent, string transformName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                var child = queue.Dequeue();
                if (child.name == transformName)
                {
                    return child;
                }
                foreach (Transform t in child)
                {
                    queue.Enqueue(t);
                }
            }
            return null;
        }

        /// <summary>
        /// Finds children by name, depth first
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="transformName"></param>
        /// <returns></returns>
        public static Transform FindDeepChildDepthFirst(this Transform parent, string transformName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == transformName)
                {
                    return child;
                }

                var result = child.FindDeepChildDepthFirst(transformName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}