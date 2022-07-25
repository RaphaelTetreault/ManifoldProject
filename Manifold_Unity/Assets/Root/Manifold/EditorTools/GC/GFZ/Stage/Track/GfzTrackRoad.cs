using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzTrackRoad : GfzTrackSegmentShapeNode,
        IRailSegment
    {
        // Mesh stuff
        [field: SerializeField, Min(1)] public int WidthDivisions { get; private set; } = 1;
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
            var trackMeshTristrips = TristripGenerator.CreateTempTrackRoad(this, WidthDivisions, LengthDistance, true);
            // convert to GameCube format
            var dlists = TristripGenerator.TristripsToDisplayLists(trackMeshTristrips, GameCube.GFZ.GfzGX.VAT);

            // Compute bounding sphere
            var allVertices = new List<Vector3>();
            foreach (var tristrip in trackMeshTristrips)
                allVertices.AddRange(tristrip.positions);
            var boundingSphere = TristripGenerator.CreateBoundingSphereFromPoints(allVertices, allVertices.Count);

            // Note: this template is both sides, we do not YET need to sort front/back facing tristrips.
            var template = GfzAssetTemplates.MeshTemplates.DebugTemplates.CreateLitVertexColored();
            var gcmf = template.Gcmf;
            gcmf.BoundingSphere = boundingSphere;
            gcmf.Submeshes[0].PrimaryDisplayListsTranslucid = dlists;
            gcmf.Submeshes[0].VertexAttributes = dlists[0].Attributes; // hacky
            gcmf.Submeshes[0].UnkAlphaOptions.Origin = boundingSphere.origin;
            gcmf.PatchTevLayerIndexes();

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
