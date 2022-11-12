using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapeRoadModulated : GfzShape
    {
        [Header("Modulated Road")]
        [SerializeField] private RoadMeshStyle roadStyle;
        [SerializeField] private EndcapMode endcapModeIn = EndcapMode.Automatic;
        [SerializeField] private EndcapMode endcapModeOut = EndcapMode.Automatic;
        [SerializeField, Min(1f)] private float lengthDistance = 10f;
        [SerializeField, Min(1)] private int subdivisionsTop = 32;
        [SerializeField, Min(1)] private int subdivisionsBottom = 16;
        [SerializeField] private UnityEngine.AnimationCurve widthCurve = new UnityEngine.AnimationCurve(new(0, 0), new(1, 0));
        [SerializeField] private UnityEngine.AnimationCurve lengthCurve = new UnityEngine.AnimationCurve(new(0, 0), new(1, 0));
        [Header("Rails")]
        [SerializeField] private bool hasRailLeft = true;
        [SerializeField] private bool hasRailRight = true;
        [SerializeField] private bool overrideRailHeights = false;
        [SerializeField, Min(0f)] private float overrideRailLeftHeight = 3f;
        [SerializeField, Min(0f)] private float overrideRailRightHeight = 3f;


        public int SubdivisionsTop => subdivisionsTop;
        public int SubdivisionsBottom => subdivisionsBottom;

        public UnityEngine.AnimationCurve WidthCurve => widthCurve;
        public UnityEngine.AnimationCurve LengthCurve => lengthCurve;



        public override ShapeID ShapeIdentifier => ShapeID.road;
        public override EndcapMode EndcapModeIn => endcapModeIn;
        public override EndcapMode EndcapModeOut => endcapModeOut;

        public bool HasRailLeft => hasRailLeft;
        public bool HasRailRight => hasRailRight;
        public bool OverrideRailHeights => overrideRailHeights;
        public float OverrideRailLeftHeight => overrideRailLeftHeight;
        public float OverrideRailRightHeight => overrideRailRightHeight;

        //public float RailHeightLeft => throw new System.NotImplementedException();
        //public float RailHeightRight => throw new System.NotImplementedException();


        public override AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            return new AnimationCurveTRS();
        }

        public override GcmfTemplate[] GetGcmfTemplates()
        {
            switch (roadStyle)
            {
                case RoadMeshStyle.MuteCity:
                case RoadMeshStyle.MuteCityCom:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.MuteCityCOM.RoadTopEmbeddedDividers(),
                        GcmfTemplates.MuteCity.RoadSides(),
                        GcmfTemplates.MuteCity.RoadEmbelishments(),
                        GcmfTemplates.MuteCity.RoadBottom(),
                        GcmfTemplates.MuteCity.RoadSides(),
                        GcmfTemplates.MuteCity.RoadRails(),
                    };

                default:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided(),
                    };
            }
        }

        public override Tristrip[][] GetTristrips(bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance);
            //matrices = TristripGenerator.StripHeight(matrices);
            var maxTime = GetRoot().GetMaxTime();

            switch (roadStyle)
            {
                case RoadMeshStyle.MuteCity:
                case RoadMeshStyle.MuteCityCom:
                    return TristripTemplates.RoadModulated.MuteCityCOM.Shape(matrices, this, maxTime, isGfzCoordinateSpace);

                default:
                    return TristripTemplates.RoadModulated.MuteCityCOM.Shape(matrices, this, maxTime, isGfzCoordinateSpace);
            }
        }

        public override TrackSegment CreateTrackSegment()
        {
            var maxTime = GetMaxTime();
            var widthKeys = widthCurve.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var gfzPositionYCurve = new UnityEngine.AnimationCurve(widthKeys);
            var gfzPositionXCurve = new UnityEngine.AnimationCurve(new(0, -0.5f), new(GetMaxTime(), 0.5f));


            // has p.x -0.5 to 0.5, has p.y width modulation
            var modulationWidth = new TrackSegment();
            modulationWidth.OrderIndentifier = name + "_mod_width";
            modulationWidth.SegmentType = TrackSegmentType.IsEmbed;
            modulationWidth.EmbeddedPropertyType = TrackEmbeddedPropertyType.IsModulated;
            modulationWidth.BranchIndex = GetBranchIndex();
            // Set up TRS
            var trsChild = new AnimationCurveTRS();
            trsChild.Position.x = gfzPositionXCurve;
            trsChild.Position.x.SetGfzTangentMode(UnityEditor.AnimationUtility.TangentMode.Linear);
            trsChild.Position.y = gfzPositionYCurve;
            modulationWidth.AnimationCurveTRS = trsChild.ToTrackSegment();
            SetRails(modulationWidth, roadStyle);
            
            // has s.y length modulation
            var modulationLength = new TrackSegment();
            modulationLength.OrderIndentifier = name + "_mod_length";
            modulationLength.SegmentType = TrackSegmentType.IsMatrix;
            modulationLength.BranchIndex = GetBranchIndex();
            modulationLength.Children = new TrackSegment[] { modulationWidth };
            var trsParent = new AnimationCurveTRS();
            trsParent.Scale.y = lengthCurve;
            modulationLength.AnimationCurveTRS = trsParent.ToTrackSegment();

            return modulationLength;
        }

        public override void UpdateTRS()
        {
            // do nothing :)
            var maxTime = GetMaxTime();
            var lengthKeys = lengthCurve.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            //var widthKeys = widthCurve.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            lengthCurve = new UnityEngine.AnimationCurve(lengthKeys);
            //widthCurve = new UnityEngine.AnimationCurve(widthKeys);
        }

        private float GetStyleRailHeight(RoadMeshStyle style)
        {
            switch (style)
            {
                case RoadMeshStyle.MuteCity:
                case RoadMeshStyle.MuteCityCom:
                    return 6;
                case RoadMeshStyle.OuterSpace:
                    return 10;
                default:
                    return 3;
            }
        }

        private void SetRails(TrackSegment trackSegment, RoadMeshStyle style)
        {
            float defaultRailHeight = GetStyleRailHeight(style);
            var railHeightLeft = hasRailLeft ? (OverrideRailHeights ? OverrideRailLeftHeight : defaultRailHeight) : 0;
            var railHeightRight = hasRailRight ? (OverrideRailHeights ? OverrideRailRightHeight : defaultRailHeight) : 0;
            trackSegment.SetRails(railHeightLeft, railHeightRight);
        }

    }
}
