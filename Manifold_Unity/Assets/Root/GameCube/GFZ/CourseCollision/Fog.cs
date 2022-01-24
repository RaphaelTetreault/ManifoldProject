using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Defines parameters to produce and render fog.
    /// </summary>
    [Serializable]
    public class Fog :
        IBinaryAddressable,
        IBinarySerializable,
        IDeepCopyable<Fog>
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        // Note that default values appear in AX tests and similar. A good default set.
        public FogType interpolation = FogType.Exponential;
        public Range fogRange = new Range(20f, 100f); // Not confirmed, but suspected. Near/far for fog. Negative near = always affected.
        public float3 colorRGBA = float3.zero; // color as 3 floats in order RGB
        public float3 zero0x18 = float3.zero; // Always zero. Perhaps always black? The last 2 values are not used in anim curve, though.

        // NOTES
        // Below are notes on the observed values in all stage files for AX and GX.
        // + Inline comments for Aeropolis and Green Plant. They have quirks.
        // + GPI and GPMR differ from GPS. Soft dull blue vs soft dull green, respectively.

        // Imprecision Differences
        // Some color values are barely different between stages/venues. It appears that
        // someone went and standardize this later on, but some differences still exist.
        // 
        // + Aeropolis - GX tracks AM ADS vs AX track ASD (track, not game)
        // + Port Town - GX tracks PTAD PTLP vs AX track PTCW (track, not game)
        // + Green Plant - between GX tracks.

        // AX Color Differences
        // I did not write template fields for AX _story_ stages since they are WIP values.
        //
        // + Big Blue Story. AX has default value of black. This changed in GX.
        // + Casino Palace [Double Branches]. AX has non-black value #0a0a0a. In anim data, is #18303c (blue). GX uses black.
        // + Phantom Road. AX has near-black value #00001a. GX uses black.
        // + Port Town [Long Pipe]. AX has different value, #785028 deeper brown than GX.
        // + Port Town Story. AX has near-black #050505. GX uses black.
        // + Sand Ocean. AX has non-black value #FFFFFF pure white. GX uses black.
        // + AX Loop Cross test (broken) has #0a050f, near black purple value. GX uses #181a1e - very dark blue.


        // TEMPLATES
        public static Fog GetFogDefault(Venue venue)
        {
            return venue switch
            {
                Venue.BigBlue       => BigBlue(),
                Venue.CasinoPalace  => CasinoPalace(),
                Venue.MuteCityCOM   => MuteCityCOM(),
                Venue.CosmoTerminal => CosmoTerminal(),
                Venue.FireField     => FireField(),
                Venue.GreenPlant    => GreenPlant(),
                Venue.Lightning     => Lightning(),
                Venue.PortTown      => PortTown(),
                Venue.OuterSpace    => OuterSpace(),
                Venue.MuteCity      => MuteCity(),
                Venue.PhantomRoad   => PhantomRoad(),
                Venue.SandOcean     => SandOcean(),
                Venue.Aeropolis     => Aeropolis(),
                Venue.VictoryLap    => VictoryLap(),
                Venue.GrandPrixPodium => GrandPrixPodium(),

                // Story
                Venue.StoryBigBlue      => StoryBigBlue(),
                Venue.StoryMuteCityCOM  => StoryMuteCityCOM(),
                Venue.StoryFireField    => StoryFireField(),
                Venue.StoryPortTown     => StoryPortTown(),
                Venue.StorySandOcean    => StorySandOcean(),

                // Default for none
                Venue.None => new Fog(),

                _ => throw new NotImplementedException(),
            };
        }

        public static Fog Aeropolis()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(300f, 3500f),
                // #ffffec - soft yellow, almost beige
                colorRGBA = new float3(1f, 1f, 0.924f),
            };
        }
        public static Fog BigBlue()
        {
            return new Fog()
            {
                interpolation = FogType.ExponentialSquared,
                fogRange = new Range(500f, 4000f),
                // #afdcff - soft blue
                colorRGBA = new float3(0.686f, 0.862f, 1f),
            };
        }
        public static Fog CasinoPalace()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(0f, 20000f),
                // #000000 - black
                colorRGBA = new float3(0f, 0f, 0f),
            };
        }
        public static Fog CosmoTerminal()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(4500f, 9000f),
                // #000000 - black
                colorRGBA = new float3(0f, 0f, 0f),
            };
        }
        public static Fog FireField()
        {
            return new Fog()
            {
                interpolation = FogType.Linear,
                fogRange = new Range(0f, 5500f),
                // #c79064 - mid-tone brown
                colorRGBA = new float3(0.78f, 0.566f, 0.391f),
            };
        }
        public static Fog GrandPrixPodium()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(0f, 12000f),
                // #2d3c50 - deep blue
                colorRGBA = new float3(0.176f, 0.235f, 0.313f),
            };
        }
        public static Fog GreenPlant()
        {
            return new Fog()
            {
                interpolation = FogType.ExponentialSquared,
                // Intersection: -500, Mobius Ring: -250,
                fogRange = new Range(-375f, 2400f),
                // 0.701, 0.780, 0.898 - dull blue (like sky color)
                colorRGBA = new float3(0.703f, 0.781f, 0.898f),
            };
        }
        public static Fog GreenPlantSpiral()
        {
            return new Fog()
            {
                interpolation = FogType.ExponentialSquared,
                fogRange = new Range(0f, 7500f),
                // #b3bd9f - dull green
                colorRGBA = new float3(0.703f, 0.7422f, 0.625f),
            };
        }
        public static Fog Lightning()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                // Thunder Road: 3000f
                fogRange = new Range(0f, 2400f),
                // #181a1e - very dark blue, almost black
                colorRGBA = new float3(0.094f, 0.102f, 0.117f),
            };
        }
        public static Fog MuteCity()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(20f, 100f),
                // #000000 - black
                colorRGBA = new float3(0f, 0f, 0f),
            };
        }
        public static Fog MuteCityCOM()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(0f, 12000f),
                // #2d3c50 - dark blue
                colorRGBA = new float3(0.1765f, 0.2353f, 0.3137f),
            };
        }
        public static Fog OuterSpace()
        {
            return new Fog()
            {
                interpolation = FogType.ExponentialSquared,
                fogRange = new Range(2000f, 15000f),
                // #000000 - black
                colorRGBA = new float3(0f, 0f, 0f),
            };
        }
        public static Fog PhantomRoad()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(0f, 8000f),
                // #000000 - black
                colorRGBA = new float3(0f, 0f, 0f),
            };
        }
        public static Fog PhantomRoadAX()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(0f, 16000f),
                // #000000 - very, very dark blue, consider black
                colorRGBA = new float3(0f, 0f, 0.1f),
            };
        }
        public static Fog PortTown()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(0f, 16000f),
                // #81634a - brown
                colorRGBA = new float3(0.505f, 0.388f, 0.29f),
            };
        }
        public static Fog SandOcean()
        {
            return new Fog()
            {
                interpolation = FogType.ExponentialSquared,
                fogRange = new Range(20f, 100f),
                // #000000 - black
                colorRGBA = new float3(0f, 0f, 0f),
            };
        }
        public static Fog VictoryLap()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(0f, 12000f),
                // #2d3c50 - dark blue
                colorRGBA = new float3(0.176f, 0.235f, 0.313f),
            };
        }

        public static Fog StoryBigBlue()
        {
            return new Fog()
            {
                interpolation = FogType.ExponentialSquared,
                fogRange = new Range(-3000f, 20000f),
                // #373250 - dark indigo
                colorRGBA = new float3(0.2156f, 0.196f, 0.3137f),
            };
        }
        public static Fog StoryFireField()
        {
            return new Fog()
            {
                interpolation = FogType.Linear,
                fogRange = new Range(0f, 1500f),
                // #ff7832 - saturated orange
                colorRGBA = new float3(1f, 0.469f, 0.1954f),
            };
        }
        public static Fog StoryLightning()
        {
            return new Fog()
            {
                interpolation = FogType.Linear,
                fogRange = new Range(200f, 1350f),
                // #db5528 - mid-tone red-orange
                colorRGBA = new float3(0.8594f, 0.332f, 0.1563f),
            };
        }
        public static Fog StoryMuteCityCOM()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(600f, 7470f),
                // #00c864 - saturated green-teal
                colorRGBA = new float3(0f, 0.7843f, 0.3922f),
            };
        }
        public static Fog StoryPortTown()
        {
            return new Fog()
            {
                interpolation = FogType.ExponentialSquared,
                fogRange = new Range(0f, 20000f),
                // #000000 - black
                colorRGBA = new float3(0f, 0f, 0f),
            };
        }
        public static Fog StorySandOcean()
        {
            return new Fog()
            {
                interpolation = FogType.Exponential,
                fogRange = new Range(50f, 70000f),
                // #fffaf2 - light beige
                colorRGBA = new float3(1f, 0.98f, 0.95f),

            };
        }


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public FogCurves ToFogCurves(Fog fog)
        {
            var value = new FogCurves();
            value.animationCurves = fog.ToAnimationCurves();
            return value;
        }

        /// <summary>
        /// Method to convert this object into UnkStageAnimationCurves animation curves.
        /// </summary>
        /// <returns></returns>
        public AnimationCurve[] ToAnimationCurves()
        {
            return new AnimationCurve[]
            {
                ToAnimationCurve(fogRange.near),
                ToAnimationCurve(fogRange.far),
                ToAnimationCurve(colorRGBA.x),
                ToAnimationCurve(colorRGBA.y),
                ToAnimationCurve(colorRGBA.z),
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
                keyableAttributes = new KeyableAttribute[]
                {
                    new KeyableAttribute()
                    {
                        easeMode = InterpolationMode.unknown1,
                        time = 0f,
                        value = value,
                        zTangentIn = 0f,
                        zTangentOut = 0f,
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
                reader.ReadX(ref fogRange, true);
                reader.ReadX(ref colorRGBA);
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
                writer.WriteX(interpolation);
                writer.WriteX(fogRange);
                writer.WriteX(colorRGBA);
                writer.WriteX(zero0x18);
            }
            this.RecordEndAddress(writer);
        }

        public Fog CreateDeepCopy()
        {
            return new Fog()
            {
                interpolation = interpolation,
                fogRange = fogRange,
                colorRGBA = colorRGBA,
                zero0x18 = zero0x18,
            };
        }

        public override string ToString()
        {
            return 
                $"{nameof(Fog)}(" +
                $"{nameof(interpolation)}: {interpolation}, " +
                $"{nameof(fogRange)}: {fogRange}, " +
                $"{nameof(colorRGBA)}(r:{colorRGBA.x}, g:{colorRGBA.y}, b:{colorRGBA.z})" +
                $")";
        }

    }
}
