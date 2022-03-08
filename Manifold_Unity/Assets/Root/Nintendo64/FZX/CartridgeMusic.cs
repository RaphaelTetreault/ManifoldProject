using System.ComponentModel;

namespace Nintendo64.FZX
{
    public enum CartridgeMusic : byte
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

        [Description("Title")]
        Title = 0x0D,

        [Description("Menu")]
        Menu = 0x0E,

        [Description("Records")]
        Records = 0x0F,

        [Description("Race Results")]
        RaceResults = 0x10,

        [Description("GP Ending")]
        GpEnding = 0x15,

        [Description("Credits")]
        Credits = 0x16,

        [Description("Death Race")]
        DeathRace = 0x18,
    }

}
