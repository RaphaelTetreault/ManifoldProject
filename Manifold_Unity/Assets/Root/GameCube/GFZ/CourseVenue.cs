using System.ComponentModel;

namespace GameCube.GFZ
{
    public enum CourseVenue
    {
        [Description("Aeropolis")]
        Aeropolis,
        [Description("Big Blue")]
        BigBlue,
        [Description("Casino Palace")]
        CasinoPalace,
        [Description("Cosmo Terminal")]
        CosmoTerminal,
        [Description("Fire Field")]
        FireField,
        [Description("Green Plant")]
        GreenPlant,
        [Description("Lightning")]
        Lightning,
        [Description("Null")]
        None,
        [Description("Mute City")]
        MuteCity,
        [Description("Outer Space")]
        OuterSpace,
        [Description("Phantom Road")]
        PhantomRoad,
        [Description("Port Town")]
        PortTown,
        [Description("Sand Ocean")]
        SandOcean,
        [Description("Mute City (Versus Mode)")]
        Win,
        [Description("Mute City (Grand Prix)")]
        Win_GX,
    }
}