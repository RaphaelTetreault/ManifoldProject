using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

// NOTE: When unk_0x0C is (0, 0, 0), then there is no pointer in ColiCourse header at 0x80
// Review notes in UnknownStageData2

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnkStageRanges :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public RangeOption rangeOption = RangeOption.B_0x04;
        public MinMax minMax0x04 = new MinMax(20f, 100f);
        public float4 unk0x0C;
        //public MinMax minMax0x0C = new MinMax(0f, 0f);
        //public MinMax minMax0x14 = new MinMax(0f, 0f);
        public int zero0x1C;
        public int zero0x20;


        // PROPERTIES


        // NOTES
        // Did not do AX story missions since they were likely WIP
        // Inline comments for Aeropolis and Green Plant. They have quirks.
        // AX CPDB has different light values
        // GP -I and -MR differ from GPS (GPS is more "purple")
        // AX broken Loop Cross is it's own thing
        public UnkStageRanges Aeropolis()
        {
            return new UnkStageRanges()
            {
                rangeOption = RangeOption.B_0x04,
                minMax0x04 = new MinMax(300f, 3500f),
                minMax0x0C = new MinMax(1.000f, 1.000f),
                minMax0x14 = new MinMax(0.924f, 0.000f),
            };
        }
        public UnkStageRanges BigBlue()
        {
            return new UnkStageRanges()
            {
                rangeOption = RangeOption.C_0x05,
                minMax0x04 = new MinMax(500f, 4000f),
                minMax0x0C = new MinMax(0.686f, 0.862f),
                minMax0x14 = new MinMax(1.000f, 0.000f),
            };
        }
        //public VenueGlobalParameters CasinoPalace()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //            max = 20000f,
        //        },
        //        unkColorRGB = new float3(0f, 0f, 0f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters CosmoTerminal()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 4500f,
        //            max = 9000f,
        //        },
        //        unkColorRGB = new float3(0f, 0f, 0f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters FireField()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.A_0x02,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //            max = 5500f,
        //        },
        //        unkColorRGB = new float3(0.78f, 0.566f, 0.391f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters GrandPrixPodium()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //            max = 12000f,
        //        },
        //        unkColorRGB = new float3(0.176f, 0.235f, 0.313f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters GreenPlant()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.C_0x05,
        //        minMax0x04 = new MinMax()
        //        {
        //             Intersection: -500, Mobius Ring: -250, 
        //            min = -375f,
        //            max = 2400f,
        //        },
        //        unkColorRGB = new float3(0.703f, 0.781f, 0.898f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters GreenPlantSpiral()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.C_0x05,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //            max = 7500f,
        //        },
        //        unkColorRGB = new float3(0.703f, 0.7422f, 0.625f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters Lightning()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //             Thunder Road: 3000f
        //            max = 2400f,
        //        },
        //        unkColorRGB = new float3(0.094f, 0.102f, 0.117f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters MuteCity()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 20f,
        //            max = 100f,
        //        },
        //        unkColorRGB = new float3(0f, 0f, 0f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters MuteCityCOM()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //            max = 12000f,
        //        },
        //        unkColorRGB = new float3(0.1765f, 0.2353f, 0.3137f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters OuterSpace()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.C_0x05,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 2000f,
        //            max = 15000f,
        //        },
        //        unkColorRGB = new float3(0f, 0f, 0f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters PhantomRoad()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //            max = 8000f,
        //        },
        //        unkColorRGB = new float3(0f, 0f, 0f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters PhantomRoadAX()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //            max = 16000f,
        //        },
        //        unkColorRGB = new float3(0f, 0f, 0.1f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters PortTown()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //            max = 16000f,
        //        },
        //        unkColorRGB = new float3(0.505f, 0.388f, 0.29f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters SandOcean()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.C_0x05,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 20f,
        //            max = 100f,
        //        },
        //        unkColorRGB = new float3(0f, 0f, 0f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters VictoryLap()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //            max = 12000f,
        //        },
        //        unkColorRGB = new float3(0.176f, 0.235f, 0.313f),
        //        zero0x18 = float3.zero,
        //    };
        //}

        //public VenueGlobalParameters StoryBigBlue()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.C_0x05,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = -3000f,
        //            max = 20000f,
        //        },
        //        unkColorRGB = new float3(0.2156f, 0.196f, 0.3137f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters StoryFireField()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.A_0x02,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //            max = 1500f,
        //        },
        //        unkColorRGB = new float3(1f, 0.469f, 0.1954f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters StoryLightning()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.A_0x02,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 200f,
        //            max = 1350f,
        //        },
        //        unkColorRGB = new float3(0.8594f, 0.332f, 0.1563f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters StoryMuteCityCOM()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 600f,
        //            max = 7470f,
        //        },
        //        unkColorRGB = new float3(0f, 0.7843f, 0.3922f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters StoryPortTown()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.C_0x05,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 0f,
        //            max = 20000f,
        //        },
        //        unkColorRGB = new float3(0f, 0f, 0f),
        //        zero0x18 = float3.zero,
        //    };
        //}
        //public VenueGlobalParameters StorySandOcean()
        //{
        //    return new VenueGlobalParameters()
        //    {
        //        rangeOption = RangeOption.B_0x04,
        //        minMax0x04 = new MinMax()
        //        {
        //            min = 50f,
        //            max = 70000f,
        //        },
        //        unkColorRGB = new float3(1f, 0.98f, 0.95f),
        //        zero0x18 = float3.zero,
        //    };
        //}


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref rangeOption);
                reader.ReadX(ref minMax0x04, true);
                reader.ReadX(ref minMax0x0C, true);
                reader.ReadX(ref minMax0x14, true);
                reader.ReadX(ref zero0x1C);
                reader.ReadX(ref zero0x20);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero0x1C == 0);
                Assert.IsTrue(zero0x20 == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zero0x1C == 0);
                Assert.IsTrue(zero0x20 == 0);
            }
            this.RecordStartAddress(writer);
            {
               writer.WriteX(rangeOption);
               writer.WriteX(minMax0x04);
               writer.WriteX(minMax0x0C);
               writer.WriteX(minMax0x14);
               writer.WriteX(zero0x1C);
               writer.WriteX(zero0x20);
            }
            this.RecordEndAddress(writer);
        }

    }
}
