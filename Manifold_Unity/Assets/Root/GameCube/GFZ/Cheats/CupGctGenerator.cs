using GameCube.Cheats;
using System;

namespace GameCube.GFZ.Cheats
{
    public class CupGctGenerator
    {
        // Reference:
        // https://pastebin.com/4W2uLHSY
        // JP: http://www.gc-forever.com/forums/viewtopic.php?f=38&t=2242&p=49095&hilit=f+zero#p49095
        // EN: http://www.gc-forever.com/forums/viewtopic.php?f=38&t=2241&p=49097&hilit=f+zero#p49097
        // EU: http://www.gc-forever.com/forums/viewtopic.php?f=38&t=2240&p=49099&hilit=f+zero#p49099


        const ulong CodeConst = 0x401A9B84_00000000;

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
