using GameCube.GX;
using GameCube.GFZ.GMA;
using GameCube.GFZ.LZ;
using GameCube.GFZ.Stage;
using Manifold;
using Manifold.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public static class TristripGenerator
    {
        public static Matrix4x4[] GenerateMatrixIntervals(HierarchichalAnimationCurveTRS hacTRS, float maxStep)
            => GenerateMatrixIntervals(hacTRS, maxStep, 0f, hacTRS.GetSegmentLength());
        public static Matrix4x4[] GenerateMatrixIntervals(HierarchichalAnimationCurveTRS hacTRS, float maxStep, float min, float max)
        {
            float range = max - min;
            float step = range / maxStep;
            int totalIterations = (int)math.ceil(step);
            var matrices = new Matrix4x4[totalIterations + 1]; // +1 since we do <= total, since we want 0 to 1 inclusve

            for (int i = 0; i <= totalIterations; i++)
            {
                double percentage = i / (double)totalIterations;
                double sampleTime = min + percentage * range;
                var matrix = hacTRS.EvaluateHierarchyMatrix(sampleTime);
                matrices[i] = matrix;
            }
            return matrices;
        }
        public static Matrix4x4[] CreatePathMatrices(GfzSegmentNode node, bool useGfzCoordSpace, float maxStep)
            => CreatePathMatrices(node, useGfzCoordSpace, maxStep, 0f, node.GetMaxTime());
        public static Matrix4x4[] CreatePathMatrices(GfzSegmentNode node, bool useGfzCoordSpace, float maxStep, float min, float max)
        {
            var hacTRS = node.CreateHierarchichalAnimationCurveTRS(useGfzCoordSpace);
            var matrices = GenerateMatrixIntervals(hacTRS, maxStep, min, max);
            return matrices;
        }

        public static Matrix4x4[] GetMatricesDefaultScale(Matrix4x4[] matrices, Vector3 scale)
        {
            var matricesDefaultScale = new Matrix4x4[matrices.Length];
            for (int i = 0; i < matricesDefaultScale.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.Rotation();

                matricesDefaultScale[i] = Matrix4x4.TRS(position, rotation, scale);
            }
            return matricesDefaultScale;
        }
        public static Matrix4x4[] ModifyMatrixScales(Matrix4x4[] matrices, Vector3 addToScale)
        {
            var matricesDefaultScale = new Matrix4x4[matrices.Length];
            for (int i = 0; i < matricesDefaultScale.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.Rotation();
                var scale = matrix.Scale();
                scale += addToScale;

                matricesDefaultScale[i] = Matrix4x4.TRS(position, rotation, scale);
            }
            return matricesDefaultScale;
        }
        public static Matrix4x4[] ModifyMatrixScaledPositions(Matrix4x4[] matrices, Vector3 addToPosition)
        {
            var modifiedMatrices = new Matrix4x4[matrices.Length];
            for (int i = 0; i < modifiedMatrices.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.Rotation();
                var scale = matrix.Scale();
                var offset = rotation * new Vector3(addToPosition.x * scale.x, addToPosition.y * scale.y, addToPosition.z * scale.z);

                modifiedMatrices[i] = Matrix4x4.TRS(position + offset, rotation, scale);
            }
            return modifiedMatrices;
        }
        public static Matrix4x4[] ModifyMatrixPositions(Matrix4x4[] matrices, Vector3 addToPosition)
        {
            var modifiedMatrices = new Matrix4x4[matrices.Length];
            for (int i = 0; i < modifiedMatrices.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.Rotation();
                var scale = matrix.Scale();
                var offset = rotation * new Vector3(addToPosition.x, addToPosition.y, addToPosition.z);

                modifiedMatrices[i] = Matrix4x4.TRS(position + offset, rotation, scale);
            }
            return modifiedMatrices;
        }
        public static Matrix4x4[] GetNormalizedMatrixWithPositionOffset(Matrix4x4[] matrices, float direction)
        {
            var matricesDefaultScale = new Matrix4x4[matrices.Length];
            for (int i = 0; i < matricesDefaultScale.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.Rotation();
                var scale = matrix.Scale();
                var offset = rotation * new Vector3(scale.x * 0.5f * direction, 0, 0);

                matricesDefaultScale[i] = Matrix4x4.TRS(position + offset, rotation, new Vector3(1f, scale.y, scale.z));
            }
            return matricesDefaultScale;
        }
        public static Matrix4x4[] StripHeight(Matrix4x4[] matrices)
        {
            for (int i = 0; i < matrices.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.rotation;
                var scale = new Vector3(matrix.lossyScale.x, 1, 1);
                matrices[i] = Matrix4x4.TRS(position, rotation, scale);
            }
            return matrices;
        }

        public static Tristrip[] CreateTristrips(Matrix4x4[] matrices, Vector3[] vertices, Vector3[] normals)
        {
            if (vertices.Length != normals.Length)
                throw new System.ArgumentException($"{nameof(vertices)}.Length != {nameof(normals)}.Length.");

            // Get sizes
            int nTriangleStrips = vertices.Length - 1;
            int nVertsPerStrip = matrices.Length * 2;
            int lastTristripIndex = nTriangleStrips - 1;

            // Init tristrips
            Tristrip[] tristrips = new Tristrip[nTriangleStrips];
            for (int i = 0; i < tristrips.Length; i++)
            {
                tristrips[i] = new Tristrip();
                var tristrip = tristrips[i];
                tristrip.positions = new Vector3[nVertsPerStrip];
                tristrip.normals = new Vector3[nVertsPerStrip]; // could be compute after, no?
            }

            // Compute vertex for all tristrips
            for (int m = 0; m < matrices.Length; m++)
            {
                var matrix = matrices[m];
                // Compute indexes
                int tristripBaseIndex = m * 2;
                var index0 = tristripBaseIndex + 0;
                var index1 = tristripBaseIndex + 1;
                // Compute first and last
                var firstVertex = matrix.MultiplyPoint(vertices[0]);
                var lastVertex = matrix.MultiplyPoint(vertices[vertices.Length - 1]);
                tristrips[0].positions[index0] = firstVertex;
                tristrips[lastTristripIndex].positions[index1] = lastVertex;
                //
                var firstNormal = matrix.rotation * normals[0];
                var lastNormal = matrix.rotation * normals[vertices.Length - 1];
                tristrips[0].normals[index0] = firstNormal;
                tristrips[lastTristripIndex].normals[index1] = lastNormal;

                // And then everything else in-between. The vertex between 2 tristrips is the same,
                // so we can copy them to both tristrips, just offset (n+0.prev, n+1.next).
                for (int t = 0; t < lastTristripIndex; t++)
                {
                    int t0 = t + 0;
                    int t1 = t + 1;
                    //
                    var vertex = matrix.MultiplyPoint(vertices[t1]);
                    tristrips[t0].positions[index1] = vertex;
                    tristrips[t1].positions[index0] = vertex;
                    //
                    var normal = matrix.rotation * normals[t1];
                    tristrips[t0].normals[index1] = normal;
                    tristrips[t1].normals[index0] = normal;
                }
            }
            return tristrips;
        }
        public static Tristrip[] CreateTristrips(Matrix4x4[] matrices, Vector3[] vertices)
        {
            // Get sizes
            int nTriangleStrips = vertices.Length - 1;
            int nVertsPerStrip = matrices.Length * 2;
            int lastTristripIndex = nTriangleStrips - 1;

            // Init tristrips
            Tristrip[] tristrips = new Tristrip[nTriangleStrips];
            for (int i = 0; i < tristrips.Length; i++)
            {
                tristrips[i] = new Tristrip();
                var tristrip = tristrips[i];
                tristrip.positions = new Vector3[nVertsPerStrip];
            }

            // Compute vertex for all tristrips
            for (int m = 0; m < matrices.Length; m++)
            {
                var matrix = matrices[m];
                // Compute indexes
                int tristripBaseIndex = m * 2;
                var index0 = tristripBaseIndex + 0;
                var index1 = tristripBaseIndex + 1;
                // Compute first and last
                var firstVertex = matrix.MultiplyPoint(vertices[0]);
                var lastVertex = matrix.MultiplyPoint(vertices[vertices.Length - 1]);
                tristrips[0].positions[index0] = firstVertex;
                tristrips[lastTristripIndex].positions[index1] = lastVertex;
                // And then everything else in-between. The vertex between 2 tristrips is the same,
                // so we can copy them to both tristrips, just offset (n+0.prev, n+1.next).
                for (int t = 0; t < lastTristripIndex; t++)
                {
                    int t0 = t + 0;
                    int t1 = t + 1;
                    var vertex = matrix.MultiplyPoint(vertices[t1]);
                    tristrips[t0].positions[index1] = vertex;
                    tristrips[t1].positions[index0] = vertex;
                }
            }
            return tristrips;
        }

        public static Tristrip[] GenerateHorizontalLineWithNormals(Matrix4x4[] matrices, Vector3 endpointA, Vector3 endpointB, Vector3 normal, int nTristrips, bool isBackFacing, bool isDoubleSided = false)
        {
            var vertices = CreateLineVertices(nTristrips, endpointA, endpointB);
            var normals = ArrayUtility.DefaultArray(normal, vertices.Length);
            var tristrips = CreateTristrips(matrices, vertices, normals);
            AssignTristripMetadata(tristrips, isBackFacing, isDoubleSided);

            return tristrips;
        }
        public static Tristrip[] CreateLineFrom(Matrix4x4[] matrices, Vector3[] vertices, bool isGfzCoordinateSpace, bool invertNormals, bool isBackFacing, bool isDoubleSided = false)
        {
            var tristrips = CreateTristrips(matrices, vertices, new Vector3[vertices.Length]);
            SetNormalsFromTristripVertices(tristrips, invertNormals, false, isGfzCoordinateSpace);
            AssignTristripMetadata(tristrips, isBackFacing, isDoubleSided);

            return tristrips;
        }

        public static Tristrip[] GenerateCircle(Matrix4x4[] matrices, bool normalOutwards, int nTristrips, float angleFrom = 0, float angleTo = 360)
        {
            // Create tristrips vedrtices (positions)
            var vertices = CreateCircleVertices(nTristrips, angleFrom, angleTo);
            var tristrips = CreateTristrips(matrices, vertices);

            // Set triangle facings
            AssignTristripMetadata(tristrips, normalOutwards, false);

            // That's it!
            return tristrips;
        }
        public static Tristrip[] GenerateCircleWithNormals(Matrix4x4[] matrices, bool normalOutwards, int nTristrips, bool smoothEnds, bool isGfzCoordinateSpace, float angleFrom = 0, float angleTo = 360)
        {
            var tristrips = GenerateCircle(matrices, normalOutwards, nTristrips, angleFrom, angleTo);

            // Add normals based on vertices. Smooth normals.
            bool invertNormals = !normalOutwards;
            SetNormalsFromTristripVertices(tristrips, invertNormals, smoothEnds, isGfzCoordinateSpace);

            return tristrips;
        }
        public static Tristrip[] GenerateOpenCircleWithNormals(Matrix4x4[] matrices, UnityEngine.AnimationCurve gapCurve, int nTristrips, bool isPipe, bool smoothEnds, bool isGfzCoordinateSpace)
        {
            int vertexCount = matrices.Length * 2;
            var tristrips = new Tristrip[nTristrips];
            for (int i = 0; i < tristrips.Length; i++)
            {
                tristrips[i] = new Tristrip();
                tristrips[i].positions = new Vector3[vertexCount];
            }

            // When pipe, 180+-180; when cylinder, 0+-180.
            int baseAngle = isPipe ? 180 : 0;
            int from = baseAngle + 180;
            int to = baseAngle - 180;

            for (int i = 0; i < matrices.Length; i++)
            {
                var matrix = matrices[i];

                var time = i / (matrices.Length - 1f);
                var ratio = math.clamp(gapCurve.EvaluateNormalized(time), 0f, 1f);

                float angleFrom = math.lerp(baseAngle, from, ratio);
                float angleTo = math.lerp(baseAngle, to, ratio);

                var vertices = CreateCircleVertices(nTristrips, angleFrom, angleTo);

                int index0 = i * 2;
                int index1 = index0 + 1;
                for (int j = 0; j < vertices.Length - 1; j++)
                {
                    var vertex0 = vertices[j];
                    var vertex1 = vertices[j + 1];
                    tristrips[j].positions[index0] = matrix.MultiplyPoint(vertex0);
                    tristrips[j].positions[index1] = matrix.MultiplyPoint(vertex1);
                }
            }
            // Add normals based on vertices. Smooth normals.
            bool invertNormals = !isPipe;
            SetNormalsFromTristripVertices(tristrips, invertNormals, smoothEnds, isGfzCoordinateSpace);

            return tristrips;
        }

        public static Tristrip[] GenerateOpenCircleWithNormals2(Matrix4x4[] matrices, UnityEngine.AnimationCurve gapCurve, float from, float to, int nTristrips, bool isPipe, bool smoothEnds, bool isGfzCoordinateSpace)
        {
            int vertexCount = matrices.Length * 2;
            var tristrips = new Tristrip[nTristrips];
            for (int i = 0; i < tristrips.Length; i++)
            {
                tristrips[i] = new Tristrip();
                tristrips[i].positions = new Vector3[vertexCount];
            }

            float baseAngle = isPipe ? 180 : 0;
            for (int i = 0; i < matrices.Length; i++)
            {
                var matrix = matrices[i];

                var time = i / (matrices.Length - 1f);
                var ratio = math.clamp(gapCurve.EvaluateNormalized(time), 0f, 1f);

                // defines angle for semi-circle
                float angleFrom = math.lerp(baseAngle, from, ratio);
                float angleTo = math.lerp(baseAngle, to, ratio);

                var vertices = CreateCircleVertices(nTristrips, angleFrom, angleTo);

                int index0 = i * 2;
                int index1 = index0 + 1;
                for (int j = 0; j < vertices.Length - 1; j++)
                {
                    var vertex0 = vertices[j];
                    var vertex1 = vertices[j + 1];
                    tristrips[j].positions[index0] = matrix.MultiplyPoint(vertex0);
                    tristrips[j].positions[index1] = matrix.MultiplyPoint(vertex1);
                }
            }
            // Add normals based on vertices. Smooth normals.
            bool invertNormals = !isPipe;
            SetNormalsFromTristripVertices(tristrips, invertNormals, smoothEnds, isGfzCoordinateSpace);

            return tristrips;
        }

        public static Vector3[] CreateLineVertices(int nTristrips, Vector3 endpointA, Vector3 endpointB)
        {
            // Sample left and right vertices
            var vertices = new Vector3[nTristrips + 1];
            for (int i = 0; i < vertices.Length; i++)
            {
                float percentage = i / (float)nTristrips;
                vertices[i] = Vector3.Lerp(endpointA, endpointB, percentage);
            }
            return vertices;
        }
        public static Vector3[] CreateCircleVertices(int nTristrips, float angleFrom = 0, float angleTo = 360)
        {
            float from = angleFrom * Mathf.Deg2Rad;
            float to = angleTo * Mathf.Deg2Rad;

            Vector3[] vertices = new Vector3[nTristrips + 1];
            for (int i = 0; i <= nTristrips; i++) // loop wraps and reconnects final vertices
            {
                float percentage = i / (float)nTristrips;
                float theta = math.lerp(from, to, percentage);
                float x = Mathf.Sin(theta);
                float y = Mathf.Cos(theta);
                Vector3 point = new Vector3(x, y, 0) * 0.5f;
                vertices[i] = point;
            }
            return vertices;
        }
        public static void MutateScaleVertices(Vector3[] vertices, float scale)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] *= scale;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrices"></param>
        /// <param name="defaultNormal"></param>
        /// <remarks>
        /// Assumption is made that number of normals = <paramref name="matrices"/> * 2.
        /// </remarks>
        public static Vector3[] CreateNormals(Matrix4x4[] matrices, Vector3 defaultNormal)
        {
            var normals = new Vector3[matrices.Length * 2];
            for (int i = 0; i < matrices.Length; i++)
            {
                int index = i * 2;
                normals[index + 0] = matrices[i].rotation * defaultNormal;
                normals[index + 1] = matrices[i].rotation * defaultNormal;
            }
            return normals;
        }

        public static Vector3[] GenericNormalsFromTristripVertices(Tristrip tristrip, bool invertNormals, bool isGfzCoordinateSpace)
        {
            bool doInvertNormals = invertNormals ^ isGfzCoordinateSpace;


            Vector3[] normals = new Vector3[tristrip.VertexCount];
            for (int i = 0; i < normals.Length - 2; i += 2) // two at a time
            {
                int index0 = i + 0;
                int index1 = i + 1;
                int index2 = i + 2;
                Vector3 vertex0 = tristrip.positions[index0];
                Vector3 vertex1 = tristrip.positions[index1];
                Vector3 vertex2 = tristrip.positions[index2];
                Vector3 dir01 = vertex1 - vertex0;
                Vector3 dir02 = vertex2 - vertex0;
                Vector3 normal = doInvertNormals ? math.cross(dir01, dir02) : math.cross(dir02, dir01);
                normal = math.normalize(normal);
                normals[index0] = normal;
                normals[index1] = normal;
            }

            // Compute normals for last vertices on tristrip
            {
                Vector3 vertex0 = tristrip.positions[normals.Length - 2];
                Vector3 vertex1 = tristrip.positions[normals.Length - 1];
                Vector3 vertex2 = tristrip.positions[normals.Length - 3];
                Vector3 dir01 = vertex1 - vertex0;
                Vector3 dir02 = vertex2 - vertex0;
                Vector3 normal = doInvertNormals ? math.cross(dir02, dir01) : math.cross(dir01, dir02);
                normal = math.normalize(normal);
                normals[normals.Length - 2] = normal;
                normals[normals.Length - 1] = normal;
            }

            return normals;
        }
        public static void SetNormalsFromTristripVertices(Tristrip[] tristrips, bool invertNormals, bool smoothEnds, bool isGfzCoordinateSpace)
        {
            foreach (var tristrip in tristrips)
            {
                tristrip.normals = GenericNormalsFromTristripVertices(tristrip, invertNormals, isGfzCoordinateSpace);
            }

            // Averages normals on shared vertices between tristrips
            for (int i = 0; i < tristrips.Length - 1; i++)
            {
                var tristrip0 = tristrips[i + 0];
                var tristrip1 = tristrips[i + 1];
                for (int j = 0; j < tristrip0.normals.Length; j += 2)
                {
                    Vector3 normal0 = tristrip0.normals[j + 1];
                    Vector3 normal1 = tristrip1.normals[j + 0];
                    Vector3 average = (normal0 + normal1) * 0.5f;
                    tristrip0.normals[j + 1] = average;
                    tristrip1.normals[j + 0] = average;
                }
            }

            if (smoothEnds)
            {
                int firstIndex = 0;
                int lastIndex = tristrips.Length - 1;
                var lastTristrip = tristrips[lastIndex];
                var firstTristrip = tristrips[firstIndex];
                for (int j = 0; j < lastTristrip.normals.Length; j += 2)
                {
                    Vector3 normal0 = lastTristrip.normals[j + 1];
                    Vector3 normal1 = firstTristrip.normals[j + 0];
                    Vector3 average = (normal0 + normal1) * 0.5f;
                    lastTristrip.normals[j + 1] = average;
                    firstTristrip.normals[j + 0] = average;
                }
            }
        }
        public static void SetNormalsFromTristripVerticesNoSmooth(Tristrip[] tristrips, bool invertNormals, bool isGfzCoordinateSpace)
        {
            foreach (var tristrip in tristrips)
            {
                tristrip.normals = GenericNormalsFromTristripVertices(tristrip, invertNormals, isGfzCoordinateSpace);
            }
        }


        public static Vector2[] CreateUVsSideways(int verticesLength)
        {
            int baseIndex = 0;
            var uvs = new Vector2[verticesLength];
            for (int i = 0; i < verticesLength; i += 2)
            {
                uvs[i + 0] = new Vector2(baseIndex, 0);
                uvs[i + 1] = new Vector2(baseIndex, 1);
                baseIndex++;
                baseIndex %= 2;
            }
            return uvs;
        }

        public static Vector2[] UvStripForward(Tristrip tristrip, float repetitions, float left = 0f, float right = 1f)
            => UvStripForward(tristrip.VertexCount, repetitions, left, right);
        public static Vector2[] UvStripForward(int vertexCount, float repetitions, float left = 0f, float right = 1f)
        {
            float increment = repetitions / (vertexCount / 2 - 1);

            float forwardValue = 0;
            var uvs = new Vector2[vertexCount];
            for (int i = 0; i < vertexCount; i += 2) // process 2 at a time
            {
                uvs[i + 0] = new Vector2(left, forwardValue);
                uvs[i + 1] = new Vector2(right, forwardValue);
                forwardValue += increment;
            }
            return uvs;
        }

        public static Vector2[] UvStripSideways(Tristrip tristrip, float repetitions, float bottom = 0f, float top = 1f)
            => UvStripSideways(tristrip.VertexCount, repetitions, bottom, top);
        public static Vector2[] UvStripSideways(int vertexCount, float repetitions, float bottom = 0f, float top = 1f)
        {
            var uvs = UvStripForward(vertexCount, repetitions, bottom, top);
            MutateSwizzleUVs(uvs);
            return uvs;
        }

        public static Vector2[] OffsetUVs(Vector2[] uvs, Vector2 offset)
        {
            var uvCopy = ArrayUtility.CopyArray(uvs);
            MutateOffsetUVs(uvCopy, offset);
            return uvCopy;
        }
        public static Vector2[] OffsetUVs(Vector2[] uvs, float offset)
            => OffsetUVs(uvs, new Vector2(offset, offset));
        public static Vector2[] ScaleUVs(Vector2[] uvs, Vector2 scale)
        {
            var uvCopy = ArrayUtility.CopyArray(uvs);
            MutateScaleUVs(uvCopy, scale);
            return uvCopy;
        }
        public static Vector2[] ScaleUVs(Vector2[] uvs, float scale)
            => ScaleUVs(uvs, new Vector2(scale, scale));
        public static Vector2[] SwizzleUVs(Vector2[] uvs)
        {
            var uvCopy = ArrayUtility.CopyArray(uvs);
            MutateSwizzleUVs(uvCopy);
            return uvCopy;
        }

        public static Vector2[][] SwizzleUVs(Vector2[][] uvs)
        {
            for (int i =0; i < uvs.Length; i++)
            {
                uvs[i] = SwizzleUVs(uvs[i]);
            }
            return uvs;
        }

        public static void MutateScaleUVs(Vector2[] uvs, Vector2 scale)
        {
            for (int i = 0; i < uvs.Length; i++)
                uvs[i] = new Vector2(uvs[i].x * scale.x, uvs[i].y * scale.y);
        }
        public static void MutateOffsetUVs(Vector2[] uvs, Vector2 offset)
        {
            for (int i = 0; i < uvs.Length; i++)
                uvs[i] = new Vector2(uvs[i].x + offset.x, uvs[i].y + offset.y);
        }
        public static void MutateOffsetUVs(Vector2[] uvs, float offset)
            => MutateOffsetUVs(uvs, new Vector2(offset, offset));
        public static void MutateSwizzleUVs(Vector2[] uvs)
        {
            for (int i = 0; i < uvs.Length; i++)
                uvs[i] = new Vector2(uvs[i].y, uvs[i].x);
        }

        private static void MutateArray<T>(T[] array, T value, System.Func<T, T, T> function)
        {
            for (int i = 0; i < array.Length; i++)
                function(array[i], value);
        }
        private static void MutateArray2D<T>(T[][] array2D, T value, System.Action<T[], T> function)
        {
            for (int i = 0; i < array2D.Length; i++)
                function(array2D[i], value);
        }
        private static T[] ModifyArrayCopy<T>(T[] array, T value, System.Func<T, T, T> function)
        {
            var newArray = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
                newArray[i] = function(array[i], value);
            return newArray;
        }
        private static T[][] ModifyArray2DCopy<T>(T[][] array2D, T value, System.Func<T[], T, T[]> function)
        {
            var newArray2D = new T[array2D.Length][];
            for (int i = 0; i < array2D.Length; i++)
                newArray2D[i] = function(array2D[i], value);
            return newArray2D;
        }

        private static Vector2 ScaleUV(Vector2 uv, Vector2 scale)
        {
            return new Vector2(uv.x * scale.x, uv.y * scale.y);
        }
        public static Vector2[] ScaleUV(Vector2[] uvs, Vector2 scale) => ModifyArrayCopy(uvs, scale, ScaleUV);
        public static Vector2[][] ScaleUV(Vector2[][] uvs, Vector2 scale) => ModifyArray2DCopy(uvs, scale, ScaleUV);
        public static void MutateScaleUV(Vector2[] uvs, Vector2 scale) => MutateArray(uvs, scale, ScaleUV);
        public static void MutateScaleUV(Vector2[][] uvs, Vector2 scale) => MutateArray2D(uvs, scale, MutateScaleUV);
        public static Vector2[] ScaleUV(Vector2[] uvs, float scale) => ScaleUV(uvs, new Vector2(scale, scale));
        public static Vector2[][] ScaleUV(Vector2[][] uvs, float scale) => ScaleUV(uvs, new Vector2(scale, scale));
        public static void MutateScaleUV(Vector2[] uvs, float scale) => ScaleUV(uvs, new Vector2(scale, scale));
        public static void MutateScaleUV(Vector2[][] uvs, float scale) => ScaleUV(uvs, new Vector2(scale, scale));

        private static Vector2 OffsetUV(Vector2 uv, Vector2 offset)
        {
            return new Vector2(uv.x + offset.x, uv.y + offset.y);
        }
        public static Vector2[] OffsetUV(Vector2[] uvs, Vector2 offset) => ModifyArrayCopy(uvs, offset, OffsetUV);
        public static Vector2[][] OffsetUV(Vector2[][] uvs, Vector2 offset) => ModifyArray2DCopy(uvs, offset, OffsetUV);
        public static void MutateOffsetUV(Vector2[] uvs, Vector2 offset) => MutateArray(uvs, offset, OffsetUV);
        public static void MutateOffsetUV(Vector2[][] uvs, Vector2 offset) => MutateArray2D(uvs, offset, MutateOffsetUV);
        public static Vector2[] OffsetUV(Vector2[] uvs, float offset) => OffsetUV(uvs, new Vector2(offset, offset));
        public static Vector2[][] OffsetUV(Vector2[][] uvs, float offset) => OffsetUV(uvs, new Vector2(offset, offset));
        public static void MutateOffsetUV(Vector2[] uvs, float offset) => OffsetUV(uvs, new Vector2(offset, offset));
        public static void MutateOffsetUV(Vector2[][] uvs, float offset) => OffsetUV(uvs, new Vector2(offset, offset));


        public static float GetTexRepetitions(float segmentLength, float texLength)
        {
            float repetitionsAlongLength = math.ceil(segmentLength / texLength);
            return repetitionsAlongLength;
        }
        public static Vector2[][] CreateTristripScaledUVs(Tristrip[] tristrips, float widthRepeats, float lengthRepeats)
        {
            var allUVs = new Vector2[tristrips.Length][];
            for (int i = 0; i < tristrips.Length; i++)
            {
                var tristrip = tristrips[i];
                var uvs = new Vector2[tristrip.VertexCount];
                float uvLeft = (float)(i + 0) / tristrips.Length * widthRepeats;
                float uvRight = (float)(i + 1) / tristrips.Length * widthRepeats;

                for (int j = 0; j < tristrip.VertexCount; j += 2)
                {
                    float percentForward = (j / 2f / (tristrip.VertexCount / 2 - 1));
                    float uvLength = percentForward * lengthRepeats;
                    uvs[j + 0] = new Vector2(uvLeft, uvLength);
                    uvs[j + 1] = new Vector2(uvRight, uvLength);
                }

                allUVs[i] = uvs;
            }
            return allUVs;
        }
        public static Vector2[][] CreateTristripScaledUVs(Tristrip[] tristrips, float widthRepeats, float segmentLength, float texLength)
        {
            float repetitionsAlongLength = math.ceil(segmentLength / texLength);
            var uvs = CreateTristripScaledUVs(tristrips, widthRepeats, repetitionsAlongLength);
            return uvs;
        }



        public static float[] CreateEvenlySpacedTimes(int count, float min = 0, float max = 1)
        {
            float[] times = new float[count+1];
            for (int i = 0; i < times.Length; i++)
            {
                float time = i / (times.Length - 1f);
                times[i] = Mathf.Lerp(min, max, time);
            }
            return times;
        }
        public static float[] InsertTimes(float[] timesA, float[] timesB)
        {
            int indexA = 0;
            int indexB = 0;
            var times = new float[timesA.Length + timesB.Length];
            for (int i = 0; i < times.Length; i++)
            {
                float a = indexA > timesA.Length ? float.PositiveInfinity : timesA[indexA];
                float b = indexB > timesB.Length ? float.NegativeInfinity : timesB[indexB];
                bool aIsSmallest = a < b;
                if (aIsSmallest)
                    indexA++;
                else
                    indexB++;
                float value = aIsSmallest ? a : b;
                times[i] = value;
            }
            return times;
        }
        public static Vector3[] CreateVertexLineFromTimes(float[] times, Vector3 endpointA, Vector3 endpointB)
        {
            Vector3[] vertices = new Vector3[times.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                float time = times[i];
                Vector3 vertex = Vector3.Lerp(endpointA, endpointB, time);
                vertices[i] = vertex;
            }
            return vertices;
        }
        public static Vector2[][] CreateUVsFromTimes(Tristrip[] tristrips, float[] times, float repsWidth, float repsLength)
        {
            bool isValid = tristrips.Length == times.Length - 1;
            if (!isValid)
                throw new System.ArgumentException($"Incorrect number of tristrips and times provided. {tristrips.Length}/{times.Length}");

            // Scale times
            float[] scaledTimes = new float[times.Length];
            for (int i = 0; i < times.Length; i++)
                scaledTimes[i] = times[i] * repsWidth;

            // Gen UVs
            Vector2[][] uvs = new Vector2[tristrips.Length][];
            for (int i = 0; i < tristrips.Length; i++)
            {
                uvs[i] = new Vector2[tristrips[i].VertexCount];
                float uv0x = scaledTimes[i + 0];
                float uv1x = scaledTimes[i + 1];
                for (int j = 0; j < uvs[i].Length; j += 2)
                {
                    float lengthTime = j / (uvs[i].Length - 2f);
                    float uv_y = lengthTime * repsLength;
                    Vector2 uv0 = new Vector2(uv0x, uv_y);
                    Vector2 uv1 = new Vector2(uv1x, uv_y);
                    uvs[i][j + 0] = uv0;
                    uvs[i][j + 1] = uv1;
                }
            }
            return uvs;
        }
        public static Vector2[][] CreateCustomUVs(Tristrip[] tristrips, float repsLength, params float[] times)
            => CreateUVsFromTimes(tristrips, times, 1f, repsLength);
        public static Vector2[][] CreateUVsStrip(Tristrip[] tristrips, float repsLength)
            => CreateUVsFromTimes(tristrips, new float[] { 0, 1 }, 1f, repsLength);
        public static Tristrip[] VertexLinesToTristrips(Vector3[][] lines)
        {
            int nVertices = lines.Length * 2;
            int nTristrips = lines[0].Length - 1;
            var tristrips = new Tristrip[nTristrips];
            for (int i = 0; i < tristrips.Length; i++)
            {
                tristrips[i] = new Tristrip();
                tristrips[i].positions = new Vector3[nVertices];
            }

            for (int x = 0; x < nTristrips; x++)
            {
                for (int z = 0; z < lines.Length; z++)
                {
                    tristrips[x].positions[z * 2 + 0] = lines[z][x + 0];
                    tristrips[x].positions[z * 2 + 1] = lines[z][x + 1];
                }
            }

            return tristrips;
        }
        public static Tristrip InterleaveVertexLines(Vector3[] vertexLineA, Vector3[] vertexLineB)
        {
            bool isValid = vertexLineA.Length == vertexLineB.Length;
            if (!isValid)
                throw new System.ArgumentException();

            int singleLineVertexCount = vertexLineA.Length;
            var vertices = new Vector3[singleLineVertexCount * 2];
            for (int i = 0; i < singleLineVertexCount; i++)
            {
                vertices[i * 2 + 0] = vertexLineA[i];
                vertices[i * 2 + 1] = vertexLineB[i];
            }

            var tristrip = new Tristrip();
            tristrip.positions = vertices;
            return tristrip;
        }


        // Conversion from Unity to GFZ
        public static DisplayList[] TristripsToDisplayLists(Tristrip[] tristrips, VertexAttributeTable vat)
        {
            var displayLists = new DisplayList[tristrips.Length];
            for (int i = 0; i < displayLists.Length; i++)
            {
                var tristrip = tristrips[i];

                // Initialize display list
                var dlist = new DisplayList(0, vat);
                dlist.GxCommand = new DisplayCommand()
                {
                    Primitive = Primitive.GX_TRIANGLESTRIP,
                    VertexFormat = VertexFormat.GX_VTXFMT0,
                };

                Copy(tristrip.positions, ref dlist.pos);
                Copy(tristrip.normals, ref dlist.nrm);
                Copy(tristrip.binormals, ref dlist.bnm);
                Copy(tristrip.tangents, ref dlist.tan);
                Copy(tristrip.tex0, ref dlist.tex0);
                Copy(tristrip.tex1, ref dlist.tex1);
                Copy(tristrip.tex2, ref dlist.tex2);
                Copy(tristrip.color0, ref dlist.clr0);

                // Set attributes based on provided data
                dlist.Attributes = dlist.ComponentsToGXAttributes();
                dlist.VertexCount = checked((ushort)tristrip.positions.Length);

                displayLists[i] = dlist;
            }
            return displayLists;
        }
        private static void Copy(Vector3[] vector3s, ref float3[] float3s)
        {
            if (vector3s is null)
                return;

            float3s = new float3[vector3s.Length];
            for (int i = 0; i < float3s.Length; i++)
                float3s[i] = vector3s[i];
        }
        private static void Copy(Vector2[] vector2s, ref float2[] float2s)
        {
            if (vector2s is null)
                return;

            float2s = new float2[vector2s.Length];
            for (int i = 0; i < float2s.Length; i++)
                float2s[i] = vector2s[i];
        }
        private static void Copy(Color32[] color32s, ref GXColor[] gxColors)
        {
            if (color32s is null)
                return;

            gxColors = new GXColor[color32s.Length];
            for (int i = 0; i < gxColors.Length; i++)
            {
                var c = color32s[i];
                gxColors[i] = new GXColor(c.r, c.g, c.b, c.a);
            }
        }

        public static void AssignTristripMetadata(Tristrip[] tristrips, bool isBackFacing, bool isDoubleSided)
        {
            foreach (var tristrip in tristrips)
            {
                tristrip.isBackFacing = isBackFacing;
                tristrip.isDoubleSided = isDoubleSided;
            }
        }

        /// <summary>
        /// Concatenate multiple Tristrip[] together into a single Tristrip[]
        /// </summary>
        /// <param name="tristripArrays"></param>
        /// <returns></returns>
        public static Tristrip[] ConcatTristrips(params Tristrip[][] tristripArrays)
        {
            int count = 0;
            for (int i = 0; i < tristripArrays.Length; i++)
                count += tristripArrays[i].Length;
            var tristripsConcat = new Tristrip[count];

            int baseIndex = 0;
            for (int i = 0; i < tristripArrays.Length; i++)
            {
                var tristrips = tristripArrays[i];
                tristrips.CopyTo(tristripsConcat, baseIndex);
                baseIndex += tristrips.Length;
            }

            return tristripsConcat;
        }
        public static Tristrip[] ConcatTristrips(params Tristrip[] tristrips)
        {
            return tristrips;
        }


        #region TODO DELETE
        public static Tristrip[] SelectTristrips(Tristrip[] tristrips, float percentFrom, float percentTo, bool inclusiveFrom, bool inclusiveTo)
        {
            int maxIndex = tristrips.Length - 1;
            float first = percentFrom * maxIndex;
            float last = percentTo * maxIndex;
            int firstIndex = (int)(inclusiveFrom ? math.floor(first) : math.ceil(first));
            int lastIndex = (int)(inclusiveTo ? math.ceil(last) : math.floor(last));
            var subselection = tristrips.CopyRange(firstIndex, lastIndex);
            return subselection;
        }
        public static Tristrip[] SelectTritrips(Tristrip[] tristrips, int indexFrom, int indexTo)
        {
            var subselection = tristrips.CopyRange(indexFrom, indexTo);
            return subselection;
        }
        public static Vector2[][] SwapUV(Vector2[][] uvs)
        {
            var newUVs = new Vector2[uvs.Length][];
            for (int i = 0; i < uvs.Length; i++)
            {
                newUVs[i] = new Vector2[uvs[i].Length];
                for (int j = 0; j < uvs[i].Length; j++)
                {
                    newUVs[i][j] = new Vector2(uvs[i][j].y, uvs[i][j].x);
                }
            }

            return newUVs;
        }

        #endregion



        public static void ApplyColor0(Tristrip[] tristrips, Color32 color)
        {
            foreach (var tristrip in tristrips)
                tristrip.color0 = ArrayUtility.DefaultArray(color, tristrip.VertexCount);
        }

        public static void ApplyTex0(Tristrip[] tristrips, Vector2[][] uvs)
        {
            for (int i = 0; i < tristrips.Length; i++)
                tristrips[i].tex0 = uvs[i];
        }
        public static void ApplyTex1(Tristrip[] tristrips, Vector2[][] uvs)
        {
            for (int i = 0; i < tristrips.Length; i++)
                tristrips[i].tex1 = uvs[i];
        }
        public static void ApplyTex2(Tristrip[] tristrips, Vector2[][] uvs)
        {
            for (int i = 0; i < tristrips.Length; i++)
                tristrips[i].tex2 = uvs[i];
        }

        public static Vector2[][] CopyTex0(Tristrip[] tristrips)
        {
            Vector2[][] uvs = new Vector2[tristrips.Length][];
            for (int i = 0; i < uvs.Length; i++)
                uvs[i] = ArrayUtility.CopyArray(tristrips[i].tex0);
            return uvs;
        }
        public static Vector2[][] CopyTex1(Tristrip[] tristrips)
        {
            Vector2[][] uvs = new Vector2[tristrips.Length][];
            for (int i = 0; i < uvs.Length; i++)
                uvs[i] = ArrayUtility.CopyArray(tristrips[i].tex1);
            return uvs;
        }
        public static Vector2[][] CopyTex2(Tristrip[] tristrips)
        {
            Vector2[][] uvs = new Vector2[tristrips.Length][];
            for (int i = 0; i < uvs.Length; i++)
                uvs[i] = ArrayUtility.CopyArray(tristrips[i].tex2);
            return uvs;
        }




        public static Vector3 Forward(bool isGfzCoordinateSpace)
        {
            return isGfzCoordinateSpace ? Vector3.back : Vector3.forward;
        }
        public static Vector3 Back(bool isGfzCoordinateSpace)
        {
            return isGfzCoordinateSpace ? Vector3.forward : Vector3.back;
        }

    }
}
