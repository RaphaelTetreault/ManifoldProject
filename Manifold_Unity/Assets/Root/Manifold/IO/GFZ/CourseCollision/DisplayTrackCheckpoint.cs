using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayTrackCheckpoint : MonoBehaviour, IColiCourseDisplayable
    {
        public float debugSize = 1f;

        [SerializeField] protected ColiSceneSobj sceneSobj;
        [SerializeField] protected Color[] debugColor = new Color[] { Color.black, Color.red, Color.blue, Color.green };

        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }

        private void OnDrawGizmos()
        {
            if (sceneSobj == null)
                return;

            var mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Root/Resources/normal-cylinder-16-hollowed.fbx");

            foreach (var node in sceneSobj.Value.trackNodes)
            {
                for (int i = 0; i < node.points.Length; i++)
                {
                    var point = node.points[i];
                    var from = point.positionStart;
                    var to = point.positionEnd;
                    var halfWidth = point.trackWidth / 2f;
                    var scaleFrom = new Vector3(halfWidth, halfWidth, 1f);
                    var scaleTo = Vector3.one * 2f * debugSize;

                    Gizmos.color = debugColor[i];
                    Gizmos.DrawLine(from, to);
                    Gizmos.DrawMesh(mesh, 0, from, Quaternion.LookRotation(point.tangentStart), scaleFrom);
                    Gizmos.DrawWireMesh(mesh, 0, to, Quaternion.LookRotation(point.tangentEnd), scaleTo);
                }
            }
        }
    }
}