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
        //public Matrix4x4 StaticMatrix { get; set; } = new();
        public AnimationCurveTRS AnimationCurveTRS { get; set; } = new();
        public HierarchichalAnimationCurveTRS Parent { get; set; } = null;
        public HierarchichalAnimationCurveTRS[] Children { get; set; } = new HierarchichalAnimationCurveTRS[0];

        public bool IsRoot => Parent is null;
        public bool HasParent => Parent is not null;
        public bool HasChildren => Children.IsNullOrEmpty();

        public float GetMaxTime() => AnimationCurveTRS.GetMaxTime();
        public float GetRootMaxTime() => GetRoot().GetMaxTime();
        public float GetSegmentLength() => GetRootMaxTime();

        // PROBABLY WRONG
        public Matrix4x4 EvaluateHierarchyMatrix(double time)
        {
            // Get parent matrix. This is recursive.
            var parentMatrix = HasParent
                ? Parent.EvaluateHierarchyMatrix(time)
                : Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

            var selfAnimationMatrix = AnimationCurveTRS.EvaluateMatrix(time);
            //var selfMatrix = StaticMatrix * selfAnimationMatrix;

            //var finalMatrix = parentMatrix * selfMatrix;
            var finalMatrix = parentMatrix * selfAnimationMatrix;
            return finalMatrix;
        }

        //public Matrix4x4 EvaluateHierarchyMatrix2(double time)
        //{
        //    var finalStaticMatrix = EvaluateStaticMatrices();
        //    var finalAnimationMatrix = EvaluateAnimationMatrices(time);
        //    var finalMatrix = finalAnimationMatrix * finalStaticMatrix;
        //    return finalMatrix;
        //}

        public Vector3 EvaluateHierarchyPosition(double time)
        {
            // Get parent matrix. This is recursive.
            var parentPosition = HasParent
                ? Parent.EvaluateHierarchyPosition(time)
                : Vector3.zero;

            var selfAnimationPosition = AnimationCurveTRS.Position.Evaluate(time);
            //var selfPosition = StaticMatrix.Position() + selfAnimationPosition;

            //var finalMatrix = parentPosition + selfPosition;
            var finalMatrix = parentPosition + selfAnimationPosition;
            return finalMatrix;
        }
        public float3 EvaluatePosition(double time) => EvaluateHierarchyPosition(time);

        //public Matrix4x4 EvaluateStaticMatrices()
        //{
        //    var parentStaticMatrix = HasParent
        //        ? Parent.EvaluateStaticMatrices()
        //        : Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

        //    var staticMatrix = parentStaticMatrix * StaticMatrix;
        //    return staticMatrix;
        //}
        public Matrix4x4 EvaluateAnimationMatrices(double time)
        {
            var parentAniamtionMatrix = HasParent
                ? Parent.EvaluateAnimationMatrices(time)
                : Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

            var animationMatrix = AnimationCurveTRS.EvaluateMatrix(time);
            var staticMatrix = parentAniamtionMatrix * animationMatrix;
            return staticMatrix;
        }


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
