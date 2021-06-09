using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class StageFog :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        // Note that default values appear in AX tests and similar. A good default set.
        public StageFogOption option = StageFogOption.B_0x04;
        public Range fogRange = new Range(20f, 100f); // Not confirmed, but suspected. Near/far for fog. Negative near = always affected.
        public float3 colorRGB = float3.zero; // color as 3 floats in order RGB
        public float3 zero0x18 = float3.zero; // Always zero. Perhaps always black? The last 2 values are not used in anim curve, though.

        // NOTES
        // Below are notes on the observed values in all stage files for AX and GX.
        // + Inline comments for Aeropolis and Green Plant. They have quirks.
        // + GP -I and -MR differ from GPS. Soft dull blue vs soft dull green, respectively.

        // Imprecision Differences
        // Some color values are barely different between stages/venues. It appears that
        // some went and standardize this later on, but some differences still exist.
        // 
        // + Aeropolis - GX tracks AM ADS vs AX track ASD (track, not game)
        // + Port Town - GX tracks PTAD PTLP vs AX track PTCW (track, not game)
        // + Green Plant - between GX tracks.

        // AX Color Differences
        // I did not write template fields for AX _story_ stages since they are WIP values.
        //
        // + Big Blue Story. AX has default value of black. This changed in GX.
        // + Casino Palace [Double Branches]. AX has non-black value #0a0a0a. GX uses black.
        // + Phantom Road. AX has near-black value #00001a. GX uses black.
        // + Port Town [Long Pipe]. AX has different value, #785028 deeper brown than GX.
        // + Port Town Story. AX has near-black #050505. GX uses black.
        // + Sand Ocean. AX has non-black value #FFFFFF pure white. GX uses black.
        // + AX Loop Cross test (broken) has #0a050f, near black purple value. GX uses #181a1e - very dark blue.


        // TEMPLATES
        public StageFog Aeropolis()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(300f, 3500f),
                // #ffffec - soft yellow, almost beige
                colorRGB = new float3(1f, 1f, 0.924f),
            };
        }
        public StageFog BigBlue()
        {
            return new StageFog()
            {
                option = StageFogOption.C_0x05,
                fogRange = new Range(500f, 4000f),
                // #afdcff - soft blue
                colorRGB = new float3(0.686f, 0.862f, 1f),
            };
        }
        public StageFog CasinoPalace()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(0f, 20000f),
                // #000000 - black
                colorRGB = new float3(0f, 0f, 0f),
            };
        }
        public StageFog CosmoTerminal()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(4500f, 9000f),
                // #000000 - black
                colorRGB = new float3(0f, 0f, 0f),
            };
        }
        public StageFog FireField()
        {
            return new StageFog()
            {
                option = StageFogOption.A_0x02,
                fogRange = new Range(0f, 5500f),
                // #c79064 - mid-tone brown
                colorRGB = new float3(0.78f, 0.566f, 0.391f),
            };
        }
        public StageFog GrandPrixPodium()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(0f, 12000f),
                // #2d3c50 - deep blue
                colorRGB = new float3(0.176f, 0.235f, 0.313f),
            };
        }
        public StageFog GreenPlant()
        {
            return new StageFog()
            {
                option = StageFogOption.C_0x05,
                // Intersection: -500, Mobius Ring: -250,
                fogRange = new Range(-375f, 2400f),
                // 0.701, 0.780, 0.898 - dull blue (like sky color)
                colorRGB = new float3(0.703f, 0.781f, 0.898f),
            };
        }
        public StageFog GreenPlantSpiral()
        {
            return new StageFog()
            {
                option = StageFogOption.C_0x05,
                fogRange = new Range(0f, 7500f),
                // #b3bd9f - dull green
                colorRGB = new float3(0.703f, 0.7422f, 0.625f),
            };
        }
        public StageFog Lightning()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                // Thunder Road: 3000f
                fogRange = new Range(0f, 2400f),
                // #181a1e - very dark blue, almost black
                colorRGB = new float3(0.094f, 0.102f, 0.117f),
            };
        }
        public StageFog MuteCity()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(20f, 100f),
                // #000000 - black
                colorRGB = new float3(0f, 0f, 0f),
            };
        }
        public StageFog MuteCityCOM()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(0f, 12000f),
                // #2d3c50 - dark blue
                colorRGB = new float3(0.1765f, 0.2353f, 0.3137f),
            };
        }
        public StageFog OuterSpace()
        {
            return new StageFog()
            {
                option = StageFogOption.C_0x05,
                fogRange = new Range(2000f, 15000f),
                // #000000 - black
                colorRGB = new float3(0f, 0f, 0f),
            };
        }
        public StageFog PhantomRoad()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(0f, 8000f),
                // #000000 - black
                colorRGB = new float3(0f, 0f, 0f),
            };
        }
        public StageFog PhantomRoadAX()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(0f, 16000f),
                // #000000 - very, very dark blue, consider black
                colorRGB = new float3(0f, 0f, 0.1f),
            };
        }
        public StageFog PortTown()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(0f, 16000f),
                // #81634a - brown
                colorRGB = new float3(0.505f, 0.388f, 0.29f),
            };
        }
        public StageFog SandOcean()
        {
            return new StageFog()
            {
                option = StageFogOption.C_0x05,
                fogRange = new Range(20f, 100f),
                // #000000 - black
                colorRGB = new float3(0f, 0f, 0f),
            };
        }
        public StageFog VictoryLap()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(0f, 12000f),
                // #2d3c50 - dark blue
                colorRGB = new float3(0.176f, 0.235f, 0.313f),
            };
        }

        public StageFog StoryBigBlue()
        {
            return new StageFog()
            {
                option = StageFogOption.C_0x05,
                fogRange = new Range(-3000f, 20000f),
                // #373250 - dark indigo
                colorRGB = new float3(0.2156f, 0.196f, 0.3137f),
            };
        }
        public StageFog StoryFireField()
        {
            return new StageFog()
            {
                option = StageFogOption.A_0x02,
                fogRange = new Range(0f, 1500f),
                // #ff7832 - saturated orange
                colorRGB = new float3(1f, 0.469f, 0.1954f),
            };
        }
        public StageFog StoryLightning()
        {
            return new StageFog()
            {
                option = StageFogOption.A_0x02,
                fogRange = new Range(200f, 1350f),
                // #db5528 - mid-tone red-orange
                colorRGB = new float3(0.8594f, 0.332f, 0.1563f),
            };
        }
        public StageFog StoryMuteCityCOM()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(600f, 7470f),
                // #00c864 - saturated green-teal
                colorRGB = new float3(0f, 0.7843f, 0.3922f),
            };
        }
        public StageFog StoryPortTown()
        {
            return new StageFog()
            {
                option = StageFogOption.C_0x05,
                fogRange = new Range(0f, 20000f),
                // #000000 - black
                colorRGB = new float3(0f, 0f, 0f),
            };
        }
        public StageFog StorySandOcean()
        {
            return new StageFog()
            {
                option = StageFogOption.B_0x04,
                fogRange = new Range(50f, 70000f),
                // #fffaf2 - light beige
                colorRGB = new float3(1f, 0.98f, 0.95f),

            };
        }


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
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
                ToAnimationCurve(colorRGB.x),
                ToAnimationCurve(colorRGB.y),
                ToAnimationCurve(colorRGB.z),
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
                reader.ReadX(ref option);
                reader.ReadX(ref fogRange, true);
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
               writer.WriteX(option);
               writer.WriteX(fogRange);
               writer.WriteX(colorRGB);
               writer.WriteX(zero0x18);
            }
            this.RecordEndAddress(writer);
        }

    }
}
