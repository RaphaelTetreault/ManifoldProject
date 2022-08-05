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
            private static readonly Vector3 edgeLeft = new Vector3(-0.5f, kEmbedHeight, 0);
            private static readonly Vector3 edgeRight = new Vector3(+0.5f, kEmbedHeight, 0);
            private const float kEmbedHeight = 0.30f;
            private const float kHealHeight = 0.10f;
            private const float kTrimOffset = 0.75f;
            private const float kTrimRepetitions = 64f;
            private const float kEmbedFlashSlipReps = 20f;
            private const float kEmbedFlashLavaReps = 10f;
            private const float kEmbedFlashDirtReps = 10f;
            private const float kEmbedFlashHealReps = 10f;

            private static void GetNormalizedValues(GfzPropertyEmbed embed, int count, out float[] halfWidth, out float[] offsets, out float[] length)
            {
                halfWidth = new float[count];
                offsets = new float[count];
                length = new float[count];

                for (int i = 0; i < count; i++)
                {
                    float percentage = i / (float)(count - 1);
                    float embedWidth = embed.WidthCurve.EvaluateNormalized(percentage);
                    float embedOffset = embed.OffsetCurve.EvaluateNormalized(percentage);
                    halfWidth[i] = embedWidth * 0.5f;
                    offsets[i] = embedOffset;
                    length[i] = percentage;
                }
            }
            private static Vector2[][] CreateTrackSpaceUVs(GfzPropertyEmbed embed, Tristrip[] tristrips, float[] halfWidth, float[] offsets, float[] lengths, float offsetOffset = 0f)
            {
                var allUVs = new Vector2[tristrips.Length][];
                for (int i = 0; i < tristrips.Length; i++)
                {
                    var tristrip = tristrips[i];
                    var uvs = new Vector2[tristrip.VertexCount];
                    float percent0 = (float)(i + 0) / embed.WidthDivisions;
                    float percent1 = (float)(i + 1) / embed.WidthDivisions;

                    for (int j = 0; j < tristrip.VertexCount; j += 2)
                    {
                        int index = j / 2;
                        float halfWidth0 = halfWidth[index];       // (0 to 0.5) * scaleW (width)
                        float offset0 = offsets[index] + offsetOffset;             // (-0.5 to 0.5) * scaleW (width), some
                        float min = offset0 - halfWidth0;                   // farthest left edge
                        float max = offset0 + halfWidth0;                   // farthest right edge
                        float uvLeft = Mathf.Lerp(min, max, percent0);
                        float uvRight = Mathf.Lerp(min, max, percent1);
                        float uvLength = lengths[index];           // (0 to 1) * scaleL (length)
                        uvs[j + 0] = new Vector2(uvLength, uvLeft);
                        uvs[j + 1] = new Vector2(uvLength, uvRight);
                        //uvs[j + 0] = new Vector2(uvLeft, uvLength);
                        //uvs[j + 1] = new Vector2(uvRight, uvLength);
                    }

                    allUVs[i] = uvs;
                }
                return allUVs;
            }
            private static Vector2[][] ScaleByParentWidthAndCustomLength(Vector2[][] allUVs, Matrix4x4[] parentMatrices, float scaleW, float scaleL)
            {
                var newUVs = new Vector2[allUVs.Length][];
                for (int tristripIndex = 0; tristripIndex < allUVs.Length; tristripIndex++)
                {
                    int count = allUVs[tristripIndex].Length;
                    newUVs[tristripIndex] = new Vector2[count];
                    for (int stepIndex = 0; stepIndex < count; stepIndex += 2)
                    {
                        int matrixIndex = stepIndex / 2;
                        float parentWidth = parentMatrices[matrixIndex].lossyScale.x;
                        Vector2 scale = new Vector2(scaleL, parentWidth * scaleW);
                        newUVs[tristripIndex][stepIndex + 0] = allUVs[tristripIndex][stepIndex + 0] * scale;
                        newUVs[tristripIndex][stepIndex + 1] = allUVs[tristripIndex][stepIndex + 1] * scale;
                    }
                }
                return newUVs;
            }

            public static Tristrip[] CreateSlipGX(Matrix4x4[] matrices, Matrix4x4[] parentMatrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateTristripsLine(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                // Scaling parameters
                const float scaleW = 5f; // makes ice look good, same as game
                float segmentLength = embed.GetRangeLength();
                float scaleL = math.ceil(segmentLength / scaleW);
                // Normalized values used to generate UVs
                GetNormalizedValues(embed, matrices.Length, out float[] halfWidths, out float[] offsets, out float[] lengths);
                // Create UVs
                var uvsNormalized = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths);
                var uvs0 = ScaleByParentWidthAndCustomLength(uvsNormalized, parentMatrices, 1 / scaleW, scaleL);
                var uvs1 = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths, 0.5f);
                // Assign UVS
                for (int i = 0; i < tristrips.Length; i++)
                {
                    tristrips[i].tex0 = uvs0[i]; // blue squares

                    var tex1 = ScaleUVs(uvs1[i], embed.RepeatFlashingUV);
                    tex1 = OffsetUVs(tex1, embed.RepeatFlashingUVOffset);
                    tristrips[i].tex1 = tex1; // flashing
                }

                return tristrips;
            }
            public static Tristrip[] CreateSlipAX(Matrix4x4[] matrices, Matrix4x4[] parentMatrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateTristripsLine(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                // Scaling parameters
                float segmentLength = embed.GetRangeLength();
                float repetitionsAlongLength = math.ceil(segmentLength / 64f);
                // Normalized values used to generate UVs
                GetNormalizedValues(embed, matrices.Length, out float[] halfWidths, out float[] offsets, out float[] lengths);
                // Create UVs
                var uvsNormalized = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths);
                var uvs0 = ScaleByParentWidthAndCustomLength(uvsNormalized, parentMatrices, 1 / 64f, repetitionsAlongLength);
                var uvs1 = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths, 0.5f);
                // Scale
                //for (int i = 0; i < lengths.Length; i++)
                //    lengths[i] *= repetitionsAlongLength;
                //var uvs0 = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths);

                // Assign UVS
                for (int i = 0; i < tristrips.Length; i++)
                {
                    tristrips[i].tex0 = uvs0[i]; // blue squares

                    var tex1 = ScaleUVs(uvs1[i], embed.RepeatFlashingUV);
                    tex1 = OffsetUVs(tex1, embed.RepeatFlashingUVOffset);
                    tristrips[i].tex1 = tex1; // flashing
                }

                return tristrips;
            }

            public static Tristrip[] CreateDirtNoise(Matrix4x4[] matrices, Matrix4x4[] parentMatrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateTristripsLine(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                // Scaling parameters
                const float scaleW = 8f; // just a guess
                float segmentLength = embed.GetRangeLength();
                float scaleL = math.ceil(segmentLength / scaleW);
                // Normalized values used to generate UVs
                GetNormalizedValues(embed, matrices.Length, out float[] halfWidths, out float[] offsets, out float[] lengths);
                // Create UVs
                var uvsNormalized = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths);
                var uvs = ScaleByParentWidthAndCustomLength(uvsNormalized, parentMatrices, 1 / scaleW, scaleL);
                Vector2 uvs1Offset = new Vector2(0.5f, 0.33f) * kEmbedFlashDirtReps;
                // Assign UVS.
                for (int i = 0; i < tristrips.Length; i++)
                {
                    tristrips[i].tex0 = uvs[i]; // noise 1
                    tristrips[i].tex1 = OffsetUVs(ScaleUVs(uvs[i], -1), uvs1Offset); // noise 2, mirrored, offset
                }

                return tristrips;
            }
            public static Tristrip[] CreateDirtAlpha(Matrix4x4[] matrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateTristripsLine(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                // This layer is just a color, so UVS can be whatever
                for (int i = 0; i < tristrips.Length; i++)
                {
                    int nVertices = tristrips[i].VertexCount;
                    tristrips[i].tex0 = new Vector2[nVertices];
                }
                // ... do you even need UVs?

                return tristrips;
            }

            public static Tristrip[] CreateLavaCrag(Matrix4x4[] matrices, Matrix4x4[] parentMatrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateTristripsLine(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                // Scaling parameters
                const float scaleW = 25f; // a guess
                float segmentLength = embed.GetRangeLength();
                float scaleL = math.ceil(segmentLength / scaleW);
                // Normalized values used to generate UVs
                GetNormalizedValues(embed, matrices.Length, out float[] halfWidths, out float[] offsets, out float[] lengths);
                // Create UVs
                var uvsNormalized = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths);
                var uvs0 = ScaleByParentWidthAndCustomLength(uvsNormalized, parentMatrices, 1 / scaleW, scaleL);
                // Assign UVS
                for (int i = 0; i < tristrips.Length; i++)
                {
                    tristrips[i].tex0 = uvs0[i];
                }

                return tristrips;
            }
            public static Tristrip[] CreateLavaAlpha(Matrix4x4[] matrices, Matrix4x4[] parentMatrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateTristripsLine(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                // Scaling parameters
                const float scaleW0 = 15f; // red dot
                const float scaleW1 = 40f; // scan lines.
                float segmentLength = embed.GetRangeLength();
                float scaleL0 = math.ceil(segmentLength / scaleW0);
                float scaleL1 = math.ceil(segmentLength / scaleW1);
                // Normalized values used to generate UVs
                GetNormalizedValues(embed, matrices.Length, out float[] halfWidths, out float[] offsets, out float[] lengths);
                // Create UVs
                var uvsNormalized = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths);
                var uvs0 = ScaleByParentWidthAndCustomLength(uvsNormalized, parentMatrices, 1 / scaleW0, scaleL0);
                var uvs1 = ScaleByParentWidthAndCustomLength(uvsNormalized, parentMatrices, 1 / scaleW1, scaleL1);

                for (int i = 0; i < tristrips.Length; i++)
                {
                    tristrips[i].tex0 = uvs0[i]; // red dots

                    var tex1 = ScaleUVs(uvs1[i], embed.RepeatFlashingUV);
                    tex1 = OffsetUVs(tex1, embed.RepeatFlashingUVOffset);
                    tristrips[i].tex1 = SwapUV(tex1); // scan line effect
                }

                return tristrips;
            }

            public static Tristrip[] CreateRecoverBase(Matrix4x4[] matrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateTristripsLine(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                // Scaling parameters
                float segmentLength = embed.GetRangeLength();
                float repetitionsAlongLength = math.ceil(segmentLength / kEmbedFlashHealReps);
                // Normalized values used to generate UVs
                GetNormalizedValues(embed, matrices.Length, out float[] halfWidths, out float[] offsets, out float[] lengths);
                // Create UVs
                var uvs0 = CreateTristripScaledUVs(tristrips, 2f, repetitionsAlongLength);
                var uvs1 = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths, 0.5f); // flashing white stripe
                // Assign UVS. Both layers use the same UVs
                for (int i = 0; i < tristrips.Length; i++)
                {
                    tristrips[i].tex0 = uvs0[i];

                    var tex1 = ScaleUVs(uvs1[i], embed.RepeatFlashingUV);
                    tex1 = OffsetUVs(tex1, embed.RepeatFlashingUVOffset);
                    tristrips[i].tex1 = SwapUV(tex1);
                }

                return tristrips;
            }
            public static Tristrip[] CreateRecoverAlpha(Matrix4x4[] matrices, GfzPropertyEmbed embed)
            {
                var edgeLeft = General.edgeLeft + Vector3.up * kHealHeight;
                var edgeRight = General.edgeRight + Vector3.up * kHealHeight;
                var tristrips = GenerateTristripsLine(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                // Scaling parameters
                float segmentLength = embed.GetRangeLength();
                float repetitionsAlongLength = math.ceil(segmentLength / kEmbedFlashSlipReps);

                // TODO: do not hardcode, expose as parameter
                var uvs1 = CreateTristripScaledUVs(tristrips, 1f, repetitionsAlongLength);

                // Assign UVS. Both layers use the same UVs
                for (int i = 0; i < tristrips.Length; i++)
                {
                    // just color
                    tristrips[i].tex0 = new Vector2[tristrips[i].VertexCount];
                    // alpha square
                    tristrips[i].tex1 = uvs1[i];
                }

                return tristrips;
            }

            public static Tristrip[] CreateTrim(Matrix4x4[] matrices, GfzPropertyEmbed embed)
            {
                var allTristrips = new List<Tristrip>();

                float angleLeftRight = Mathf.Tan(kEmbedHeight / kTrimOffset) * Mathf.Rad2Deg;
                // Scaling parameters
                float segmentLength = embed.GetRangeLength();
                float repetitionsAlongLength = math.ceil(segmentLength / kTrimRepetitions); // TODO: parameter, not hard coded (or const instead?)

                if (embed.IncludeTrimLeft)
                {
                    var normal = Quaternion.Euler(0, 0, +angleLeftRight) * Vector3.up;
                    var leftTristrips = CreateTrimSide(matrices, normal, -1, kTrimOffset, repetitionsAlongLength);
                    allTristrips.AddRange(leftTristrips);
                }
                if (embed.IncludeTrimRight)
                {
                    var normal = Quaternion.Euler(0, 0, -angleLeftRight) * Vector3.up;
                    var leftTristrips = CreateTrimSide(matrices, normal, +1, kTrimOffset, repetitionsAlongLength);
                    allTristrips.AddRange(leftTristrips);
                }

                var trimEndcapTristrips = CreateTrimEndcaps(matrices, embed, kTrimOffset);
                allTristrips.AddRange(trimEndcapTristrips);

                return allTristrips.ToArray();
            }
            private static Tristrip[] CreateTrimSide(Matrix4x4[] matrices, Vector3 defaultNormal, float directionLR, float width, float repetitionsAlongLength)
            {
                var offsetMatrices = GetNormalizedMatrixWithPositionOffset(matrices, directionLR);

                var outerEdge = new Vector3(width * directionLR, 0, 0);
                var innerEdge = new Vector3(0, kEmbedHeight, 0);
                bool isBackFacing = directionLR > 0;
                var tristrips = GenerateTristripsLine(offsetMatrices, innerEdge, outerEdge, defaultNormal, 1, isBackFacing);

                var uvs = CreateTristripScaledUVs(tristrips, 1, repetitionsAlongLength);
                for (int i = 0; i < tristrips.Length; i++)
                {
                    tristrips[i].tex0 = uvs[i];
                }

                return tristrips;
            }
            private static Tristrip[] CreateTrimEndcaps(Matrix4x4[] matrices, GfzPropertyEmbed embed, float width)
            {
                var allTristrips = new List<Tristrip>();

                //var matricesTop = ModifyMatrixScales(matrices, new Vector3(0, kEmbedHeight, 0)); // raise to top
                var matricesTop = ModifyMatrixPositions(matrices, new Vector3(0, kEmbedHeight, 0));
                var matricesBottom = ModifyMatrixScales(matrices, new Vector3(width * 2f, 0, 0)); // widen base
                int index0 = 0;
                int index1 = matrices.Length - 1;

                var endpointA = new Vector3(-0.5f, 0, 0);
                var endpointB = new Vector3(+0.5f, 0, 0);

                if (embed.IncludeTrimStart)
                {
                    var mtx0 = matricesTop[index0];
                    var mtx1 = matricesBottom[index0];
                    var tristrips = GenerateTristripsLine(new Matrix4x4[] { mtx0, mtx1 }, endpointA, endpointB, Vector3.back, embed.WidthDivisions, false);
                    foreach (var tristrip in tristrips)
                        tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount);
                    allTristrips.AddRange(tristrips);
                }

                if (embed.IncludeTrimEnd)
                {
                    var mtx0 = matricesTop[index1];
                    var mtx1 = matricesBottom[index1];
                    var tristrips = GenerateTristripsLine(new Matrix4x4[] { mtx0, mtx1 }, endpointA, endpointB, Vector3.forward, embed.WidthDivisions, true);
                    foreach (var tristrip in tristrips)
                        tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount);
                    allTristrips.AddRange(tristrips);
                }

                return allTristrips.ToArray();
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

                // TODO: de-hardcode these, put in road as param
                public const float kLengthTrim = 60f;
                public const float kLengthRoadTop = 80f;
                public const float kLengthRoadBottom = kLengthRoadTop / 2f;
                public const float kLengthLaneDivider = kLengthRoadTop / 4f;

                public static Tristrip[] CreateRoadTop(Matrix4x4[] matrices, GfzShapeRoad road, float length)
                {
                    var matricesInset = ModifyMatrixScales(matrices, new Vector3(-3.75f * 2, 0, 0));
                    var endpointA = new Vector3(-0.5f, 0, 0);
                    var endpointB = new Vector3(+0.5f, 0, 0);
                    var tristrips = GenerateTristripsLine(matricesInset, endpointA, endpointB, Vector3.up, road.WidthDivisions, true);

                    float repetitionsAlongLength = math.ceil(length / kLengthRoadTop);
                    var uvs = CreateTristripScaledUVs(tristrips, road.TexRepeatWidthTop, repetitionsAlongLength);
                    for (int i = 0; i < tristrips.Length; i++)
                        tristrips[i].tex0 = uvs[i];

                    return tristrips;
                }

                public static Tristrip[] CreateRoadBottom(Matrix4x4[] matrices, GfzShapeRoad road, float length)
                {
                    var endpointA = new Vector3(-0.5f, -1.0f, 0);
                    var endpointB = new Vector3(+0.5f, -1.0f, 0);
                    var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.down, road.WidthDivisions, false);

                    float repetitionsAlongLength = math.ceil(length / kLengthRoadBottom);
                    var uvs = CreateTristripScaledUVs(tristrips, road.TexRepeatWidthBottom, repetitionsAlongLength);
                    for (int i = 0; i < tristrips.Length; i++)
                        tristrips[i].tex0 = uvs[i];

                    return tristrips;
                }

                // TODO: width is for endcaps
                public static Tristrip[] CreateRoadTrim(Matrix4x4[] matrices, GfzShapeRoad road, float length)
                {
                    var allTristrips = new List<Tristrip>();

                    // left
                    {
                        var endpointA = new Vector3(-0.5f, +0.5f, 0);
                        var endpointB = new Vector3(-0.5f, -1.0f, 0);
                        var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.right, 1, false);

                        float repetitions = math.ceil(length / kLengthTrim);
                        for (int i = 0; i < tristrips.Length; i++)
                        {
                            var tristrip = tristrips[i];
                            float increment = repetitions / (tristrip.VertexCount / 2 - 1); // length of verts, but not both sides
                            tristrip.tex0 = CreateUVsForward(tristrip.VertexCount, 0, 1, increment);
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

                        float repetitions = math.ceil(length / kLengthTrim);
                        for (int i = 0; i < tristrips.Length; i++)
                        {
                            var tristrip = tristrips[i];
                            float left = (i + 0) % 2;
                            float right = (i + 1) % 2;
                            float increment = repetitions / (tristrip.VertexCount / 2 - 1); // length of verts, but not both sides
                            tristrip.tex0 = CreateUVsForward(tristrip.VertexCount, left, right, increment);
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
                        float repetitions = math.ceil(length / kLengthTrim);
                        for (int i = 0; i < tristripsLeft.Length; i++)
                        {
                            var tristripLeft = tristripsLeft[i];
                            var tristripRight = tristripsRight[i];
                            float increment = repetitions / (tristripLeft.VertexCount / 2 - 1); // length of verts, but not both sides
                            tristripLeft.tex0 = CreateUVsForward(tristripLeft.VertexCount, 0, 1, increment);
                            tristripRight.tex0 = CreateUVsForward(tristripRight.VertexCount, 0, 1, increment);
                        }
                        allTristrips.AddRange(tristripsLeft);
                        allTristrips.AddRange(tristripsRight);
                    }

                    // endcaps
                    {
                        var mtxOffset = Matrix4x4.TRS(new(0, -1.5f, 0), Quaternion.identity, Vector3.one);
                        var endpointA = new Vector3(-0.5f, +0.5f, 0);
                        var endpointB = new Vector3(+0.5f, +0.5f, 0);

                        var from = road.GetRoot().Prev.CreateHierarchichalAnimationCurveTRS(false);
                        var self = road.GetRoot().CreateHierarchichalAnimationCurveTRS(false);
                        var to = road.GetRoot().Next.CreateHierarchichalAnimationCurveTRS(false);
                        bool isContinuousFrom = CheckpointUtility.IsContinuousBetweenFromTo(from, self);
                        bool isContinuousTo = CheckpointUtility.IsContinuousBetweenFromTo(self, to);

                        if (!isContinuousFrom)
                        {
                            var mtx0 = matrices[0];
                            var mtx1 = mtx0 * mtxOffset;
                            var tristrips = GenerateTristripsLine(new Matrix4x4[] { mtx0, mtx1 }, endpointA, endpointB, Vector3.back, 1, false);
                            foreach (var tristrip in tristrips)
                                tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount);

                            allTristrips.AddRange(tristrips);
                        }

                        if (!isContinuousTo)
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

                public static Tristrip[] CreateRoadEmbellishments(Matrix4x4[] matrices, GfzShapeRoad road, float length)
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
                        var normalLeft = Quaternion.Euler(0, 0, +6.34f) * Vector3.up; // with a x=2.25, y=0.5, angle is 6.34 degrees
                        var normalRight = Quaternion.Euler(0, 0, -6.34f) * Vector3.up; // rotate normal, assign. TODO: need coord system??
                        var tristripsLeft = GenerateTristripsLine(matricesLeft, leftOuter, leftInner, normalLeft, 1, true);
                        var tristripsRight = GenerateTristripsLine(matricesRight, rightOuter, rightInner, normalRight, 1, false);
                        Assert.IsTrue(tristripsLeft.Length == tristripsRight.Length);
                        float repetitions = math.ceil(length / kLengthTrim);
                        for (int i = 0; i < tristripsLeft.Length; i++)
                        {
                            var tristripLeft = tristripsLeft[i];
                            var tristripRight = tristripsRight[i];
                            float increment = repetitions / (tristripLeft.VertexCount / 2 - 1); // length of verts, but not both sides
                            var uvs0 = CreateUVsForward(tristripLeft.VertexCount, 0, 1, increment);
                            uvs0 = OffsetUVs(uvs0, new Vector2(0, 0.5f)); // offset so light part is not cut off at ends. !! has to match COM road !!

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

                public static Tristrip[] CreateRails(Matrix4x4[] matrices, GfzShapeRoad road)
                {
                    var allTristrips = new List<Tristrip>();

                    // rail left
                    if (road.RailHeightLeft > 0f)
                    {
                        var endpointA = new Vector3(-0.5f, +1.0f, 0);
                        var endpointB = new Vector3(-0.5f, road.RailHeightLeft - 1.0f, 0);
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
                    if (road.RailHeightRight > 0f)
                    {
                        var endpointA = new Vector3(+0.5f, +0.75f, 0);
                        var endpointB = new Vector3(+0.5f, road.RailHeightRight - 1.0f, 0);
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

                public static Tristrip[] CreateLaneDividers(Matrix4x4[] matrices, GfzShapeRoad road, float length)
                {
                    if (road.LaneDividers < 0)
                        return new Tristrip[0];

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
                    var endpointA = new Vector3(-1.0f, 0.04f, 0);
                    var endpointB = new Vector3(+1.0f, 0.04f, 0);
                    var tristrips = GenerateTristripsLine(matrices, endpointA, endpointB, Vector3.up, 1, true);

                    float repetitionsAlongLength = math.ceil(length / kLengthLaneDivider);
                    var uvs = CreateTristripScaledUVs(tristrips, 1, repetitionsAlongLength);
                    for (int i = 0; i < tristrips.Length; i++)
                        tristrips[i].tex0 = uvs[i];

                    // only one element!
                    return tristrips[0];
                }
            }

            public static class MuteCityCOM
            {
                public const float RoadTexStride = 32f;

                public static Tristrip[] Top(Matrix4x4[] matrices, GfzShapeRoad road, float length)
                {
                    var matricesInset = ModifyMatrixScales(matrices, new Vector3(-3.75f * 2, 0, 0));
                    var endpointA = new Vector3(-0.5f, 0, 0);
                    var endpointB = new Vector3(+0.5f, 0, 0);
                    var tristrips = GenerateTristripsLine(matricesInset, endpointA, endpointB, Vector3.up, road.WidthDivisions, true);

                    float repetitionsRoadTexture = math.ceil(length / RoadTexStride);
                    var uvs0 = CreateTristripScaledUVs(tristrips, 8, repetitionsRoadTexture);

                    float repetitionsLaneDividers = Round(math.ceil(length / (MuteCity.kLengthTrim/2)), 2); // div-by-2 due to mirroring (need 2 reps for fulll texture)
                    Vector2[][] uvs1 = CreateTristripScaledUVs(tristrips, 2, repetitionsLaneDividers);
                    Vector2 offset1 = new Vector2(0, 1);

                    for (int i = 0; i < tristrips.Length; i++)
                    {
                        tristrips[i].tex0 = uvs0[i];
                        tristrips[i].tex1 = OffsetUVs(uvs1[i], offset1);
                    }

                    return tristrips;
                }

            }


            private static float Round(float value, float roundToNearest)
            {
                var newValue = math.ceil(value / roundToNearest) * roundToNearest;
                return newValue;
            }
        }

    }
}