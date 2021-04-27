using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class TempLodView : MonoBehaviour, IColiCourseDisplayable
    {
        public ColiSceneSobj sceneSobj;
        public Color color = new Color32(255, 0, 0, 127);
        public bool drawOnSelectedOnly = true;
        public float scale = 10f;
        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }

        private void OnDrawGizmos()
        {
            if (!drawOnSelectedOnly)
                return;

            Draw();
        }

        private void OnDrawGizmosSelected()
        {
            if (drawOnSelectedOnly)
                return;

            Draw();
        }

        private void Draw()
        {
            int index = 0;
            var mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");

            Gizmos.color = color;
            Debug.Log($"Scene objects count: {sceneSobj.Value.sceneObjects.Length}");
            foreach (var item in sceneSobj.Value.sceneObjects)
            {
                index++;

                var gfzTransform = item.transform;
                var lodNear = item.lodNear.radius;
                var lodFar = item.lodFar.radius;
                Gizmos.DrawMesh(mesh, 0, gfzTransform.Position, Quaternion.identity, Vector3.one * lodFar * scale);
                Gizmos.DrawWireMesh(mesh, 0, gfzTransform.Position, Quaternion.identity, Vector3.one * lodNear * scale);
            }
        }
    }
}
