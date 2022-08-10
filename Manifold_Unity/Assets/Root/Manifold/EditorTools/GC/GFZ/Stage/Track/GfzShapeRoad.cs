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
    public class GfzShapeRoad : GfzSegmentShape,
        IRailSegment
    {
        [field: Header("Mesh Properties")]
        [field: SerializeField] public RoadMeshStyle MeshStyle { get; private set; }
        [field: SerializeField, Range(1, 32)] public int WidthDivisions { get; private set; } = 4;
        [field: SerializeField, Min(1f)] public float LengthDistance { get; private set; } = 10f;
        [field: SerializeField, Min(1f)] public float TexRepeatWidthTop { get; private set; } = 4f;
        [field: SerializeField, Min(1f)] public float TexRepeatWidthBottom { get; private set; } = 4f;

        [field: Header("Road Properties")]
        [field: SerializeField, Min(0f)] public float RailHeightLeft { get; private set; } = 3f;
        [field: SerializeField, Min(0f)] public float RailHeightRight { get; private set; } = 3f;
        [field: SerializeField] public bool HasLaneDividers { get; private set; } = true;



        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            return new AnimationCurveTRS();
        }

        public override Gcmf CreateGcmf(out GcmfTemplate[] gcmfTemplates, ref Dictionary<string, ushort> textureHashesToIndex)
        {
            var tristripsCollections = GetTristrips(MeshStyle, true);
            gcmfTemplates = GetGcmfTemplates(MeshStyle);
            var gcmf = GcmfTemplate.CreateGcmf(gcmfTemplates, tristripsCollections, ref textureHashesToIndex);
            return gcmf;
        }

        public GcmfTemplate[] GetGcmfTemplates(RoadMeshStyle roadMeshStyle)
        {
            switch (roadMeshStyle)
            {
                case RoadMeshStyle.MuteCity:
                    return GcmfTemplates.MuteCity.Road();

                case RoadMeshStyle.MuteCityCom:
                    if (HasLaneDividers)
                        return GcmfTemplates.MuteCityCOM.Road();
                    else
                        return GcmfTemplates.MuteCityCOM.RoadNoDividers();

                case RoadMeshStyle.OuterSpace:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.OuterSpace.Top(),
                        GcmfTemplates.OuterSpace.BottomAndSides(),
                        GcmfTemplates.OuterSpace.CurbAndLaneDividerTop(),
                        GcmfTemplates.OuterSpace.CurbAndLaneDividerSlope(),
                        GcmfTemplates.OuterSpace.RailsAngle(),
                        GcmfTemplates.OuterSpace.RailsLights(),
                        GcmfTemplates.OuterSpace.EndCap(),
                    };

                default:
                    return new GcmfTemplate[] { GcmfTemplates.Debug.CreateLitVertexColored() };
            }
        }

        public Tristrip[][] GetTristrips(RoadMeshStyle meshStyle, bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, LengthDistance);
            var maxTime = GetRoot().GetMaxTime();

            switch (meshStyle)
            {
                case RoadMeshStyle.MuteCity:
                    return new Tristrip[][]
                    {
                        TristripTemplates.Road.MuteCity.CreateRoadTop(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRoadBottom(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRoadTrim(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRoadEmbellishments(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateLaneDividers(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRails(matrices, this),
                    };
                case RoadMeshStyle.MuteCityCom:
                    return new Tristrip[][]
                    {
                        TristripTemplates.Road.MuteCityCOM.Top(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRoadBottom(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRoadTrim(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRoadEmbellishments(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRails(matrices, this),
                    };
                case RoadMeshStyle.OuterSpace:
                    return new Tristrip[][]
                    {
                        TristripTemplates.Road.OuterSpace.Top(matrices, this, maxTime),
                        TristripTemplates.Road.OuterSpace.BottomAndSides(matrices, this, maxTime),
                        TristripTemplates.Road.OuterSpace.CurbAndLaneDividerFlat(matrices, this, maxTime),
                        TristripTemplates.Road.OuterSpace.CurbAndLaneDividerSlants(matrices, this, maxTime),
                        TristripTemplates.Road.OuterSpace.RailsAngle(matrices, this, maxTime),
                        TristripTemplates.Road.OuterSpace.RailsLights(matrices, this, maxTime),
                        TristripTemplates.Road.OuterSpace.EndCaps(matrices, this, maxTime),
                    };

                default:
                    return new Tristrip[][]
                    {
                        TristripTemplates.Road.CreateDebug(matrices, this, WidthDivisions, LengthDistance, isGfzCoordinateSpace),
                    };
            }
        }

        public Tristrip[] GetTristripsLinear(RoadMeshStyle meshStyle, bool isGfzCoordinateSpace)
        {
            var tristripsCollection = GetTristrips(meshStyle, isGfzCoordinateSpace);

            var allTristrips = new List<Tristrip>();
            foreach (var tristrips in tristripsCollection)
                allTristrips.AddRange(tristrips);

            return allTristrips.ToArray();
        }

        public override Mesh CreateMesh()
        {
            var tristrips = GetTristripsLinear(MeshStyle, false);

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
