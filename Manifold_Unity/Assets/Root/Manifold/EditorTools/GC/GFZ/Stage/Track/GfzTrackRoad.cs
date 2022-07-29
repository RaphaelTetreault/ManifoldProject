using GameCube.GX;
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
            // Get path matrices
            var matrices = TristripGenerator.Road.CreatePathMatrices(this, LengthDistance, true);
            var maxTime = GetRoot().GetMaxTime();

            // NOTE: Always do alpha last
            var tristripsCollections = new Tristrip[][]
            {
                TristripGenerator.Road.MuteCity.CreateRoadTop(matrices, WidthDivisions, maxTime),
                TristripGenerator.Road.MuteCity.CreateRoadBottom(matrices, WidthDivisions, maxTime),
                TristripGenerator.Road.MuteCity.CreateRoadSides(matrices, 60f, maxTime),
                TristripGenerator.Road.MuteCity.CreateLaneDividers(matrices, maxTime),
                TristripGenerator.Road.MuteCity.CreateRails(matrices, this),
            };
            var templates = new GcmfTemplate[]
            {
                GfzAssetTemplates.MeshTemplates.MuteCity.CreateRoadTop(),
                GfzAssetTemplates.MeshTemplates.MuteCity.CreateRoadBottom(),
                GfzAssetTemplates.MeshTemplates.MuteCity.CreateRoadSides(),
                GfzAssetTemplates.MeshTemplates.MuteCity.CreateLaneDividers(),
                GfzAssetTemplates.MeshTemplates.MuteCity.CreateRails(),
            };
            var gcmf = GcmfTemplate.CreateGcmf(templates, tristripsCollections);
            return gcmf;
        }

        public override Mesh CreateMesh()
        {
            var tristrips = TristripGenerator.CreateTempTrackRoad(this, WidthDivisions, LengthDistance, false);
            //var matrices = TristripGenerator.Road.SimpleMatrices(this, LengthDistance, true);
            //var tristrips = TristripGenerator.Road.CreateMuteCityRoadTop(this, matrices, WidthDivisions);
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
