namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    public enum StaticMeshColliderProperty
    {
        driveable,
        recover,
        wall,
        boost,
        jump,
        ice,
        dirt,
        damage,
        outOfBounds,
        /// <summary>
        /// Used to tag ground collider such as Sand Ocean sand.
        /// GX behaviour: instant kill.
        /// AX behaviour: Health set to 0, vehicle bounces of of it as if wall.
        /// </summary>
        deathGround,
        death1,
        death2,
        death3,
        death4,
    }
}
