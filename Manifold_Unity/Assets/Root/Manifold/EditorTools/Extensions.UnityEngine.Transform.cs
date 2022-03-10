using UnityEngine;

namespace Manifold.EditorTools
{
    public static partial class TransformExtensions
    {
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
