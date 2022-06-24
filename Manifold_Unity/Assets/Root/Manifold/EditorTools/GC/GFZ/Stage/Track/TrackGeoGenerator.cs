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

            Vector3[] vertexes = new Vector3[((lengthSamples+1) * (widthSamples+1))];
            Vector3[] normals = new Vector3[vertexes.Length];

            for (int l = 0; l < lengthSamples; l++)
            {
                double samplePoint = l / (double)lengthSamples;
                var animMtx = trackSegment.AnimationCurveTRS.EvaluateMatrix(samplePoint);
                var sampleMtx = mtx * animMtx;

                var position = sampleMtx.Position();
                var rotation = sampleMtx.Rotation();
                var scale = sampleMtx.Scale();
                var tempR = rotation.eulerAngles;
                rotation = Quaternion.Euler(tempR.x, tempR.y, -tempR.z);

                var width = scale.x;
                var halfWidth = width / 2;
                var beginLeft = position + rotation * (left * halfWidth);
                var directionRight = rotation * right;

                for (int w = 0; w <= widthSamples; w++)
                {
                    var percent = w / (float)widthSamples;
                    var offsetRight = directionRight * percent * width;
                    var vertexPos = beginLeft + offsetRight;

                    int index = l * (widthSamples+1) + w;
                    vertexes[index] = vertexPos;

                    //
                    normals[index] = rotation * Vector3.up;
                }
            }


            int[] indexes = new int[lengthSamples * widthSamples * 6];
            int currIndex = 0;
            for (int w = 0; w < widthSamples; w++)
            {
                // iterations = length-wise direction
                for (int l = 0; l < lengthSamples; l++)
                {
                    // Create a quad along the path in the Z/forward direction
                    int stride = widthSamples + 1;
                    int baseIndex = l * stride;
                    int v0 = baseIndex;
                    int v1 = baseIndex + 1;
                    int v2 = baseIndex + stride;
                    int v3 = baseIndex + stride + 1;

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
    }
}
