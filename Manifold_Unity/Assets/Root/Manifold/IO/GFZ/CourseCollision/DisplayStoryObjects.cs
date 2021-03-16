using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayStoryObjects : MonoBehaviour, IColiCourseDisplayable
    {
        [SerializeField]
        private ColiSceneSobj sceneSobj;
        public float size = 15f;

        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }



        private void OnDrawGizmos()
        {
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            foreach (var item in sceneSobj.Value.storyObjects)
            {
                var scale = item.scale * size;
                if (item.Difficulty == 0)
                {
                    Gizmos.color = Color.green;
                    scale += Vector3.one * size;
                }
                else if (item.Difficulty == 1)
                {
                    Gizmos.color = Color.yellow;
                    scale += Vector3.one * size / 2f;
                }
                else if (item.Difficulty == 2)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.magenta;
                }

                var rotation = Quaternion.Euler(item.rotation);
                Gizmos.DrawMesh(mesh, 0, item.position, rotation, scale);
            }
        }
    }
}
