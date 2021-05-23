using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.IO
{
    public static class MeshUtility
    {
        public static int[] GetTrianglesFromTriangleStrip(int numVerts, bool baseCCW)
        {
            // Construct triangles from GameCube GX TRIANGLE_STRIP
            // For one, note that we need to unwind the tristrip.
            // We can use the index to know if the indice is odd or even.
            // However, in GFZ, the winding for different display lists
            // inverts based on it's "index," so to speak.
            // To compensate, we need to XOR the odd/even value with whether
            // the base index of the strip is meant to be CCW or CW.

            const int vertexStride = 3;

            var nTriangles = numVerts - 2;
            int[] triangles = new int[nTriangles * vertexStride];
            for (int i = 0; i < nTriangles; i++)
            {
                var triIndex = i * vertexStride;
                var indexIsCW = (i % 2) > 0;
                var isCCW = (baseCCW ^ indexIsCW);

                if (isCCW)
                {
                    triangles[triIndex + 0] = i + 0;
                    triangles[triIndex + 1] = i + 1;
                    triangles[triIndex + 2] = i + 2;
                }
                else
                {
                    triangles[triIndex + 0] = i + 0;
                    triangles[triIndex + 1] = i + 2;
                    triangles[triIndex + 2] = i + 1;
                }
            }

            return triangles;
        }

        public static int[] GetTrianglesFromTriangleStrip(int[] indexes)
        {
            // HACK
            if (indexes.Length < 3)
                return new int[0];

            const int vertexStride = 3;

            var totalTriangles = indexes.Length - 2;
            int[] triangleIndexes = new int[totalTriangles * vertexStride];

            for (int i = 0; i < totalTriangles; i++)
            {
                var stride = i * vertexStride;

                triangleIndexes[stride + 0] = indexes[i + 0];
                triangleIndexes[stride + 1] = indexes[i + 1];
                triangleIndexes[stride + 2] = indexes[i + 2];

                //    triangles[triIndex + 0] = i + 0;
                //    triangles[triIndex + 1] = i + 2;
                //    triangles[triIndex + 2] = i + 1;
            }

            return triangleIndexes;
        }

    }
}
