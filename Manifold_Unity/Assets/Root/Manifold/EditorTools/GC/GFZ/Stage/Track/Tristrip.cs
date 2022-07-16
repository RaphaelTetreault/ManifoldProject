using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class Tristrip
    {
        public Vector3[] positions { get; set; }
        public Vector3[] normals { get; set; }
        public Vector3[] binormals { get; set; }
        public Vector3[] tangents { get; set; }
        public Vector2[] uv0 { get; set; }
        public Vector2[] uv1 { get; set; }
        public Vector2[] uv2 { get; set; }
        public Color32[] color0 { get; set; }
        public Color32[] color1 { get; set; }
        public bool reverse { get; set; }


        public int TrianglesCount => ((positions.Length / 2) - 1) * 6;

        public int[] GetIndices()
        {
            int nTriangles = TrianglesCount;
            int[] indexes = new int[nTriangles];

            // Process 1 triangle at a time
            int vertexBaseIndex = 0;
            for (int i = 0; i < indexes.Length; i += 6)
            {
                indexes[i + 0] = vertexBaseIndex + 0;
                indexes[i + 1] = vertexBaseIndex + 1;
                indexes[i + 2] = vertexBaseIndex + 2;
                vertexBaseIndex++;
                indexes[i + 3] = vertexBaseIndex + 0;
                indexes[i + 4] = vertexBaseIndex + 2;
                indexes[i + 5] = vertexBaseIndex + 1;
                vertexBaseIndex++;
            }

            if (reverse)
                indexes = indexes.Reverse().ToArray();

            return indexes;
        }
    }
}
