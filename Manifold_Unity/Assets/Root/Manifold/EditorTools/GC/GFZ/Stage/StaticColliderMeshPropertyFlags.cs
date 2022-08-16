using GameCube.GFZ.Stage;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [System.Flags]
    public enum StaticColliderMeshPropertyFlags
    {
        driveable = 1 << StaticColliderMeshProperty.driveable,
        recover = 1 << StaticColliderMeshProperty.recover,
        wall = 1 << StaticColliderMeshProperty.wall,
        dash = 1 << StaticColliderMeshProperty.dash,
        jump = 1 << StaticColliderMeshProperty.jump,
        slip = 1 << StaticColliderMeshProperty.ice,
        dirt = 1 << StaticColliderMeshProperty.dirt,
        damage = 1 << StaticColliderMeshProperty.damage,
        outOfBounds = 1 << StaticColliderMeshProperty.outOfBounds,
        deathGround = 1 << StaticColliderMeshProperty.deathGround,
        death1 = 1 << StaticColliderMeshProperty.death1,
        death2 = 1 << StaticColliderMeshProperty.death2,
        death3 = 1 << StaticColliderMeshProperty.death3,
        death4 = 1 << StaticColliderMeshProperty.death4,
    }
}
