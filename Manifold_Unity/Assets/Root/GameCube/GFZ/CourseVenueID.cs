// TODO: add story mode IDs

namespace GameCube.GFZ
{
    [System.Flags]
    public enum CourseVenueID
    {
        /// <summary>
        /// Unused venue ID
        /// </summary>
        AUR = 1 << 0,
        /// <summary>
        /// Big Blue
        /// </summary>
        BIG = 1 << 1,
        /// <summary>
        /// Big Blue (Story)
        /// </summary>
        BIG_S = BIG + isStory,
        /// <summary>
        /// Casino Palace
        /// </summary>
        CAS = 1 << 2,
        /// <summary>
        /// Mute City Sonic Oval
        /// </summary>
        /// <remarks>COM for Commercial? COM for Combined MUT+CAS?</remarks>
        COM = 1 << 3,
        /// <summary>
        /// Mute City Sonic Oval (Story)
        /// </summary>
        /// <remarks>COM for Commercial? COM for Combined MUT+CAS?</remarks>
        COM_S = COM + isStory,
        /// <summary>
        /// Cosmo Terminal
        /// </summary>
        ELE = 1 << 4,
        /// <summary>
        /// Fire Field
        /// </summary>
        FIR = 1 << 5,
        /// <summary>
        /// Fire Field (Story)
        /// </summary>
        FIR_S = FIR + isStory,
        /// <summary>
        /// Green Plant
        /// </summary>
        FOR = 1 << 6,
        /// <summary>
        /// Lightning
        /// </summary>
        LIG = 1 << 7,
        /// <summary>
        /// Mute City
        /// </summary>
        MUT = 1 << 8,
        /// <summary>
        /// Outer Space
        /// </summary>
        MET = 1 << 9,
        /// <summary>
        /// Undefined ID
        /// </summary>
        None = 0,
        /// <summary>
        /// Port Town
        /// </summary>
        POR = 1 << 10,
        /// <summary>
        /// Port Town (Story)
        /// </summary>
        POR_S = POR + isStory,
        /// <summary>
        /// Phantom Road
        /// </summary>
        /// <remarks>RAI for Rainbow Road</remarks>
        RAI = 1 << 11,
        /// <summary>
        /// Sand Ocean
        /// </summary>
        SAN = 1 << 12,
        /// <summary>
        /// Sand Ocean (Story)
        /// </summary>
        SAN_S = SAN + isStory,
        /// <summary>
        /// Aeropolis
        /// </summary>
        TOW = 1 << 13,
        /// <summary>
        /// Victory Lap
        /// </summary>
        WIN = 1 << 14,
        /// <summary>
        /// Grand Prix Podium
        /// </summary>
        WIN_GX = 1 << 15,

        /// <summary>
        /// Flag for story mode missions
        /// </summary>
        isStory = 1 << 31,
    }
}
