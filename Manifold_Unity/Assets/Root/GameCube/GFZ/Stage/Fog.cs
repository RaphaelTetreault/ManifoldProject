using Manifold;
using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Defines parameters to produce and render fog.
    /// </summary>
    /// <remarks>
    /// These are notes on the observed fog color values in all stage files for AX and GX.
    /// + Inline comments for Aeropolis and Green Plant. They have quirks.
    /// + GPI and GPMR differ from GPS. Soft dull blue vs soft dull green, respectively.
    ///
    /// Imprecision Differences
    /// Some color values are barely different between stages/venues. It appears that
    /// someone went and standardize this later on, but some differences still exist.
    /// 
    /// + Aeropolis - GX tracks AM ADS vs AX track ASD (track, not game)
    /// + Port Town - GX tracks PTAD PTLP vs AX track PTCW (track, not game)
    /// + Green Plant - between GX tracks.
    ///
    /// AX Color Differences
    /// I did not write template fields methods for AX _story_ stages since they are WIP values.
    ///
    /// + Big Blue Story. AX has default value of black. This changed in GX.
    /// + Casino Palace [Double Branches]. AX has non-black value #0a0a0a. In anim data, is #18303c (blue). GX uses black.
    /// + Phantom Road. AX has near-black value #00001a. GX uses black.
    /// + Port Town [Long Pipe]. AX has different value, #785028 deeper brown than GX's #81634a.
    /// + Port Town Story. AX has near-black #050505. GX uses black.
    /// + Sand Ocean. AX has non-black value #FFFFFF pure white. GX uses black.
    /// + AX Loop Cross test (broken) has #0a050f, near black purple value. GX uses #181a1e - very dark blue.
    /// </remarks>
    [Serializable]
    public sealed class Fog :
        IBinaryAddressable,
        IBinarySerializable,
        IDeepCopyable<Fog>,
        ITextPrintable
    {
        // FIELDS
        // Note that default values appear in AX tests and similar. A good default set.
        private FogType interpolation = FogType.Exponential;
        private ViewRange fogRange = new ViewRange(20f, 100f);
        private float3 colorRGB = float3.zero;
        private float3 zero0x18 = float3.zero; // Always zero. Perhaps always black? The last 2 values are not used in anim curve, though.


        // PROPERTIES
        public AddressRange AddressRange { get; set; }

        /// <summary>
        /// Fog interpolation mode.
        /// </summary>
        public FogType Interpolation { get => interpolation; set => interpolation = value; }

        /// <summary>
        /// Near/far for fog. A negative near value means the fog will always be applied regardless of proximity to camera.
        /// </summary>
        public ViewRange FogRange { get => fogRange; set => fogRange = value; }

        /// <summary>
        /// Color as 3 floats in order RGB.
        /// </summary>
        public float3 ColorRGB { get => colorRGB; set => colorRGB = value; }


        // STATIC METHODS
        public static Fog GetFogDefault(Venue venue)
        {
            return venue switch
            {
                Venue.BigBlue => BigBlue(),
                Venue.CasinoPalace => CasinoPalace(),
                Venue.MuteCityCOM => MuteCityCOM(),
                Venue.CosmoTerminal => CosmoTerminal(),
                Venue.FireField => FireField(),
                Venue.GreenPlant => GreenPlant(),
                Venue.Lightning => Lightning(),
                Venue.PortTown => PortTown(),
                Venue.OuterSpace => OuterSpace(),
                Venue.MuteCity => MuteCity(),
                Venue.PhantomRoad => PhantomRoad(),
                Venue.SandOcean => SandOcean(),
                Venue.Aeropolis => Aeropolis(),
                Venue.VictoryLap => VictoryLap(),
                Venue.GrandPrixPodium => GrandPrixPodium(),

                // Story
                Venue.StoryBigBlue => StoryBigBlue(),
                Venue.StoryMuteCityCOM => StoryMuteCityCOM(),
                Venue.StoryFireField => StoryFireField(),
                Venue.StoryPortTown => StoryPortTown(),
                Venue.StorySandOcean => StorySandOcean(),

                // Default for none
                Venue.None => new Fog(),

                _ => throw new NotImplementedException(),
            };
        }
        // Cup Stages
        public static Fog Aeropolis()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(300f, 3500f),
                // #ffffec - soft yellow, almost beige
                ColorRGB = new float3(1f, 1f, 0.924f),
            };
        }
        public static Fog BigBlue()
        {
            return new Fog()
            {
                Interpolation = FogType.ExponentialSquared,
                FogRange = new ViewRange(500f, 4000f),
                // #afdcff - soft blue
                ColorRGB = new float3(0.686f, 0.862f, 1f),
            };
        }
        public static Fog CasinoPalace()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(0f, 20000f),
                // #000000 - black
                ColorRGB = new float3(0f, 0f, 0f),
            };
        }
        public static Fog CosmoTerminal()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(4500f, 9000f),
                // #000000 - black
                ColorRGB = new float3(0f, 0f, 0f),
            };
        }
        public static Fog FireField()
        {
            return new Fog()
            {
                Interpolation = FogType.Linear,
                FogRange = new ViewRange(0f, 5500f),
                // #c79064 - mid-tone brown
                ColorRGB = new float3(0.78f, 0.566f, 0.391f),
            };
        }
        public static Fog GrandPrixPodium()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(0f, 12000f),
                // #2d3c50 - deep blue
                ColorRGB = new float3(0.176f, 0.235f, 0.313f),
            };
        }
        public static Fog GreenPlant()
        {
            return new Fog()
            {
                Interpolation = FogType.ExponentialSquared,
                // Intersection: -500, Mobius Ring: -250,
                FogRange = new ViewRange(-375f, 2400f),
                // 0.701, 0.780, 0.898 - dull blue (like sky color)
                ColorRGB = new float3(0.703f, 0.781f, 0.898f),
            };
        }
        public static Fog GreenPlantSpiral()
        {
            return new Fog()
            {
                Interpolation = FogType.ExponentialSquared,
                FogRange = new ViewRange(0f, 7500f),
                // #b3bd9f - dull green
                ColorRGB = new float3(0.703f, 0.7422f, 0.625f),
            };
        }
        public static Fog Lightning()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                // Thunder Road: 3000f
                FogRange = new ViewRange(0f, 2400f),
                // #181a1e - very dark blue, almost black
                ColorRGB = new float3(0.094f, 0.102f, 0.117f),
            };
        }
        public static Fog MuteCity()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(20f, 100f),
                // #000000 - black
                ColorRGB = new float3(0f, 0f, 0f),
            };
        }
        public static Fog MuteCityCOM()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(0f, 12000f),
                // #2d3c50 - dark blue
                ColorRGB = new float3(0.1765f, 0.2353f, 0.3137f),
            };
        }
        public static Fog OuterSpace()
        {
            return new Fog()
            {
                Interpolation = FogType.ExponentialSquared,
                FogRange = new ViewRange(2000f, 15000f),
                // #000000 - black
                ColorRGB = new float3(0f, 0f, 0f),
            };
        }
        public static Fog PhantomRoad()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(0f, 8000f),
                // #000000 - black
                ColorRGB = new float3(0f, 0f, 0f),
            };
        }
        public static Fog PhantomRoadAX()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(0f, 16000f),
                // #000000 - very, very dark blue, consider black
                ColorRGB = new float3(0f, 0f, 0.1f),
            };
        }
        public static Fog PortTown()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(0f, 16000f),
                // #81634a - brown
                ColorRGB = new float3(0.505f, 0.388f, 0.29f),
            };
        }
        public static Fog SandOcean()
        {
            return new Fog()
            {
                Interpolation = FogType.ExponentialSquared,
                FogRange = new ViewRange(20f, 100f),
                // #000000 - black
                ColorRGB = new float3(0f, 0f, 0f),
            };
        }
        public static Fog VictoryLap()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(0f, 12000f),
                // #2d3c50 - dark blue
                ColorRGB = new float3(0.176f, 0.235f, 0.313f),
            };
        }
        // Story Missions
        public static Fog StoryBigBlue()
        {
            return new Fog()
            {
                Interpolation = FogType.ExponentialSquared,
                FogRange = new ViewRange(-3000f, 20000f),
                // #373250 - dark indigo
                ColorRGB = new float3(0.2156f, 0.196f, 0.3137f),
            };
        }
        public static Fog StoryFireField()
        {
            return new Fog()
            {
                Interpolation = FogType.Linear,
                FogRange = new ViewRange(0f, 1500f),
                // #ff7832 - saturated orange
                ColorRGB = new float3(1f, 0.469f, 0.1954f),
            };
        }
        public static Fog StoryLightning()
        {
            return new Fog()
            {
                Interpolation = FogType.Linear,
                FogRange = new ViewRange(200f, 1350f),
                // #db5528 - mid-tone red-orange
                ColorRGB = new float3(0.8594f, 0.332f, 0.1563f),
            };
        }
        public static Fog StoryMuteCityCOM()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(600f, 7470f),
                // #00c864 - saturated green-teal
                ColorRGB = new float3(0f, 0.7843f, 0.3922f),
            };
        }
        public static Fog StoryPortTown()
        {
            return new Fog()
            {
                Interpolation = FogType.ExponentialSquared,
                FogRange = new ViewRange(0f, 20000f),
                // #000000 - black
                ColorRGB = new float3(0f, 0f, 0f),
            };
        }
        public static Fog StorySandOcean()
        {
            return new Fog()
            {
                Interpolation = FogType.Exponential,
                FogRange = new ViewRange(50f, 70000f),
                // #fffaf2 - light beige
                ColorRGB = new float3(1f, 0.98f, 0.95f),

            };
        }


        // METHODS
        public FogCurves ToFogCurves(Fog fog)
        {
            var value = new FogCurves();
            value.animationCurves = fog.ToAnimationCurves();
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AnimationCurve[] ToAnimationCurves()
        {
            return new AnimationCurve[]
            {
                ToAnimationCurve(FogRange.near),
                ToAnimationCurve(FogRange.far),
                ToAnimationCurve(ColorRGB.x),
                ToAnimationCurve(ColorRGB.y),
                ToAnimationCurve(ColorRGB.z),
                ToAnimationCurve(0f),
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private AnimationCurve ToAnimationCurve(float value)
        {
            return new AnimationCurve()
            {
                KeyableAttributes = new KeyableAttribute[]
                {
                    new KeyableAttribute()
                    {
                        EaseMode = InterpolationMode.unknown1,
                        Time = 0f,
                        Value = value,
                        TangentIn = 0f,
                        TangentOut = 0f,
                    }
                }
            };
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref interpolation);
                reader.ReadX(ref fogRange);
                reader.ReadX(ref colorRGB);
                reader.ReadX(ref zero0x18);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero0x18.Equals(float3.zero));
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zero0x18.Equals(float3.zero));
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(Interpolation);
                writer.WriteX(FogRange);
                writer.WriteX(ColorRGB);
                writer.WriteX(zero0x18);
            }
            this.RecordEndAddress(writer);
        }

        public Fog CreateDeepCopy()
        {
            return new Fog()
            {
                Interpolation = Interpolation,
                FogRange = FogRange,
                ColorRGB = ColorRGB,
                zero0x18 = zero0x18,
            };
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(Fog));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Interpolation)}: {Interpolation}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(FogRange)}: {FogRange}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(ColorRGB)}(r:{ColorRGB.x}, g:{ColorRGB.y}, b:{ColorRGB.z})");
        }

        public string PrintSingleLine()
        {
            return $"{nameof(Fog)}";
        }

        public override string ToString() => PrintSingleLine();

    }
}
