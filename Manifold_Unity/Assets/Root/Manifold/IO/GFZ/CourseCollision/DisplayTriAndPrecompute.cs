using UnityEngine;
using static Unity.Mathematics.math;

namespace Manifold.IO.GFZ.CourseCollision
{
    using Unity.Mathematics;

    public class DisplayTriAndPrecompute : MonoBehaviour, IColiCourseDisplayable
    {
        public int index;
        public int objectIndex;
        public float rot = 0f;
        public float scale = 1f;
        public float power = 2f;

        public ColiSceneSobj sceneSobj;
        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }

        private void OnDrawGizmos()
        {
            const float size = 50000f;
            Debug.DrawLine(Vector3.right * size, -Vector3.right * size, Color.red);
            Debug.DrawLine(Vector3.up * size, -Vector3.up * size, Color.green);
            Debug.DrawLine(Vector3.forward * size, -Vector3.forward * size, Color.blue);

            var scene = sceneSobj.Value;
            var sceneObject = scene.sceneObjects[objectIndex];
            var colliderGeo = sceneObject.instanceReference.colliderGeometry;

            index = Mathf.Clamp(index, 0, colliderGeo.triCount - 1);

            //var tri = colliderGeo.tris[index];
            var tri = colliderGeo.quads[index];
            var verts = tri.GetVerts();
            var vertCenter = tri.VertCenter();
            var precomputes = tri.GetPrecomputes();
            var precomputeCenter = tri.PrecomputeCenter();
            var normal = tri.normal;

            var rotation = Quaternion.AngleAxis(rot, normal);

            float3 v0 = verts[0] - vertCenter;
            float3 v1 = verts[1] - vertCenter;
            float3 v2 = verts[2] - vertCenter;
            float3 v3 = verts[3] - vertCenter;
            //v0 *= scale;
            //v1 *= scale;
            //v2 *= scale;
            //v3 *= scale;


            //// Given the point P, returns it's coordinates
            //float3 p = float3.zero;
            //float areaABC = dot(normal, cross(v1 - v0, v2 - v0));
            //float areaPBC = dot(normal, cross(v1 - p, v2 - p));
            //float areaPCA = dot(normal, cross(v2 - p, v0 - p));
            //float bx = areaPBC / areaABC;
            //float by = areaPCA / areaABC;
            //float bz = 1f - bx - by;
            //Debug.Log($"bx:{bx}, by:{by}, bz:{bz}");

            //var myVerts = new Vector3[] { v0, v1, v2 };
            var myVerts = new Vector3[] { v0, v1, v2, v3 };
            var mv0 = Vector3.ProjectOnPlane(verts[0] - vertCenter, normal);
            var mv1 = Vector3.ProjectOnPlane(verts[1] - vertCenter, normal);
            var mv2 = Vector3.ProjectOnPlane(verts[2] - vertCenter, normal);
            var mv3 = Vector3.ProjectOnPlane(verts[3] - vertCenter, normal);

            float3 mx0 = mv1 - mv0;
            float3 mx1 = mv2 - mv1;
            float3 mx2 = mv3 - mv2;
            float3 mx3 = mv0 - mv3;

            var mv = new Vector3[] { mv0, mv1, mv2, mv3 };
            var mx = new Vector3[] { mx0, mx1, mx2, mx3 };

            //var lengthPre01 = Vector3.Distance(precomputes[0], precomputes[1]);
            //var lengthPre12 = Vector3.Distance(precomputes[1], precomputes[2]);
            //var lengthPre20 = Vector3.Distance(precomputes[2], precomputes[0]);
            //Debug.Log($"Precompute sides 01:{lengthPre01}, 12:{lengthPre12}, 20:{lengthPre20}");

            //var x = ((Vector3)precomputes[0]).magnitude;
            //var y = ((Vector3)precomputes[1]).magnitude;
            //var z = ((Vector3)precomputes[2]).magnitude;
            //Debug.Log($"Precompute dist 0: {x}, 1: {y}, 2: {z}");
            ////var lengthMy01 = Vector3.Distance(myVert0, myVert1) / lengthPre01;
            ////var lengthMy12 = Vector3.Distance(myVert1, myVert2) / lengthPre12;
            ////var lengthMy20 = Vector3.Distance(myVert2, myVert0) / lengthPre20;
            ////Debug.Log($"My verts: 01:{lengthMy01}, 12:{lengthMy12}, 20:{lengthMy20}");

            //var a = ((Vector3)myVert0).magnitude / ((Vector3)precomputes[0]).magnitude;
            //var b = ((Vector3)myVert1).magnitude / ((Vector3)precomputes[1]).magnitude;
            //var c = ((Vector3)myVert2).magnitude / ((Vector3)precomputes[2]).magnitude;
            //Debug.Log($"Ratio 0: {a}, 1: {b}, 2: {c}");

            int vertLength = verts.Length;
            for (int i = 0; i < vertLength; i++)
            {
                int curr = i;
                int next = (i + 1) % vertLength;
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(verts[curr], verts[next]);
                Gizmos.DrawLine(myVerts[curr], myVerts[next]);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(mv[curr], mv[next]);
                //Gizmos.color = Color.yellow;
                //Gizmos.DrawLine(mx[curr], mx[next]);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(precomputes[curr], precomputes[next]);
                Gizmos.DrawLine(rotation* precomputes[curr]/scale, rotation * precomputes[next]/scale);
            }
            Gizmos.DrawLine(precomputeCenter, precomputeCenter + normal * 10f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(vertCenter, vertCenter + normal * 10f);
        }
    }
}
