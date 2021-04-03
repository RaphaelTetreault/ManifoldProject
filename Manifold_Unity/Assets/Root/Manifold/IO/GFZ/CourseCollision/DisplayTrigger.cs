using UnityEngine;
using GameCube.GFZ.CourseCollision;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayTrigger : MonoBehaviour, IColiCourseDisplayable
    {
        [SerializeField]
        private ColiSceneSobj sceneSobj;
        private float size = 10f;

        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }

        private void OnDrawGizmos()
        {
            var green = new Color32(0, 255, 0, 127);
            DisplayTriggerArray(sceneSobj.Value.unknownTrigger1s, green);
        }

        public void DisplayTriggerArray(UnknownTrigger1[] unknownTrigger1s, Color32 color, bool drawNumber = false)
        {
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            Gizmos.color = color;

            var style = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 32,
            };

            int count = 0;
            foreach (var item in unknownTrigger1s)
            {
                count++;

                var transform = item.transform;
                Gizmos.DrawMesh(mesh, 0, transform.Position, transform.Rotation, transform.Scale * size);
                Gizmos.DrawWireMesh(mesh, 0, transform.Position, transform.Rotation, transform.Scale * size);
                //if (drawNumber)
                UnityEditor.Handles.Label(transform.Position, count.ToString(), style);
            }
        }
    }
}
