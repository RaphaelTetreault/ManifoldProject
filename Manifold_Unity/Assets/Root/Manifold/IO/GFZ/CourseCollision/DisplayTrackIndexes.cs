using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayTrackIndexes : MonoBehaviour, IColiCourseDisplayable
    {
        [SerializeField]
        private ColiSceneSobj sceneSobj;
        public float size = 1f;

        public Color[] gizmosColor = new Color[]
        {
            new Color32 (255,   0,   0, 128),
            new Color32 (255, 255,   0, 128),
            new Color32 (  0, 255,   0, 128),
            new Color32 (  0, 255, 255, 128),
            new Color32 (  0,   0, 255, 128),
            new Color32 (255,   0, 255, 128),
        };

        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }


        private void OnDrawGizmos()
        {
            //var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            int colorIndex = 0;
            var trackNodes = sceneSobj.Value.trackNodes;

            foreach (var indexList in sceneSobj.Value.trackCheckpointTable8x8.indexLists)
            {
                Gizmos.color = gizmosColor[colorIndex];
                for (int i = 0; i < indexList.Length - 1; i++)
                {
                    var offset = new Unity.Mathematics.float3(1, 1, 1) * colorIndex;
                    int curr = indexList.Indexes[i + 0];
                    int next = indexList.Indexes[i + 1];
                    var currNode = trackNodes[curr];
                    var nextNode = trackNodes[next];


                    for (int j = 0; j < 1; j++) //currNode.points.Length; j++)
                    {
                        //if (currNode.points.Length > nextNode.points.Length)
                        //    break;

                        var start = currNode.points[j].positionStart + offset;
                        var end = nextNode.points[j].positionEnd + offset;
                        Gizmos.DrawLine(start, end);
                        Gizmos.DrawSphere(start, 5f);
                    }
                }
                colorIndex = (colorIndex + 1) % gizmosColor.Length;
            }
        }
    }
}

