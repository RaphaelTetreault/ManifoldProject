namespace GameCube.GFZ.Stage
{
    // TODO: confirm indices for AX stages.

    /// <summary>
    /// Indicates which property a StaticColliderMeshMatrix represents. F-Zero AX and
    /// F-Zero GX differ in the higher end.
    /// </summary>
    public enum StaticColliderMeshProperty
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
