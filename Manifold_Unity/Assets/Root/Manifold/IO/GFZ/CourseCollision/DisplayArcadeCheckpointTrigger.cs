using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
    {
        public class DisplayArcadeCheckpointTrigger : MonoBehaviour, IColiCourseDisplayable
        {
            public ColiSceneSobj sceneSobj;
            public Color color = new Color32(0, 0, 255, 127);

            public ColiSceneSobj SceneSobj
            {
                get => sceneSobj;
                set => sceneSobj = value;
            }

            private void OnDrawGizmos()
            {
                var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
                Gizmos.color = color;
                foreach (var item in sceneSobj.Value.arcadeCheckpointTriggers)
                {
                    var transform = item.transform;
                    Gizmos.DrawMesh(mesh, 0, transform.Position, transform.Rotation, transform.Scale * 10f);
                    Gizmos.DrawWireMesh(mesh, 0, transform.Position, transform.Rotation, transform.Scale * 10f);
                }
            }

        }
    }
