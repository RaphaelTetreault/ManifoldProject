using GameCube.GFZ.CourseCollision;
using UnityEditor;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayTrackCheckpoint : MonoBehaviour, IColiCourseDisplayable
    {
        public float kRadius = 10f;

        [SerializeField] protected ColiSceneSobj sceneSobj;
        [SerializeField] protected Color[] debugColor = new Color[] { Color.black, Color.red, Color.blue, Color.green };
        //[SerializeField] protected Color transformColor = Color.white;
        [SerializeField] protected Mesh mesh;

        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }

        private void OnDrawGizmos()
        {
            if (sceneSobj == null)
                return;

            foreach (var node in sceneSobj.Value.trackNodes)
            {
                for (int i = 0; i < node.points.Length; i++)
                {
                    var point = node.points[i];
                    Gizmos.color = debugColor[i];

                    var from = point.positionStart;
                    var to = point.positionEnd;
                    Gizmos.DrawLine(from, to);
                    Gizmos.DrawWireSphere(to, kRadius);
                    Gizmos.DrawSphere(from, kRadius);
                }
            }
        }

        public void DrawRecursive(TrackTransform trackTransform, Quaternion parentRotation, Vector3 parentPosition, Vector3 parentScale)
        {
            if (mesh != null)
            {
                // Calc position
                var position = Vector3.zero;
                position += parentRotation * trackTransform.localPosition;
                //position.x *= parentScale.x;
                //position.y *= parentScale.y;
                //position.z *= parentScale.z;
                position += parentPosition;

                // Calc scale
                var scale = trackTransform.localScale;
                //scale.x *= Mathf.Abs(parentScale.x);
                //scale.y *= Mathf.Abs(parentScale.y);
                //scale.z *= Mathf.Abs(parentScale.z);

                // Calc rotation
                var rotation = Quaternion.Euler(trackTransform.localRotation);
                rotation *= parentRotation;

                Gizmos.DrawWireMesh(mesh, position, rotation, scale);

                foreach (var child in trackTransform.children)
                {
                    DrawRecursive(child, rotation, position, scale);
                }
            }

            foreach (var param in trackTransform.topologyParameters.Params())
            {

            }
        }


    }

    [CustomEditor(typeof(DisplayTrackCheckpoint))]
    public class TempTrackVisEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Import As Objects"))
            {
                var editorTarget = target as DisplayTrackCheckpoint;

                var root = new GameObject();
                root.name = editorTarget.SceneSobj.name;

                foreach (var transform in editorTarget.SceneSobj.Value.trackTransforms)
                {
                    CreateTransform(transform, root.transform);
                }
            }

            base.OnInspectorGUI();
        }

        public void CreateTransform(TrackTransform track, UnityEngine.Transform parent)
        {
            var pos = track.localPosition;
            var rot = Quaternion.Euler(track.localRotation);
            var sca = track.localScale;
            var obj = new GameObject();
            var comp = obj.AddComponent<TempDebugEmpty>();
            //comp.track = track;

            obj.transform.localPosition = pos;
            obj.transform.localRotation = rot;
            //obj.transform.localScale = sca;
            obj.transform.parent = parent;

            foreach (var child in track.children)
            {
                CreateTransform(child, obj.transform);
            }
        }
    }
}
