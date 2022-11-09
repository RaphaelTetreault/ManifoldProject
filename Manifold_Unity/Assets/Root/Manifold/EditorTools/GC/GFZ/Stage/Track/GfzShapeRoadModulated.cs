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
        [SerializeField] private UnityEngine.AnimationCurve widthCurve = new UnityEngine.AnimationCurve(new(0,0), new(1,0));
        [SerializeField] private UnityEngine.AnimationCurve lengthCurve = new UnityEngine.AnimationCurve(new(0,0), new(1,0));


        public int SubdivisionsTop => subdivisionsTop;
        public int SubdivisionsBottom => subdivisionsBottom;

        public UnityEngine.AnimationCurve WidthCurve => widthCurve;
        public UnityEngine.AnimationCurve LengthCurve => lengthCurve;

        public override ShapeID ShapeIdentifier => ShapeID.road;
        public override EndcapMode EndcapModeIn => endcapModeIn;
        public override EndcapMode EndcapModeOut => endcapModeOut;


        public override AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            return new AnimationCurveTRS();
        }

        public override GcmfTemplate[] GetGcmfTemplates()
        {
            switch (roadStyle)
            {
                case RoadMeshStyle.MuteCity:
                    return new GcmfTemplate[]
                    {
                        GcmfTemplates.MuteCityCOM.RoadTopEmbeddedDividers(),
                        GcmfTemplates.MuteCity.RoadSides(),
                        GcmfTemplates.MuteCity.RoadEmbelishments(),
                        GcmfTemplates.MuteCity.RoadBottom(),
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
            var maxTime = GetRoot().GetMaxTime();

            switch (roadStyle)
            {
                default:
                    //return new Tristrip[][]
                    //{
                    return TristripTemplates.RoadModulated.MuteCityCOM(matrices, this, maxTime, isGfzCoordinateSpace);
                    //};
            }
        }

        public override TrackSegment CreateTrackSegment()
        {
            // has p.x -0.5 to 0.5, has p.y width modulation
            var modulationWidth = new TrackSegment();
            modulationWidth.OrderIndentifier = name + "_mod_width";
            modulationWidth.SegmentType = TrackSegmentType.IsEmbed;
            modulationWidth.EmbeddedPropertyType = TrackEmbeddedPropertyType.IsModulated;
            modulationWidth.BranchIndex = GetBranchIndex();
            //modulationWidth.Children = CreateChildTrackSegments();
            var trs2 = new AnimationCurveTRS();
            trs2.Position.x = new UnityEngine.AnimationCurve(new(0, -0.5f), new(GetMaxTime(), 0.5f));
            trs2.Position.x.SetGfzTangentMode(UnityEditor.AnimationUtility.TangentMode.Linear);
            trs2.Position.y = widthCurve;
            modulationWidth.AnimationCurveTRS = trs2.ToTrackSegment();
            modulationWidth.SetRails(3, 3);

            // has s.y length modulation
            var modulationLength = new TrackSegment();
            modulationLength.OrderIndentifier = name + "_mod_length";
            modulationLength.SegmentType = TrackSegmentType.IsMatrix;
            modulationLength.BranchIndex = GetBranchIndex();
            modulationLength.Children = new TrackSegment[] { modulationWidth };
            var trs1 = new AnimationCurveTRS();
            trs1.Scale.y = lengthCurve;
            modulationLength.AnimationCurveTRS = trs1.ToTrackSegment();

            return modulationLength;
        }

        public override void UpdateTRS()
        {
            // do nothing :)
            var maxTime = GetMaxTime();
            var lengthKeys = lengthCurve.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var widthKeys = widthCurve.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            lengthCurve = new UnityEngine.AnimationCurve(lengthKeys);
            widthCurve = new UnityEngine.AnimationCurve(widthKeys);
        }

    }
}
