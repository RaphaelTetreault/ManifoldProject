namespace GameCube.GFZ.CourseCollision
{
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
        deathNoDestroy, //Insta-Kill Collider. In AX, it would kill you but you'd skip/recochet off of it
        death1,
        death2,
        death3,
        death4,
    }
}
