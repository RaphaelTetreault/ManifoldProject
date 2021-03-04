
namespace GameCube.GFZ.CourseCollision
{
    public enum CollisionProperty : byte
    {
        /// <summary>
        /// Drivable surface collision type.
        /// </summary>
        Road,
        /// <summary>
        /// Recharge areas collision type.
        /// </summary>
        Recover,
        /// <summary>
        /// Possibly Out-Of-Bounds designation?
        /// </summary>
        Unknown2,
        /// <summary>
        /// Wall (impassable) collision type.
        /// </summary>
        Wall,
        /// <summary>
        /// Boost pad collision type.
        /// </summary>
        Speed,
        /// <summary>
        /// Jump pad collision type.
        /// </summary>
        Jump,
        /// <summary>
        /// Slip area (ice) collision type.
        /// </summary>
        Slip,
        /// <summary>
        /// Dirt area collision type.
        /// </summary>
        Dirt,
        /// <summary>
        /// Mine collision type. (TODO: know if this disappears after first contact.)
        /// </summary>
        Mine,
        /// <summary>
        /// Lava area collision type. (TODO: confirm. Old models use "InvG" notation.)
        /// </summary>
        Lava_InvG,
    }
}
