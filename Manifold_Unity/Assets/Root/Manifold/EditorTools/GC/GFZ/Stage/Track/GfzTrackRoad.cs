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
            var matrices = TristripGenerator.Road.SimpleMatrices(this, LengthDistance, true);
            var maxTime = GetRoot().GetMaxTime();

            // Construct tristrips
            // Note: Always do alpha last
            var tristripsCollections = new Tristrip[][]
            {
                TristripGenerator.Road.CreateMuteCityRoadTop(this, matrices, WidthDivisions, maxTime),
                TristripGenerator.Road.CreateMuteCityRoadBottom(this, matrices, WidthDivisions, maxTime),
                TristripGenerator.Road.CreateMuteCityRoadSides(this, matrices, 60f, maxTime),
                TristripGenerator.Road.CreateMuteCityLaneDividers(this, matrices, maxTime),
                TristripGenerator.Road.CreateMuteCityRails(this, matrices),
            };
            var templates = new MeshTemplate[]
            {
                GfzAssetTemplates.MeshTemplates.MuteCity.CreateRoadTop(),
                GfzAssetTemplates.MeshTemplates.MuteCity.CreateRoadBottom(),
                GfzAssetTemplates.MeshTemplates.MuteCity.CreateRoadSides(),
                GfzAssetTemplates.MeshTemplates.MuteCity.CreateLaneDividers(),
                GfzAssetTemplates.MeshTemplates.MuteCity.CreateRail(),
            };

            // Create bounding sphere for mesh
            var allTristrips = new List<Tristrip>();
            foreach (var tristrips in tristripsCollections)
                allTristrips.AddRange(tristrips);
            var globalBoundingSphere = TristripGenerator.CreateBoundingSphereFromTristrips(allTristrips);

            // Create GCMF. First, combine templates. Then, assign display lists. Finally, assign bounding sphere + origins.
            var gcmf = MeshTemplate.CombineTemplates(templates);
            AssignDisplayListsToGcmf(gcmf, tristripsCollections);
            gcmf.BoundingSphere = globalBoundingSphere;
            foreach (var submesh in gcmf.Submeshes)
                submesh.UnkAlphaOptions.Origin = globalBoundingSphere.origin;

            return gcmf;
        }

        public static void AssignDisplayListsToGcmf(Gcmf gcmf, Tristrip[][] tristrips)
        {
            if (tristrips.Length != gcmf.Submeshes.Length)
                throw new System.ArgumentException("lengths do not match!");

            for (int i = 0; i < gcmf.Submeshes.Length; i++)
            {
                if (tristrips[i].Length == 0)
                    continue;

                var frontfacing = new List<Tristrip>();
                var backfacing = new List<Tristrip>();
                foreach (var tristrip in tristrips[i])
                {
                    if (tristrip.isBackFacing)
                        backfacing.Add(tristrip);
                    else
                        frontfacing.Add(tristrip);
                }
                var submesh = gcmf.Submeshes[i];
                submesh.PrimaryFrontFacing = TristripGenerator.TristripsToDisplayLists(frontfacing.ToArray(), GameCube.GFZ.GfzGX.VAT);
                submesh.PrimaryBackFacing = TristripGenerator.TristripsToDisplayLists(backfacing.ToArray(), GameCube.GFZ.GfzGX.VAT);
                submesh.VertexAttributes = TristripGenerator.TristripToAttribute(tristrips[i]);
            }
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
