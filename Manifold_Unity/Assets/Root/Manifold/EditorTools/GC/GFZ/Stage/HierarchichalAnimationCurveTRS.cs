using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [System.Serializable]
    public class HierarchichalAnimationCurveTRS :
        IPositionEvaluable
    {
        public Matrix4x4 StaticMatrix { get; set; } = new();
        public AnimationCurveTRS AnimationCurveTRS { get; set; } = new();
        public HierarchichalAnimationCurveTRS Parent { get; set; } = null;
        public HierarchichalAnimationCurveTRS[] Children { get; set; } = new HierarchichalAnimationCurveTRS[0];

        public bool IsRoot => Parent is null;
        public bool HasParent => Parent is not null;
        public bool HasChildren => Children.IsNullOrEmpty();

        public float GetMaxTime() => AnimationCurveTRS.GetMaxTime();


        // Fuck, hard problem. ALL nodes in hierarchy NEED the same max time for this recusion to work.
        // Normalized func needs to call other normalized functions.
        public Matrix4x4 EvaluateHierarchyMatrix(double time)
        {
            // Get parent matrix. This is recursive.
            var parentMatrix = HasParent
                ? Parent.EvaluateHierarchyMatrix(time)
                : Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

            var selfAnimationMatrix = AnimationCurveTRS.EvaluateMatrix(time);
            var selfMatrix = StaticMatrix * selfAnimationMatrix;

            var finalMatrix = parentMatrix * selfMatrix;
            return finalMatrix;
        }
        public Matrix4x4 EvaluateHierarchyMatrixNormalized(double time01)
        {
            var maxTime = GetMaxTime();
            var matrix = EvaluateHierarchyMatrix(time01 * maxTime);
            return matrix;
        }

        public Vector3 EvaluateHierarchyPosition(double time)
        {
            // Get parent matrix. This is recursive.
            var parentPosition = HasParent
                ? Parent.EvaluateHierarchyPosition(time)
                : Vector3.zero;

            var selfAnimationPosition = AnimationCurveTRS.Position.Evaluate(time);
            var selfPosition = StaticMatrix.Position() + selfAnimationPosition;

            var finalMatrix = parentPosition + selfPosition;
            return finalMatrix;
        }
        public Vector3 EvaluateHierarchyPositionNormalized(double time01)
        {
            var maxTime = GetMaxTime();
            var matrix = EvaluateHierarchyPosition(time01 * maxTime);
            return matrix;
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


        // Maybe shouldn't be here?
        public static bool IsContinuousBetweenFromTo(HierarchichalAnimationCurveTRS from, HierarchichalAnimationCurveTRS to)
        {
            var endMaxTime = from.GetMaxTime();
            var endMatrix = from.EvaluateHierarchyMatrix(endMaxTime);
            var startMatrix = to.EvaluateHierarchyMatrix(0);

            var startPosition = startMatrix.Position();
            var endPosition = endMatrix.Position();

            bool isContinuousBetween = Vector3.Distance(endPosition, startPosition) < 0.01f; // 1cm
            return isContinuousBetween;
        }



        public static double ComputeApproximateLength(HierarchichalAnimationCurveTRS hacTRS, double timeStart, double timeEnd)
        {
            // Entire curve approximate length to within X meters
            const int nStartIterDistance = 50;
            double entireCurveApproximateLength = CurveLengthUtility.GetDistanceBetweenRepeated(hacTRS, timeStart, timeEnd, nStartIterDistance, 2, 1);
            int nApproximationIterations = (int)(entireCurveApproximateLength * (1.0 / nStartIterDistance / 2.0));

            // TimeDelta: difference between start and end times
            double timeDelta = timeEnd - timeStart;
            // Total distance that has been approximated
            double approximateDistance = 0f;
            // How many times we should sample curve.
            double inverseNIterations = 1.0 / nApproximationIterations;

            //// Approximate (segment of) curve length to greater degree.
            //// Since this is a bezier, it means points may not be evenly spaced out.
            //double approximateDistanceTotal = 0;
            //double[] approximateDistances = new double[nApproximationIterations];
            //for (int i = 0; i < nApproximationIterations; i++)
            //{
            //    var distance1 = CurveLengthUtility.GetDistanceBetweenRepeated(evaluable, timeStart, timeEnd, 10f, 2, 1);
            //    approximateDistances[i] = distance1;
            //    approximateDistanceTotal += distance1;
            //}
            //// Figure out how long each segment should be if they were equally spaced
            //double distancePerSegment = approximateDistanceTotal / nApproximationIterations;

            // Sample distance using uneven lengths to compensate and compute a more accurate distance
            for (int i = 0; i < nApproximationIterations; i++)
            {
                // throttle sample rate to distance per segment
                //double sampleNormalizer = distancePerSegment / approximateDistances[i];

                // TODO: pretty this is wrong since smaple normalizer is not corrected between iterations?
                double currDistance = timeStart + (timeDelta * inverseNIterations * (i + 0));
                double nextDistance = timeStart + (timeDelta * inverseNIterations * (i + 1));

                // Compute the distance between these 2 points
                float3 currPosition = hacTRS.EvaluatePosition(currDistance);
                float3 nextPosition = hacTRS.EvaluatePosition(nextDistance);

                // Get distance between 2 points, store delta
                double delta = math.distance(currPosition, nextPosition);
                approximateDistance += delta;
            }

            return approximateDistance;
        }


        public double ComputeApproximateLength(double timeStart, double timeEnd) => ComputeApproximateLength(this, timeStart, timeEnd);
    }
}
