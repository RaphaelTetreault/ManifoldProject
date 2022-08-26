using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapePipeCylinder : GfzSegmentShape
    {
        [Header("Pipe/Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;
        [SerializeField] private PipeStyle pipeStyle;
        [SerializeField] private CylinderStyle cylinderStyle;
        [SerializeField, Min(1f)] private float lengthDistance = 10f;

        public PipeCylinderType Type
        {
            get => type;
            set => type = value;
        }
        public enum PipeStyle
        {
            Debug,
        }
        public enum CylinderStyle
        {
            Debug,
        }

        public override AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            return new AnimationCurveTRS();
        }

        public override Gcmf CreateGcmf(out GcmfTemplate[] gcmfTemplates, TplTextureContainer tpl)
        {
            bool isPipe = type == PipeCylinderType.Pipe;

            var tristripsCollections = isPipe
                ? GetPipeTristrips(pipeStyle, true)
                : GetCylinderTristrips(cylinderStyle, true);
            gcmfTemplates = isPipe
                ? GetPipeGcmfTemplates(pipeStyle)
                : GetCylinderGcmfTemplates(cylinderStyle);

            var gcmf = GcmfTemplate.CreateGcmf(gcmfTemplates, tristripsCollections, tpl);
            return gcmf;
        }

        public GcmfTemplate[] GetPipeGcmfTemplates(PipeStyle pipeStyle)
        {
            switch (pipeStyle)
            {
                default:
                    return new GcmfTemplate[] { GcmfTemplates.Debug.CreateLitVertexColored() };
            }
        }
        public GcmfTemplate[] GetCylinderGcmfTemplates(CylinderStyle cylinderStyle)
        {
            switch (cylinderStyle)
            {
                default:
                    return new GcmfTemplate[] { GcmfTemplates.Debug.CreateLitVertexColored() };
            }
        }

        public Tristrip[][] GetPipeTristrips(PipeStyle pipeStyle, bool isGfzCoordinateSpace)
        {
            var originalMatrice = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance);
            var matrices = TristripGenerator.StripHeight(originalMatrice);
            var maxTime = GetRoot().GetMaxTime();

            switch (pipeStyle)
            {
                default:
                    return new Tristrip[][]
                    {
                        //TristripTemplates.Road.CreateDebug(matrices, this, WidthDivisions, LengthDistance, isGfzCoordinateSpace),
                    };
            }
        }
        public Tristrip[][] GetCylinderTristrips(CylinderStyle cylinderStyle, bool isGfzCoordinateSpace)
        {
            var originalMatrice = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance);
            var matrices = TristripGenerator.StripHeight(originalMatrice);
            var maxTime = GetRoot().GetMaxTime();

            switch (cylinderStyle)
            {
                default:
                    return new Tristrip[][]
                    {
                        //TristripTemplates.Road.CreateDebug(matrices, this, WidthDivisions, LengthDistance, isGfzCoordinateSpace),
                    };
            }
        }

        public override Mesh CreateMesh()
        {
            var tristrips = new Tristrip[0];

            var mesh = TristripsToMesh(tristrips);
            mesh.name = $"Auto Gen - {name}";
            return mesh;
        }

        public override TrackSegment CreateTrackSegment()
        {
            var children = CreateChildTrackSegments();

            var trackSegment = new TrackSegment();
            trackSegment.OrderIndentifier = name;
            trackSegment.SegmentType = TrackSegmentType.IsTrack;
            trackSegment.BranchIndex = GetBranchIndex();
            trackSegment.Children = children;

            return trackSegment;
        }

        public override void UpdateTRS()
        {
            // do nothing :)
        }

    }
}
