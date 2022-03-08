using System.ComponentModel;

namespace GameCube.GFZ
{
    /// <summary>
    /// Flags for each venue. Order is arbitrary (alphabetical). Flags are used
    /// since since some stages are both a regular venue but with story elements.
    /// </summary>
    [System.Flags]
    public enum VenueID
    {
        /// <summary>
        /// Unused venue ID
        /// </summary>
        [Description("Unknown (AUR)")]
        AUR = 1 << 0,

        /// <summary>
        /// Big Blue
        /// </summary>
        [Description("Big Blue")]
        BIG = 1 << 1,

        /// <summary>
        /// Big Blue (Story)
        /// </summary>
        [Description("Big Blue (Story)")]
        BIG_S = BIG + isStory,

        /// <summary>
        /// Casino Palace
        /// </summary>
        [Description("Casino Palace")]
        CAS = 1 << 2,

        /// <summary>
        /// Mute City Sonic Oval
        /// </summary>
        /// <remarks>COM for Commercial? COM for Combined MUT+CAS?</remarks>
        [Description("Mute City (COM)")]
        COM = 1 << 3,

        /// <summary>
        /// Mute City Sonic Oval (Story)
        /// </summary>
        /// <remarks>COM for Commercial? COM for Combined MUT+CAS?</remarks>
        [Description("Mute City (Story COM)")]
        COM_S = COM + isStory,

        /// <summary>
        /// Cosmo Terminal
        /// </summary>
        [Description("Cosmo Terminal")]
        ELE = 1 << 4,

        /// <summary>
        /// Fire Field
        /// </summary>
        [Description("Fire Field")]
        FIR = 1 << 5,

        /// <summary>
        /// Fire Field (Story)
        /// </summary>
        [Description("Fire Field (Story)")]
        FIR_S = FIR + isStory,

        /// <summary>
        /// Green Plant
        /// </summary>
        [Description("Green Plant")]
        FOR = 1 << 6,

        /// <summary>
        /// Lightning
        /// </summary>
        [Description("Lightning")]
        LIG = 1 << 7,

        /// <summary>
        /// Mute City
        /// </summary>
        [Description("Mute City")]
        MUT = 1 << 8,

        /// <summary>
        /// Outer Space
        /// </summary>
        [Description("Outer Space")]
        MET = 1 << 9,

        /// <summary>
        /// Undefined ID
        /// </summary>
        [Description("NULL")]
        None = 0,

        /// <summary>
        /// Port Town
        /// </summary>
        [Description("Port Town")]
        POR = 1 << 10,

        /// <summary>
        /// Port Town (Story)
        /// </summary>
        [Description("Port Town (Story)")]
        POR_S = POR + isStory,

        /// <summary>
        /// Phantom Road
        /// </summary>
        /// <remarks>RAI for Rainbow Road</remarks>
        [Description("Phantom Road")]
        RAI = 1 << 11,

        /// <summary>
        /// Sand Ocean
        /// </summary>
        [Description("Sand Ocean")]
        SAN = 1 << 12,

        /// <summary>
        /// Sand Ocean (Story)
        /// </summary>
        [Description("Sand Ocean (Story)")]
        SAN_S = SAN + isStory,

        /// <summary>
        /// Aeropolis
        /// </summary>
        [Description("Aeropolis")]
        TOW = 1 << 13,

        /// <summary>
        /// Victory Lap
        /// </summary>
        [Description("Victory Lap")]
        WIN = 1 << 14,

        /// <summary>
        /// Grand Prix Podium
        /// </summary>
        [Description("Grand Prix Podium")]
        WIN_GX = 1 << 15,

        /// <summary>
        /// Flag for story mode missions
        /// </summary>
        [Description("Flag: Is Story")]
        isStory = 1 << 31,
    }
}
