using GameCube.GFZ.Stage;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzStaticColliderMesh : MonoBehaviour
    {
        [field: SerializeField] public StaticColliderMeshPropertyFlags ColliderType { get; set; } = StaticColliderMeshPropertyFlags.dash;
        [field: SerializeField] public MeshFilter MeshFilter { get; protected set; }
        [field: SerializeField] public bool ExportEntireMesh { get; protected set; } = true;
        [field: SerializeField] public int SubmeshIndex { get; protected set; }


        public ColliderTriangle[] CreateColliderTriangles()
        {
            var mesh = MeshFilter.sharedMesh;
            var triangles = new ColliderTriangle[mesh.triangles.Length / 3];
            var matrix = GetGfzMatrix();
            for (int i = 0; i < triangles.Length; i++)
            {
                int i0 = i * 3;
                int i1 = i0 + 1;
                int i2 = i0 + 2;
                int v0 = mesh.triangles[i0];
                int v1 = mesh.triangles[i1];
                int v2 = mesh.triangles[i2];

                var triangle = new ColliderTriangle();
                triangle.Vertex0 = matrix.MultiplyPoint(mesh.vertices[v0]);
                triangle.Vertex1 = matrix.MultiplyPoint(mesh.vertices[v1]);
                triangle.Vertex2 = matrix.MultiplyPoint(mesh.vertices[v2]);
                triangle.Update();

                triangles[i] = triangle;
            }

            return triangles;
        }

        public void CreateCollider(out ColliderTriangle[] triangles, out ColliderQuad[] quads)
        {
            triangles = CreateColliderTriangles();
            quads = new ColliderQuad[0];
        }

        public void CreateColliderOptimized(out ColliderTriangle[] triangles, out ColliderQuad[] quads)
        {
            var mesh = MeshFilter.sharedMesh;
            var trisList = new List<ColliderTriangle>();
            var quadsList = new List<ColliderQuad>();
            var matrix = GetGfzMatrix();
            int indexes = mesh.triangles.Length / 3;
            for (int i = 0; i < indexes; i++)
            {
                int i0 = i * 3;
                int i1 = i0 + 1;
                int i2 = i0 + 2;
                int v0 = mesh.triangles[i0];
                int v1 = mesh.triangles[i1];
                int v2 = mesh.triangles[i2];

                var triangle = new ColliderTriangle();
                triangle.Vertex0 = matrix.MultiplyPoint(mesh.vertices[v0]);
                triangle.Vertex1 = matrix.MultiplyPoint(mesh.vertices[v1]);
                triangle.Vertex2 = matrix.MultiplyPoint(mesh.vertices[v2]);
                triangle.Update();
                trisList.Add(triangle);
            }

            // Reverse order, collapse triangles into quad when necessary
            for (int i = indexes-1; i > 0; i--)
            {
                var triangle0 = trisList[i-0];
                var triangle1 = trisList[i-1];
                float sameness = math.dot(triangle0.Normal, triangle1.Normal);
                // TODO: make sure triangles share 2/3 vertices
                if (sameness > 0.995f)
                {
                    var quad = new ColliderQuad();
                    // TODO: select correct vertices to collapse, ensure 4 verts make circle
                    quad.Vertex0 = triangle0.Vertex0;
                    quad.Vertex1 = triangle0.Vertex2;
                    quad.Vertex2 = triangle1.Vertex0;
                    quad.Vertex3 = triangle1.Vertex2;
                    quad.Update();
                    quadsList.Add(quad);

                    if (float.IsNaN(quad.EdgeNormal0.x))
                        throw new System.Exception("Bad assumption on quad collapse.");

                    trisList.Remove(triangle0);
                    trisList.Remove(triangle1);
                    i--;
                }
            }

            triangles = trisList.ToArray();
            quads = quadsList.ToArray();
        }

        public Matrix4x4 GetGfzMatrix()
        {
            // Create matrix
            var position = transform.position;
            var rotation = transform.rotation.eulerAngles;
            var scale = transform.lossyScale;
            ////
            position.z = -position.z;
            rotation.x = -rotation.x;
            rotation.y = -rotation.y;
            ////
            var matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);
            return matrix;
        }

        public static Color32[] DebugColor => new Color32[]
        {
            new Color32(  0,   0,   0, 195), // unhandled
            new Color32(127, 127, 127, 195), // driveable with camera
            new Color32( 95,  95,  95, 195), // driveable no camera
            new Color32(255,  63, 127, 195), // recover
            new Color32(255,  63,  63, 195), // damage
            new Color32( 38, 184, 255, 195), // slip
            new Color32(127,  63,  47, 195), // dirt
            new Color32(255, 255,  38, 195), // dash
            new Color32(255, 180,  60, 195), // jump
            new Color32(194, 114, 255, 195), // out-of-bounds
            new Color32(173, 129,  75, 195), // death collider
            new Color32( 44,  38, 142, 195), // death trigger
            new Color32( 60,  38,  93, 195), // unknown 1
            new Color32( 65,  38,  59, 195), // unknown and death trigger
        };

        public Color32 GetDebugColor()
        {
            int index = ColliderType switch
            {
                StaticColliderMeshPropertyFlags.driveableWithCamera => 1,
                StaticColliderMeshPropertyFlags.driveableNoCamera => 2,
                StaticColliderMeshPropertyFlags.recover => 3,
                StaticColliderMeshPropertyFlags.damage => 4,
                StaticColliderMeshPropertyFlags.slip => 5,
                StaticColliderMeshPropertyFlags.dirt => 6,
                StaticColliderMeshPropertyFlags.dash => 7,
                StaticColliderMeshPropertyFlags.jump => 8,
                StaticColliderMeshPropertyFlags.outOfBounds => 9,
                StaticColliderMeshPropertyFlags.deathCollider => 10,
                StaticColliderMeshPropertyFlags.deathTrigger => 11,
                StaticColliderMeshPropertyFlags.unknown => 12,
                StaticColliderMeshPropertyFlags.unknownAndDeathTrigger => 13,
                _ => 0,
            };

            var color = DebugColor[index];
            return color;
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (MeshFilter == null)
                MeshFilter = GetComponent<MeshFilter>();

            // Maintain appropriate index
            if (MeshFilter != null)
                if (SubmeshIndex > MeshFilter.sharedMesh.subMeshCount)
                    SubmeshIndex = MeshFilter.sharedMesh.subMeshCount - 1;
            if (SubmeshIndex < 0)
                SubmeshIndex = 0;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = GetDebugColor();

            if (ExportEntireMesh)
            {
                Gizmos.DrawMesh(MeshFilter.sharedMesh, transform.position, transform.rotation, transform.localScale);
            }
            else
            {
                Gizmos.DrawMesh(MeshFilter.sharedMesh, SubmeshIndex, transform.position, transform.rotation, transform.localScale);
            }
        }
    }
}
