using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayVisualEffectTrigger : MonoBehaviour, IColiCourseDisplayable
    {
        public ColiSceneSobj sceneSobj;
        public int fontSize = 32;
        public Color color = new Color32(255, 0, 0, 127);

        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }

        private void OnDrawGizmos()
        {
            int index = 0;
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var style = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = fontSize,
            };

            Gizmos.color = color;
            foreach (var item in sceneSobj.Value.visualEffectTriggers)
            {
                index++;

                var transform = item.transform;
                Gizmos.DrawMesh(mesh, 0, transform.Position, transform.Rotation, transform.Scale);
                Gizmos.DrawWireMesh(mesh, 0, transform.Position, transform.Rotation, transform.Scale);
                UnityEditor.Handles.Label(transform.Position, $"idx:{index}, vfx:{(int)item.visualEffect:X4}", style);
            }
        }

    }
}
