using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapeOpenPipeCylinder : GfzShape
    {
        [Header("Half-Pipe / Half-Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;
        [SerializeField] private OpenPipeStyle pipeStyle;
        [SerializeField] private OpenCylinderStyle cylinderStyle;
        [SerializeField, Min(2f)] private float lengthDistance = 20f;
        [SerializeField, Min(8)] private int subdivisionsInside = 32;
        [SerializeField, Min(6)] private int subdivisionsOutside = 16;
        [SerializeField] private AnimationCurveTRS trs;

        public UnityEngine.AnimationCurve scaleY => new UnityEngine.AnimationCurve(trs.Scale.y.GetRenormalizedKeyRangeAndTangents(0, GetRoot().GetMaxTime()));

        public enum OpenPipeStyle
        {

        }
        public enum OpenCylinderStyle
        {

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
            //var temp = trs.CreateDeepCopy();
            //trs.Scale.y = new UnityEngine.AnimationCurve(trs.Scale.y.GetRenormalizedKeyRangeAndTangents(0, GetRoot().GetMaxTime()));
            //return temp;

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
        public GcmfTemplate[] GetPipeGcmfTemplates(OpenPipeStyle pipeStyle)
        {
            switch (pipeStyle)
            {
                //case PipeStyle.MuteCity:
                //    return new GcmfTemplate[]
                //    {
                //        GcmfTemplates.MuteCityCOM.RoadTopNoDividers(),
                //        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                //        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                //    };

                default:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                        //GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                        //GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                    };
            }
        }
        public GcmfTemplate[] GetCylinderGcmfTemplates(OpenCylinderStyle cylinderStyle)
        {
            switch (cylinderStyle)
            {
                //case CylinderStyle.MuteCityCOM:
                //    return new GcmfTemplate[]
                //    {
                //        GcmfTemplates.MuteCityCOM.RoadTopNoDividers(),
                //        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                //    };

                default:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                        //GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
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
        public Tristrip[][] GetPipeTristrips(OpenPipeStyle pipeStyle, bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance);
            var maxTime = GetRoot().GetMaxTime();

            switch (pipeStyle)
            {
                //case OpenPipeStyle.MuteCity:
                //    return new Tristrip[][]
                //    {
                //        TristripTemplates.Pipe.GenericInsideOneTexture(matrices, this, maxTime, isGfzCoordinateSpace),
                //        TristripTemplates.Pipe.DebugOutside(matrices, this, isGfzCoordinateSpace),
                //        TristripTemplates.Pipe.DebugRingEndcap(matrices, this),
                //    };

                default:
                    return new Tristrip[][]
                    {
                        TristripTemplates.OpenPipe.GenericFlatToSemiCircleNoTex(matrices, this, isGfzCoordinateSpace),
                        //TristripTemplates.Pipe.DebugOutside(matrices, this, isGfzCoordinateSpace),
                        //TristripTemplates.Pipe.DebugRingEndcap(matrices, this),
                    };
            }
        }
        public Tristrip[][] GetCylinderTristrips(OpenCylinderStyle cylinderStyle, bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance);
            var maxTime = GetRoot().GetMaxTime();

            switch (cylinderStyle)
            {
                //case OpenCylinderStyle.MuteCityCOM:
                //    return new Tristrip[][]
                //    {
                //        TristripTemplates.Cylinder.GenericOneTexture(matrices, this, maxTime, isGfzCoordinateSpace),
                //        TristripTemplates.Cylinder.DebugEndcap(matrices, this),
                //    };

                default:
                    return new Tristrip[][]
                    {
                        TristripTemplates.OpenPipe.GenericFlatToSemiCircleNoTex(matrices, this, isGfzCoordinateSpace),
                        //TristripTemplates.Cylinder.DebugEndcap(matrices, this),
                    };
            }
        }

        public override TrackSegment CreateTrackSegment()
        {
            // Set flag on if cylinder
            var typeFlags = type == PipeCylinderType.Cylinder ? TrackPipeCylinderFlags.IsCylinderNotPipe : 0;

            var leafEmbed = new TrackSegment();
            leafEmbed.OrderIndentifier = name + "_leaf";
            leafEmbed.SegmentType = TrackSegmentType.IsEmbed;
            leafEmbed.PipeCylinderFlags = TrackPipeCylinderFlags.IsOpenPipeOrCylinder;
            leafEmbed.BranchIndex = GetBranchIndex();
            leafEmbed.AnimationCurveTRS = trs.ToTrackSegment();
            leafEmbed.Children = CreateChildTrackSegments();

            // can define rail height... but ignored

            var rootEmbed = new TrackSegment();
            rootEmbed.OrderIndentifier = name + "_root";
            rootEmbed.SegmentType = TrackSegmentType.IsEmbed;
            rootEmbed.EmbeddedPropertyType = TrackEmbeddedPropertyType.IsOpenPipeOrCylinder;
            rootEmbed.PerimeterFlags = TrackPerimeterFlags.hasRailHeightLeft | TrackPerimeterFlags.hasRailHeightRight;
            rootEmbed.RailHeightLeft = 3;
            rootEmbed.RailHeightRight = 3;
            rootEmbed.PipeCylinderFlags = typeFlags; // DEFINES IF PIPE OR CYLINDER
            rootEmbed.BranchIndex = GetBranchIndex();
            rootEmbed.Children = new TrackSegment[] { leafEmbed };

            return rootEmbed;
        }

        public override void UpdateTRS()
        {
            // do nothing :)
        }

    }
}
