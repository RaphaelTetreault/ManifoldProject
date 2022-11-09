using GameCube.GX;
using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapeRoad : GfzShape,
        IRailSegment
    {
        [Header("Mesh Properties")]
        [SerializeField, FormerlySerializedAs("<MeshStyle>k__BackingField")] private RoadMeshStyle meshStyle;
        [SerializeField] public EndcapMode endcapModeIn = EndcapMode.Automatic;
        [SerializeField] public EndcapMode endcapModeOut = EndcapMode.Automatic;
        [SerializeField, FormerlySerializedAs("<WidthDivisions>k__BackingField"), Range(1, 32)] private int widthDivisions = 4;
        [SerializeField, FormerlySerializedAs("<LengthDistance>k__BackingField"), Min(1f)] public float lengthDistance  = 10f;
        // TODO: DEPRECATE
        [SerializeField, FormerlySerializedAs("<TexRepeatWidthTop>k__BackingField"), Min(1f), HideInInspector] public float TexRepeatWidthTop  = 4f;
        [SerializeField, FormerlySerializedAs("<TexRepeatWidthBottom>k__BackingField"), Min(1f), HideInInspector] public float TexRepeatWidthBottom  = 4f;

        [Header("Road Properties")]
        [SerializeField, FormerlySerializedAs("<RailHeightLeft>k__BackingField"), Min(0f)] public float railHeightLeft  = 3f;
        [SerializeField, FormerlySerializedAs("<RailHeightRight>k__BackingField"), Min(0f)] public float railHeightRight = 3f;
        [SerializeField, FormerlySerializedAs("<HasLaneDividers>k__BackingField")] public bool hasLaneDividers = true;

        public override ShapeID ShapeIdentifier => ShapeID.road;
        public override EndcapMode EndcapModeIn => endcapModeIn;
        public override EndcapMode EndcapModeOut => endcapModeOut;
        public int WidthDivisions => widthDivisions;
        public float LengthDistance => lengthDistance;
        public float RailHeightLeft => railHeightLeft;
        public float RailHeightRight => railHeightRight;
        public bool HasLaneDividers => hasLaneDividers;


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

        public override GcmfTemplate[] GetGcmfTemplates()
        {
            switch (meshStyle)
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
                    return new GcmfTemplate[] { GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided() };
            }
        }

        public override Tristrip[][] GetTristrips(bool isGfzCoordinateSpace)
        {
            var originalMatrice = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, LengthDistance);
            var matrices = TristripGenerator.StripHeight(originalMatrice);
            var maxTime = GetRoot().GetMaxTime();

            switch (meshStyle)
            {
                case RoadMeshStyle.MuteCity:
                    return new Tristrip[][]
                    {
                        TristripTemplates.Road.MuteCity.Top(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRoadBottom(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRoadTrim(matrices, this, maxTime, isGfzCoordinateSpace),
                        TristripTemplates.Road.MuteCity.CreateRoadEmbellishments(matrices, maxTime),
                        TristripTemplates.Road.MuteCity.CreateLaneDividers(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRails(matrices, this),
                    };
                case RoadMeshStyle.MuteCityCom:
                    return new Tristrip[][]
                    {
                        TristripTemplates.Road.MuteCityCOM.Top(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRoadBottom(matrices, this, maxTime),
                        TristripTemplates.Road.MuteCity.CreateRoadTrim(matrices, this, maxTime, isGfzCoordinateSpace),
                        TristripTemplates.Road.MuteCity.CreateRoadEmbellishments(matrices, maxTime),
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

        public override TrackSegment CreateTrackSegment()
        {
            var trackSegment = new TrackSegment();
            trackSegment.OrderIndentifier = name;
            trackSegment.SegmentType = TrackSegmentType.IsTrack;
            trackSegment.BranchIndex = GetBranchIndex();
            trackSegment.SetRails(RailHeightLeft, RailHeightRight);
            trackSegment.Children = CreateChildTrackSegments(); ;

            return trackSegment;
        }

        public override void UpdateTRS()
        {
            // do nothing :)
        }

    }
}
