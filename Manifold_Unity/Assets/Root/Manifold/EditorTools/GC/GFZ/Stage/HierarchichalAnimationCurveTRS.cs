using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [System.Serializable]
    public class HierarchichalAnimationCurveTRS
    {
        public Matrix4x4 StaticMatrix { get; set; } = new();
        public AnimationCurveTRS AnimationTRS { get; set; } = new();
        public HierarchichalAnimationCurveTRS Parent { get; set; } = null;
        public HierarchichalAnimationCurveTRS[] Children { get; set; } = null;

        public bool IsRoot => Parent is null;
        public bool HasParent => Parent is not null;
        public bool HasChildren => Children.IsNullOrEmpty();

        public Matrix4x4 EvaluateHierarchyMatrix(double time)
        {
            // Get parent matrix. This is recursive.
            var parentMatrix = HasParent
                ? Parent.EvaluateHierarchyMatrix(time)
                : Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

            var selfAnimationMatrix = AnimationTRS.EvaluateMatrix(time);
            var selfMatrix = StaticMatrix * selfAnimationMatrix;

            var finalMatrix = parentMatrix * selfMatrix;
            return finalMatrix;
        }

        public HierarchichalAnimationCurveTRS[] Temp()
        {
            // Get the root. From there, make a list of all leaves, recursively
            // collect all children, then return that.

            var root = GetRoot();
            var leaves = new List<HierarchichalAnimationCurveTRS>();
            root.GetLeaves(leaves);
            return leaves.ToArray();
        }

        private HierarchichalAnimationCurveTRS GetRoot()
        {
            var root = IsRoot ? this : Parent.GetRoot();
            return root;
        }

        /// <summary>
        /// Recursively dig into hierachy and store <paramref name="leaves"/> inside list.
        /// </summary>
        /// <param name="leaves"></param>
        /// <returns></returns>
        private void GetLeaves(List<HierarchichalAnimationCurveTRS> leaves)
        {
            foreach (var child in Children)
            {
                if (child.HasChildren)
                    child.GetLeaves(leaves);
                else
                    leaves.Add(child);
            }
        }

    }
}
