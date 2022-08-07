using Manifold.IO;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [System.Serializable]
    public class HierarchichalAnimationCurveTRS :
        IPositionEvaluable
    {
        public AnimationCurveTRS AnimationCurveTRS { get; set; } = new();
        public HierarchichalAnimationCurveTRS Parent { get; set; } = null;
        public HierarchichalAnimationCurveTRS[] Children { get; set; } = new HierarchichalAnimationCurveTRS[0];

        public bool IsRoot => Parent is null;
        public bool HasParent => Parent is not null;
        public bool HasChildren => Children.IsNullOrEmpty();

        public float GetMaxTime() => AnimationCurveTRS.GetMaxTime();
        public float GetRootMaxTime() => GetRoot().GetMaxTime();
        public float GetSegmentLength() => GetRootMaxTime();

        public Matrix4x4 EvaluateHierarchyMatrix(double time)
        {
            // Get parent matrix. This is recursive.
            var parentAnimMatrix = HasParent
                ? Parent.EvaluateHierarchyMatrix(time)
                : Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

            var animMatrix = AnimationCurveTRS.EvaluateMatrix(time);
            var finalMatrix = parentAnimMatrix * animMatrix;
            return finalMatrix;
        }

        public Vector3 EvaluateHierarchyPosition(double time)
        {
            // Get parent matrix. This is recursive.
            var parentPosition = HasParent
                ? Parent.EvaluateHierarchyPosition(time)
                : Vector3.zero;

            var animPosition = AnimationCurveTRS.Position.Evaluate(time);
            var finalMatrix = parentPosition + animPosition;
            return finalMatrix;
        }
        public float3 EvaluatePosition(double time) => EvaluateHierarchyPosition(time);


        public HierarchichalAnimationCurveTRS[] GetLeaves()
        {
            // Get the root. From there, make a list of all leaves, recursively
            // collect all children, then return that.

            var root = GetRoot();
            var leaves = new List<HierarchichalAnimationCurveTRS>();
            root.GetLeaves(leaves);
            return leaves.ToArray();
        }
        public HierarchichalAnimationCurveTRS GetRoot()
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
