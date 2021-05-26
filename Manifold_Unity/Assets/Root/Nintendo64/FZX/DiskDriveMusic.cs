using System.ComponentModel;

namespace Nintendo64.FZX
{
    public enum DiskDriveMusic
    {
        [Description("(No Music)")]
        None = 0xFF,

        [Description("Mute City")]
        MuteCity = 0x00,

        [Description("Silence")]
        Silence = 0x01,

        [Description("Sand Ocean")]
        SandOcean = 0x02,

        [Description("Port Town")]
        PortTown = 0x03,

        [Description("Big Blue")]
        BigBlue = 0x04,

        [Description("Devil's Forest")]
        DevilsForest = 0x05,

        [Description("Red Canyon")]
        RedCanyon = 0x06,

        [Description("Sector")]
        Sector = 0x07,

        [Description("White Land")]
        WhiteLand = 0x08,

        [Description("Rainbow Road")]
        RainbowRoad = 0x20,

        [Description("Big Foot")]
        BigFoot = 0x21,

        [Description("Japon")]
        Japon = 0x22,

        [Description("Regeneration")]
        Regeneration = 0x23,

        [Description("Roller Coaster")]
        RollerCoaster = 0x24,
    }

}
