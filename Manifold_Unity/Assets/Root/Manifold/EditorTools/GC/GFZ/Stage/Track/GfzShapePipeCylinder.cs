using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapePipeCylinder : GfzShape
    {
        [Header("Pipe/Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;
        [SerializeField] private PipeStyle pipeStyle;
        [SerializeField] private CylinderStyle cylinderStyle;
        [SerializeField] public EndcapMode endcapModeIn = EndcapMode.Automatic;
        [SerializeField] public EndcapMode endcapModeOut = EndcapMode.Automatic;
        [SerializeField, Min(2f)] private float lengthDistance = 20f;
        [SerializeField, Min(8)] private int subdivisionsInside = 32;
        [SerializeField, Min(6)] private int subdivisionsOutside = 16;


        public int SubdivisionsInside => subdivisionsInside;
        public int SubdivisionsOutside => subdivisionsOutside;


        public PipeCylinderType Type
        {
            get => type;
            set => type = value;
        }
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
        public override EndcapMode EndcapModeIn => endcapModeIn;
        public override EndcapMode EndcapModeOut => endcapModeOut;

        public override AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            return new AnimationCurveTRS();
        }

        public override GcmfTemplate[] GetGcmfTemplates()
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
                case PipeStyle.MuteCity:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.MuteCityCOM.RoadTopNoDividers(),
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                    };

                default:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                    };
            }
        }
        public GcmfTemplate[] GetCylinderGcmfTemplates(CylinderStyle cylinderStyle)
        {
            switch (cylinderStyle)
            {
                case CylinderStyle.MuteCityCOM:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.MuteCityCOM.RoadTopNoDividers(),
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                    };

                default:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                    };
            }
        }

        public override Tristrip[][] GetTristrips(bool isGfzCoordinateSpace)
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
            var maxTime = GetRoot().GetMaxTime();

            switch (pipeStyle)
            {
                case PipeStyle.MuteCity:
                    return new Tristrip[][]
                    {
                        TristripTemplates.Pipe.GenericInsideOneTexture(matrices, this, maxTime, isGfzCoordinateSpace),
                        TristripTemplates.Pipe.DebugOutside(matrices, this, isGfzCoordinateSpace),
                        TristripTemplates.Pipe.DebugRingEndcap(matrices, this),
                    };

                default:
                    return new Tristrip[][]
                    {
                        TristripTemplates.Pipe.DebugInside(matrices, this, isGfzCoordinateSpace),
                        TristripTemplates.Pipe.DebugOutside(matrices, this, isGfzCoordinateSpace),
                        TristripTemplates.Pipe.DebugRingEndcap(matrices, this),
                    };
            }
        }
        public Tristrip[][] GetCylinderTristrips(CylinderStyle cylinderStyle, bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance);
            var maxTime = GetRoot().GetMaxTime();

            switch (cylinderStyle)
            {
                case CylinderStyle.MuteCityCOM:
                    return new Tristrip[][]
                    {
                        TristripTemplates.Cylinder.GenericOneTexture(matrices, this, maxTime, isGfzCoordinateSpace),
                        TristripTemplates.Cylinder.DebugEndcap(matrices, this),
                    };

                default:
                    return new Tristrip[][]
                    {
                        TristripTemplates.Cylinder.Debug(matrices, this, isGfzCoordinateSpace),
                        TristripTemplates.Cylinder.DebugEndcap(matrices, this),
                    };
            }
        }

        public override TrackSegment CreateTrackSegment()
        {
            // Set flag on if cylinder
            var typeFlags = type == PipeCylinderType.Cylinder ? TrackPipeCylinderFlags.IsCylinderNotPipe : 0;

            var trackSegment = new TrackSegment();
            trackSegment.OrderIndentifier = name;
            trackSegment.SegmentType = TrackSegmentType.IsPipeOrCylinder;
            trackSegment.PipeCylinderFlags = typeFlags;
            trackSegment.BranchIndex = GetBranchIndex();
            trackSegment.Children = CreateChildTrackSegments();

            return trackSegment;
        }

        public override void UpdateTRS()
        {
            // do nothing :)
        }

    }
}
