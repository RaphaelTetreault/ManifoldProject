using System.ComponentModel;

namespace Nintendo64.FZX
{
    /// <summary>
    /// Left in order of FZEP dropdown
    /// </summary>
    public enum Image : ushort
    {
        [Description("Mute City A (Light)")]
        MuteCityLightA = 0,
        [Description("Mute City B (Light)")]
        MuteCityLightB,
        [Description("Mute City C (Light)")]
        MuteCityLightC,
        [Description("Mute City D (Light)")]
        MuteCityLightD,

        [Description("Devil's Forest A")]
        DevilsForestA,
        [Description("Devil's Forest B")]
        DevilsForestB,
        [Description("Devil's Forest C")]
        DevilsForestC,
        [Description("Devil's Forest D")]
        DevilsForestD,

        [Description("Port Town A (Light)")]
        PortTownLightA,
        [Description("Port Town B (Light)")]
        PortTownLightB,
        [Description("Port Town C (Light)")]
        PortTownLightC,
        [Description("Port Town D (Light)")]
        PortTownLightD,

        [Description("Port Town A (Dark)")]
        PortTownDarkA,
        [Description("Port Town B (Dark)")]
        PortTownDarkB,
        [Description("Port Town C (Dark)")]
        PortTownDarkC,
        [Description("Port Town D (Dark)")]
        PortTownDarkD,

        [Description("Red Canyon A")]
        RedCanyonA,
        [Description("Red Canyon B")]
        RedCanyonB,
        [Description("Red Canyon C")]
        RedCanyonC,
        [Description("Red Canyon D")]
        RedCanyonD,

        [Description("Sector A")]
        SectorA,
        [Description("Sector B")]
        SectorB,
        [Description("Sector C")]
        SectorC,

        [Description("X Cup Exclusive A")]
        XCupExclusiveA,

        [Description("Big Hand A")]
        BigHandA,
        [Description("Big Hand B")]
        BigHandB,
        [Description("Big Hand C")]
        BigHandC,
        [Description("Big Hand D")]
        BigHandD,

        [Description("White Land A")]
        WhiteLandA,
        [Description("White Land B")]
        WhiteLandB,
        [Description("White Land C")]
        WhiteLandC,

        [Description("X Cup Exclusive B")]
        XCupExclusiveB,

        [Description("Big Blue A")]
        BigBlueA,
        [Description("Big Blue B")]
        BigBlueB,
        [Description("Big Blue C")]
        BigBlueC,
        [Description("Big Blue D")]
        BigBlueD,

        [Description("Silence A (Light)")]
        SilenceLightA,
        [Description("Silence B (Light)")]
        SilenceLightB,

        [Description("Mute City A (Dark)")]
        MuteCityDarkA,
        [Description("Mute City B (Dark)")]
        MuteCityDarkB,
        [Description("Mute City C (Dark)")]
        MuteCityDarkC,
        [Description("Mute City D (Dark)")]
        MuteCityDarkD,

        [Description("GP Ending A")]
        GpEndingA,

        [Description("\"Jack Cup\"")]
        JackCup,

        [Description("GP Ending B")]
        GpEndingB,
        [Description("GP Ending C")]
        GpEndingC,
        [Description("GP Ending D")]
        GpEndingD,

        [Description("\"Queen Cup\"")]
        QueenCup,
        [Description("\"King Cup\"")]
        KingCup,
        [Description("\"Joker Cup\"")]
        JokerCup,
        [Description("\"X Cup\"")]
        XCup,
        [Description("\"Edit Cup\"")]
        EditCup,

        [Description("X Cup Exclusive C")]
        XCupExclusiveC,

        [Description("Silence A (Dark)")]
        SilenceDarkA,
        [Description("Silence B (Dark)")]
        SilenceDarkB,
    }

}
