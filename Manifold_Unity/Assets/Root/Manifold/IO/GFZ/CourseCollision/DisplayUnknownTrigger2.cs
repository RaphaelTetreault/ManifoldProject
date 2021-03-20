using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayUnknownTrigger2 : MonoBehaviour, IColiCourseDisplayable
    {
        public ColiSceneSobj sceneSobj;
        public float size = 15f;
        public Color color = new Color32(255, 0, 255, 127);

        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }

        private void OnDrawGizmos()
        {
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            Gizmos.color = color;
            foreach (var item in sceneSobj.Value.unknownTrigger2s)
            {
                var transform = item.transform;
                Gizmos.DrawMesh(mesh, 0, transform.Position, transform.Rotation, transform.Scale * size);
                Gizmos.DrawWireMesh(mesh, 0, transform.Position, transform.Rotation, transform.Scale * size);
            }
        }

    }
}
