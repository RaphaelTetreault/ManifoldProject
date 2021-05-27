using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Manifold
{
    public class FzxFzepBezier : MonoBehaviour
    {
        public List<Transform> nodes = new List<Transform>();

        [ContextMenu("OnDrawGizmos")]
        private void OnDrawGizmos()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                int curr = i;
                int next = (i + 1) % nodes.Count;

                var nodeCurr = nodes[curr];
                var nodeNext = nodes[next];

                //var direction = nodeNext.position - nodeCurr.position;

                //Handles.DrawBezier(nodeCurr.position, nodeNext.position, direction, -direction, Color.red, null, 5f);

                var posCurr = nodeCurr.position;
                var posNext = nodeNext.position;
                var handleCurr = Vector3.Lerp(posCurr, posNext, 1f/3f);

                Gizmos.DrawSphere(posCurr, 10f);
                Gizmos.DrawSphere(handleCurr, 10f);

                var points = Handles.MakeBezierPoints(posCurr, posNext, handleCurr, handleCurr, 20);
                Handles.DrawAAPolyLine(points);
            }
        }
    }
}
