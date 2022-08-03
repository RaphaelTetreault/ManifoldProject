using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{


    public class GfzPropertyEmbed : GfzSegmentShape
    {
        [SerializeField] private SurfaceEmbedType type = SurfaceEmbedType.Recover;
        [SerializeField, Min(1)] private int widthDivisions = 1;
        [SerializeField, Min(1f)] private float lengthDistance = 10f;
        [SerializeField, Range(0f, 1f)] private float from = 0f;
        [SerializeField, Range(0f, 1f)] private float to = 1f;
        [SerializeField] private UnityEngine.AnimationCurve widthCurve = new UnityEngine.AnimationCurve(new(0, 0.5f), new(1, 0.5f));
        [SerializeField] private UnityEngine.AnimationCurve offsetCurve = new UnityEngine.AnimationCurve(new(0, 0), new(1, 0));
        [SerializeField] private AnimationCurveTRS animationCurveTRS = new();
        //[SerializeField] private float range;

        public SurfaceEmbedType Type { get => type; }
        public int WidthDivisions { get => widthDivisions; }
        public float LengthDistance { get => lengthDistance; }
        public float From { get => from; }
        public float To { get => to; }
        public UnityEngine.AnimationCurve WidthCurve { get => widthCurve; }
        public UnityEngine.AnimationCurve OffsetCurve { get => offsetCurve; }
        public AnimationCurveTRS AnimationCurveTRS { get => animationCurveTRS; }

        public float GetRangeLength()
        {
            float length = GetRoot().GetSegmentLength();
            float range = to - from;
            float total = length * range;
            return total;
        }


        public enum SurfaceEmbedType : byte
        {
            Recover,
            Damage,
            Slip,
            Dirt,
        }

        public enum Jusification : byte
        {
            Center,
            Left,
            Right,
        }

        public Color32 GetColor() => GetColor(type);
        public static Color32 GetColor(SurfaceEmbedType type)
        {
            switch (type)
            {
                case SurfaceEmbedType.Recover: return new Color32(240, 25, 55, 255); // hot-pink (sampled from GFZ)
                case SurfaceEmbedType.Damage: return new Color32(255, 95, 0, 255); // orange
                case SurfaceEmbedType.Slip: return new Color32(109, 170, 210, 255); // blue (approximate, sampled from GFZ)
                case SurfaceEmbedType.Dirt: return new Color32(78, 42, 12, 255); // brown (1.5x brightness compared to GFZ)
                //case SurfaceEmbedType.Dirt: return new Color32(52, 28, 8, 255); // brown (sampled from GFZ)
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

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            // NOTE to document:
            // Looks like GFZ uses the max time range on this node, so if other "blank"
            // animation curves have earlier or later times, it will use those instead.

            var maxTime = GetMaxTime();
            var trs = new AnimationCurveTRS();

            var posx = new UnityEngine.AnimationCurve(offsetCurve.keys);
            var sclx = new UnityEngine.AnimationCurve(widthCurve.keys);
            var keysPX = posx.GetRenormalizedKeyRangeAndTangents(from * maxTime, to * maxTime);
            var keysSX = sclx.GetRenormalizedKeyRangeAndTangents(from * maxTime, to * maxTime);

            trs.Position.x = new UnityEngine.AnimationCurve(keysPX);
            trs.Scale.x = new UnityEngine.AnimationCurve(keysSX);

            if (isGfzCoordinateSpace)
                trs = trs.CreateGfzCoordinateSpace();

            return trs;
        }

        //public override Gcmf CreateGcmf(out GcmfTemplate[] gcmfTemplates, ref Dictionary<string, ushort> textureHashesToIndex)
        //{
        //    // Get path matrices
        //    var animKeys = animationCurveTRS.Position.x.keys;
        //    var min = animKeys[0].time;
        //    var max = animKeys[animKeys.Length - 1].time;
        //    var matrices = TristripGenerator.CreatePathMatrices(this, true, lengthDistance, min, max);

        //    // NOTE: Always do alpha last
        //    var tristripsCollections = new Tristrip[][]
        //    {
        //        TristripTemplates.Road.CreateDebugEmbed(matrices, this, widthDivisions, 0),
        //    };
        //    gcmfTemplates = new GcmfTemplate[]
        //    {
        //        GfzAssetTemplates.MeshTemplates.DebugTemplates.CreateUnlitVertexColored(),
        //    };

        //    var gcmf = GcmfTemplate.CreateGcmf(gcmfTemplates, tristripsCollections, ref textureHashesToIndex);
        //    return gcmf;
        //}


        public override Gcmf CreateGcmf(out GcmfTemplate[] gcmfTemplates, ref Dictionary<string, ushort> textureHashesToIndex)
        {
            var tristripsCollections = GetTristrips(Type, true);
            gcmfTemplates = GetGcmfTemplates(Type);
            var gcmf = GcmfTemplate.CreateGcmf(gcmfTemplates, tristripsCollections, ref textureHashesToIndex);
            return gcmf;
        }

        public GcmfTemplate[] GetGcmfTemplates(SurfaceEmbedType embedType)
        {
            // NOTE: Always do alpha last
            switch (embedType)
            {
                case SurfaceEmbedType.Slip:
                    return new GcmfTemplate[]
                    {
                        GfzAssetTemplates.MeshTemplates.General.CreateSlipGX(),
                    };
                case SurfaceEmbedType.Dirt:
                    return new GcmfTemplate[]
                    {
                        GfzAssetTemplates.MeshTemplates.General.CreateDirtNoise(),
                        GfzAssetTemplates.MeshTemplates.General.CreateDirtBaseAlpha(),
                    };
                case SurfaceEmbedType.Damage:
                    return new GcmfTemplate[]
                    {
                        GfzAssetTemplates.MeshTemplates.General.CreateLavaCrag(),
                        GfzAssetTemplates.MeshTemplates.General.CreateLavaAlpha(),
                    };

                default:
                    return new GcmfTemplate[]
                    {
                        GfzAssetTemplates.MeshTemplates.DebugTemplates.CreateLitVertexColored(),
                    };
            }
        }

        public Tristrip[][] GetTristrips(SurfaceEmbedType embedType, bool isGfzCoordinateSpace)
        {
            // Get path matrices
            var animKeys = animationCurveTRS.Scale.x.keys;
            var min = animKeys[0].time;
            var max = animKeys[animKeys.Length - 1].time;
            var matrices = TristripGenerator.CreatePathMatrices(this, true, lengthDistance, min, max);
            //
            var parent = GetParent();
            var parentMatrices = TristripGenerator.CreatePathMatrices(parent, true, lengthDistance, min, max);

            switch (embedType)
            {
                //case SurfaceEmbedType.Recover:
                //case SurfaceEmbedType.Damage:
                case SurfaceEmbedType.Slip:
                    return new Tristrip[][]
                    {
                        TristripTemplates.General.CreateEmbed(matrices, parentMatrices, this, false),
                    };
                case SurfaceEmbedType.Dirt:
                    return new Tristrip[][]
                    {
                        TristripTemplates.General.CreateEmbed(matrices, parentMatrices, this, false),
                        TristripTemplates.General.CreateEmbed(matrices, parentMatrices, this, true, 1),
                    };
                case SurfaceEmbedType.Damage:
                    return new Tristrip[][]
                    {
                        TristripTemplates.General.CreateEmbed(matrices, parentMatrices, this, false),
                        TristripTemplates.General.CreateEmbed(matrices, parentMatrices, this, true),
                    };
                default:
                    return new Tristrip[][]
                    {
                        TristripTemplates.Road.CreateDebugEmbed(matrices, this, widthDivisions, lengthDistance),
                    };
            }
        }


        public override Mesh CreateMesh()
        {
            var hacTRS = CreateHierarchichalAnimationCurveTRS(false);
            var maxTime = hacTRS.GetRootMaxTime();
            var min = from * maxTime;
            var max = to * maxTime;
            //Debug.Log($"MeshUnity -- Min: {min}, Max: {max}, MaxTime: {maxTime}");
            var matrices = TristripGenerator.GenerateMatrixIntervals(hacTRS, lengthDistance, min, max);

            //
            var endpointA = new Vector3(-0.5f, 0.33f, 0f);
            var endpointB = new Vector3(+0.5f, 0.33f, 0f);
            var color0 = GetColor(type);
            var tristrips = TristripGenerator.CreateTristrips(matrices, endpointA, endpointB, widthDivisions, color0, Vector3.up, 0, true);
            var mesh = TristripsToMesh(tristrips);
            //var tristrips = GetTristrips(Type, false);
            //var mesh = TristripsToMesh(Tristrip.Linearize(tristrips));
            mesh.name = $"Auto Gen - {this.name}";

            return mesh;
        }

        public override TrackSegment CreateTrackSegment()
        {
            var children = CreateChildTrackSegments();
            var trs = CreateAnimationCurveTRS(false);

            var trackSegment = new TrackSegment();
            trackSegment.SegmentType = TrackSegmentType.IsEmbed;
            trackSegment.EmbeddedPropertyType = GetEmbedProperty(type);
            trackSegment.AnimationCurveTRS = trs.ToTrackSegment();
            trackSegment.BranchIndex = 0; // these kinds of embeds do not specify branch
            trackSegment.Children = children;

            return trackSegment;

        }

        public override void UpdateTRS()
        {
            animationCurveTRS = CreateAnimationCurveTRS(false);
        }

        public void SetOffsets(Jusification jusification)
        {
            float scaler = JustificationToScalar(jusification);
            // Anchor left/center/right
            float anchor = 0.5f * scaler;
            // Define how width is applied based on justification
            float offset = -scaler;

            var keys = widthCurve.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                float halfWidth = key.value * 0.5f;
                key.value = anchor + halfWidth * offset;
                key.inTangent *= offset;
                key.outTangent *= offset;
                keys[i] = key;
            }
            offsetCurve = new UnityEngine.AnimationCurve(keys);
            InvokeUpdates();
        }

        private float JustificationToScalar(Jusification jusification)
        {
            switch (jusification)
            {
                case Jusification.Center: return 0f;
                case Jusification.Left: return -1f;
                case Jusification.Right: return +1f;
                default: throw new System.NotImplementedException();
            }
        }
    }
}
