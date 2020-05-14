using GameCube.GFZX01.CourseCollision;
using UnityEditor;
using UnityEngine;

namespace Manifold.IO.GFZX01.CourseCollision
{
    public class TempTrackVis : MonoBehaviour
    {
        public float kRadius = 10f;

        [SerializeField] protected ColiSceneSobj coli;
        [SerializeField] protected Color debugColor = Color.white;
        [SerializeField] protected Color transformColor = Color.white;
        [SerializeField] protected Mesh mesh;

        public ColiSceneSobj Coli => coli;

        private void OnDrawGizmos()
        {
            if (coli == null)
                return;

            Gizmos.color = debugColor;
            foreach (var node in coli.Value.trackNodes)
            {
                var from = node.point.positionStart;
                var to = node.point.positionEnd;
                Gizmos.DrawLine(from, to);
                Gizmos.DrawWireSphere(to, kRadius);
                Gizmos.DrawSphere(from, kRadius);
            }

            Gizmos.color = transformColor;
            foreach (var trackTransform in coli.Value.trackTransforms)
            {
                DrawRecursive(trackTransform, Quaternion.identity, Vector3.zero, Vector3.one);
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

    [CustomEditor(typeof(TempTrackVis))]
    public class TempTrackVisEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Import As Objects"))
            {
                var editorTarget = target as TempTrackVis;

                var root = new UnityEngine.GameObject();
                root.name = editorTarget.Coli.name;

                foreach (var transform in editorTarget.Coli.Value.trackTransforms)
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
            var obj = new UnityEngine.GameObject();
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
