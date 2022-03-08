namespace GameCube.GFZ.Stage
{
    // TODO: confirm indices for AX stages.

    /// <summary>
    /// Indicates which collision property a StaticColliderMeshGrid represents.
    /// F-Zero AX and F-Zero GX differ in the higher end.
    /// </summary>
    public enum StaticColliderMeshProperty
    {
        /// <summary>
        /// Indicate the <cref>StaticColliderMeshGrid</cref> mesh is a driveable surface.
        /// </summary>
        driveable,

        /// <summary>
        /// Indicate the <cref>StaticColliderMeshGrid</cref> mesh is a (recovery) pit.
        /// </summary>
        recover,

        /// <summary>
        /// Indicate the <cref>StaticColliderMeshGrid</cref> mesh is a wall / undriveable surface.
        /// </summary>
        wall,

        /// <summary>
        /// Indicate the <cref>StaticColliderMeshGrid</cref> mesh is a dash plate.
        /// </summary>
        boost, // TODO: rename as dash. Will break scene gen based on name.

        /// <summary>
        /// Indicate the <cref>StaticColliderMeshGrid</cref> mesh is a jump plate.
        /// </summary>
        jump,

        /// <summary>
        /// Indicate the <cref>StaticColliderMeshGrid</cref> mesh is an ice surface.
        /// </summary>
        ice,

        /// <summary>
        /// Indicate the <cref>StaticColliderMeshGrid</cref> mesh is a dirt surface.
        /// </summary>
        dirt,

        /// <summary>
        /// Indicate the <cref>StaticColliderMeshGrid</cref> mesh is a damage area.
        /// This could be a lava surface, a lazer beam (LHP), etc.
        /// </summary>
        damage,

        /// <summary>
        /// Indicate the <cref>StaticColliderMeshGrid</cref> mesh is an out-of-bounds plane.
        /// </summary>
        outOfBounds,

        /// <summary>
        /// Indicate the <cref>StaticColliderMeshGrid</cref> mesh is an ground plane.
        /// This is used for Sand Ocean's sand colliders.
        /// </summary>
        /// <remarks>
        /// GX behaviour: instant kill.
        /// AX behaviour: health set to 0, vehicle bounces off of it as if wall.
        /// </remarks>
        deathGround,

        /// <summary>
        /// 
        /// </summary>
        death1,

        /// <summary>
        /// 
        /// </summary>
        death2,

        /// <summary>
        /// 
        /// </summary>
        death3,

        /// <summary>
        /// 
        /// </summary>
        death4,
    }
}
