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
        [field: SerializeField, Min(1)] public int WidthDivisions { get; private set; } = 2;
        [field: SerializeField, Min(1f)] public float LengthDistance { get; private set; } = 10f;

        [field: Header("Properties")]
        [field: SerializeField] public SurfaceEmbedType Type { get; private set; } = SurfaceEmbedType.Recover;
        [field: SerializeField, Range(0f, 1f)] public float From { get; private set; } = 0f;
        [field: SerializeField, Range(0f, 1f)] public float To { get; private set; } = 1f;
        [field: SerializeField] public UnityEngine.AnimationCurve Width { get; private set; } = new UnityEngine.AnimationCurve(new(0, 0.5f), new(1, 0.5f));
        [field: SerializeField] public UnityEngine.AnimationCurve Offset { get; private set; } = new UnityEngine.AnimationCurve(new(0, 0), new(1, 0));

        [field: Header("Debug")]
        [field: SerializeField] public AnimationCurveTRS trs { get; private set; } = new();


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

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            // NOTE to document:
            // Looks like GFZ uses the max time range on this node, so if other "blank"
            // animation curves have earlier or later times, it will use those instead.

            var maxTime = GetMaxTime();
            var trs = new AnimationCurveTRS();

            var posx = new UnityEngine.AnimationCurve(Offset.keys);
            var sclx = new UnityEngine.AnimationCurve(Width.keys);
            var keysPX = posx.GetRenormalizedKeyRangeAndTangents(From * maxTime, To * maxTime);
            var keysSX = sclx.GetRenormalizedKeyRangeAndTangents(From * maxTime, To * maxTime);

            trs.Position.x = new UnityEngine.AnimationCurve(keysPX);
            trs.Scale.x = new UnityEngine.AnimationCurve(keysSX);

            // Don't need this right? We flip p.z and r.y
            //if (isGfzCoordinateSpace)
            //    trs = trs.CreateGfzCoordinateSpace();

            this.trs = trs;

            return trs;
        }

        public override Gcmf CreateGcmf()
        {
            // Make the vertex data
            var color0 = GetColor(Type);
            var trackMeshTristrips = TristripGenerator.CreateTempTrackRoadEmbed(this, WidthDivisions, LengthDistance, color0, true);
            // convert to GameCube format
            var dlists = TristripGenerator.TristripsToDisplayLists(trackMeshTristrips, GameCube.GFZ.GfzGX.VAT);

            // Compute bounding sphere
            var allVertices = new List<Vector3>();
            foreach (var tristrip in trackMeshTristrips)
                allVertices.AddRange(tristrip.positions);
            var boundingSphere = TristripGenerator.CreateBoundingSphereFromPoints(allVertices);

            // Note: this template is both sides, we do not YET need to sort front/back facing tristrips.
            var template = GfzAssetTemplates.MeshTemplates.DebugTemplates.CreateLitVertexColored();
            var gcmf = template.Gcmf;
            gcmf.Submeshes[0].RenderFlags |= RenderFlags.unlit;
            gcmf.BoundingSphere = boundingSphere;
            gcmf.Submeshes[0].PrimaryDisplayListsTranslucid = dlists;
            gcmf.Submeshes[0].VertexAttributes = dlists[0].Attributes; // hacky
            gcmf.Submeshes[0].UnkAlphaOptions.Origin = boundingSphere.origin;
            gcmf.PatchTevLayerIndexes();

            return gcmf;
        }

        public override Mesh CreateMesh()
        {
            var hacTRS = CreateHierarchichalAnimationCurveTRS(false);
            var maxTime = hacTRS.GetRootMaxTime();
            var min = From * maxTime;
            var max = To * maxTime;
            Debug.Log($"MeshUnity -- Min: {min}, Max: {max}, MaxTime: {maxTime}");
            var matrices = TristripGenerator.GenerateMatrixIntervals(hacTRS, LengthDistance, min, max);

            //
            var endpointA = new Vector3(-0.5f, 0.33f, 0f);
            var endpointB = new Vector3(+0.5f, 0.33f, 0f);
            var color0 = GetColor(Type);
            var tristrips = TristripGenerator.CreateTristrips(matrices, endpointA, endpointB, WidthDivisions, color0, Vector3.up, 0, true);
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
            trackSegment.FallbackPosition = transform.localPosition;
            trackSegment.FallbackRotation = transform.localRotation.eulerAngles;
            trackSegment.FallbackScale = transform.localScale;
            trackSegment.AnimationCurveTRS = trs.ToTrackSegment();
            trackSegment.BranchIndex = 0; // these kinds of embeds do not specify branch
            trackSegment.Children = children;

            return trackSegment;

        }

    }
}
