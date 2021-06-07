using UnityEngine;
using GameCube.GFZ.CourseCollision;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayCourseMetadataTrigger : MonoBehaviour , IColiCourseDisplayable
    {
        public ColiSceneSobj sceneSobj;
        public float size = 15f;
        public Color color = new Color32(255, 255, 0, 127);

        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }

        private void OnDrawGizmos()
        {
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            Gizmos.color = color;
            foreach (var item in sceneSobj.Value.courseMetadataTriggers)
            {
                var defaultScale = Vector3.one * size;
                var isPath =
                    item.courseMetadata == CourseMetadataType.Lightning_Lightning ||
                    item.courseMetadata == CourseMetadataType.OuterSpace_Meteor;

                if (isPath)
                {
                    Gizmos.DrawMesh(mesh, 0, item.PositionFrom, item.Rotation, defaultScale);
                    Gizmos.DrawWireMesh(mesh, 0, item.PositionTo, item.Rotation, defaultScale);
                    Gizmos.DrawLine(item.PositionFrom, item.PositionTo);
                }
                else
                {
                    // size = 10 like other triggers?
                    // BBO seems to work well with 27.5f
                    Gizmos.DrawMesh(mesh, 0, item.Position, item.Rotation, item.Scale * size);
                }
            }
        }

    }
}
