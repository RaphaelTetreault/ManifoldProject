using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzTrackRoad : GfzTrackSegmentShapeNode,
        IRailSegment
    {
        // Mesh stuff
        [field: SerializeField, Range(1, 32)] public int WidthDivisions { get; private set; } = 1;
        [field: SerializeField, Min(1f)] public float LengthDistance { get; private set; } = 10f;

        [field: Header("Road Properties")]
        [field: SerializeField, Min(0f)] public float RailHeightLeft { get; private set; } = 3f;
        [field: SerializeField, Min(0f)] public float RailHeightRight { get; private set; } = 3f;

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            return new AnimationCurveTRS();
        }

        public override Gcmf CreateGcmf()
        {
            // Make the vertex data
            //var trackMeshTristrips = TristripGenerator.CreateTempTrackRoad(this, WidthDivisions, LengthDistance, true);
            //var rails = TristripGenerator.Road.CreateMuteCityRailsOnlyUV(this, WidthDivisions, LengthDistance, true);
            var matrices = TristripGenerator.Road.SimpleMatrices(this, LengthDistance, true);
            var rails = TristripGenerator.Road.CreateMuteCityRails(this, matrices);
            var top = TristripGenerator.Road.CreateMuteCityRoadTop(this, matrices, WidthDivisions);

            var allTristrips = new List<Tristrip>();
            allTristrips.AddRange(rails);
            allTristrips.AddRange(top);
            // Compute bounding sphere
            var globalBoundingSphere = TristripGenerator.CreateBoundingSphereFromTristrips(allTristrips);

            // convert to GameCube format
            var railsDlist = TristripGenerator.TristripsToDisplayLists(rails, GameCube.GFZ.GfzGX.VAT);
            var topDlist = TristripGenerator.TristripsToDisplayLists(top, GameCube.GFZ.GfzGX.VAT);

            // Note: this template is both sides, we do not YET need to sort front/back facing tristrips.
            //var template = GfzAssetTemplates.MeshTemplates.DebugTemplates.CreateLitVertexColored();
            MeshTemplate railsTemplate = GfzAssetTemplates.MeshTemplates.MuteCity.CreateRail();
            MeshTemplate topTemplate = GfzAssetTemplates.MeshTemplates.MuteCity.CreateRoadTop();

            //
            var gcmf = MeshTemplate.CombineTemplates(railsTemplate, topTemplate);
            gcmf.BoundingSphere = globalBoundingSphere;
            //
            gcmf.Submeshes[0].PrimaryFrontFacing = railsDlist;
            gcmf.Submeshes[0].VertexAttributes = railsDlist[0].Attributes;
            gcmf.Submeshes[0].UnkAlphaOptions.Origin = globalBoundingSphere.origin;
            //
            gcmf.Submeshes[1].PrimaryBackFacing = topDlist;
            gcmf.Submeshes[1].VertexAttributes = topDlist[0].Attributes;
            gcmf.Submeshes[1].UnkAlphaOptions.Origin = globalBoundingSphere.origin;

            return gcmf;
        }



        public override Mesh CreateMesh()
        {
            var tristrips = TristripGenerator.CreateTempTrackRoad(this, WidthDivisions, LengthDistance, false);
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
            trackSegment.SetRails(RailHeightLeft, RailHeightRight);
            trackSegment.Children = children;

            return trackSegment;
        }

        public override void UpdateTRS()
        {
            // do nothing :)
        }

    }
}
