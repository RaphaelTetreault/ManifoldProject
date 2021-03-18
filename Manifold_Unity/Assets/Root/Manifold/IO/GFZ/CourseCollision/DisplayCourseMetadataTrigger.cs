using UnityEngine;
using GameCube.GFZ.CourseCollision;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayCourseMetadataTrigger : MonoBehaviour
    {
        public ColiSceneSobj sceneSobj;
        public float size = 15f;
        public Color color = Color.green;

        private void OnDrawGizmos()
        {
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            Gizmos.color = new Color32(255, 255, 0, 127);
            foreach (var item in sceneSobj.Value.courseMetadataTriggers)
            {
                var defaultScale = Vector3.one * size;
                var isPath =
                    item.metadata == CourseMetadataFlag.Lightning ||
                    item.metadata == CourseMetadataFlag.OuterSpace;

                if (isPath)
                {
                    Gizmos.DrawMesh(mesh, 0, item.PositionFrom, item.rotation, defaultScale);
                    Gizmos.DrawWireMesh(mesh, 0, item.PositionTo, item.rotation, defaultScale);
                    Gizmos.DrawLine(item.PositionFrom, item.PositionTo);
                }
                else
                {
                    // size = 10 like other triggers?
                    // BBO seems to work well with 27.5f
                    Gizmos.DrawMesh(mesh, 0, item.position, item.rotation, item.Scale * size);
                }
            }
        }

    }
}
