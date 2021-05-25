using System.ComponentModel;

namespace Nintendo64.FZX
{
    public enum Venue
    {
        [Description("Mute City")]
        MuteCity = 0,

        [Description("Port Town")]
        PortTown,

        [Description("Big Blue")]
        BigBlue,

        [Description("Sand Ocean")]
        SandOcean,

        [Description("Devil's Forest")]
        DevilsForest,

        [Description("White Land")]
        WhiteLand,

        [Description("Sector")]
        Sector,

        [Description("Red Canyon")]
        RedCanyon,

        [Description("Fire Field")]
        FireField,

        [Description("Silence")]
        Silence,

        [Description("GP Ending")]
        GpEnding,
    }

}
