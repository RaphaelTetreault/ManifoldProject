using Manifold.IO;
using Manifold.EditorTools.GC.GFZ.Stage.Track;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using static Manifold.EditorTools.GC.GFZ.Stage.Track.TristripGenerator;


namespace Manifold.EditorTools.GC.GFZ
{
    public static class TristripTemplates
    {
        public static class General
        {
            public static Tristrip[] CreateEmbed(Matrix4x4[] matrices, GfzPropertyEmbed embed)
            {
                // I want some kind of offset data on widths so I can map the textures on properly...

                var edgeLeft = new Vector3(-0.5f, 0.40f, 0);
                var edgeRight = new Vector3(+0.5f, 0.40f, 0);
                var tristrips = GenerateTristripsLine(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                float repetitions = math.ceil(embed.GetRangeLength() / 10f);
                for (int i = 0; i < tristrips.Length; i++)
                {
                    var tristrip = tristrips[i];
                    float increment = repetitions / (tristrip.VertexCount / 2 - 1); // length of verts, but not both sides
                    tristrip.tex0 = CreateUVsForward(tristrip.VertexCount, (i + 0) * 2f, (i + 1) * 2f, increment);
                }

                return tristrips;
            }
        }

        public static class Road
        {
            public static Tristrip[] CreateDebug(Matrix4x4[] matrices, GfzSegmentNode node, int nTristrips, float maxStep, bool useGfzCoordSpace)
            {
                var allTriStrips = new List<Tristrip>();
                var hacTRS = node.CreateHierarchichalAnimationCurveTRS(useGfzCoordSpace);
                var segmentLength = hacTRS.GetSegmentLength();

                var settings = GfzProjectWindow.GetSettings();
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
            public static Tristrip[] CreateDebugEmbed(Matrix4x4[] matrices, GfzPropertyEmbed node, int nTristrips, float length)
            {
                var endpointA = new Vector3(-0.5f, 0.33f, 0);
                var endpointB = new Vector3(+0.5f, 0.33f, 0);
                var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.up, nTristrips, true);
                Color32 color0 = node.GetColor();

                for (int i = 0; i < tristrips.Length; i++)
                {
                    var tristrip = tristrips[i];
                    tristrip.color0 = ArrayUtility.DefaultArray(color0, tristrip.VertexCount);
                }

                return tristrips;
            }

            public static class MuteCity
            {
                // TODO: embellishments (part with red), adjust verts to match game, fix "end caps"
                // TODO: for future elements, make this generic when possible. ie: reuse mesh between, say,
                //          Mute City and Green Plant? etc.

                private const float LengthSides = 64f; // 16x1 texture, sooo?

                public static Tristrip[] CreateRoadTop(Matrix4x4[] matrices, float length, int nTristrips, int repeatUvX = 2)
                {
                    var matricesInset = ModifyMatrixScales(matrices, new Vector3(-3.75f, 0, 0));
                    var endpointA = new Vector3(-0.5f, 0, 0);
                    var endpointB = new Vector3(+0.5f, 0, 0);
                    var tristrips = GenerateTristripsLine(matricesInset, endpointA, endpointB, Vector3.up, nTristrips, true);

                    float repetitions = math.ceil(length / 80f);
                    float modulus = float.PositiveInfinity;
                    for (int i = 0; i < tristrips.Length; i++)
                    {
                        var tristrip = tristrips[i];
                        float increment = repetitions / (tristrip.VertexCount / 2 - 1); // length of verts, but not both sides
                        tristrip.tex0 = CreateUVsForward(tristrip.VertexCount, i + 0, i + 1, increment, modulus);
                    }

                    return tristrips;
                }

                public static Tristrip[] CreateRoadBottom(Matrix4x4[] matrices, float length, int nTristrips)
                {
                    var endpointA = new Vector3(-0.5f, -1.0f, 0);
                    var endpointB = new Vector3(+0.5f, -1.0f, 0);
                    var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.down, nTristrips, false);

                    float repetitions = math.ceil(length / 40f);
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

                // TODO: width is for endcaps
                public static Tristrip[] CreateRoadSides(Matrix4x4[] matrices, float length, float width)
                {
                    var allTristrips = new List<Tristrip>();

                    // left
                    {
                        var endpointA = new Vector3(-0.5f, +0.5f, 0);
                        var endpointB = new Vector3(-0.5f, -1.0f, 0);
                        var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.right, 1, false);

                        float repetitions = math.ceil(length / LengthSides);
                        float modulus = float.PositiveInfinity;
                        for (int i = 0; i < tristrips.Length; i++)
                        {
                            var tristrip = tristrips[i];
                            float increment = repetitions / (tristrip.VertexCount / 2 - 1); // length of verts, but not both sides
                            tristrip.tex0 = CreateUVsForward(tristrip.VertexCount, 0, 1, increment, modulus);
                            //tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount, 0, 1, increment, modulus);
                        }

                        allTristrips.AddRange(tristrips);
                    }
                    // COPY PASTED. MAKE MODULAR?
                    // right
                    {
                        var endpointA = new Vector3(+0.5f, +0.5f, 0);
                        var endpointB = new Vector3(+0.5f, -1.0f, 0);
                        var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.left, 1, true);

                        float repetitions = math.ceil(length / LengthSides);
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

                    // NOTE: you can pack like all of these into a single for loop, no?
                    // so long as tristrip count matches.

                    // tops
                    var matricesLeft = GetNormalizedMatrixWithPositionOffset(matrices, -1f);
                    var matricesRight = GetNormalizedMatrixWithPositionOffset(matrices, +1f);
                    // top left
                    {
                        var endpointA = new Vector3(+0.0f, +0.5f, 0);
                        var endpointB = new Vector3(+1.5f, +0.5f, 0);
                        var endpointC = new Vector3(-1.5f, +0.5f, 0);
                        var tristripsLeft = GenerateTristripsLine(matricesLeft, endpointA, endpointB, Vector3.up, 1, true);
                        var tristripsRight = GenerateTristripsLine(matricesRight, endpointA, endpointC, Vector3.up, 1, false);
                        Assert.IsTrue(tristripsLeft.Length == tristripsRight.Length);
                        float repetitions = math.ceil(length / LengthSides);
                        for (int i = 0; i < tristripsLeft.Length; i++)
                        {
                            var tristripLeft = tristripsLeft[i];
                            var tristripRight = tristripsRight[i];
                            float increment = repetitions / (tristripLeft.VertexCount / 2 - 1); // length of verts, but not both sides
                            tristripLeft.tex0 = CreateUVsForward(tristripLeft.VertexCount, 0, 1, increment, float.PositiveInfinity);
                            tristripRight.tex0 = CreateUVsForward(tristripRight.VertexCount, 0, 1, increment, float.PositiveInfinity);
                        }
                        allTristrips.AddRange(tristripsLeft);
                        allTristrips.AddRange(tristripsRight);
                    }

                    // endcaps
                    {
                        var mtxOffset = Matrix4x4.TRS(new(0, -1.0f, 0), Quaternion.identity, Vector3.one);
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

                public static Tristrip[] CreateRoadEmbellishments(Matrix4x4[] matrices, float length)
                {
                    var allTristrips = new List<Tristrip>();

                    // tops
                    var matricesLeft = GetNormalizedMatrixWithPositionOffset(matrices, -1f);
                    var matricesRight = GetNormalizedMatrixWithPositionOffset(matrices, +1f);
                    // top left
                    {
                        var leftOuter = new Vector3(+1.5f, +0.5f, 0);
                        var rightOuter = new Vector3(-1.5f, +0.5f, 0);
                        var leftInner = new Vector3(+3.75f, +0.0f, 0);
                        var rightInner = new Vector3(-3.75f, +0.0f, 0);
                        var normalLeft = Quaternion.Euler(0, 0, +6.34f) * new Vector3(0, 1); // with a x=2.25, y=0.5, angle is 6.34 degrees
                        var normalRight = Quaternion.Euler(0, 0, -6.34f) * new Vector3(0, 1); // rotate normal, assign. TODO: need coord system??
                        var tristripsLeft = GenerateTristripsLine(matricesLeft, leftOuter, leftInner, normalLeft, 1, true);
                        var tristripsRight = GenerateTristripsLine(matricesRight, rightOuter, rightInner, normalRight, 1, false);
                        Assert.IsTrue(tristripsLeft.Length == tristripsRight.Length);
                        float repetitions = math.ceil(length / LengthSides);
                        for (int i = 0; i < tristripsLeft.Length; i++)
                        {
                            var tristripLeft = tristripsLeft[i];
                            var tristripRight = tristripsRight[i];
                            float increment = repetitions / (tristripLeft.VertexCount / 2 - 1); // length of verts, but not both sides
                            var uvs0 = CreateUVsForward(tristripLeft.VertexCount, 0, 1, increment, float.PositiveInfinity);

                            // these uvs are double on each side
                            var uvs1 = new Vector2[uvs0.Length];
                            uvs0.CopyTo(uvs1, 0);
                            for (int j = 0; j < uvs1.Length; j++)
                                uvs1[j] *= 2f;

                            tristripLeft.tex0 = uvs0;
                            tristripLeft.tex1 = uvs1;

                            tristripRight.tex0 = uvs0;
                            tristripRight.tex1 = uvs1;
                        }
                        allTristrips.AddRange(tristripsLeft);
                        allTristrips.AddRange(tristripsRight);
                    }
                    return allTristrips.ToArray();
                }

                public static Tristrip[] CreateRails(Matrix4x4[] matrices, GfzShapeRoad node)
                {
                    var allTristrips = new List<Tristrip>();

                    // rail left
                    if (node.RailHeightLeft > 0f)
                    {
                        var endpointA = new Vector3(-0.5f, +1.0f, 0);
                        var endpointB = new Vector3(-0.5f, node.RailHeightLeft - 1.0f, 0);
                        var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.right, 1, true);
                        foreach (var tristrip in tristrips)
                        {
                            var uvs = CreateUVsSideways(tristrip.VertexCount);
                            tristrip.tex0 = uvs;
                            tristrip.tex1 = uvs;
                            tristrip.tex2 = uvs;
                        }
                        allTristrips.AddRange(tristrips);
                    }

                    // rail right
                    if (node.RailHeightRight > 0f)
                    {
                        var endpointA = new Vector3(+0.5f, +0.75f, 0);
                        var endpointB = new Vector3(+0.5f, node.RailHeightRight - 1.0f, 0);
                        var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.left, 1, true);
                        foreach (var tristrip in tristrips)
                        {
                            var uvs = CreateUVsSideways(tristrip.VertexCount);
                            tristrip.tex0 = uvs;
                            tristrip.tex1 = uvs;
                            tristrip.tex2 = uvs;
                        }
                        allTristrips.AddRange(tristrips);
                    }

                    return allTristrips.ToArray();
                }

                public static Tristrip[] CreateLaneDividers(Matrix4x4[] matrices, float length)
                {
                    //var matricesLeft = new Matrix4x4[matrices.Length];
                    //var matricesRight = new Matrix4x4[matrices.Length];

                    //var left = Matrix4x4.TRS(new(-0.475f, 0, 0), Quaternion.identity, Vector3.one);
                    //var right = Matrix4x4.TRS(new(+0.475f, 0, 0), Quaternion.identity, Vector3.one);
                    //for (int i = 0; i < matrices.Length; i++)
                    //{
                    //    matricesLeft[i] = matrices[i] * left;
                    //    matricesRight[i] = matrices[i] * right;
                    //}

                    var matricesNoScale = GetMatricesDefaultScale(matrices, Vector3.one);
                    //var matricesLeftNoScale = GetMatricesDefaultScale(matricesLeft, Vector3.one);
                    //var matricesRightNoScale = GetMatricesDefaultScale(matricesRight, Vector3.one);

                    var tristrips = new Tristrip[]
                    {
                    GetLaneDivider(matricesNoScale, length),
                    //GetLaneDivider(matricesLeftNoScale, length),
                    //GetLaneDivider(matricesRightNoScale, length),
                    };

                    return tristrips;
                }

                private static Tristrip GetLaneDivider(Matrix4x4[] matrices, float length)
                {
                    var endpointA = new Vector3(-1.0f, 0.01f, 0);
                    var endpointB = new Vector3(+1.0f, 0.01f, 0);
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