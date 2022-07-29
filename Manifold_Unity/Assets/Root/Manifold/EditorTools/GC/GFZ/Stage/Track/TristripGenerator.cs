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
        {
            float segmentLength = hacTRS.GetSegmentLength();
            float step = segmentLength / maxStep;
            int totalIterations = (int)math.ceil(step);
            var matrices = new Matrix4x4[totalIterations + 1]; // +1 since we do <= total, since we want 0 to 1 inclusve

            for (int i = 0; i <= totalIterations; i++)
            {
                double percentage = i / (double)totalIterations;
                double sampleTime = percentage * segmentLength;
                var matrix = hacTRS.EvaluateAnimationMatrices(sampleTime);
                matrices[i] = matrix;
            }
            return matrices;
        }
        public static Matrix4x4[] GenerateMatrixIntervals(HierarchichalAnimationCurveTRS hacTRS, float maxStep, float min, float max)
        {
            //float maxTime = hacTRS.GetMaxTime();
            float subLength = max - min;
            float step = subLength / maxStep;
            int totalIterations = (int)math.ceil(step);
            var matrices = new Matrix4x4[totalIterations + 1]; // +1 since we do <= total, since we want 0 to 1 inclusve

            Debug.Log($"MeshGfz -- Min: {min}, Max: {max}, MaxTime: {-1}");

            for (int i = 0; i <= totalIterations; i++)
            {
                double percentage = i / (double)totalIterations;
                double sampleTime = min + percentage * subLength;
                var matrix = hacTRS.EvaluateAnimationMatrices(sampleTime);
                matrices[i] = matrix;
            }
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
        public static Matrix4x4[] StripHeight(Matrix4x4[] matrices)
        {
            var matricesDefaultScale = new Matrix4x4[matrices.Length];
            for (int i = 0; i < matricesDefaultScale.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.Rotation();
                var scale = matrix.Scale();

                matricesDefaultScale[i] = Matrix4x4.TRS(position, rotation, new Vector3(scale.x, 1f, 1f));
            }
            return matricesDefaultScale;
        }

        // TODO: deprecate, make simple "unlit" tristrip method
        public static Tristrip[] CreateTristrips(Matrix4x4[] matrices, Vector3 endpointA, Vector3 endpointB, int nTristrips, Color32? color0, Vector3? normal, int uvs, bool isBackFacing)
        {
            var vertices = CreateVerticesLine(nTristrips, endpointA, endpointB);
            var temp = normal is null ? Vector3.up : (Vector3)normal;
            var normals = ArrayUtility.DefaultArray(temp, vertices.Length);
            var tristrips = GenerateTristrips(matrices, vertices, normals);

            // Assign data
            // CLR0
            foreach (var tristrip in tristrips)
            {
                // COLOR
                if (color0 is not null)
                {
                    var color = (Color32)color0;
                    var nVertices = tristrip.positions.Length;
                    tristrip.color0 = new Color32[nVertices];
                    for (int i = 0; i < nVertices; i++)
                    {
                        tristrip.color0[i] = color;
                    }
                }

                // NORMALS
                if (normal is not null)
                    tristrip.normals = CreateNormals(matrices, (Vector3)normal);

                // UVs
                if (uvs > 0)
                    tristrip.tex0 = CreateUVsSideways(tristrip.positions.Length);
                if (uvs > 1)
                    tristrip.tex1 = CreateUVsSideways(tristrip.positions.Length);
                if (uvs > 2)
                    tristrip.tex2 = CreateUVsSideways(tristrip.positions.Length);

                tristrip.isBackFacing = isBackFacing;
            }

            return tristrips;
        }

        public static Tristrip[] GenerateTristrips(Matrix4x4[] matrices, Vector3[] vertices, Vector3[] normals)
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
        public static Tristrip[] GenerateTristripsLine(Matrix4x4[] matrices, Vector3 endpointA, Vector3 endpointB, Vector3 normal, int nTristrips, bool isBackFacing)
        {
            var vertices = CreateVerticesLine(nTristrips, endpointA, endpointB);
            var normals = ArrayUtility.DefaultArray(normal, vertices.Length);
            var tristrips = GenerateTristrips(matrices, vertices, normals);

            foreach (var tristrip in tristrips)
                tristrip.isBackFacing = isBackFacing;

            return tristrips;
        }

        public static Vector3[] CreateVerticesLine(int nTristrips, Vector3 endpointA, Vector3 endpointB)
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
        public static Vector2[] CreateUVsSideways(int verticesLength, float bottom = 0f, float top = 1f, float increment = 1f, float modulus = 2f)
        {
            float forwardValue = 0;
            var uvs = new Vector2[verticesLength];
            for (int i = 0; i < verticesLength; i += 2) // process 2 at a time
            {
                uvs[i + 0] = new Vector2(forwardValue, bottom);
                uvs[i + 1] = new Vector2(forwardValue, top);
                forwardValue += increment;
                forwardValue %= modulus;
            }
            return uvs;
        }
        public static Vector2[] CreateUVsForward(int verticesLength, float left = 0f, float right = 1f, float increment = 1f, float modulus = 2f)
        {
            float forwardValue = 0;
            var uvs = new Vector2[verticesLength];
            for (int i = 0; i < verticesLength; i += 2) // process 2 at a time
            {
                uvs[i + 0] = new Vector2(left, forwardValue);
                uvs[i + 1] = new Vector2(right, forwardValue);
                forwardValue += increment;
                forwardValue %= modulus;
            }
            return uvs;
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


        public static Tristrip[] CreateTempTrackRoad(GfzTrackSegmentNode node, int nTristrips, float maxStep, bool useGfzCoordSpace)
            => Road.CreateDebug(node, nTristrips, maxStep, useGfzCoordSpace);


        public static class Road
        {
            public static Tristrip[] CreateDebug(GfzTrackSegmentNode node, int nTristrips, float maxStep, bool useGfzCoordSpace)
            {
                var allTriStrips = new List<Tristrip>();
                var hacTRS = node.CreateHierarchichalAnimationCurveTRS(useGfzCoordSpace);
                var segmentLength = hacTRS.GetSegmentLength();
                var matrices = GenerateMatrixIntervals(hacTRS, maxStep);

                //
                var settings = GfzProjectWindow.GetSettings();

                // For road, we strip height data
                matrices = StripHeight(matrices);

                // track top
                {
                    var endpointA = new Vector3(-0.5f, 0, 0);
                    var endpointB = new Vector3(+0.5f, 0, 0);
                    var color0 = settings.DebugTrackSurface;
                    var normal = Vector3.up;
                    var trackTopTristrips = CreateTristrips(matrices, endpointA, endpointB, nTristrips, color0, normal, 0, true);
                    allTriStrips.AddRange(trackTopTristrips);
                }

                // track bottom
                {
                    var endpointA = new Vector3(-0.5f, -2.0f, 0);
                    var endpointB = new Vector3(+0.5f, -2.0f, 0);
                    var color0 = settings.DebugTrackUnderside;
                    var normal = Vector3.down;
                    var trackBottomTristrips = CreateTristrips(matrices, endpointA, endpointB, nTristrips, color0, normal, 0, false);
                    allTriStrips.AddRange(trackBottomTristrips);
                }

                // track left
                {
                    var endpointA = new Vector3(-0.5f, +0.0f, 0);
                    var endpointB = new Vector3(-0.5f, -2.0f, 0);
                    var color0 = settings.DebugTrackLeft;
                    var normal = Vector3.left;
                    var trackLeftTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, normal, 0, false);
                    allTriStrips.AddRange(trackLeftTristrips);
                }

                // track right
                {
                    var endpointA = new Vector3(+0.5f, +0.0f, 0);
                    var endpointB = new Vector3(+0.5f, -2.0f, 0);
                    var color0 = settings.DebugTrackRight;
                    var normal = Vector3.right;
                    var trackRightTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, normal, 0, true);
                    allTriStrips.AddRange(trackRightTristrips);
                }

                var isTypeofRail = node is IRailSegment;
                if (isTypeofRail)
                {
                    var rails = node as IRailSegment;

                    // rail left
                    if (rails.RailHeightLeft > 0f)
                    {
                        var endpointA = new Vector3(-0.5f, +0.0f, 0);
                        var endpointB = new Vector3(-0.5f, rails.RailHeightLeft, 0);
                        var color0 = settings.DebugRailLeft;
                        var trackRightTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, null, 0, false);
                        allTriStrips.AddRange(trackRightTristrips);
                    }

                    // rail right
                    if (rails.RailHeightRight > 0f)
                    {
                        var endpointA = new Vector3(+0.5f, +0.0f, 0);
                        var endpointB = new Vector3(+0.5f, rails.RailHeightRight, 0);
                        var color0 = settings.DebugRailRight;
                        var trackRightTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, null, 0, true);
                        allTriStrips.AddRange(trackRightTristrips);
                    }
                }

                //
                var intervals = (int)math.ceil(segmentLength / 100.0); // per ~100 meters
                var intervalsInverse = 1.0 / intervals;
                // width dividers
                for (int i = 0; i < intervals; i++)
                {
                    var time = (i * intervalsInverse) * segmentLength;
                    var matrix0 = hacTRS.EvaluateAnimationMatrices(time);
                    var matrix1 = hacTRS.EvaluateAnimationMatrices(time + 3f);// 3 units forward
                    var matrices01 = new Matrix4x4[] { matrix0, matrix1 };

                    var endpointA = new Vector3(-0.5f, +0.10f, 0);
                    var endpointB = new Vector3(+0.5f, +0.10f, 0);
                    var color0 = settings.DebugWidthDivider;
                    var normal = Vector3.up;
                    var widthDividers = CreateTristrips(matrices01, endpointA, endpointB, nTristrips, color0, normal, 0, true);
                    allTriStrips.AddRange(widthDividers);
                }

                // REMOVE SCALE.X
                var matricesNoScale = GetMatricesDefaultScale(matrices, Vector3.one);
                // center line
                {
                    var endpointA = new Vector3(-0.5f, +0.15f, 0);
                    var endpointB = new Vector3(+0.5f, +0.15f, 0);
                    var color0 = settings.DebugLaneDivider;
                    var normal = Vector3.up;
                    var trackLaneDivider = CreateTristrips(matricesNoScale, endpointA, endpointB, 1, color0, normal, 0, true);
                    allTriStrips.AddRange(trackLaneDivider);
                }

                return allTriStrips.ToArray();
            }

            //public static Tristrip[] CreateMuteCity(GfzTrackSegmentNode node, int nTristrips, float maxStep, bool useGfzCoordSpace)
            //{
            //    var allTriStrips = new List<Tristrip>();
            //    var hacTRS = node.CreateHierarchichalAnimationCurveTRS(useGfzCoordSpace);
            //    var matrices = GenerateMatrixIntervals(hacTRS, maxStep);

            //    //
            //    var settings = GfzProjectWindow.GetSettings();

            //    // For road, we strip height data
            //    matrices = StripHeight(matrices);

            //    // TEX
            //    // top: 1
            //    // bot: 1
            //    // embellishments l/r: 2
            //    // left/right side: 1 (note: is also on top of track
            //    // lane dividers: 1


            //    // track top
            //    {
            //        var endpointA = new Vector3(-0.5f, 0, 0);
            //        var endpointB = new Vector3(+0.5f, 0, 0);
            //        var color0 = settings.DebugTrackSurface;
            //        var normal = Vector3.up;
            //        var trackTopTristrips = CreateTristrips(matrices, endpointA, endpointB, nTristrips, color0, normal, 1, true);
            //        allTriStrips.AddRange(trackTopTristrips);
            //    }

            //    // track bottom
            //    {
            //        var endpointA = new Vector3(-0.5f, -1.5f, 0);
            //        var endpointB = new Vector3(+0.5f, -1.5f, 0);
            //        var color0 = settings.DebugTrackUnderside;
            //        var normal = Vector3.down;
            //        var trackBottomTristrips = CreateTristrips(matrices, endpointA, endpointB, nTristrips, color0, normal, 1, false);
            //        allTriStrips.AddRange(trackBottomTristrips);
            //    }

            //    // track left
            //    {
            //        var endpointA = new Vector3(-0.5f, +0.0f, 0);
            //        var endpointB = new Vector3(-0.5f, -1.5f, 0);
            //        var color0 = settings.DebugTrackLeft;
            //        var normal = Vector3.left;
            //        var trackLeftTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, normal, 0, false);
            //        allTriStrips.AddRange(trackLeftTristrips);
            //    }

            //    // track right
            //    {
            //        var endpointA = new Vector3(+0.5f, +0.0f, 0);
            //        var endpointB = new Vector3(+0.5f, -1.5f, 0);
            //        var color0 = settings.DebugTrackRight;
            //        var normal = Vector3.right;
            //        var trackRightTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, normal, 0, true);
            //        allTriStrips.AddRange(trackRightTristrips);
            //    }

            //    var isTypeofRail = node is IRailSegment;
            //    if (isTypeofRail)
            //    {
            //        var rails = node as IRailSegment;

            //        // rail left
            //        if (rails.RailHeightLeft > 0f)
            //        {
            //            var endpointA = new Vector3(-0.5f, +0.0f, 0);
            //            var endpointB = new Vector3(-0.5f, rails.RailHeightLeft, 0);
            //            var color0 = settings.DebugRailLeft;
            //            var trackRightTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, null, 3, false);
            //            allTriStrips.AddRange(trackRightTristrips);
            //        }

            //        // rail right
            //        if (rails.RailHeightRight > 0f)
            //        {
            //            var endpointA = new Vector3(+0.5f, +0.0f, 0);
            //            var endpointB = new Vector3(+0.5f, rails.RailHeightRight, 0);
            //            var color0 = settings.DebugRailRight;
            //            var trackRightTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, null, 3, true);
            //            allTriStrips.AddRange(trackRightTristrips);
            //        }
            //    }

            //    GetLaneDividers(matrices, settings.DebugLaneDivider, 1, allTriStrips);

            //    return allTriStrips.ToArray();
            //}

            public static Matrix4x4[] CreatePathMatrices(GfzTrackSegmentNode node, float maxStep, bool useGfzCoordSpace)
            {
                var hacTRS = node.CreateHierarchichalAnimationCurveTRS(useGfzCoordSpace);
                var matrices = GenerateMatrixIntervals(hacTRS, maxStep);
                return matrices;
            }
            public static Matrix4x4[] CreatePathMatrices(GfzTrackSegmentNode node, float maxStep, float min, float max, bool useGfzCoordSpace)
            {
                var hacTRS = node.CreateHierarchichalAnimationCurveTRS(useGfzCoordSpace);
                var matrices = GenerateMatrixIntervals(hacTRS, maxStep, min, max);
                return matrices;
            }

            public static Tristrip[] CreateEmbed(Matrix4x4[] matrices, GfzTrackSurfaceEmbed node, int nTristrips, float length)
            {
                var endpointA = new Vector3(-0.5f, 0.33f, 0);
                var endpointB = new Vector3(+0.5f, 0.33f, 0);
                var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.up, nTristrips, true);
                Color32 color0 = node.GetColor();

                //float repetitions = math.ceil(length / 80f);
                //float modulus = float.PositiveInfinity;
                for (int i = 0; i < tristrips.Length; i++)
                {
                    var tristrip = tristrips[i];
                    //float left = (i + 0) % 2;
                    //float right = (i + 1) % 2;
                    //float increment = repetitions / (tristrip.VertexCount / 2 - 1); // length of verts, but not both sides
                    //tristrip.tex0 = CreateUVsForward(tristrip.VertexCount, left, right, increment, modulus);
                    tristrip.color0 = ArrayUtility.DefaultArray(color0, tristrip.VertexCount);
                }

                return tristrips;
            }


            public static class MuteCity
            {
                // TODO: embellishments (part with red), adjust verts to match game, fix "end caps"
                // TODO: for future elements, make this generic when possible. ie: reuse mesh between, say,
                //          Mute City and Green Plant? etc.

                public static Tristrip[] CreateRoadTop(Matrix4x4[] matrices, int nTristrips, float length)
                {
                    var endpointA = new Vector3(-0.5f, 0, 0);
                    var endpointB = new Vector3(+0.5f, 0, 0);
                    var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.up, nTristrips, true);

                    float repetitions = math.ceil(length / 80f);
                    float modulus = float.PositiveInfinity;
                    for (int i = 0; i < tristrips.Length; i++)
                    {
                        var tristrip = tristrips[i];
                        float left = (i + 0) % 2;
                        float right = (i + 1) % 2;
                        float increment = repetitions / (tristrip.VertexCount / 2 - 1); // length of verts, but not both sides
                        tristrip.tex0 = CreateUVsForward(tristrip.VertexCount, left, right, increment, modulus);
                    }

                    return tristrips;
                }

                public static Tristrip[] CreateRoadBottom(Matrix4x4[] matrices, int nTristrips, float length)
                {
                    var endpointA = new Vector3(-0.5f, -1.5f, 0);
                    var endpointB = new Vector3(+0.5f, -1.5f, 0);
                    var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.up, nTristrips, false);

                    float repetitions = math.ceil(length / 40f);
                    float modulus = float.PositiveInfinity;
                    for (int i = 0; i < tristrips.Length; i++)
                    {
                        var tristrip = tristrips[i];
                        float left = (i + 0) % 2;
                        float right = (i + 1) % 2;
                        float increment = repetitions / (tristrip.VertexCount / 2 - 1); // length of verts, but not both sides
                        tristrip.tex0 = CreateUVsForward(tristrip.VertexCount, left, right, increment, modulus);
                        tristrip.color0 = ArrayUtility.DefaultArray(new Color32(128, 128, 128, 255), tristrip.VertexCount);
                    }

                    return tristrips;
                }

                public static Tristrip[] CreateRoadSides(Matrix4x4[] matrices, float width, float length)
                {
                    var allTristrips = new List<Tristrip>();

                    // left
                    {
                        var endpointA = new Vector3(-0.5f, +0.0f, 0);
                        var endpointB = new Vector3(-0.5f, -1.5f, 0);
                        var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.up, 1, false);

                        float repetitions = math.ceil(length / 100f);
                        float modulus = float.PositiveInfinity;
                        for (int i = 0; i < tristrips.Length; i++)
                        {
                            var tristrip = tristrips[i];
                            float left = (i + 0) % 2;
                            float right = (i + 1) % 2;
                            float increment = repetitions / (tristrip.VertexCount / 2 - 1); // length of verts, but not both sides
                            tristrip.tex0 = CreateUVsForward(tristrip.VertexCount, left, right, increment, modulus);
                            //tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount, 0, 1, increment, modulus);
                        }

                        allTristrips.AddRange(tristrips);
                    }

                    // COPY PASTED. MAKE MODULAR?
                    // right
                    {
                        var endpointA = new Vector3(+0.5f, +0.0f, 0);
                        var endpointB = new Vector3(+0.5f, -1.5f, 0);
                        var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.up, 1, true);

                        float repetitions = math.ceil(length / 40f);
                        float modulus = float.PositiveInfinity;
                        for (int i = 0; i < tristrips.Length; i++)
                        {
                            var tristrip = tristrips[i];
                            float left = (i + 0) % 2;
                            float right = (i + 1) % 2;
                            float increment = repetitions / (tristrip.VertexCount / 2 - 1); // length of verts, but not both sides
                            tristrip.tex0 = CreateUVsForward(tristrip.VertexCount, left, right, increment, modulus);
                            //tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount, 0, 1, increment, modulus);
                        }

                        allTristrips.AddRange(tristrips);
                    }

                    {
                        var mtxOffset = Matrix4x4.TRS(new(0, -1.5f, 0), Quaternion.identity, Vector3.one);
                        var endpointA = new Vector3(-0.5f, +0.0f, 0);
                        var endpointB = new Vector3(+0.5f, +0.0f, 0);
                        {
                            var mtx0 = matrices[0];
                            var mtx1 = mtx0 * mtxOffset;
                            var tristrips = GenerateTristripsLine(new Matrix4x4[] { mtx0, mtx1 }, endpointA, endpointB, Vector3.back, 1, false);
                            foreach (var tristrip in tristrips)
                                tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount);

                            allTristrips.AddRange(tristrips);
                        }
                        {
                            var mtx0 = matrices[matrices.Length - 1];
                            var mtx1 = mtx0 * mtxOffset;
                            var tristrips = GenerateTristripsLine(new Matrix4x4[] { mtx0, mtx1 }, endpointA, endpointB, Vector3.forward, 1, true);
                            foreach (var tristrip in tristrips)
                                tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount);

                            allTristrips.AddRange(tristrips);
                        }
                    }

                    return allTristrips.ToArray();
                }

                public static Tristrip[] CreateRails(Matrix4x4[] matrices, GfzTrackRoad node)
                {
                    var allTristrips = new List<Tristrip>();

                    // rail left
                    if (node.RailHeightLeft > 0f)
                    {
                        var endpointA = new Vector3(-0.5f, +0.0f, 0);
                        var endpointB = new Vector3(-0.5f, node.RailHeightLeft, 0);
                        var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.right, 1, true);
                        foreach (var tristrip in tristrips)
                        {
                            tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount);
                            tristrip.tex1 = CreateUVsSideways(tristrip.VertexCount);
                            tristrip.tex2 = CreateUVsSideways(tristrip.VertexCount);
                        }
                        allTristrips.AddRange(tristrips);
                    }

                    // rail right
                    if (node.RailHeightRight > 0f)
                    {
                        var endpointA = new Vector3(+0.5f, +0.0f, 0);
                        var endpointB = new Vector3(+0.5f, node.RailHeightRight, 0);
                        var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.right, 1, true);
                        foreach (var tristrip in tristrips)
                        {
                            tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount);
                            tristrip.tex1 = CreateUVsSideways(tristrip.VertexCount);
                            tristrip.tex2 = CreateUVsSideways(tristrip.VertexCount);
                        }
                        allTristrips.AddRange(tristrips);
                    }

                    return allTristrips.ToArray();
                }

                public static Tristrip[] CreateLaneDividers(Matrix4x4[] matrices, float length)
                {
                    var matricesLeft = new Matrix4x4[matrices.Length];
                    var matricesRight = new Matrix4x4[matrices.Length];

                    var left = Matrix4x4.TRS(new(-0.475f, 0, 0), Quaternion.identity, Vector3.one);
                    var right = Matrix4x4.TRS(new(+0.475f, 0, 0), Quaternion.identity, Vector3.one);
                    for (int i = 0; i < matrices.Length; i++)
                    {
                        matricesLeft[i] = matrices[i] * left;
                        matricesRight[i] = matrices[i] * right;
                    }

                    var matricesNoScale = GetMatricesDefaultScale(matrices, Vector3.one);
                    var matricesLeftNoScale = GetMatricesDefaultScale(matricesLeft, Vector3.one);
                    var matricesRightNoScale = GetMatricesDefaultScale(matricesRight, Vector3.one);

                    var tristrips = new Tristrip[]
                    {
                    GetLaneDivider(matricesNoScale, length),
                    GetLaneDivider(matricesLeftNoScale, length),
                    GetLaneDivider(matricesRightNoScale, length),
                    };

                    return tristrips;
                }

                private static Tristrip GetLaneDivider(Matrix4x4[] matrices, float length)
                {
                    var endpointA = new Vector3(-1.0f, 0.15f, 0);
                    var endpointB = new Vector3(+1.0f, 0.15f, 0);
                    var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.up, 1, true);

                    float repetitions = math.ceil(length / 20f);
                    for (int i = 0; i < tristrips.Length; i++)
                    {
                        var tristrip = tristrips[i];
                        float increment = repetitions / (tristrip.VertexCount / 2 - 1); // length of verts, but not both sides
                        tristrip.tex0 = CreateUVsForward(tristrip.VertexCount, 0, 1, increment, float.PositiveInfinity);
                    }

                    // only one element!
                    return tristrips[0];
                }
            }
        }

    }
}
