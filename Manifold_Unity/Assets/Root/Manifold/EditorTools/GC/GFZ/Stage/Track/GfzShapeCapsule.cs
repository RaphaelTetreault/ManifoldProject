using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapeCapsule : GfzSegmentShape
    {
        [Header("Pipe/Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;
        [SerializeField] private PipeStyle pipeStyle;
        [SerializeField] private CylinderStyle cylinderStyle;
        [SerializeField, Min(2)] private int lengthDistance = 20;
        [SerializeField, Min(4)] private int subdivideSemiCircle = 16;
        [SerializeField, Min(1)] private int subdivideLinee = 1;
        [SerializeField] private UnityEngine.AnimationCurve capsuleWidth = new(new(0, 45), new(1, 45));

        public int SubdivideSemiCircle => subdivideSemiCircle;
        public int SubdivideLine => subdivideLinee;

        public enum PipeStyle
        {
            Debug,
            MuteCity,
        }
        public enum CylinderStyle
        {
            Debug,
            MuteCityCOM,
        }

        public PipeCylinderType Type
        {
            get => type;
            set => type = value;
        }

        public override ShapeID ShapeIdentifier
        {
            get
            {
                bool isPipe = type == PipeCylinderType.Pipe;

                if (isPipe)
                    return ShapeID.pipe;
                else
                    return ShapeID.cylinder;
            }
        }

        public override AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            return new AnimationCurveTRS();
        }

        public override Gcmf CreateGcmf(out GcmfTemplate[] gcmfTemplates, TplTextureContainer tpl)
        {
            var tristripsCollections = GetTristrips(true);
            gcmfTemplates = GetGcmfTemplates();
            var gcmf = GcmfTemplate.CreateGcmf(gcmfTemplates, tristripsCollections, tpl);
            return gcmf;
        }

        public GcmfTemplate[] GetGcmfTemplates()
        {
            bool isPipe = type == PipeCylinderType.Pipe;
            var gcmfTemplates = isPipe
                ? GetPipeGcmfTemplates(pipeStyle)
                : GetCylinderGcmfTemplates(cylinderStyle);
            return gcmfTemplates;
        }
        public GcmfTemplate[] GetPipeGcmfTemplates(PipeStyle pipeStyle)
        {
            switch (pipeStyle)
            {
                //case PipeStyle.MuteCity:
                //    return new GcmfTemplate[]
                //    {
                //        GcmfTemplates.MuteCityCOM.RoadTopNoDividers(),
                //        GcmfTemplates.Debug.CreateLitVertexColored(),
                //        GcmfTemplates.Debug.CreateLitVertexColored(),
                //    };

                default:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.Debug.CreateLitVertexColored(),
                        //GcmfTemplates.Debug.CreateLitVertexColored(),
                        //GcmfTemplates.Debug.CreateLitVertexColored(),
                    };
            }
        }
        public GcmfTemplate[] GetCylinderGcmfTemplates(CylinderStyle cylinderStyle)
        {
            switch (cylinderStyle)
            {
                //case CylinderStyle.MuteCityCOM:
                //    return new GcmfTemplate[]
                //    {
                //        GcmfTemplates.MuteCityCOM.RoadTopNoDividers(),
                //        GcmfTemplates.Debug.CreateLitVertexColored(),
                //    };

                default:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.Debug.CreateLitVertexColored(),
                        //GcmfTemplates.Debug.CreateLitVertexColored(),
                    };
            }
        }

        public Tristrip[][] GetTristrips(bool isGfzCoordinateSpace)
        {
            bool isPipe = type == PipeCylinderType.Pipe;
            var tristrips = isPipe
                ? GetPipeTristrips(pipeStyle, isGfzCoordinateSpace)
                : GetCylinderTristrips(cylinderStyle, isGfzCoordinateSpace);
            return tristrips;
        }
        public Tristrip[][] GetPipeTristrips(PipeStyle pipeStyle, bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance);
            var matricesLeft = SemiCirclesMatrices(matrices, capsuleWidth, true);
            var matricesRight = SemiCirclesMatrices(matrices, capsuleWidth, false);
            var matricesTop = LineMatricesPosition(matrices, capsuleWidth, true);
            var matricesBottom = LineMatricesPosition(matrices, capsuleWidth, false);
            var maxTime = GetRoot().GetMaxTime();

            switch (pipeStyle)
            {
                //case PipeStyle.MuteCity:
                //return new Tristrip[][]
                //{
                //    TristripTemplates.Pipe.GenericInsideOneTexture(matrices, this, maxTime, isGfzCoordinateSpace),
                //    TristripTemplates.Pipe.DebugOutside(matrices, this, isGfzCoordinateSpace),
                //    TristripTemplates.Pipe.DebugRingEndcap(matrices, this),
                //};

                default:
                    return new Tristrip[][]
                    {
                        TristripTemplates.CapsulePipe.DebugInside(matricesLeft, matricesRight, matricesTop, matricesBottom, this, isGfzCoordinateSpace),
                        //TristripTemplates.Pipe.DebugOutside(matrices, this, isGfzCoordinateSpace),
                        //TristripTemplates.Pipe.DebugRingEndcap(matrices, this),
                    };
            }
        }
        public Tristrip[][] GetCylinderTristrips(CylinderStyle cylinderStyle, bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance);
            var matricesLeft = SemiCirclesMatrices(matrices, capsuleWidth, true);
            var matricesRight = SemiCirclesMatrices(matrices, capsuleWidth, false);
            var matricesTop = LineMatricesPosition(matrices, capsuleWidth, true);
            var matricesBottom = LineMatricesPosition(matrices, capsuleWidth, false);
            var maxTime = GetRoot().GetMaxTime();


            switch (cylinderStyle)
            {
                //case CylinderStyle.MuteCityCOM:
                //    return new Tristrip[][]
                //    {
                //        TristripTemplates.Cylinder.GenericOneTexture(matrices, this, maxTime, isGfzCoordinateSpace),
                //        TristripTemplates.Cylinder.DebugEndcap(matrices, this),
                //    };

                default:
                    return new Tristrip[][]
                    {
                        TristripTemplates.CapsulePipe.DebugOutside(matricesLeft, matricesRight, matricesTop, matricesBottom, this, isGfzCoordinateSpace),
                        //TristripTemplates.Cylinder.DebugEndcap(matrices, this),
                    };
            }
        }

        public override Mesh CreateMesh()
        {
            var tristripsColletion = GetTristrips(false);
            var tristrips = CombinedTristrips(tristripsColletion);
            var mesh = TristripsToMesh(tristrips);
            mesh.name = $"Auto Gen - {name}";
            return mesh;
        }

        public override TrackSegment CreateTrackSegment()
        {
            var typeFlags = type == PipeCylinderType.Cylinder ? TrackPipeCylinderFlags.IsCylinderNotPipe : 0;

            var capsuleTRS = new AnimationCurveTRS();
            capsuleTRS.Position.x = CreateWidthCurve(capsuleWidth, GetMaxTime());

            var capsule = new TrackSegment();
            capsule.OrderIndentifier = name + "_capsule";
            capsule.SegmentType = TrackSegmentType.IsEmbed;
            capsule.EmbeddedPropertyType = TrackEmbeddedPropertyType.IsCapsulePipe;
            //capsule.PipeCylinderFlags = typeFlags;
            capsule.BranchIndex = GetBranchIndex();
            //capsule.AnimationCurveTRS = capsuleTRS.ToTrackSegment();
            capsule.FallbackPosition = new Unity.Mathematics.float3(30f/90f, 0, 0);

            var matrix = new TrackSegment();
            matrix.OrderIndentifier = name + "_matrix";
            matrix.SegmentType = TrackSegmentType.IsMatrix;
            matrix.BranchIndex = GetBranchIndex();
            matrix.Children = new TrackSegment[] { capsule };
            matrix.PipeCylinderFlags = typeFlags;

            return matrix;
        }


        public override void UpdateTRS()
        {
            // do nothing :)
        }



        private static Matrix4x4[] SemiCirclesMatrices(Matrix4x4[] matrices, UnityEngine.AnimationCurve animationCurve, bool isLeftSide)
        {
            var offsetMatrices = new Matrix4x4[matrices.Length];
            float direction = isLeftSide ? 0.5f : -0.5f;
            for (int i = 0; i < matrices.Length; i++)
            {
                float percentage = i / (matrices.Length - 1);
                float width = animationCurve.EvaluateNormalized(percentage);
                Vector3 widthOffset = new Vector3(width * direction, 0, 0);

                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.rotation;
                var scale = matrix.lossyScale;
                position += rotation * widthOffset;

                offsetMatrices[i] = Matrix4x4.TRS(position, rotation, scale);
            }
            return offsetMatrices;
        }
        private static Matrix4x4[] LineMatricesPosition(Matrix4x4[] matrices, UnityEngine.AnimationCurve animationCurve, bool isTop)
        {
            Quaternion rotationOffset = isTop
                ? Quaternion.Euler(new Vector3(0, 0, 180))
                : Quaternion.identity;
            float direction = isTop ? 0.5f : -0.5f;

            var offsetMatrices = new Matrix4x4[matrices.Length];
            for (int i = 0; i < matrices.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.rotation;
                var scale = matrix.lossyScale;

                float percentage = i / (matrices.Length - 1);

                //
                float width = animationCurve.EvaluateNormalized(percentage);
                Vector3 heightOffset = new Vector3(0, scale.y * direction, 0);

                position += rotation * heightOffset;
                rotation = rotation * rotationOffset;
                scale = new Vector3(width, scale.y, scale.z);

                offsetMatrices[i] = Matrix4x4.TRS(position, rotation, scale);
            }
            return offsetMatrices;
        }
        private static UnityEngine.AnimationCurve CreateWidthCurve(UnityEngine.AnimationCurve animationCurve, float maxTime)
        {
            var keys = animationCurve.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var newAnimationCurve = new UnityEngine.AnimationCurve(keys);
            return newAnimationCurve;
        }

    }
}
