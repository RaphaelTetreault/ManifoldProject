using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapeCapsule : GfzShape
    {
        [Header("Capsule")]
        [SerializeField] private CapsuleStyle capsuleStyle = CapsuleStyle.MuteCity;
        [SerializeField] public EndcapMode endcapModeIn = EndcapMode.Automatic;
        [SerializeField] public EndcapMode endcapModeOut = EndcapMode.Automatic;
        [SerializeField, Min(2)] private int lengthDistanceInside = 25;
        [SerializeField, Min(2)] private int lengthDistanceOutside = 50;
        [SerializeField, Min(4)] private int subdivideSemiCircleInside = 16;
        [SerializeField, Min(2)] private int subdivideSemiCircleOutside = 6;
        [SerializeField, Min(1)] private int subdivideLineInside = 4;
        [SerializeField, Min(1)] private int subdivideLineOutside = 1;

        public int SubdivideSemiCircleInside => subdivideSemiCircleInside;
        public int SubdivideSemiCircleOutside => subdivideSemiCircleOutside;
        public int SubdivideLineInside => subdivideLineInside;
        public int SubdivideLineOutside => subdivideLineOutside;

        public enum CapsuleStyle
        {
            Debug,
            MuteCity,
        }

        public override ShapeID ShapeIdentifier => ShapeID.pipe;
        public override EndcapMode EndcapModeIn => endcapModeIn;
        public override EndcapMode EndcapModeOut => endcapModeOut;


        public override AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            return new AnimationCurveTRS();
        }

        public override GcmfTemplate[] GetGcmfTemplates()
        {
            switch (capsuleStyle)
            {
                case CapsuleStyle.MuteCity:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.MuteCity.RoadTop(),
                        GcmfTemplates.MuteCityCOM.RoadTopNoDividers(),
                        GcmfTemplates.Debug.CreateLitVertexColored(),
                        GcmfTemplates.Debug.CreateLitVertexColored(),
                    };

                default:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.Debug.CreateLitVertexColored(),
                        GcmfTemplates.Debug.CreateLitVertexColored(),
                        GcmfTemplates.Debug.CreateLitVertexColored(),
                    };
            }
        }

        public override Tristrip[][] GetTristrips(bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistanceInside);
            var matricesLeft = SemiCirclesMatrices(matrices, true);
            var matricesRight = SemiCirclesMatrices(matrices, false);
            var matricesTop = LineMatricesPosition(matrices, true);
            var matricesBottom = LineMatricesPosition(matrices, false);
            var segmentLength = GetRoot().GetMaxTime();

            // diff distance
            var xmatrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistanceOutside);
            var xmatricesLeft = SemiCirclesMatrices(xmatrices, true);
            var xmatricesRight = SemiCirclesMatrices(xmatrices, false);
            var xmatricesTop = LineMatricesPosition(xmatrices, true);
            var xmatricesBottom = LineMatricesPosition(xmatrices, false);

            switch (capsuleStyle)
            {
                case CapsuleStyle.MuteCity:
                    return new Tristrip[][]
                    {
                        TristripTemplates.CapsulePipe.SemiCirclesTex0(matricesLeft, matricesRight, this, segmentLength, isGfzCoordinateSpace, 12),
                        TristripTemplates.CapsulePipe.LinesTex0(matricesTop, matricesBottom, this, segmentLength, 8),
                        TristripTemplates.CapsulePipe.DebugOutside(xmatricesLeft, xmatricesRight, xmatricesTop, xmatricesBottom, this, isGfzCoordinateSpace),
                        TristripTemplates.CapsulePipe.DebugEndcap(matricesLeft, matricesRight, this),
                    };

                default:
                    return new Tristrip[][]
                    {
                        TristripTemplates.CapsulePipe.DebugInside(matricesLeft, matricesRight, matricesTop, matricesBottom, this, isGfzCoordinateSpace),
                        TristripTemplates.CapsulePipe.DebugOutside(xmatricesLeft, xmatricesRight, xmatricesTop, xmatricesBottom, this, isGfzCoordinateSpace),
                        TristripTemplates.CapsulePipe.DebugEndcap(matricesLeft, matricesRight, this),
                        //TristripTemplates.Pipe.DebugOutside(matrices, this, isGfzCoordinateSpace),
                        //TristripTemplates.Pipe.DebugRingEndcap(matrices, this),
                    };
            }
        }

        public override TrackSegment CreateTrackSegment()
        {
            var capsule = new TrackSegment();
            capsule.OrderIndentifier = name + "_capsule";
            capsule.SegmentType = TrackSegmentType.IsEmbed;
            capsule.EmbeddedPropertyType = TrackEmbeddedPropertyType.IsCapsulePipe;
            capsule.BranchIndex = GetBranchIndex();
            capsule.FallbackPosition = new Unity.Mathematics.float3(0.5f, 0, 0);
            capsule.Children = CreateChildTrackSegments();
            // 0.5f since this is the left/right position of the circle-ends radii centers.
            // This results in the track transform's scale.x being 1:1 with the width of the capsule.

            return capsule;
        }

        public override void UpdateTRS()
        {
            // do nothing :)
        }


        private static Matrix4x4[] SemiCirclesMatrices(Matrix4x4[] matrices, bool isLeftSide)
        {
            var offsetMatrices = new Matrix4x4[matrices.Length];
            float direction = isLeftSide ? 0.5f : -0.5f;
            for (int i = 0; i < matrices.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.rotation;
                var scale = matrix.lossyScale;

                Vector3 widthOffset = new Vector3(scale.x * direction, 0, 0);
                position += rotation * widthOffset;
                scale = new Vector3(scale.y, scale.y, 1);

                offsetMatrices[i] = Matrix4x4.TRS(position, rotation, scale);
            }
            return offsetMatrices;
        }
        private static Matrix4x4[] LineMatricesPosition(Matrix4x4[] matrices, bool isTop)
        {
            float direction = isTop ? 0.5f : -0.5f;
            Quaternion rotationOffset = isTop
                ? Quaternion.Euler(new Vector3(0, 0, 180))
                : Quaternion.identity;

            var offsetMatrices = new Matrix4x4[matrices.Length];
            for (int i = 0; i < matrices.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.rotation;
                var scale = matrix.lossyScale;

                Vector3 heightOffset = new Vector3(0, scale.y * direction, 0);

                position += rotation * heightOffset;
                rotation = rotation * rotationOffset;

                offsetMatrices[i] = Matrix4x4.TRS(position, rotation, scale);
            }
            return offsetMatrices;
        }

    }
}
