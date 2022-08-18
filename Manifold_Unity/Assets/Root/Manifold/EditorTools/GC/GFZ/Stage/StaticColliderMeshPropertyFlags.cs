using GameCube.GFZ.Stage;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public enum StaticColliderMeshPropertyFlags
    {
        driveableWithCamera = 1 << StaticColliderMeshProperty.driveable,
        driveableNoCamera = 1 << StaticColliderMeshProperty.wall,
        recover = 1 << StaticColliderMeshProperty.recover,
        damage = 1 << StaticColliderMeshProperty.damage,
        slip = 1 << StaticColliderMeshProperty.ice,
        dirt = 1 << StaticColliderMeshProperty.dirt,
        dash = 1 << StaticColliderMeshProperty.dash,
        jump = 1 << StaticColliderMeshProperty.jump, outOfBounds = 1 << StaticColliderMeshProperty.outOfBounds,
        deathCollider = 1 << StaticColliderMeshProperty.deathGround,
        deathTrigger = (1 << StaticColliderMeshProperty.death1) + (1 << StaticColliderMeshProperty.death2),
        unknown = (1 << StaticColliderMeshProperty.death3) + (1 << StaticColliderMeshProperty.death4),
        unknownAndDeathTrigger = unknown + deathTrigger,
    }
}
