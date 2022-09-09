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
        [Header("Open-Pipe / Open-Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;
        [SerializeField] private OpenPipeStyle pipeStyle;
        [SerializeField] private OpenCylinderStyle cylinderStyle;
        [SerializeField, Min(2f)] private float lengthDistance = 20f;
        [SerializeField, Min(8)] private int subdivisionsInside = 32;
        [SerializeField, Min(6)] private int subdivisionsOutside = 16;
        [SerializeField] private UnityEngine.AnimationCurve opennessCurve = new(new(0, 0.5f), new(1, 1));

        public PipeCylinderType Type1 { get => type; set => type = value; }
        public OpenPipeStyle PipeStyle { get => pipeStyle; set => pipeStyle = value; }
        public OpenCylinderStyle CylinderStyle { get => cylinderStyle; set => cylinderStyle = value; }
        public float LengthDistance { get => lengthDistance; set => lengthDistance = value; }
        public int SubdivisionsInside { get => subdivisionsInside; set => subdivisionsInside = value; }
        public int SubdivisionsOutside { get => subdivisionsOutside; set => subdivisionsOutside = value; }

        public UnityEngine.AnimationCurve OpennessCurveDenormalized => new UnityEngine.AnimationCurve(opennessCurve.GetRenormalizedKeyRangeAndTangents(0, GetRoot().GetMaxTime()));


        public enum OpenPipeStyle
        {
            MuteCityCOM,
        }
        public enum OpenCylinderStyle
        {
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
                case OpenPipeStyle.MuteCityCOM: return TristripTemplates.OpenPipe.MuteCityCOM.OpenPipeMaterials();

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
                case OpenCylinderStyle.MuteCityCOM: return TristripTemplates.OpenCylinder.MuteCityCOM.OpenCylinderMaterials();

                default:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
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
        public Tristrip[][] GetPipeTristrips(OpenPipeStyle pipeStyle, bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance);
            var maxTime = GetRoot().GetMaxTime();

            switch (pipeStyle)
            {
                case OpenPipeStyle.MuteCityCOM: return TristripTemplates.OpenPipe.MuteCityCOM.OpenPipe(matrices, this, maxTime, isGfzCoordinateSpace);

                default:
                    //return TristripTemplates.OpenPipe.GenericOpenPipe2Piece(matrices, this, isGfzCoordinateSpace);
                    return new Tristrip[][]
                    {
                        TristripTemplates.OpenPipe.DebugOpenPipe(matrices, this, isGfzCoordinateSpace),
                    };
            }
        }
        public Tristrip[][] GetCylinderTristrips(OpenCylinderStyle cylinderStyle, bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance);
            var maxTime = GetRoot().GetMaxTime();

            switch (cylinderStyle)
            {
                case OpenCylinderStyle.MuteCityCOM: return TristripTemplates.OpenCylinder.MuteCityCOM.OpenCylinder(matrices, this, maxTime, isGfzCoordinateSpace);

                default:
                    return new Tristrip[][]
                    {
                        TristripTemplates.OpenCylinder.DebugOpenCylinder(matrices, this, isGfzCoordinateSpace),
                        TristripTemplates.OpenCylinder.GenericOpenCylinderBottomCapNoTex(matrices, this, isGfzCoordinateSpace),
                        TristripTemplates.OpenCylinder.GenericOpenCylinderEndCapNoTex(matrices, this, isGfzCoordinateSpace),
                    };
            }
        }

        public override TrackSegment CreateTrackSegment()
        {
            // Set flag on if cylinder
            var typeFlags = type == PipeCylinderType.Cylinder ? TrackPipeCylinderFlags.IsCylinderNotPipe : 0;
            // TRS is empty but with Scale.Y acting as the "openness" curve
            var trs = new AnimationCurveTRS();
            trs.Scale.y = OpennessCurveDenormalized;

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
