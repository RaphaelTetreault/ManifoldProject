using UnityEngine;

namespace Manifold.EditorTools
{
    /// <summary>
    /// Extensions to Unity's Transform class.
    /// </summary>
    public static partial class TransformExtensions
    {
        /// <summary>
        /// Get an array of all children of this Transform in order they appear in the hierarchy.
        /// </summary>
        /// <param name="transform">The transform whom's children we wish to get.</param>
        /// <returns>
        /// An array of <paramref name="transform"/>'s child transforms.
        /// </returns>
        public static Transform[] GetChildren(this Transform transform)
        {
            var children = new Transform[transform.childCount];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = transform.GetChild(i);
            }

            return children;
        }
        
    }
}
