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
        [SerializeField] private SurfaceEmbedType type = SurfaceEmbedType.RecoverLight;
        [SerializeField, Min(1)] private int widthDivisions = 1;
        [SerializeField, Min(1f)] private float lengthDistance = 10f;
        [SerializeField, Range(0f, 1f)] private float from = 0f;
        [SerializeField, Range(0f, 1f)] private float to = 1f;
        [SerializeField] private UnityEngine.AnimationCurve widthCurve = new UnityEngine.AnimationCurve(new(0, 0.5f), new(1, 0.5f));
        [SerializeField] private UnityEngine.AnimationCurve offsetCurve = new UnityEngine.AnimationCurve(new(0, 0), new(1, 0));
        [SerializeField] private AnimationCurveTRS animationCurveTRS = new();
        //[SerializeField] private float range;
        [SerializeField] private bool includeTrimLeft = true;
        [SerializeField] private bool includeTrimRight = true;
        [SerializeField] private bool includeTrimStart = true;
        [SerializeField] private bool includeTrimEnd = true;
        [SerializeField] private Vector2 repeatFlashingUV = Vector2.one;
        [SerializeField] private Vector2 repeatFlashingUVOffset = Vector2.zero;


        public SurfaceEmbedType Type { get => type; }
        public int WidthDivisions { get => widthDivisions; }
        public float LengthDistance { get => lengthDistance; }
        public float From { get => from; }
        public float To { get => to; }
        public UnityEngine.AnimationCurve WidthCurve { get => widthCurve; }
        public UnityEngine.AnimationCurve OffsetCurve { get => offsetCurve; }
        public AnimationCurveTRS AnimationCurveTRS { get => animationCurveTRS; }
        public bool IncludeTrimLeft { get => includeTrimLeft; set => includeTrimLeft = value; }
        public bool IncludeTrimRight { get => includeTrimRight; set => includeTrimRight = value; }
        public bool IncludeTrimStart { get => includeTrimStart; set => includeTrimStart = value; }
        public bool IncludeTrimEnd { get => includeTrimEnd; set => includeTrimEnd = value; }
        public Vector2 RepeatFlashingUV { get => repeatFlashingUV; set => repeatFlashingUV = value; }
        public Vector2 RepeatFlashingUVOffset { get => repeatFlashingUVOffset; set => repeatFlashingUVOffset = value; }


        public float GetRangeLength()
        {
            float length = GetRoot().GetSegmentLength();
            float range = to - from;
            float total = length * range;
            return total;
        }


        public enum SurfaceEmbedType : byte
        {
            RecoverLight = 0,
            RecoverDark = 5,
            SlipGX = 2,
            SlipAX = 4,
            Dirt = 3,
            Lava = 1,
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
                case SurfaceEmbedType.RecoverLight:
                    return new Color32(240, 50, 110, 255); // hot-pink (sampled from GFZ)
                case SurfaceEmbedType.RecoverDark:
                    return new Color32(240, 25, 55, 255); // hot-pink (sampled from GFZ)
                case SurfaceEmbedType.Lava:
                    return new Color32(255, 95, 0, 255); // orange
                case SurfaceEmbedType.SlipGX:
                    return new Color32(109, 170, 210, 255); // GX blue (approximate, sampled from GFZ)
                case SurfaceEmbedType.SlipAX:
                    return new Color32(54, 99, 164, 255); // AX blue (approximate, sampled from GFZ)
                case SurfaceEmbedType.Dirt:
                    return new Color32(78, 42, 12, 255); // brown (1.5x brightness compared to GFZ)
                //case SurfaceEmbedType.Dirt: return new Color32(52, 28, 8, 255); // brown (sampled from GFZ)
                default:
                    throw new System.ArgumentException();
            }
        }

        public TrackEmbeddedPropertyType GetEmbedProperty(SurfaceEmbedType type)
        {
            switch (type)
            {
                case SurfaceEmbedType.RecoverLight:
                case SurfaceEmbedType.RecoverDark:
                    return TrackEmbeddedPropertyType.IsRecover;
                case SurfaceEmbedType.Lava:
                    return TrackEmbeddedPropertyType.IsDamage;
                case SurfaceEmbedType.SlipGX:
                case SurfaceEmbedType.SlipAX:
                    return TrackEmbeddedPropertyType.IsSlip;
                case SurfaceEmbedType.Dirt:
                    return TrackEmbeddedPropertyType.IsDirt;
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
                case SurfaceEmbedType.SlipGX:
                    return new GcmfTemplate[]
                    {
                        GfzAssetTemplates.MeshTemplates.General.CreateTrim(),
                        GfzAssetTemplates.MeshTemplates.General.CreateSlipGX(),
                    };
                case SurfaceEmbedType.SlipAX:
                    return new GcmfTemplate[]
                    {
                        GfzAssetTemplates.MeshTemplates.General.CreateTrim(),
                        GfzAssetTemplates.MeshTemplates.General.CreateSlipAX(),
                    };
                case SurfaceEmbedType.Dirt:
                    return new GcmfTemplate[]
                    {
                        GfzAssetTemplates.MeshTemplates.General.CreateTrim(),
                        GfzAssetTemplates.MeshTemplates.General.CreateDirtNoise(),
                        GfzAssetTemplates.MeshTemplates.General.CreateDirtAlpha(),
                    };
                case SurfaceEmbedType.Lava:
                    return new GcmfTemplate[]
                    {
                        GfzAssetTemplates.MeshTemplates.General.CreateTrim(),
                        GfzAssetTemplates.MeshTemplates.General.CreateLavaCrag(),
                        GfzAssetTemplates.MeshTemplates.General.CreateLavaAlpha(),
                    };
                case SurfaceEmbedType.RecoverLight:
                    return new GcmfTemplate[]
                    {
                        GfzAssetTemplates.MeshTemplates.General.CreateTrim(),
                        GfzAssetTemplates.MeshTemplates.General.CreateRecoverLightSubBase(),
                        GfzAssetTemplates.MeshTemplates.General.CreateRecoverLightBase(),
                        GfzAssetTemplates.MeshTemplates.General.CreateRecoverLightAlpha(),
                    };
                case SurfaceEmbedType.RecoverDark:
                    return new GcmfTemplate[]
                    {
                        GfzAssetTemplates.MeshTemplates.General.CreateTrim(),
                        GfzAssetTemplates.MeshTemplates.General.CreateRecoverDarkBase(),
                        GfzAssetTemplates.MeshTemplates.General.CreateRecoverDarkAlpha(),
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
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance, min, max);
            //
            var parent = GetParent();
            var parentMatrices = TristripGenerator.CreatePathMatrices(parent, isGfzCoordinateSpace, lengthDistance, min, max);

            switch (embedType)
            {
                case SurfaceEmbedType.SlipGX:
                    return new Tristrip[][]
                    {
                        TristripTemplates.General.CreateTrim(matrices, this),
                        TristripTemplates.General.CreateSlipGX(matrices, parentMatrices, this),
                    };
                case SurfaceEmbedType.SlipAX:
                    return new Tristrip[][]
                    {
                        TristripTemplates.General.CreateTrim(matrices, this),
                        TristripTemplates.General.CreateSlipAX(matrices, parentMatrices, this),
                    };
                case SurfaceEmbedType.Dirt:
                    return new Tristrip[][]
                    {
                        TristripTemplates.General.CreateTrim(matrices, this),
                        TristripTemplates.General.CreateDirtNoise(matrices, parentMatrices, this),
                        TristripTemplates.General.CreateDirtAlpha(matrices, this),
                    };
                case SurfaceEmbedType.Lava:
                    return new Tristrip[][]
                    {
                        TristripTemplates.General.CreateTrim(matrices, this),
                        TristripTemplates.General.CreateLavaCrag(matrices, parentMatrices, this),
                        TristripTemplates.General.CreateLavaAlpha(matrices, parentMatrices, this),
                    };
                case SurfaceEmbedType.RecoverLight:
                    return new Tristrip[][]
                    {
                        TristripTemplates.General.CreateTrim(matrices, this),
                        TristripTemplates.General.CreateRecoverBase(matrices, this),
                        TristripTemplates.General.CreateRecoverBase(matrices, this),
                        TristripTemplates.General.CreateRecoverAlpha(matrices, this),
                    };
                case SurfaceEmbedType.RecoverDark:
                    return new Tristrip[][]
                    {
                        TristripTemplates.General.CreateTrim(matrices, this),
                        TristripTemplates.General.CreateRecoverBase(matrices, this),
                        TristripTemplates.General.CreateRecoverAlpha(matrices, this),
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
