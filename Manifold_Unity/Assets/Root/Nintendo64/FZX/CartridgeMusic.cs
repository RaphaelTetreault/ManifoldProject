using System.ComponentModel;

namespace Nintendo64.FZX
{
    public enum CartridgeMusic
    {
        [Description("[No Music]")]
        None = -1,

        [Description("Mute City")]
        MuteCity = 0,

        [Description("Silence")]
        Silence,

        [Description("Sand Ocean")]
        SandOcean,

        [Description("Port Town")]
        PortTown,

        [Description("Big Blue")]
        BigBlue,

        [Description("Devil's Forest")]
        DevilsForest,

        [Description("Red Canyon")]
        RedCanyon,

        [Description("Sector")]
        Sector,

        [Description("White Land")]
        WhiteLand,

        [Description("Title")]
        Title,

        [Description("Menu")]
        Menu,

        [Description("Records")]
        Records,

        [Description("Race Results")]
        RaceResults,

        [Description("GP Ending")]
        GpEnding,

        [Description("Credits")]
        Credits,

        [Description("Death Race")]
        DeathRace,
    }

}
