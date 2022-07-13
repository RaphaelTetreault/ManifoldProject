using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public enum SurfaceEmbedType : byte
    {
        Recover,
        Damage,
        Slip,
        Dirt,
    }


    public class GfzTrackSurfaceEmbed : GfzTrackSegmentShapeNode
    {
        [field: Header("Properties")]
        [field: SerializeField] public SurfaceEmbedType Type { get; private set; } = SurfaceEmbedType.Recover;
        [field: SerializeField, Range(0f, 1f)] public float From { get; private set; } = 0f;
        [field: SerializeField, Range(0f, 1f)] public float To { get; private set; } = 1f;
        [field: SerializeField] public UnityEngine.AnimationCurve Width { get; private set; } = new UnityEngine.AnimationCurve(new(0, 0.5f), new(1, 0.5f));
        [field: SerializeField] public UnityEngine.AnimationCurve Offset { get; private set; } = new UnityEngine.AnimationCurve(new(0, 0), new(1, 0));


        public Color32 GetColor(SurfaceEmbedType type)
        {
            switch (type)
            {
                case SurfaceEmbedType.Recover: return new Color32(240, 25, 55, 255); // hot-pink (sampled from GFZ)
                case SurfaceEmbedType.Damage: return new Color32(255, 0, 0, 255); // red
                case SurfaceEmbedType.Slip: return new Color32(109, 170, 210, 255); // blue (approximate, sampled from GFZ)
                case SurfaceEmbedType.Dirt: return new Color32(52, 28, 8, 255); // brown (sampled from GFZ)
                default:
                    throw new System.ArgumentException();
            }
        }

        public TrackEmbeddedPropertyType GetEmbedProperty(SurfaceEmbedType type)
        {
            switch (type)
            {
                case SurfaceEmbedType.Recover: return TrackEmbeddedPropertyType.IsRecover;
                case SurfaceEmbedType.Damage: return TrackEmbeddedPropertyType.IsDamage;
                case SurfaceEmbedType.Slip: return TrackEmbeddedPropertyType.IsSlip;
                case SurfaceEmbedType.Dirt: return TrackEmbeddedPropertyType.IsDirt;
                default:
                    throw new System.ArgumentException();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
        }

        public override TrackSegmentType TrackSegmentType => throw new System.NotImplementedException();

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            var maxTime = GetMaxTime();
            var trs = AnimationCurveTRS.CreateDefault(maxTime);

            var posx = new UnityEngine.AnimationCurve(Offset.keys);
            var sclx = new UnityEngine.AnimationCurve(Width.keys);
            RenormalizeKeyTimes(posx, maxTime);
            RenormalizeKeyTimes(sclx, maxTime);

            trs.Position.x = posx;
            trs.Scale.x = sclx;

            if (isGfzCoordinateSpace)
                trs = trs.CreateGfzCoordinateSpace();

            return trs;
        }

        public override Gcmf CreateGcmf()
        {
            throw new System.NotImplementedException();
        }

        public override Mesh CreateMesh()
        {
            var hacTRS = CreateHierarchichalAnimationCurveTRS(false);
            var matrices = TrackGeoGenerator.GenerateMatrixIntervals(hacTRS, 10f);

            //
            var endpointA = new Vector3(-0.5f, 0.20f, 0f);
            var endpointB = new Vector3(+0.5f, 0.20f, 0f);
            var color0 = GetColor(Type);
            var tristrips = TrackGeoGenerator.CreateTristrips(matrices, endpointA, endpointB, 4, color0, Vector3.up, 0, true);
            var mesh = TristripsToMesh(tristrips);
            mesh.name = $"Auto Gen - {this.name}";

            return mesh;
        }

        public override TrackSegment CreateTrackSegment()
        {
            var children = CreateChildTrackSegments();
            var trs = CreateAnimationCurveTRS(false);

            var trackSegment = new TrackSegment();
            trackSegment.SegmentType = TrackSegmentType.IsEmbed;
            trackSegment.EmbeddedPropertyType = GetEmbedProperty(Type);
            trackSegment.LocalPosition = transform.localPosition;
            trackSegment.LocalRotation = transform.localRotation.eulerAngles;
            trackSegment.LocalScale = transform.localScale;
            trackSegment.AnimationCurveTRS = trs.ToTrackSegment();
            trackSegment.BranchIndex = 0; // these kinds of embeds do not specify branch
            trackSegment.Children = children;

            return trackSegment;

        }

        private void RenormalizeKeyTimes(UnityEngine.AnimationCurve curve, float maxTime)
        {
            var keys = curve.keys;
            for (int i = 0; i < curve.length; i++)
            {
                var key = keys[i];
                var mappedTime = Map(key.time, From, To);
                var time = mappedTime * maxTime;
                keys[i].time = time;
            }
            curve.keys = keys;
        }

        private float Map(float time, float min, float max)
        {
            float delta = max - min;
            float rangedTime = time * delta;
            float trueTime = min + rangedTime;
            return trueTime;
        }
    }
}
