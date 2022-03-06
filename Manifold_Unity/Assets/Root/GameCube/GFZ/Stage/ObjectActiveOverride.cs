// NOTE: this is really flags, but constants are more meaningful.

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Parameter which appears to forceably enable/disable certain objects.
    /// The primary use seems to be enabling mines in Story 8.
    /// </summary>
    public enum ObjectActiveOverride : byte
    {
        /// <summary>
        /// No override.
        /// </summary>
        None = 0b_00000000,

        /// <summary>
        /// Select to activate object in Normal, Hard, and Very Hard missions
        /// </summary>
        ActiveMissionNormal = 0b_00000111,

        /// <summary>
        /// Select to activate object only in Hard and Very Hard missions
        /// </summary>
        ActiveMissionHard = 0b_00000110,

        /// <summary>
        /// Select to activate object only in Very Hard missions
        /// </summary>
        ActiveMissionVeryHard = 0b_00000100,

        /// <summary>
        /// TODO: SOLVE THIS.
        /// Appears on object primarily with collision relating to either
        /// mines, walls, or out-of-bounds / death planes.
        /// </summary>
        Unknown = 0b_00001111,
    }
}
