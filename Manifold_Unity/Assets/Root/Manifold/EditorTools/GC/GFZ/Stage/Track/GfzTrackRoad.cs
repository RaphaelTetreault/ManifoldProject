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
        [field: SerializeField, Min(1)] public int WidthDivisions { get; private set; } = 4;
        [field: SerializeField, Min(1f)] public float LengthDistance { get; private set; } = 10f;

        [field: Header("Road Properties")]
        [field: SerializeField, Min(0f)] public float RailHeightLeft { get; private set; } = 3f;
        [field: SerializeField, Min(0f)] public float RailHeightRight { get; private set; } = 3f;

        public override TrackSegmentType TrackSegmentType => TrackSegmentType.IsTrack;

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            var maxTime = GetMaxTime();
            var trs = AnimationCurveTRS.CreateDefault(maxTime);
            return trs;
        }

        public override Gcmf CreateGcmf()
        {
            var tristrips = TrackGeoGenerator.CreateAllTemp(this, WidthDivisions, LengthDistance, true);
            var gcmf = new Gcmf();

            throw new System.NotImplementedException();
        }

        public override float GetMaxTime()
        {
            return GetRoot().GetMaxTime();
        }

        public override Mesh CreateMesh()
        {
            var tristrips = TrackGeoGenerator.CreateAllTemp(this, WidthDivisions, LengthDistance, false);
            Mesh = TristripsToMesh(tristrips);
            Mesh.name = $"Auto Gen - {this.name}";
            return Mesh;
        }

        public override TrackSegment CreateTrackSegment()
        {
            var trackSegment = new TrackSegment();
            trackSegment.LocalPosition = transform.localPosition;
            trackSegment.LocalRotation = transform.localRotation.eulerAngles;
            trackSegment.LocalScale = transform.localScale;
            trackSegment.BranchIndex = GetBranchIndex();
            trackSegment.SetRails(RailHeightLeft, RailHeightRight);

            return trackSegment;
        }

    }
}
