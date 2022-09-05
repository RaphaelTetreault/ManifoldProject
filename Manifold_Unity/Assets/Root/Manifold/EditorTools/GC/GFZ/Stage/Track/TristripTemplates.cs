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
        #region CONSTANTS

        const float RoadTexStride = 32f;

        #endregion


        /// <summary>
        /// Rotates <paramref name="normal"/> in direction <paramref name="isClockwise"/> based on the
        /// retulst from Tan(<paramref name="opposite"/> / <paramref name="adjacent"/>).
        /// </summary>
        /// <param name="opposite">Tan parameter opposite</param>
        /// <param name="adjacent">Tan parameter adjacent</param>
        /// <param name="normal">Default normal before rotation</param>
        /// <param name="isClockwise">Whether or not to rotate in the clockwise direction</param>
        /// <returns></returns>
        public static Vector3 SurfaceNormalTOA(float opposite, float adjacent, Vector3 normal, bool isClockwise)
        {
            float angleRad = Mathf.Tan(opposite / adjacent);
            float angleDeg = angleRad * Mathf.Rad2Deg;

            // Clockwise rotation is negative about the Z axis
            if (isClockwise)
                angleDeg = -angleDeg;

            Quaternion rotation = Quaternion.Euler(0, 0, angleDeg);
            Vector3 rotatedNormal = rotation * normal;
            return rotatedNormal;
        }
        public static void GetContinuity(GfzSegmentNode node, out bool isContinuousFrom, out bool isContinuousTo)
        {
            var root = node.GetRoot();
            var prev = root.Prev;
            var next = root.Next;

            var from = prev.CreateHierarchichalAnimationCurveTRS(false);
            var self = root.CreateHierarchichalAnimationCurveTRS(false);
            var to = next.CreateHierarchichalAnimationCurveTRS(false);

            isContinuousFrom = CheckpointUtility.IsContinuousBetweenFromTo(from, self);
            isContinuousTo = CheckpointUtility.IsContinuousBetweenFromTo(self, to);

            var rootShapes = root.GetShapeNodes();
            var prevShapes = prev.GetShapeNodes();
            var nextShapes = next.GetShapeNodes();

            // Do a check to see if the two segments are the same shape. If not,
            // force continuity to false.
            foreach (var shape in rootShapes)
            {
                if (shape.ShapeIdentifier == GfzShape.ShapeID.embed)
                    continue;

                bool canBeContinuousFrom = CompareShapeIDs(shape, prevShapes);
                bool canBeContinuousTo = CompareShapeIDs(shape, nextShapes);
                isContinuousFrom &= canBeContinuousFrom;
                isContinuousTo &= canBeContinuousTo;
            }
        }
        private static bool CompareShapeIDs(GfzShape selfShape, GfzShape[] shapes)
        {
            foreach (var shape in shapes)
            {
                // Skip embeds
                if (shape.ShapeIdentifier == GfzShape.ShapeID.embed)
                    continue;

                // See if from-to are the same archetype
                // TODO: handle branches properly if one branch were to be a different type from another.
                if (shape.ShapeIdentifier != selfShape.ShapeIdentifier)
                    return false;
            }
            return true;
        }

        private static float RoundUp(float value, float roundToNearest)
        {
            var newValue = math.ceil(value / roundToNearest) * roundToNearest;
            return newValue;
        }
        private static float RoundDown(float value, float roundToNearest)
        {
            var newValue = math.floor(value / roundToNearest) * roundToNearest;
            return newValue;
        }


        //private static void ApplyUVs(Vector2[][] uvs, Tristrip[] tristrips)
        //{
        //    for (int i = 0; i < tristrips.Length; i++)
        //    {
        //        tristrips[i].te = uvs[i];
        //    }
        //}

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


            public static Tristrip[] CreateSlipLight(Matrix4x4[] matrices, Matrix4x4[] parentMatrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateHorizontalLineWithNormals(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

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
            public static Tristrip[] CreateSlipDarkWide(Matrix4x4[] matrices, Matrix4x4[] parentMatrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateHorizontalLineWithNormals(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                // Scaling parameters
                float segmentLength = embed.GetRangeLength();
                float repetitionsAlongLength = math.ceil(segmentLength / 64f);
                // Normalized values used to generate UVs
                GetNormalizedValues(embed, matrices.Length, out float[] halfWidths, out float[] offsets, out float[] lengths);
                // Create UVs
                var uvsNormalized = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths);
                var uvs0 = ScaleByParentWidthAndCustomLength(uvsNormalized, parentMatrices, 1 / 64f, repetitionsAlongLength);
                var uvs1 = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths, 0.5f);
                for (int i = 0; i < tristrips.Length; i++)
                {
                    tristrips[i].tex0 = uvs0[i]; // blue squares

                    var tex1 = ScaleUVs(uvs1[i], embed.RepeatFlashingUV);
                    tex1 = OffsetUVs(tex1, embed.RepeatFlashingUVOffset);
                    tristrips[i].tex1 = tex1; // flashing
                }

                return tristrips;
            }
            public static Tristrip[] CreateSlipDarkThin(Matrix4x4[] matrices, Matrix4x4[] parentMatrices, GfzPropertyEmbed embed)
            {
                var tristrips = CreateSlipDarkWide(matrices, parentMatrices, embed);
                // Shrink UVs by 4 since tex is 1/4 width
                foreach (var tristrip in tristrips)
                    MutateScaleUVs(tristrip.tex0, new Vector2(1, 4));

                return tristrips;
            }

            public static Tristrip[] CreateDirtNoise(Matrix4x4[] matrices, Matrix4x4[] parentMatrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateHorizontalLineWithNormals(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

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
                var tristrips = GenerateHorizontalLineWithNormals(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

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
                var tristrips = GenerateHorizontalLineWithNormals(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

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
                var tristrips = GenerateHorizontalLineWithNormals(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

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
                    tristrips[i].tex1 = SwizzleUVs(tex1); // scan line effect
                }

                return tristrips;
            }

            public static Tristrip[] CreateRecoverBase(Matrix4x4[] matrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateHorizontalLineWithNormals(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

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
                    tristrips[i].tex1 = SwizzleUVs(tex1);
                }

                return tristrips;
            }
            public static Tristrip[] CreateRecoverAlpha(Matrix4x4[] matrices, GfzPropertyEmbed embed)
            {
                var edgeLeft = General.edgeLeft + Vector3.up * kHealHeight;
                var edgeRight = General.edgeRight + Vector3.up * kHealHeight;
                var tristrips = GenerateHorizontalLineWithNormals(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

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
                var tristrips = GenerateHorizontalLineWithNormals(offsetMatrices, innerEdge, outerEdge, defaultNormal, 1, isBackFacing);

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
                var matricesTop = ModifyMatrixScaledPositions(matrices, new Vector3(0, kEmbedHeight, 0));
                var matricesBottom = ModifyMatrixScales(matrices, new Vector3(width * 2f, 0, 0)); // widen base
                int index0 = 0;
                int index1 = matrices.Length - 1;

                var endpointA = new Vector3(-0.5f, 0, 0);
                var endpointB = new Vector3(+0.5f, 0, 0);

                if (embed.IncludeTrimStart)
                {
                    var mtx0 = matricesTop[index0];
                    var mtx1 = matricesBottom[index0];
                    var tristrips = GenerateHorizontalLineWithNormals(new Matrix4x4[] { mtx0, mtx1 }, endpointA, endpointB, Vector3.back, embed.WidthDivisions, false);
                    foreach (var tristrip in tristrips)
                        tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount);
                    allTristrips.AddRange(tristrips);
                }

                if (embed.IncludeTrimEnd)
                {
                    var mtx0 = matricesTop[index1];
                    var mtx1 = matricesBottom[index1];
                    var tristrips = GenerateHorizontalLineWithNormals(new Matrix4x4[] { mtx0, mtx1 }, endpointA, endpointB, Vector3.forward, embed.WidthDivisions, true);
                    foreach (var tristrip in tristrips)
                        tristrip.tex0 = CreateUVsSideways(tristrip.VertexCount);
                    allTristrips.AddRange(tristrips);
                }

                return allTristrips.ToArray();
            }
        }

        public static class Road
        {
            public const float kCurbHeight = 0.5f;
            public const float kCurbSlantOuter = 1.5f;
            public const float kCurbSlantInner = kCurbSlantOuter + 2.25f; // 3.75f


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
                    var matrix0 = hacTRS.EvaluateHierarchyMatrix(time);
                    var matrix1 = hacTRS.EvaluateHierarchyMatrix(time + 3f);// 3 units forward
                    var matrices01 = new Matrix4x4[] { matrix0, matrix1 };
                    matrices01 = StripHeight(matrices01);

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
                var tristrips = GenerateHorizontalLineWithNormals(matrices, endpointA, endpointB, Vector3.up, nTristrips, true);
                Color32 color0 = node.GetColor();

                for (int i = 0; i < tristrips.Length; i++)
                {
                    var tristrip = tristrips[i];
                    tristrip.color0 = ArrayUtility.DefaultArray(color0, tristrip.VertexCount);
                }

                return tristrips;
            }

            // TODO: parameterize vertices for future modulation?
            internal static Tristrip[] StandardTop(Matrix4x4[] matrices, int widthDivisions, float inset = 3.75f)
            {
                // Make road width equal to width minus the inset on both sides
                var matricesInset = ModifyMatrixScales(matrices, new Vector3(inset * -2f, 0, 0));
                var endpointA = new Vector3(-0.5f, 0, 0); // left
                var endpointB = new Vector3(+0.5f, 0, 0); // right
                var tristrips = GenerateHorizontalLineWithNormals(matricesInset, endpointA, endpointB, Vector3.up, widthDivisions, true);
                return tristrips;
            }
            internal static Tristrip[] StandardBottom(Matrix4x4[] matrices, int widthDivisions, float thickness, float inset = 0f)
            {
                var matricesInset = ModifyMatrixScales(matrices, new Vector3(inset * -2f, thickness - 1f, 0));
                var endpointA = new Vector3(-0.5f, -1f, 0); // left
                var endpointB = new Vector3(+0.5f, -1f, 0); // right
                var tristrips = GenerateHorizontalLineWithNormals(matricesInset, endpointA, endpointB, Vector3.down, widthDivisions, false);
                return tristrips;
            }
            public static Tristrip[] StandardCurbSlant(Matrix4x4[] matrices)
            {
                var allTristrips = new List<Tristrip>();
                var matricesLeft = GetNormalizedMatrixWithPositionOffset(matrices, -1f);
                var matricesRight = GetNormalizedMatrixWithPositionOffset(matrices, +1f);

                var outerLeft = new Vector3(+kCurbSlantOuter, kCurbHeight, 0);
                var outerRight = new Vector3(-kCurbSlantOuter, kCurbHeight, 0);
                var innerLeft = new Vector3(+kCurbSlantInner, 0.0f, 0);
                var innerRight = new Vector3(-kCurbSlantInner, 0.0f, 0);
                var normalLeft = SurfaceNormalTOA(kCurbHeight, kCurbSlantOuter, Vector3.up, true);
                var normalRight = SurfaceNormalTOA(kCurbHeight, kCurbSlantOuter, Vector3.up, false);

                var tristripsLeft = GenerateHorizontalLineWithNormals(matricesLeft, outerLeft, innerLeft, normalLeft, 1, true);
                var tristripsRight = GenerateHorizontalLineWithNormals(matricesRight, outerRight, innerRight, normalRight, 1, false);
                allTristrips.AddRange(tristripsLeft);
                allTristrips.AddRange(tristripsRight);

                return allTristrips.ToArray();
            }
            public static Tristrip[] StandardCurbFlat(Matrix4x4[] matrices)
            {
                var allTristrips = new List<Tristrip>();
                var matricesLeft = GetNormalizedMatrixWithPositionOffset(matrices, -1f);
                var matricesRight = GetNormalizedMatrixWithPositionOffset(matrices, +1f);

                var outerLeft = new Vector3(0, kCurbHeight, 0);
                var outerRight = new Vector3(0, kCurbHeight, 0);
                var innerLeft = new Vector3(+kCurbSlantOuter, kCurbHeight, 0);
                var innerRight = new Vector3(-kCurbSlantOuter, kCurbHeight, 0);

                var tristripsLeft = GenerateHorizontalLineWithNormals(matricesLeft, outerLeft, innerLeft, Vector3.up, 1, true);
                var tristripsRight = GenerateHorizontalLineWithNormals(matricesRight, outerRight, innerRight, Vector3.up, 1, false);
                allTristrips.AddRange(tristripsLeft);
                allTristrips.AddRange(tristripsRight);

                return allTristrips.ToArray();
            }
            public static Tristrip[] StandardSides(Matrix4x4[] matrices, float insetTop, float insetBottom, float height, float thickness)
            {
                var matricesTop = ModifyMatrixScales(matrices, new Vector3(insetTop * -2f, 0, 0));
                var matricesBottom = ModifyMatrixScales(matrices, new Vector3(insetBottom * -2f, 0, 0));

                var upperLeft = new Vector3(-0.5f, height, 0);
                var lowerLeft = new Vector3(-0.5f, -thickness, 0);
                var upperRight = new Vector3(+0.5f, height, 0);
                var lowerRight = new Vector3(+0.5f, -thickness, 0);

                var opposite = insetTop - insetBottom;
                var adjacent = height + thickness;
                var normalLeft = SurfaceNormalTOA(opposite, adjacent, Vector3.left, true);
                var normalRight = SurfaceNormalTOA(opposite, adjacent, Vector3.right, true);

                int vertexCount = matrices.Length * 2;
                var tristripLeft = new Tristrip();
                var tristripRight = new Tristrip();
                var tristrips = new Tristrip[] { tristripLeft, tristripRight };
                foreach (Tristrip tristrip in tristrips)
                {
                    tristrip.positions = new Vector3[vertexCount];
                    tristrip.normals = new Vector3[vertexCount];
                }

                for (int i = 0; i < matrices.Length; i++)
                {
                    var mtxTop = matricesTop[i];
                    var mtxBot = matricesBottom[i];
                    int index0 = i * 2;
                    int index1 = index0 + 1;
                    tristripLeft.positions[index0] = mtxTop.MultiplyPoint(upperLeft);
                    tristripLeft.positions[index1] = mtxBot.MultiplyPoint(lowerLeft);
                    tristripRight.positions[index0] = mtxTop.MultiplyPoint(upperRight);
                    tristripRight.positions[index1] = mtxBot.MultiplyPoint(lowerRight);
                    //
                    var mtx = matrices[i];
                    var normalL = mtx.rotation * normalLeft;
                    var normalR = mtx.rotation * normalRight;
                    tristripLeft.normals[index0] = normalL;
                    tristripLeft.normals[index1] = normalL;
                    tristripRight.normals[index0] = normalR;
                    tristripRight.normals[index1] = normalR;
                }

                return tristrips;
            }
            public static Tristrip[] StandardCurbEndCap(Matrix4x4 matrix, float curbHeight, float curbSlantOuter, float curbSlantInner, float insetBottom, float thickness)
            {
                Vector3 OffsetTop = new Vector3(matrix.lossyScale.x * 0.5f, 0, 0);
                Vector3 OffsetBot = new Vector3(-insetBottom, -thickness, 0);
                Matrix4x4 matrixTopLeft = Matrix4x4.TRS(matrix.Position() + OffsetTop, matrix.rotation, Vector3.one);
                Matrix4x4 matrixTopRight = Matrix4x4.TRS(matrix.Position() - OffsetTop, matrix.rotation, Vector3.one);
                Matrix4x4 matrixBotLeft = Matrix4x4.TRS(matrixTopLeft.Position() + OffsetBot, matrix.rotation, new Vector3(1, 0, 0));
                Matrix4x4 matrixBotRight = Matrix4x4.TRS(matrixTopRight.Position() + OffsetBot, matrix.rotation, new Vector3(1, 0, 0));

                var curbOuterLeftTop = new Vector3(+0.0f, kCurbHeight, 0);
                var curbInnerLeftTop = new Vector3(-curbSlantOuter, curbHeight, 0);
                var curbInnerLeftBottom = new Vector3(-curbSlantInner, 0, 0);
                var curbOuterRightTop = new Vector3(-curbOuterLeftTop.x, curbOuterLeftTop.y, curbOuterLeftTop.z);
                var curbInnerRightTop = new Vector3(-curbInnerLeftTop.x, curbInnerLeftTop.y, curbInnerLeftTop.z);
                var curbInnerRightBottom = new Vector3(-curbInnerLeftBottom.x, curbInnerLeftBottom.y, curbInnerLeftBottom.z);
                Vector3[] VerticesTop = new Vector3[]
                {
                    matrixTopLeft.MultiplyPoint(curbOuterLeftTop),
                    matrixTopLeft.MultiplyPoint(curbInnerLeftTop),
                    matrixTopLeft.MultiplyPoint(curbInnerLeftBottom),
                    matrixTopRight.MultiplyPoint(curbInnerRightBottom),
                    matrixTopRight.MultiplyPoint(curbInnerRightTop),
                    matrixTopRight.MultiplyPoint(curbOuterRightTop),
                };
                Vector3[] VerticesBot = new Vector3[]
                {
                    matrixBotLeft.MultiplyPoint(curbOuterLeftTop),
                    matrixBotLeft.MultiplyPoint(curbInnerLeftTop),
                    matrixBotLeft.MultiplyPoint(curbInnerLeftBottom),
                    matrixBotRight.MultiplyPoint(curbInnerRightBottom),
                    matrixBotRight.MultiplyPoint(curbInnerRightTop),
                    matrixBotRight.MultiplyPoint(curbOuterRightTop),
                };
                Vector3[] positions = new Vector3[VerticesTop.Length + VerticesBot.Length];
                for (int i = 0; i < VerticesTop.Length; i++)
                {
                    int index0 = i * 2;
                    int index1 = index0 + 1;
                    positions[index0] = VerticesTop[i];
                    positions[index1] = VerticesBot[i];
                }

                // Construct tristrip.
                var tristrip = new Tristrip();
                tristrip.positions = positions;
                tristrip.normals = ArrayUtility.DefaultArray(Vector3.up, tristrip.VertexCount);

                // Make array and assign metadata.
                var tristrips = new Tristrip[] { tristrip };
                AssignTristripMetadata(tristrips, true, false);

                return tristrips;
            }
            public static Tristrip[] StandardTrapezoidEndCap(Matrix4x4 matrix, float insetTop, float insetBottom, float heightOffset, float thickness, Vector3 normal)
            {
                Vector3 OffsetTop = new Vector3(matrix.lossyScale.x * 0.5f, 0, 0);
                Vector3 OffsetBot = new Vector3(0, -thickness, 0);
                Matrix4x4 matrixLeftTop = Matrix4x4.TRS(matrix.Position() + OffsetTop, matrix.rotation, Vector3.one);
                Matrix4x4 matrixRightTop = Matrix4x4.TRS(matrix.Position() - OffsetTop, matrix.rotation, Vector3.one);
                Matrix4x4 matrixLeftBot = Matrix4x4.TRS(matrixLeftTop.Position() + OffsetBot, matrix.rotation, new Vector3(1, 0, 0));
                Matrix4x4 matrixRightBot = Matrix4x4.TRS(matrixRightTop.Position() + OffsetBot, matrix.rotation, new Vector3(1, 0, 0));

                Vector3[] positions = new Vector3[]
                {
                    matrixLeftTop.MultiplyPoint(new Vector3(-insetTop, heightOffset, 0)),
                    matrixLeftBot.MultiplyPoint(new Vector3(-insetBottom, heightOffset - thickness, 0)),
                    matrixRightTop.MultiplyPoint(new Vector3(+insetTop, heightOffset, 0)),
                    matrixRightBot.MultiplyPoint(new Vector3(+insetBottom, heightOffset - thickness, 0)),
                };

                // Construct tristrip.
                var tristrip = new Tristrip();
                tristrip.positions = positions;
                tristrip.normals = ArrayUtility.DefaultArray(normal, tristrip.VertexCount);

                // Make array and assign metadata.
                var tristrips = new Tristrip[] { tristrip };
                AssignTristripMetadata(tristrips, true, false);

                return tristrips;
            }

            public static Tristrip[] StandardCurbEndCap(Matrix4x4 matrix, float curbHeight, float curbSlantOuter, float curbSlantInner, float insetBottom, float thickness, Vector2 uv0Scale)
            {
                // Vertical UVs
                float curbTop = 0;
                float curbMid = curbHeight / (curbHeight + thickness);
                float curbBot = 1;
                // Horizontal UVs
                float trackOuter = 0.5f;
                float trackMid = trackOuter - (curbSlantOuter / matrix.lossyScale.x);
                float trackInner = trackOuter - (curbSlantInner / matrix.lossyScale.x);

                // Build out UVs from endcap going top to bottom, left to right
                Vector2[] uvs0 = new Vector2[]
                {
                    new Vector2(curbTop, 0.5f-trackOuter),  new Vector2(curbBot, 0.5f-trackOuter),
                    new Vector2(curbTop, 0.5f-trackMid),    new Vector2(curbBot, 0.5f-trackMid),
                    new Vector2(curbMid, 0.5f-trackInner),  new Vector2(curbBot, 0.5f-trackInner),
                    new Vector2(curbMid, 0.5f+trackInner),  new Vector2(curbBot, 0.5f+trackInner),
                    new Vector2(curbTop, 0.5f+trackMid),    new Vector2(curbBot, 0.5f+trackMid),
                    new Vector2(curbTop, 0.5f+trackOuter),  new Vector2(curbBot, 0.5f+trackOuter),
                };
                MutateScaleUVs(uvs0, new Vector2(uv0Scale.y, uv0Scale.x));

                // Check to make sure UVS are appropriate for tristrips
                var tristrips = StandardCurbEndCap(matrix, curbHeight, curbSlantOuter, curbSlantInner, insetBottom, thickness);
                // Make sure assumptions are checked
                Assert.IsTrue(tristrips.Length == 1);
                Assert.IsTrue(tristrips[0].positions.Length == uvs0.Length);

                // Assign and return
                tristrips[0].tex0 = uvs0;
                return tristrips;
            }
            public static Tristrip[] StandardTrapezoidEndCap(Matrix4x4 matrix, float insetTop, float insetBottom, float heightOffset, float thickness, Vector3 normal, Vector2 uv0Scale)
            {
                float scaleX = matrix.lossyScale.x;
                float upper = (scaleX - insetTop) / scaleX * 0.5f;
                float lower = (scaleX - insetBottom) / scaleX * 0.5f;

                Vector2[] uvs0 = new Vector2[]
                {
                    new Vector2(0, -upper), new Vector2(1, -lower),
                    new Vector2(0, +upper), new Vector2(1, +lower),
                };
                MutateScaleUVs(uvs0, new Vector2(uv0Scale.y, uv0Scale.x));

                // Check to make sure UVS are appropriate for tristrips
                var tristrips = StandardTrapezoidEndCap(matrix, insetTop, insetBottom, heightOffset, thickness, normal);
                // Make sure assumptions are checked
                Assert.IsTrue(tristrips.Length == 1);
                Assert.IsTrue(tristrips[0].positions.Length == uvs0.Length);

                // Assign and return
                tristrips[0].tex0 = uvs0;
                return tristrips;
            }


            public static class MuteCity
            {
                // TODO: embellishments (part with red), adjust verts to match game, fix "end caps"
                // TODO: for future elements, make this generic when possible. ie: reuse mesh between, say,
                //          Mute City and Green Plant? etc.

                // TODO: de-hardcode these, put in road as param
                public const float kLengthTrim = RoadTexStride * 2;
                public const float kLengthRoadTop = RoadTexStride;
                public const float kLengthRoadBottom = RoadTexStride;
                public const float kLengthLaneDivider = RoadTexStride;
                public const float kThickness = 1f;
                public const float kInsetTop = kCurbSlantInner;
                public const float kInsetBottom = 0f;

                public static Tristrip[] Top(Matrix4x4[] matrices, GfzShapeRoad road, float length)
                {
                    var tristrips = StandardTop(matrices, road.WidthDivisions, kInsetTop);

                    float repetitionsAlongLength = math.ceil(length / kLengthRoadTop);
                    var uvs = CreateTristripScaledUVs(tristrips, road.TexRepeatWidthTop, repetitionsAlongLength);
                    for (int i = 0; i < tristrips.Length; i++)
                        tristrips[i].tex0 = uvs[i];

                    return tristrips;
                }

                public static Tristrip[] CreateRoadBottom(Matrix4x4[] matrices, GfzShapeRoad road, float length)
                {
                    var tristrips = StandardBottom(matrices, road.WidthDivisions, kThickness, kInsetBottom);

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
                    float repetitions = math.ceil(length / kLengthTrim);

                    // Tops
                    var matricesLeft = GetNormalizedMatrixWithPositionOffset(matrices, -1f);
                    var matricesRight = GetNormalizedMatrixWithPositionOffset(matrices, +1f);
                    var endpointA = new Vector3(+0.0f, kCurbHeight, 0); // left/right edge of track
                    var endpointB = new Vector3(+1.5f, kCurbHeight, 0); // left-inwards
                    var endpointC = new Vector3(-1.5f, kCurbHeight, 0); // right-inwards
                    var tristripsLeft = GenerateHorizontalLineWithNormals(matricesLeft, endpointA, endpointB, Vector3.up, 1, true);
                    var tristripsRight = GenerateHorizontalLineWithNormals(matricesRight, endpointA, endpointC, Vector3.up, 1, false);
                    var uvs0 = CreateTristripScaledUVs(tristripsLeft, 1, repetitions);
                    for (int i = 0; i < tristripsLeft.Length; i++)
                    {
                        tristripsLeft[i].tex0 = uvs0[i];
                        tristripsRight[i].tex0 = uvs0[i];
                    }

                    // Sides
                    var sides = StandardSides(matrices, 0, 0, kCurbHeight, kThickness);
                    var sidesUvs0 = CreateTristripScaledUVs(sides, sides.Length, repetitions);
                    for (int i = 0; i < sides.Length; i++)
                        sides[i].tex0 = sidesUvs0[i];

                    // EndCaps
                    {
                        GetContinuity(road, out bool isContinuousFrom, out bool isContinuousTo);
                        if (!isContinuousFrom)
                        {
                            var matrix = matrices[0];
                            var uvRepsX = Mathf.Ceil(matrix.lossyScale.x / kLengthTrim);
                            var endcapFrom = StandardCurbEndCap(matrix, kCurbHeight, kCurbSlantOuter, kCurbSlantInner, 0, kThickness, new Vector2(uvRepsX, 1));
                            allTristrips.AddRange(endcapFrom);
                        }
                        if (!isContinuousTo)
                        {
                            var matrix = matrices[matrices.Length - 1];
                            var uvRepsX = Mathf.Ceil(matrix.lossyScale.x / kLengthTrim);
                            var endcapTo = StandardCurbEndCap(matrix, kCurbHeight, kCurbSlantOuter, kCurbSlantInner, 0, kThickness, new Vector2(uvRepsX, 1));
                            allTristrips.AddRange(endcapTo);
                        }
                    }

                    allTristrips.AddRange(tristripsLeft);
                    allTristrips.AddRange(tristripsRight);
                    allTristrips.AddRange(sides);

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
                        var leftOuter = new Vector3(+kCurbSlantOuter, kCurbHeight, 0);
                        var rightOuter = new Vector3(-kCurbSlantOuter, kCurbHeight, 0);
                        var leftInner = new Vector3(+kCurbSlantInner, 0.0f, 0);
                        var rightInner = new Vector3(-kCurbSlantInner, 0.0f, 0);
                        var normalLeft = SurfaceNormalTOA(kCurbHeight, kCurbSlantInner - kCurbSlantOuter, Vector3.up, true);
                        var normalRight = SurfaceNormalTOA(kCurbHeight, kCurbSlantInner - kCurbSlantOuter, Vector3.up, false);
                        var tristripsLeft = GenerateHorizontalLineWithNormals(matricesLeft, leftOuter, leftInner, normalLeft, 1, true);
                        var tristripsRight = GenerateHorizontalLineWithNormals(matricesRight, rightOuter, rightInner, normalRight, 1, false);
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
                        var tristrips = GenerateHorizontalLineWithNormals(matrices, endpointA, endpointB, Vector3.right, 1, true);
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
                        var tristrips = GenerateHorizontalLineWithNormals(matrices, endpointA, endpointB, Vector3.left, 1, true);
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
                    var matricesLeft = ModifyMatrixScaledPositions(matrices, Vector3.left * 0.5f);
                    var matricesRight = ModifyMatrixScaledPositions(matrices, Vector3.right * 0.5f);
                    matricesLeft = ModifyMatrixPositions(matricesLeft, Vector3.right * (kCurbSlantInner + 1f));
                    matricesRight = ModifyMatrixPositions(matricesRight, Vector3.left * (kCurbSlantInner + 1f));

                    var matricesNoScale = GetMatricesDefaultScale(matrices, Vector3.one);
                    var matricesLeftNoScale = GetMatricesDefaultScale(matricesLeft, Vector3.one * 0.75f);
                    var matricesRightNoScale = GetMatricesDefaultScale(matricesRight, Vector3.one * 0.75f);

                    var allTristrips = new List<Tristrip>();
                    var leftTristrip = GetLaneDivider(matricesLeftNoScale, length);
                    var rightTristrip = GetLaneDivider(matricesRightNoScale, length);
                    allTristrips.Add(leftTristrip);
                    allTristrips.Add(rightTristrip);

                    if (road.HasLaneDividers)
                    {
                        var centerTristrip = GetLaneDivider(matricesNoScale, length);
                        allTristrips.Add(centerTristrip);
                    }

                    return allTristrips.ToArray();
                }
                private static Tristrip GetLaneDivider(Matrix4x4[] matrices, float length)
                {
                    var endpointA = new Vector3(-1.0f, 0.04f, 0);
                    var endpointB = new Vector3(+1.0f, 0.04f, 0);
                    var tristrips = GenerateHorizontalLineWithNormals(matrices, endpointA, endpointB, Vector3.up, 1, true);

                    float repetitionsAlongLength = math.ceil(length / kLengthLaneDivider);
                    var uvs = CreateTristripScaledUVs(tristrips, 1, repetitionsAlongLength);
                    for (int i = 0; i < tristrips.Length; i++)
                    {
                        //uvs[i] = ScaleUVs(uvs[i], new Vector2(1, 2));
                        tristrips[i].tex0 = uvs[i];
                    }

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
                    var tristrips = GenerateHorizontalLineWithNormals(matricesInset, endpointA, endpointB, Vector3.up, road.WidthDivisions, true);

                    float repetitionsRoadTexture = math.ceil(length / RoadTexStride);
                    var uvs0 = CreateTristripScaledUVs(tristrips, 8, repetitionsRoadTexture);

                    float repetitionsLaneDividers = RoundUp(math.ceil(length / (MuteCity.kLengthTrim / 2)), 2); // div-by-2 due to mirroring (need 2 reps for full texture)
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

            public static class Aeropolis
            {

            }

            public static class BigBlue
            {
                public static Tristrip[] Top(Matrix4x4[] matrices, GfzShapeRoad road)
                {
                    var tristrips = StandardTop(matrices, road.WidthDivisions);

                    //return tristrips;
                    throw new System.NotImplementedException();
                }
            }

            public static class CasinoPalace
            {

            }

            public static class CosmoTerminal
            {

            }

            public static class FireField
            {

            }

            public static class GreenPlant
            {

            }

            public static class Lightning
            {

            }

            public static class OuterSpace
            {
                const float kTrackThickness = 2f;
                const float kSideInset = 2f;
                const float kLaneDividerHeight = kCurbHeight / 2f; // hack, is actually 0.5f / kCurbHeight
                const float kLaneDividerTop = 4f / 3f; // 1.33
                const float kLaneDividerTopHalf = kLaneDividerTop / 2f;
                const float kLaneDividerSide = 10f / 3f; // 3.33
                const float kLaneDividerSideHalf = kLaneDividerSide / 2f;

                const float RoadTexStride = 32f;

                const float kEndCapTexStride = RoadTexStride;
                const float kEndCapTexHeight = 0.333f;

                /// <summary>
                /// 
                /// </summary>
                /// <param name="matrices">Matrices to position railings</param>
                /// <param name="side">-1f left, +1f right</param>
                /// <param name="height">Full height of rail</param>
                /// <param name="railTristrips">List to add tristrips to</param>
                private static void RailAngle(Matrix4x4[] matrices, float side, float height, List<Tristrip> railTristrips)
                {
                    float railHeight = height * 0.40f; // 40% has 2)
                    float gapHeight = height * 0.10f; // 10% (has 2)
                    var bottom = new Vector3(0f, 0.5f);
                    var mid0 = new Vector3(railHeight * side, railHeight + 0.5f);
                    var mid1 = new Vector3(railHeight * side, railHeight + 0.5f + gapHeight);
                    var top = new Vector3(0f, railHeight * 2 + 0.5f + gapHeight);
                    Vector3 normalBottom = Quaternion.Euler(0, 0, 45 * side) * Vector3.up;
                    Vector3 normalTop = Quaternion.Euler(0, 0, 135 * side) * Vector3.up;

                    var tristripsBottom = GenerateHorizontalLineWithNormals(matrices, bottom, mid0, normalBottom, 1, false, true);
                    var tristripsTop = GenerateHorizontalLineWithNormals(matrices, mid1, top, normalTop, 1, false, true);
                    railTristrips.AddRange(tristripsBottom);
                    railTristrips.AddRange(tristripsTop);
                }
                private static void RailLights(Matrix4x4[] matrices, float side, float height, List<Tristrip> railTristrips)
                {
                    float railHeight = height * 0.40f;
                    float gapHeight = height * 0.10f;
                    var bottom0 = new Vector3(railHeight * side, railHeight + 0.5f);
                    var bottom1 = new Vector3(railHeight * side, railHeight + 0.5f + gapHeight);
                    var top0 = new Vector3(0f, railHeight * 2 + 0.5f + gapHeight);
                    var top1 = new Vector3(gapHeight * -side, railHeight * 2 + 0.5f + gapHeight * 2);
                    // TODO: normals
                    Vector3 normalBottom = Vector3.left * side;
                    Vector3 normalTop = Quaternion.Euler(0, 0, 135 * side) * Vector3.up;

                    var tristripsBottom = GenerateHorizontalLineWithNormals(matrices, bottom0, bottom1, normalBottom, 1, false, true);
                    var tristripsTop = GenerateHorizontalLineWithNormals(matrices, top0, top1, normalTop, 1, false, true);
                    railTristrips.AddRange(tristripsBottom);
                    railTristrips.AddRange(tristripsTop);
                }
                private static void OuterSpaceSlantedSide(Matrix4x4[] matrices, float direction, float inset, float thickness, Vector3 normal, List<Tristrip> railTristrips)
                {
                    var top = new Vector3(0, +kCurbHeight, 0);
                    var bottom = new Vector3(inset * -direction, -thickness, 0);
                    var tristrips = GenerateHorizontalLineWithNormals(matrices, top, bottom, normal, 1, direction > 0);
                    railTristrips.AddRange(tristrips);
                }
                private static void OuterSpaceLaneDividerSide(Matrix4x4[] matrices, float direction, Vector3 normal, List<Tristrip> railTristrips)
                {
                    var innerSide = new Vector3(kLaneDividerTopHalf * direction, kLaneDividerHeight, 0);
                    var outerSide = new Vector3(kLaneDividerSideHalf * direction, 0, 0);
                    var tristrips = GenerateHorizontalLineWithNormals(matrices, innerSide, outerSide, normal, 1, direction > 0);
                    railTristrips.AddRange(tristrips);
                }

                public static Tristrip[] Top(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    var tristrips = StandardTop(matrices, road.WidthDivisions);

                    float repetitionsRoadTexture = math.ceil(segmentLength / RoadTexStride);
                    float repetitionsRoadBloom = math.ceil(segmentLength / 100f);
                    var uvs0 = CreateTristripScaledUVs(tristrips, 6, repetitionsRoadTexture); // metal
                    var uvs1 = CreateTristripScaledUVs(tristrips, 1, repetitionsRoadBloom); // screen
                    var uvs2 = CreateTristripScaledUVs(tristrips, 6, repetitionsRoadTexture); // metal bump map
                    for (int i = 0; i < tristrips.Length; i++)
                    {
                        tristrips[i].tex0 = uvs0[i];
                        tristrips[i].tex1 = uvs1[i];
                        tristrips[i].tex2 = uvs2[i];

                        //// compute tangent and binormal
                        //int vertexCount = tristrips[i].VertexCount;
                        //tristrips[i].tangents = new Vector3[vertexCount];
                        //tristrips[i].binormals = new Vector3[vertexCount];
                        //for (int j = 0; j < tristrips[i].VertexCount; j += 2)
                        //{
                        //    int index = j / 2;
                        //    var matrix = matrices[index];
                        //    var rotation = matrix.rotation;
                        //    tristrips[i].tangents[j] = rotation * Vector3.right;
                        //    tristrips[i].binormals[j] = math.cross(tristrips[i].tangents[j], tristrips[i].normals[j]);
                        //}
                    }

                    return tristrips;
                }

                private static Tristrip[] Bottom(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    var tristrips = StandardBottom(matrices, road.WidthDivisions, kTrackThickness, kSideInset);

                    float repetitionsRoadTexture = math.ceil(segmentLength / RoadTexStride);
                    var uvs0 = CreateTristripScaledUVs(tristrips, 4, repetitionsRoadTexture); // metal
                    for (int i = 0; i < tristrips.Length; i++)
                        tristrips[i].tex0 = uvs0[i];

                    return tristrips;
                }

                private static Tristrip[] Sides(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    var sidesTristrips = new List<Tristrip>();

                    var matricesLeft = GetNormalizedMatrixWithPositionOffset(matrices, -1f);
                    var matricesRight = GetNormalizedMatrixWithPositionOffset(matrices, +1f);
                    OuterSpaceSlantedSide(matricesLeft, -1, kSideInset, kTrackThickness, Vector3.left, sidesTristrips);
                    OuterSpaceSlantedSide(matricesRight, +1, kSideInset, kTrackThickness, Vector3.right, sidesTristrips);

                    var sidesTristripsArray = sidesTristrips.ToArray();
                    float repetitionsRoadTexture = math.ceil(segmentLength / RoadTexStride);
                    var uvsLeft = CreateTristripScaledUVs(sidesTristripsArray, -0.1f, repetitionsRoadTexture); // metal
                    var uvsRight = CreateTristripScaledUVs(sidesTristripsArray, +0.1f, repetitionsRoadTexture); // metal

                    // Kind of a hack, but assign mirrored UVS based on side. Perhaps do this in the fucntion?
                    for (int i = 0; i < sidesTristripsArray.Length; i++)
                        sidesTristripsArray[i].tex0 = i == 0 ? uvsLeft[0] : uvsRight[0];

                    return sidesTristripsArray;
                }

                private static Tristrip[] CurbFlat(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    var tristrips = StandardCurbFlat(matrices);
                    // UVs same across all of this type, done in checkpoint function.
                    return tristrips;
                }

                private static Tristrip[] CurbAngle(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    var tristrips = StandardCurbSlant(matrices);
                    // UVs same across all of this type, done in checkpoint function.
                    return tristrips;
                }

                public static Tristrip[] RailsAngle(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    // Rail slants are EACH 40% of railHeight, are at 45 degree angles
                    var railTristrips = new List<Tristrip>();

                    if (road.RailHeightLeft > 0f)
                    {
                        const float leftSide = -1f;
                        var matricesLeft = GetNormalizedMatrixWithPositionOffset(matrices, leftSide);
                        RailAngle(matricesLeft, leftSide, road.RailHeightLeft, railTristrips);
                    }

                    if (road.RailHeightRight > 0f)
                    {
                        const float rightSide = +1f;
                        var matricesRight = GetNormalizedMatrixWithPositionOffset(matrices, rightSide);
                        RailAngle(matricesRight, rightSide, road.RailHeightRight, railTristrips);
                    }

                    // apply UVs, all generic
                    float repetitionsRoadTexture = math.ceil(segmentLength / RoadTexStride);
                    foreach (var tristrip in railTristrips)
                        tristrip.tex0 = CreateUVsForwardLength(tristrip.VertexCount, 0, 1, repetitionsRoadTexture);

                    return railTristrips.ToArray();
                }

                public static Tristrip[] RailsLights(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    // Rail slants are EACH 40% of railHeight, are at 45 degree angles
                    var railTristrips = new List<Tristrip>();

                    if (road.RailHeightLeft > 0f)
                    {
                        const float leftSide = -1f;
                        var matricesLeft = GetNormalizedMatrixWithPositionOffset(matrices, leftSide);
                        RailLights(matricesLeft, leftSide, road.RailHeightLeft, railTristrips);
                    }

                    if (road.RailHeightRight > 0f)
                    {
                        const float rightSide = +1f;
                        var matricesRight = GetNormalizedMatrixWithPositionOffset(matrices, rightSide);
                        RailLights(matricesRight, rightSide, road.RailHeightRight, railTristrips);
                    }

                    // apply UVs, all generic
                    float repetitionsRoadTexture = math.ceil(segmentLength / RoadTexStride);
                    foreach (var tristrip in railTristrips)
                        tristrip.tex0 = CreateUVsForwardLength(tristrip.VertexCount, 0, 1, repetitionsRoadTexture);

                    return railTristrips.ToArray();
                }

                private static Tristrip[] LaneDividerSides(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    if (!road.HasLaneDividers)
                        return new Tristrip[0];

                    var laneDividerSidesTristrips = new List<Tristrip>();
                    var matricesDefault = GetMatricesDefaultScale(matrices, Vector3.one);
                    float opposite = kLaneDividerHeight;
                    float adjacent = (kLaneDividerSide - kLaneDividerTop) / 2f;
                    var normalLeft = SurfaceNormalTOA(opposite, adjacent, Vector3.up, false);
                    var normalRight = SurfaceNormalTOA(opposite, adjacent, Vector3.up, true);
                    OuterSpaceLaneDividerSide(matricesDefault, -1, normalLeft, laneDividerSidesTristrips);
                    OuterSpaceLaneDividerSide(matricesDefault, +1, normalRight, laneDividerSidesTristrips);
                    // UVs same across all of this type, done in checkpoint function.
                    return laneDividerSidesTristrips.ToArray();
                }

                private static Tristrip[] LaneDividerTop(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    if (!road.HasLaneDividers)
                        return new Tristrip[0];

                    var matricesDefault = GetMatricesDefaultScale(matrices, Vector3.one);
                    var left = new Vector3(-kLaneDividerTopHalf, kLaneDividerHeight, 0); // left
                    var right = new Vector3(+kLaneDividerTopHalf, kLaneDividerHeight, 0); // right
                    var tristrips = GenerateHorizontalLineWithNormals(matricesDefault, left, right, Vector3.up, 1, true);
                    // UVs same across all of this type, done in checkpoint function.
                    return tristrips;
                }

                public static Tristrip[] CurbAndLaneDividerSlants(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    var allTristrips = new List<Tristrip>();
                    allTristrips.AddRange(CurbAngle(matrices, road, segmentLength));
                    allTristrips.AddRange(LaneDividerSides(matrices, road, segmentLength));

                    // UVs are simple 0-1 along width, every desired length repeat
                    float repetitionsRoadTexture = math.ceil(segmentLength / RoadTexStride);
                    for (int i = 0; i < allTristrips.Count; i++)
                        allTristrips[i].tex0 = CreateUVsForwardLength(allTristrips[i].VertexCount, 0, 1, repetitionsRoadTexture);

                    return allTristrips.ToArray();
                }

                public static Tristrip[] CurbAndLaneDividerFlat(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    var allTristrips = new List<Tristrip>();
                    allTristrips.AddRange(CurbFlat(matrices, road, segmentLength));
                    allTristrips.AddRange(LaneDividerTop(matrices, road, segmentLength));

                    // UVs are simple 0-1 along width, every desired length repeat
                    float repetitionsRoadTexture = math.ceil(segmentLength / RoadTexStride);
                    for (int i = 0; i < allTristrips.Count; i++)
                    {
                        var uvs = CreateUVsForwardLength(allTristrips[i].VertexCount, 0, 1, repetitionsRoadTexture);
                        MutateScaleUVs(uvs, new Vector2(2, 1)); // requires mirroring X, so double X
                        allTristrips[i].tex0 = uvs;
                    }

                    return allTristrips.ToArray();
                }

                public static Tristrip[] BottomAndSides(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    var allTristrips = new List<Tristrip>();
                    allTristrips.AddRange(Bottom(matrices, road, segmentLength));
                    allTristrips.AddRange(Sides(matrices, road, segmentLength));
                    // UVs are different between these, so resolved in their respective functions.
                    return allTristrips.ToArray();
                }

                public static Tristrip[] EndCaps(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    var allTristrips = new List<Tristrip>();
                    GetContinuity(road, out bool isContinuousFrom, out bool isContinuousTo);

                    if (!isContinuousFrom)
                    {
                        var matrix = matrices[0];
                        var uvRepsX = Mathf.Ceil(matrix.lossyScale.x / kEndCapTexStride);
                        var tristrips = StandardTrapezoidEndCap(matrix, 0, kSideInset, kCurbHeight, kTrackThickness, Vector3.back, new Vector2(uvRepsX, kEndCapTexHeight));
                        allTristrips.AddRange(tristrips);
                    }

                    if (!isContinuousTo)
                    {
                        var matrix = matrices[matrices.Length - 1];
                        var uvRepsX = Mathf.Ceil(matrix.lossyScale.x / kEndCapTexStride);
                        var tristrips = StandardTrapezoidEndCap(matrix, 0, kSideInset, kCurbHeight, kTrackThickness, Vector3.forward, new Vector2(uvRepsX, kEndCapTexHeight));
                        allTristrips.AddRange(tristrips);
                    }

                    return allTristrips.ToArray();
                }

            }

            public static class PhantomRoad
            {

            }

            public static class PortTown
            {

            }

            public static class SandOcean
            {

            }

        }

        public static class Pipe
        {
            private static Tristrip CircleEndcap(Matrix4x4 matrix, int nTristrips, Vector3 normal, bool isBackFacing)
            {
                var innerMatrix = matrix;
                var outerMatrix = Matrix4x4.TRS(matrix.Position(), matrix.rotation, matrix.lossyScale + Vector3.one * 5);

                var vertices = CreateCircleVertices(nTristrips);

                var tristrip = new Tristrip();
                int vertexCount = vertices.Length * 2;
                tristrip.positions = new Vector3[vertexCount];
                tristrip.normals = ArrayUtility.DefaultArray(normal, vertexCount);
                for (int i = 0; i < vertices.Length; i++)
                {
                    int index0 = i * 2;
                    int index1 = index0 + 1;
                    Vector3 vertex = vertices[i];
                    tristrip.positions[index0] = innerMatrix.MultiplyPoint(vertex);
                    tristrip.positions[index1] = outerMatrix.MultiplyPoint(vertex);
                }

                // Make array to assign data
                var tristrips = new Tristrip[] { tristrip };
                AssignTristripMetadata(tristrips, isBackFacing, false);

                return tristrip;
            }

            public static Tristrip[] DebugInside(Matrix4x4[] matrices, GfzShapePipeCylinder pipe, bool isGfzCoordinateSpace)
            {
                var inner = GenerateCircleWithNormals(matrices, false, pipe.SubdivisionsInside, true, isGfzCoordinateSpace);
                return inner;
            }
            public static Tristrip[] DebugOutside(Matrix4x4[] matrices, GfzShapePipeCylinder pipe, bool isGfzCoordinateSpace)
            {
                var grownMatrices = ModifyMatrixScales(matrices, Vector3.one * 5);
                var outer = GenerateCircleWithNormals(grownMatrices, true, pipe.SubdivisionsOutside, true, isGfzCoordinateSpace);
                return outer;
            }
            public static Tristrip[] DebugRingEndcap(Matrix4x4[] matrices, GfzShapePipeCylinder pipe)
            {
                var tristrips = new List<Tristrip>();
                GetContinuity(pipe, out bool isContinuousFrom, out bool isContinuousTo);

                if (!isContinuousFrom)
                {
                    var matrix = matrices[0];
                    var normal = matrix.rotation * Vector3.back;
                    var tristrip = CircleEndcap(matrix, pipe.SubdivisionsInside, normal, false);
                    tristrips.Add(tristrip);
                }

                if (!isContinuousTo)
                {
                    int lastIndex = matrices.Length - 1;
                    var matrix = matrices[lastIndex];
                    var normal = matrix.rotation * Vector3.forward;
                    var tristrip = CircleEndcap(matrix, pipe.SubdivisionsInside, normal, true);
                    tristrips.Add(tristrip);
                }

                return tristrips.ToArray();
            }

            public static Tristrip[] GenericInsideOneTexture(Matrix4x4[] matrices, GfzShapePipeCylinder cylinder, float segmentLength, bool isGfzCoordinateSpace)
            {
                var tristrips = DebugInside(matrices, cylinder, isGfzCoordinateSpace);

                float repetitionsRoadTexture = math.ceil(segmentLength / RoadTexStride);
                var uvs0 = CreateTristripScaledUVs(tristrips, 8, repetitionsRoadTexture);
                for (int i = 0; i < tristrips.Length; i++)
                    tristrips[i].tex0 = uvs0[i];

                return tristrips;
            }
        }

        public static class Cylinder
        {
            private static Tristrip Endcap(Matrix4x4 matrix, int nTristrips, Vector3 normal, bool isBackFacing)
            {
                var vertices = CreateCircleVertices(nTristrips);

                var tristrip = new Tristrip();
                int vertexCount = vertices.Length;
                tristrip.positions = new Vector3[vertices.Length];
                tristrip.normals = ArrayUtility.DefaultArray(normal, vertices.Length);

                for (int i = 0; i < vertexCount - 1; i++)
                {
                    bool isEven = i % 2 == 0;
                    int halfI = i / 2;
                    int index = isEven
                        ? vertexCount - halfI - 1
                        : halfI + 1;

                    Vector3 vertex = vertices[index];
                    vertex = matrix.MultiplyPoint(vertex);
                    tristrip.positions[i] = vertex;
                }

                // Make array to assign data
                var tristrips = new Tristrip[] { tristrip };
                AssignTristripMetadata(tristrips, isBackFacing, false);

                return tristrip;
            }

            public static Tristrip[] Debug(Matrix4x4[] matrices, GfzShapePipeCylinder cylinder, bool isGfzCoordinateSpace)
            {
                var tristrips = GenerateCircleWithNormals(matrices, true, cylinder.SubdivisionsInside, true, isGfzCoordinateSpace);
                return tristrips;
            }
            public static Tristrip[] DebugEndcap(Matrix4x4[] matrices, GfzShapePipeCylinder pipe)
            {
                var tristrips = new List<Tristrip>();
                GetContinuity(pipe, out bool isContinuousFrom, out bool isContinuousTo);

                if (!isContinuousFrom)
                {
                    var matrix = matrices[0];
                    var scale = matrix.lossyScale;
                    bool hasValidScale = scale.x != 0 && scale.y != 0;
                    if (hasValidScale)
                    {
                        var normal = matrix.rotation * Vector3.back;
                        var tristrip = Endcap(matrix, pipe.SubdivisionsInside, normal, false);
                        tristrips.Add(tristrip);
                    }
                }

                if (!isContinuousTo)
                {
                    int lastIndex = matrices.Length - 1;
                    var matrix = matrices[lastIndex];
                    var scale = matrix.lossyScale;
                    bool hasValidScale = scale.x != 0 && scale.y != 0;
                    if (hasValidScale)
                    {
                        var normal = matrix.rotation * Vector3.forward;
                        var tristrip = Endcap(matrix, pipe.SubdivisionsInside, normal, true);
                        tristrips.Add(tristrip);
                    }
                }

                return tristrips.ToArray();
            }

            public static Tristrip[] GenericOneTexture(Matrix4x4[] matrices, GfzShapePipeCylinder cylinder, float segmentLength, bool isGfzCoordinateSpace)
            {
                var tristrips = Debug(matrices, cylinder, isGfzCoordinateSpace);

                float repetitionsRoadTexture = math.ceil(segmentLength / RoadTexStride);
                var uvs0 = CreateTristripScaledUVs(tristrips, 8, repetitionsRoadTexture);
                for (int i = 0; i < tristrips.Length; i++)
                    tristrips[i].tex0 = uvs0[i];

                return tristrips;
            }
        }

        public static class CapsulePipe
        {
            const float capsuleThickness = 15f;
            const float capsuleThicknessHalf = capsuleThickness / 2f;

            public static Tristrip[] DebugInside(Matrix4x4[] matricesLeft, Matrix4x4[] matricesRight, Matrix4x4[] matricesTop, Matrix4x4[] matricesBottom, GfzShapeCapsule capsule, bool isGfzCoordinateSpace)
            {
                Vector3 left = Vector3.left * 0.5f;
                Vector3 right = Vector3.right * 0.5f;
                Vector3 up = Vector3.up;

                // Create all tristrips. We build in 4 sections: 2 semi-circles, 2 line segments.
                var sideLeft = GenerateCircle(matricesLeft, true, capsule.SubdivideSemiCircleInside, 180, 0);
                var sideRight = GenerateCircle(matricesRight, true, capsule.SubdivideSemiCircleInside, 360, 180);
                var sideTop = GenerateHorizontalLineWithNormals(matricesTop, left, right, up, capsule.SubdivideLineInside, true);
                var sideBot = GenerateHorizontalLineWithNormals(matricesBottom, left, right, up, capsule.SubdivideLineInside, true);

                // Combine all tristrips. Order matters! For the normalization smoothing to work, tristrips need
                // to be sequential so overlapping normals overlap the correct normals.
                var allTristrips = new List<Tristrip>();
                allTristrips.AddRange(sideTop);
                allTristrips.AddRange(sideRight);
                allTristrips.AddRange(sideBot);
                allTristrips.AddRange(sideLeft);
                var tristripsArray = allTristrips.ToArray();

                // Add normals based on vertices. Smooth normals.
                SetNormalsFromTristripVertices(tristripsArray, false, true, isGfzCoordinateSpace);

                return tristripsArray;
            }
            public static Tristrip[] DebugOutside(Matrix4x4[] matricesLeft, Matrix4x4[] matricesRight, Matrix4x4[] matricesTop, Matrix4x4[] matricesBottom, GfzShapeCapsule capsule, bool isGfzCoordinateSpace)
            {
                Vector3 left = Vector3.left * 0.5f;
                Vector3 right = Vector3.right * 0.5f;
                Vector3 up = Vector3.up;

                Vector3 scaleMod = Vector3.one * capsuleThickness;
                matricesLeft = ModifyMatrixScales(matricesLeft, scaleMod);
                matricesRight = ModifyMatrixScales(matricesRight, scaleMod);
                matricesTop = ModifyMatrixPositions(matricesTop, Vector3.down * capsuleThicknessHalf);
                matricesBottom = ModifyMatrixPositions(matricesBottom, Vector3.down * capsuleThicknessHalf);

                // Create all tristrips. We build in 4 sections: 2 semi-circles, 2 line segments.
                var sideLeft = GenerateCircle(matricesLeft, true, capsule.SubdivideSemiCircleOutside, 0, 180);
                var sideRight = GenerateCircle(matricesRight, true, capsule.SubdivideSemiCircleOutside, 180, 360);
                var sideTop = GenerateHorizontalLineWithNormals(matricesTop, right, left, up, capsule.SubdivideLineOutside, true);
                var sideBot = GenerateHorizontalLineWithNormals(matricesBottom, right, left, up, capsule.SubdivideLineOutside, true);

                // Combine all tristrips. Order matters! For the normalization smoothing to work, tristrips need
                // to be sequential so overlapping normals overlap the correct normals.
                var allTristrips = new List<Tristrip>();
                allTristrips.AddRange(sideTop);
                allTristrips.AddRange(sideLeft);
                allTristrips.AddRange(sideBot);
                allTristrips.AddRange(sideRight);
                var tristripsArray = allTristrips.ToArray();

                // Add normals based on vertices. Smooth normals.
                SetNormalsFromTristripVertices(tristripsArray, false, true, isGfzCoordinateSpace);

                return tristripsArray;
            }
            public static Tristrip[] DebugEndcap(Matrix4x4[] matricesLeft, Matrix4x4[] matricesRight, GfzShapeCapsule capsule)
            {
                var tristrips = new List<Tristrip>();
                GetContinuity(capsule, out bool isContinuousFrom, out bool isContinuousTo);

                if (!isContinuousFrom)
                {
                    const int index = 0;
                    var mtxLeft = matricesLeft[index];
                    var mtxRight = matricesRight[index];
                    var normal = mtxLeft.rotation * Vector3.back;
                    var tristrip = CircleEndcapNoTexture(mtxLeft, mtxRight, capsule.SubdivideSemiCircleInside, normal, false);
                    tristrips.Add(tristrip);
                }

                if (!isContinuousTo)
                {
                    int lastIndex = matricesLeft.Length - 1;
                    var mtxLeft = matricesLeft[lastIndex];
                    var mtxRight = matricesRight[lastIndex];
                    var normal = mtxLeft.rotation * Vector3.forward;
                    var tristrip = CircleEndcapNoTexture(mtxLeft, mtxRight, capsule.SubdivideSemiCircleInside, normal, true);
                    tristrips.Add(tristrip);
                }

                return tristrips.ToArray();
            }

            private static Tristrip CircleEndcapNoTexture(Matrix4x4 mtxLeft, Matrix4x4 mtxRight, int nTristrips, Vector3 normal, bool isBackFacing)
            {
                // Create left and right semi-circle inner and outer matrices.
                Vector3 scaleMod = Vector3.one * capsuleThickness;
                Matrix4x4 innerMatrixLeft = mtxLeft;
                Matrix4x4 outerMatrixLeft = Matrix4x4.TRS(mtxLeft.Position(), mtxLeft.rotation, mtxLeft.lossyScale + scaleMod);
                Matrix4x4 innerMatrixRight = mtxRight;
                Matrix4x4 outerMatrixRight = Matrix4x4.TRS(mtxRight.Position(), mtxRight.rotation, mtxRight.lossyScale + scaleMod);

                // Create vertices for those two sides.
                var vertsLeft = CreateCircleVertices(nTristrips, 0, 180);
                var vertsRight = CreateCircleVertices(nTristrips, 180, 360);

                // Connect those vertices. Note that betwen the first and second semi-circle, we end up
                // connecting them because of the tripstrip format. Close loop bu adding first 2 vertices
                // at the end of the list.
                var verticesLeft = CreateTristripVerts(vertsLeft, innerMatrixLeft, outerMatrixLeft);
                var verticesRight = CreateTristripVerts(vertsRight, innerMatrixRight, outerMatrixRight);
                var allVerts = new List<Vector3>();
                allVerts.AddRange(verticesLeft);
                allVerts.AddRange(verticesRight);
                allVerts.Add(verticesLeft[0]);
                allVerts.Add(verticesLeft[1]);

                // Construct tristrip.
                var tristrip = new Tristrip();
                tristrip.positions = allVerts.ToArray();
                tristrip.normals = ArrayUtility.DefaultArray(normal, tristrip.VertexCount);

                // Make array and assign metadata.
                var tristrips = new Tristrip[] { tristrip };
                AssignTristripMetadata(tristrips, isBackFacing, false);

                return tristrip;
            }
            private static Vector3[] CreateTristripVerts(Vector3[] vertices, Matrix4x4 innerMatrix, Matrix4x4 outerMatrix)
            {
                int length = vertices.Length;
                Vector3[] positions = new Vector3[length * 2];
                for (int i = 0; i < length; i++)
                {
                    int index0 = i * 2;
                    int index1 = index0 + 1;
                    Vector3 vertex = vertices[i];
                    positions[index0] = innerMatrix.MultiplyPoint(vertex);
                    positions[index1] = outerMatrix.MultiplyPoint(vertex);
                }
                return positions;
            }

            public static Tristrip[] SemiCirclesNoTexture(Matrix4x4[] matricesLeft, Matrix4x4[] matricesRight, GfzShapeCapsule capsule, bool isGfzCoordinateSpace)
            {
                // Create all tristrips.
                var sideLeft = GenerateCircle(matricesLeft, true, capsule.SubdivideSemiCircleInside, 180, 0);
                var sideRight = GenerateCircle(matricesRight, true, capsule.SubdivideSemiCircleInside, 360, 180);

                // Combine all tristrips.
                var allTristrips = new List<Tristrip>();
                allTristrips.AddRange(sideRight);
                allTristrips.AddRange(sideLeft);
                var tristripsArray = allTristrips.ToArray();

                // Add normals based on vertices. Smooth normals.
                SetNormalsFromTristripVertices(tristripsArray, false, true, isGfzCoordinateSpace);

                return tristripsArray;
            }
            public static Tristrip[] SemiCirclesTex0(Matrix4x4[] matricesLeft, Matrix4x4[] matricesRight, GfzShapeCapsule capsule, float segmentLength, bool isGfzCoordinateSpace, int texRepetitions = 8)
            {
                var tristrips = SemiCirclesNoTexture(matricesLeft, matricesRight, capsule, isGfzCoordinateSpace);

                float repetitionsRoadTexture = math.ceil(segmentLength / RoadTexStride);
                var uvs0 = CreateTristripScaledUVs(tristrips, texRepetitions, repetitionsRoadTexture);
                for (int i = 0; i < tristrips.Length; i++)
                    tristrips[i].tex0 = uvs0[i];

                return tristrips;
            }

            public static Tristrip[] LinesNoTexture(Matrix4x4[] matricesTop, Matrix4x4[] matricesBottom, GfzShapeCapsule capsule)
            {
                Vector3 left = Vector3.left * 0.5f;
                Vector3 right = Vector3.right * 0.5f;
                Vector3 up = Vector3.up;

                // Create all tristrips.
                var sideTop = GenerateHorizontalLineWithNormals(matricesTop, left, right, up, capsule.SubdivideLineInside, true);
                var sideBot = GenerateHorizontalLineWithNormals(matricesBottom, left, right, up, capsule.SubdivideLineInside, true);

                // Combine all tristrips.
                var allTristrips = new List<Tristrip>();
                allTristrips.AddRange(sideTop);
                allTristrips.AddRange(sideBot);
                var tristripsArray = allTristrips.ToArray();

                return tristripsArray;
            }
            public static Tristrip[] LinesTex0(Matrix4x4[] matricesTop, Matrix4x4[] matricesBottom, GfzShapeCapsule capsule, float segmentLength, int texRepetitions = 4)
            {
                var tristrips = LinesNoTexture(matricesTop, matricesBottom, capsule);

                float repetitionsRoadTexture = math.ceil(segmentLength / RoadTexStride);
                var uvs0 = CreateTristripScaledUVs(tristrips, texRepetitions, repetitionsRoadTexture);
                for (int i = 0; i < tristrips.Length; i++)
                    tristrips[i].tex0 = uvs0[i];

                return tristrips;
            }
        }

        public static class Objects
        {
            const float height = 0.5f;
            const float inset = 1f;

            public static Tristrip[] DashPlateBoard(float width, float length)
            {
                var metrics = new DashPlateMetrics(width, length, height, inset);
                var top = DashPlateBoardTop(metrics);
                var sides = DashPlateBoardSides(metrics);
                var tristrips = new Tristrip[] { top, sides };
                return tristrips;
            }
            public static Tristrip[] DashPlateCircle()
            {
                throw new System.NotImplementedException();
            }
            public static Tristrip[] DashPlateSign()
            {
                throw new System.NotImplementedException();
            }

            public static Tristrip[] JumpPlate()
            {
                throw new System.NotImplementedException();
            }


            private static Tristrip DashPlateBoardTop(DashPlateMetrics dashPlateMetrics)
            {
                float insideWidth = dashPlateMetrics.InsideWidth; // box sides
                float insideLength = dashPlateMetrics.InsideLength; // box forward corners
                float insideLengthTip = dashPlateMetrics.InsideLengthTip; // box tip
                float height = dashPlateMetrics.Height;

                var tristrip = new Tristrip();
                tristrip.positions = new Vector3[]
                {
                    new(+insideWidth, height, -insideLength), // lower right
                    new(-insideWidth, height, -insideLength), // lower left
                    new(+insideWidth, height, +insideLength), // upper right
                    new(-insideWidth, height, +insideLength), // upper left
                    new(           0, height, +insideLengthTip), // triangle tip
                };
                tristrip.tex0 = new Vector2[]
                {
                    new(1, 0), // lower right
                    new(0, 0), // lower left
                    new(1, DashPlateMetrics.boxPercent), // upper right
                    new(0, DashPlateMetrics.boxPercent), // upper left
                    new(0.5f, DashPlateMetrics.tipPercent), // triangle tip
                };
                tristrip.tex1 = ArrayUtility.CopyArray(tristrip.tex0); // same UVs
                tristrip.normals = ArrayUtility.DefaultArray(Vector3.up, tristrip.positions.Length);

                return tristrip;
            }
            private static Tristrip DashPlateBoardSides(DashPlateMetrics dashPlateMetrics)
            {
                float outsideWidth = dashPlateMetrics.OutsideWidth; // 
                float outsideLength = dashPlateMetrics.OutsideLength; // 
                float outsideLengthTip = dashPlateMetrics.OutsideLengthTip; // box tip
                float insideWidth = dashPlateMetrics.InsideWidth; // box side
                float insideLength = dashPlateMetrics.InsideLength; // box forward corners
                float insideLengthTip = dashPlateMetrics.InsideLengthTip; // box tip
                float height = dashPlateMetrics.Height;

                // setting vertices in counter-clockwise direction from bottom center
                var tristrip = new Tristrip();
                tristrip.positions = new Vector3[]
                {
                    // RIGHT SIDE
                    new(            0,      0, -outsideLength), // outside center bottom
                    new(            0, height, - insideLength), // inside center bottom
                    new(+outsideWidth,      0, -outsideLength), // outside right bottom
                    new(+ insideWidth, height, - insideLength), // inside right bottom
                    new(+outsideWidth,      0, +outsideLength), // outside right top
                    new(+ insideWidth, height, + insideLength), // inside right top
                    // TOP
                    new(            0,      0, +outsideLengthTip), // outside triangle tip
                    new(            0, height, + insideLengthTip), // outside triangle tip
                    // LEFT SIDE
                    new(-outsideWidth,      0, +outsideLength), // outside left top
                    new(- insideWidth, height, + insideLength), // inside left top
                    new(-outsideWidth,      0, -outsideLength), // outside left bottom
                    new(- insideWidth, height, - insideLength), // inside left bottom
                    new(            0,      0, -outsideLength), // outside center bottom
                    new(            0, height, - insideLength), // inside center bottom
                };
                tristrip.tex0 = new Vector2[]
                {
                    // TODO: center sides should have 2 reps

                    // RIGHT
                    new(0.5f, 0),
                    new(0.5f, 1),
                    new(1, 0),
                    new(1, 1),
                    new(0, 0),
                    new(0, 1),
                    // TOP
                    new(0.5f, 0),
                    new(0.5f, 1),
                    // LEFT
                    new(1, 0),
                    new(1, 1),
                    new(0, 0),
                    new(0, 1),
                    new(0.5f, 0),
                    new(0.5f, 1),
                };
                tristrip.normals = TristripGenerator.GenericNormalsFromTristripVertices(tristrip, false, true); // TODO: pass in proper coord space

                return tristrip;
            }
            internal struct DashPlateMetrics
            {
                public const float boxPercent = 0.8f;
                public const float tipPercent = 1f;

                public float OutsideWidth { get; }
                public float OutsideLength { get; }
                public float OutsideLengthTip => OutsideLength;
                public float InsideWidth { get; }
                public float InsideLength { get; }
                public float InsideLengthTip { get; }
                public float Height { get; }

                public DashPlateMetrics(float width, float length, float height, float inset)
                {
                    OutsideWidth = width * 0.5f;
                    OutsideLength = length * 0.5f;
                    InsideWidth = OutsideWidth - inset; // box sides
                    InsideLength = (OutsideLength * boxPercent) - inset; // box forward corners
                    InsideLengthTip = OutsideLength - inset; // box tip
                    Height = height;
                }
            }

        }

    }
}