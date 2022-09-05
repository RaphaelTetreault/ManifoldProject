using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class Tristrip
    {
        public Vector3[] positions { get; set; }
        public Vector3[] normals { get; set; }
        public Vector3[] binormals { get; set; }
        public Vector3[] tangents { get; set; }
        public Vector2[] tex0 { get; set; }
        public Vector2[] tex1 { get; set; }
        public Vector2[] tex2 { get; set; }
        public Color32[] color0 { get; set; }
        public bool isBackFacing { get; set; }
        public bool isDoubleSided { get; set; }


        public int TriangleIndexesCount => ((positions.Length / 2) - 1) * 6;
        public int MeshIndexesCount => isDoubleSided ? TriangleIndexesCount * 2 : TriangleIndexesCount;
        public int VertexCount => positions.Length;

        public int[] GetIndices()
        {
            int nTriangles = TriangleIndexesCount;
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

            if (isDoubleSided)
            {
                var invertedIndexes = indexes.Reverse().ToArray();
                indexes = indexes.Concat(invertedIndexes).ToArray();
            }
            else if (isBackFacing)
            {
                indexes = indexes.Reverse().ToArray();
            }

            return indexes;
        }


        public static Tristrip[] Linearize(Tristrip[][] tristripsCollection)
        {
            var allTristrips = new List<Tristrip>();
            foreach (var tristrips in tristripsCollection)
                allTristrips.AddRange(tristrips);
            return allTristrips.ToArray();
        }
    }
}
