using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameCube.Cheats
{
    public sealed class GCT : IBinarySerializable
    {
        public const ulong magic = 0x00D0C0DE_00D0C0DE;
        public const ulong fileTerminator = 0xF0000000_00000000;

        public string gameCode;
        public GctCode[] codes;

        public void Deserialize(BinaryReader reader)
        {
            BinaryIoUtility.PushEndianness(Endianness.BigEndian);

            var fileSize = (int)(reader.BaseStream.Length / 4);
            var isValidFile = (fileSize % 8) == 0;

            if (!isValidFile)
                throw new FileLoadException($"Not a valid GCT file (size not multiple of 8)");

            var header = reader.ReadX_UInt64();
            if (header != magic)
                throw new FileLoadException($"Not a valid GCT file (header is not {magic:x16})");

            var codes = new List<GctCode>();
            while (true)
            {
                // If end of file, break.
                // Do this first in case empty GCT
                var nextLine = reader.PeekUint64();
                if (nextLine == fileTerminator)
                    break;

                // Instance code, deserialize, add to list of codes
                var code = new GctCode();
                reader.ReadX(ref code, false);
                codes.Add(code);
            }
            this.codes = codes.ToArray();

            BinaryIoUtility.PopEndianness();
        }

        public void Serialize(BinaryWriter writer)
        {
            BinaryIoUtility.PushEndianness(Endianness.BigEndian);
            {
                writer.WriteX(codes, false);
            }
            BinaryIoUtility.PopEndianness();
        }

    }

    public sealed class GctCode : IBinarySerializable
    {
        public const ulong codeTerminator = 0xE0000000_80008000;

        public string name;
        public ulong[] payload;

        public void Deserialize(BinaryReader reader)
        {
            var lines = new List<ulong>();
            while (true)
            {
                // Read line of code, add to list
                var line = reader.ReadX_UInt64();
                lines.Add(line);

                // If end of code, break loop
                if (line == codeTerminator)
                    break;
            }
            payload = lines.ToArray();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(payload, false);
        }

    }


    public class GfzCupGctGen
    {
        // Reference:
        // https://pastebin.com/4W2uLHSY

        const ulong CodeConst = 0x401A9B84_00000000;

        public struct TrackList
        {
            public TrackList(byte track1, byte track2, byte track3, byte track4, byte track5)
            {
                this.track1 = track1;
                this.track2 = track2;
                this.track3 = track3;
                this.track4 = track4;
                this.track5 = track5;
            }

            public ulong track1;
            public ulong track2;
            public ulong track3;
            public ulong track4;
            public ulong track5;
        }
        public class CupList
        {
            public TrackList rubyCup;
            public TrackList sapphireCup;
            public TrackList emeraldCup;
            public TrackList diamondCup;
            public TrackList axCup;
        };

        public GCT GetCustomCups(ulong[][] gameCups, CupList cupList)
        {
            if (gameCups.Length != 5)
                throw new ArgumentException();

            var gct = new GCT();
            gct.codes = new GctCode[]
            {
                GetCustomCup(gameCups[0], cupList.rubyCup),
                GetCustomCup(gameCups[1], cupList.sapphireCup),
                GetCustomCup(gameCups[2], cupList.emeraldCup),
                GetCustomCup(gameCups[3], cupList.diamondCup),
                GetCustomCup(gameCups[4], cupList.axCup),
            };
            return gct;
        }

        public GctCode GetCustomCup(ulong[] cup, TrackList trackList)
        {
            if (cup.Length != 10)
                throw new ArgumentException();

            var code = new ulong[]
            {
              cup[0] + trackList.track1,
              cup[1] + trackList.track2,
              cup[2] + trackList.track3,
              cup[3] + trackList.track4,
              cup[4] + trackList.track5,
              cup[5] + trackList.track1,
              cup[6] + trackList.track2,
              cup[7] + trackList.track3,
              cup[8] + trackList.track4,
              cup[9] + trackList.track5,
              GctCode.codeTerminator,
            };

            return new GctCode() { payload = code };
        }


        public static readonly ulong[] GfzeRubyCup = new ulong[]
        {
              CodeConst + 0x0B058D_00,
              CodeConst + 0x0B058F_00,
              CodeConst + 0x0B0591_00,
              CodeConst + 0x0B0593_00,
              CodeConst + 0x0B0595_00,

              CodeConst + 0x0B0611_00,
              CodeConst + 0x0B0613_00,
              CodeConst + 0x0B0615_00,
              CodeConst + 0x0B0617_00,
              CodeConst + 0x0B0619_00,
        };

        public static readonly ulong[] GfzeSapphireCup = new ulong[]
        {
              CodeConst + 0x0B0599_00,
              CodeConst + 0x0B059B_00,
              CodeConst + 0x0B059D_00,
              CodeConst + 0x0B059F_00,
              CodeConst + 0x0B05a1_00,

              CodeConst + 0x0B061D_00,
              CodeConst + 0x0B061F_00,
              CodeConst + 0x0B0621_00,
              CodeConst + 0x0B0623_00,
              CodeConst + 0x0B0625_00,
        };

        public static readonly ulong[] GfzeEmeraldCup = new ulong[]
        {
              CodeConst + 0x0B05A5_00,
              CodeConst + 0x0B05A7_00,
              CodeConst + 0x0B05A9_00,
              CodeConst + 0x0B05AB_00,
              CodeConst + 0x0B05AD_00,

              CodeConst + 0x0B0629_00,
              CodeConst + 0x0B062B_00,
              CodeConst + 0x0B062D_00,
              CodeConst + 0x0B062F_00,
              CodeConst + 0x0B0631_00,
        };

        public static readonly ulong[] GfzeDiamondCup = new ulong[]
        {
              CodeConst + 0x0B05B1_00,
              CodeConst + 0x0B05B3_00,
              CodeConst + 0x0B05B5_00,
              CodeConst + 0x0B05B7_00,
              CodeConst + 0x0B05B9_00,

              CodeConst + 0x0B0635_00,
              CodeConst + 0x0B0637_00,
              CodeConst + 0x0B0639_00,
              CodeConst + 0x0B063B_00,
              CodeConst + 0x0B063D_00,
        };

        public static readonly ulong[] GfzeAxCup = new ulong[]
        {
              CodeConst + 0x0B05BD_00,
              CodeConst + 0x0B05BF_00,
              CodeConst + 0x0B05C1_00,
              CodeConst + 0x0B05C3_00,
              CodeConst + 0x0B05C5_00,

              CodeConst + 0x0B0641_00,
              CodeConst + 0x0B0643_00,
              CodeConst + 0x0B0645_00,
              CodeConst + 0x0B0647_00,
              CodeConst + 0x0B0649_00,
        };
        public static ulong[][] GfzeCups
        {
            get
            {
                var cups = new ulong[][] {
                    GfzeRubyCup,
                    GfzeSapphireCup,
                    GfzeEmeraldCup,
                    GfzeDiamondCup,
                    GfzeAxCup,
                };

                return cups;
            }
        }

    }
}
