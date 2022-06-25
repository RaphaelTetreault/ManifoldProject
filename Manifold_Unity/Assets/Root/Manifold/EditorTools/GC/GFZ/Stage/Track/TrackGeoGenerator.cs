using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold;
using Manifold.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class TempMesh
    {
        public Vector3[] vertexes;
        public Vector3[] normals;
        public int[] indexes;
    }


    public static class TrackGeoGenerator
    {
        private static readonly Vector3 left = new Vector3(-1, 0, 0);
        private static readonly Vector3 right = new Vector3(1, 0, 0);

        public static TempMesh GenerateTopSurface(GfzTrackSegment trackSegment, float minDistance, int widthSamples = 4)
        {
            var mtx = trackSegment.transform.localToWorldMatrix;
            //var maxTime = trackSegment.AnimationCurveTRS.GetMaxTime();
            var segmentLength = trackSegment.AnimationCurveTRS.GetDistanceBetweenRepeated(0, 1);
            float step = segmentLength / minDistance;
            int lengthSamples = (int)math.ceil(step);

            Vector3[] vertexes = new Vector3[((lengthSamples + 1) * (widthSamples + 1))];
            Vector3[] normals = new Vector3[vertexes.Length];

            for (int l = 0; l <= lengthSamples; l++)
            {
                double samplePoint = l / (double)lengthSamples;
                var animMtx = trackSegment.AnimationCurveTRS.EvaluateMatrix(samplePoint);
                var sampleMtx = mtx * animMtx;

                var position = sampleMtx.Position();
                var rotation = sampleMtx.Rotation();
                var scale = sampleMtx.Scale();

                var width = scale.x;
                var halfWidth = width / 2;
                var beginLeft = position + rotation * (left * halfWidth);
                var directionRight = rotation * right;

                for (int w = 0; w <= widthSamples; w++)
                {
                    var percent = w / (float)widthSamples;
                    var offsetRight = directionRight * percent * width;
                    var vertexPos = beginLeft + offsetRight;

                    int index = l * (widthSamples + 1) + w;
                    vertexes[index] = vertexPos;

                    //
                    normals[index] = rotation * Vector3.up;
                }
            }

            int divsZ = lengthSamples;
            int divsX = widthSamples;
            int strideX = divsX + 1;
            int nIndexes = vertexes.Length * 3 * 2; // 3 indexes per tri, 2 tris per quad
            int[] indexes = new int[nIndexes];
            int currIndex = 0;
            for (int x = 0; x < divsX; x++)
            {
                for (int z = 0; z < divsZ; z++)
                {
                    // Create a quad along the path in the Z/forward direction
                    int baseIndex = x + (z * strideX);
                    int v0 = baseIndex + 0;
                    int v1 = baseIndex + 1;
                    int v2 = v0 + strideX;
                    int v3 = v1 + strideX;

                    indexes[currIndex++] = v0;
                    indexes[currIndex++] = v1;
                    indexes[currIndex++] = v2;
                    indexes[currIndex++] = v2;
                    indexes[currIndex++] = v1;
                    indexes[currIndex++] = v3;
                }
            }

            var temp = new TempMesh()
            {
                vertexes = vertexes,
                normals = normals,
                indexes = indexes,
            };

            return temp;
        }

        public static void TestGmaExport()
        {
            var trackSegment = GameObject.FindObjectOfType<GfzTrackSegment>(false);
            var tempMesh = GenerateTopSurface(trackSegment, 10f, 1);

            // TODO easy way to pop in a model and get out this crap? -- note on materials, too.

            var gma = new Gma();
            gma.Models = new Model[]
            {
                new Model()
                {
                    Name = "my cool model name",
                    Gcmf = new Gcmf()
                    {
                        // attributes = default
                        TextureConfigs = new TextureConfig[0], // this is what you get from templating materials... + v
                        Submeshes = new Submesh[]
                        {
                            new Submesh()
                            {
                                Material = new GameCube.GFZ.GMA.Material(), // along with this... + ^
                                PrimaryDisplayListDescriptor = new DisplayListDescriptor(),
                                PrimaryDisplayListsOpaque = new GameCube.GX.DisplayList[]
                                {
                                    new GameCube.GX.DisplayList(GameCube.GX.GXAttributes.GX_VA_POS, GameCube.GFZ.GfzGX.VAT),
                                },
                            },
                        },
                        BoundingSphere = new GameCube.GFZ.BoundingSphere(),
                    },
                },
            };

        }
    }
}
