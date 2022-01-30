using GameCube.GFZ.CourseCollision;
using Manifold.EditorTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTrackRoad : GfzTrackSegment
    {
        [Header("Road Properties")]
        [Min(0f)]
        [SerializeField]
        private float railHeightLeft = 5f;

        [Min(0f)]
        [SerializeField]
        private float railHeightRight = 5f;

        //private readonly Vector3 gizmosScale = Vector3.one;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var mesh = Resources.GetBuiltinResource<Mesh>(EditorTools.Const.Resources.Cube);

            var increment = 1f / 100f;
            for (float p = 0; p < 1f; p += increment)
            {
                var pos = animTransform.Position.EvaluateNormalized(p);
                var rot = animTransform.Rotation.EvaluateNormalized(p);
                var scl = animTransform.Scale.EvaluateNormalized(p);

                Gizmos.DrawMesh(mesh, 0, pos, Quaternion.Euler(rot), scl);
            }
        }

        public override Mesh[] GenerateMeshes()
        {
            throw new System.NotImplementedException();
        }

        public override TrackSegment GenerateTrackSegment()
        {
            // TODO: get components, build any children

            var trackSegment = new TrackSegment();

            trackSegment.localPosition = transform.localPosition;
            trackSegment.localRotation = transform.localRotation.eulerAngles;
            trackSegment.localScale = transform.localScale;

            // Rail height
            trackSegment.railHeightLeft = railHeightLeft;
            trackSegment.railHeightRight = railHeightRight;
            // Falgs for rail height
            if (railHeightLeft > 0f)
                trackSegment.perimeterFlags |= TrackPerimeterFlags.hasRailHeightLeft;
            if (railHeightRight > 0f)
                trackSegment.perimeterFlags |= TrackPerimeterFlags.hasRailHeightRight;

            // TODO: currently hardcoded
            trackSegment.segmentType = TrackSegmentType.IsTransformLeaf;

            //
            trackSegment.unk_0x3B = unk0x3B;

            // Get animation data
            trackSegment.trackCurves = animTransform.ToTrackCurves();

            // debugging in/out
            var before = animTransform.Position.x;
            var togfz = trackSegment.trackCurves.PositionX;
            var after = AnimationCurveConverter.ToUnity(togfz);

            return trackSegment;
        }

    }
}
