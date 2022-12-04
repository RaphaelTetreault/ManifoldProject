using Manifold.IO;
using Manifold.EditorTools.GC.GFZ.Stage.Track;
using System;
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

        const float TexStrideRoad = 32f;
        const float TexStrideTrim = 64f;

        static readonly Vector3 LineLeft = new Vector3(-0.5f, 0, 0);
        static readonly Vector3 LineRight = new Vector3(+0.5f, 0, 0);

        // ROAD
        public const float kCurbHeight = 0.5f;
        public const float kCurbSlantOuter = 1.5f;
        public const float kCurbSlantInner = kCurbSlantOuter + 2.25f; // 3.75f
        public const float kCurbInset = kCurbSlantInner;

        // Embeds
        private static readonly Vector3 edgeLeft = new Vector3(-0.5f, kEmbedHeight, 0);
        private static readonly Vector3 edgeRight = new Vector3(+0.5f, kEmbedHeight, 0);
        private const float kEmbedHeight = 0.30f;
        private const float kHealHeight = 0.10f;
        public const float kTrimOffset = 0.75f;
        private const float kTrimRepetitions = 64f;
        private const float kEmbedFlashSlipReps = 20f;
        private const float kEmbedFlashLavaReps = 10f;
        private const float kEmbedFlashDirtReps = 10f;
        private const float kEmbedFlashHealReps = 10f;

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
        public static Vector3 SurfaceNormalAngleZ(float angle, Vector3 normal)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 rotatedNormal = rotation * normal;
            return rotatedNormal;
        }

        private static bool CheckContinuity(EndcapMode endcapMode)
        {
            bool doCheck = endcapMode == EndcapMode.Automatic;
            return doCheck;
        }
        private static bool GetContinuity(EndcapMode endcapMode)
        {
            switch (endcapMode)
            {
                case EndcapMode.ForceEnable: return false;
                case EndcapMode.ForceDisable: return true;
                case EndcapMode.Automatic: throw new ArgumentException();
                default: throw new ArgumentException();
            }
        }
        private static bool GetContinuity(EndcapMode endcapMode, HierarchichalAnimationCurveTRS from, HierarchichalAnimationCurveTRS to)
        {
            bool doCheck = CheckContinuity(endcapMode);
            if (doCheck)
            {
                var isContinuous = CheckpointUtility.IsContinuousBetweenFromTo(from, to);
                return isContinuous;
            }
            else
            {
                bool isContinuous = GetContinuity(endcapMode);
                return isContinuous;
            }
        }
        public static void GetContinuity(GfzShape shapeNode, out bool isContinuousFrom, out bool isContinuousTo)
        {
            var root = shapeNode.GetRoot();
            var prev = root.Prev;
            var next = root.Next;

            var from = prev.CreateHierarchichalAnimationCurveTRS(false);
            var self = root.CreateHierarchichalAnimationCurveTRS(false);
            var to = next.CreateHierarchichalAnimationCurveTRS(false);

            isContinuousFrom = GetContinuity(shapeNode.EndcapModeIn, from, self);
            isContinuousTo = GetContinuity(shapeNode.EndcapModeOut, self, to);

            var rootShapes = root.GetShapeNodes();
            var prevShapes = prev.GetShapeNodes();
            var nextShapes = next.GetShapeNodes();

            // Do a check to see if the two segments are the same shape. If not,
            // force continuity to false.
            foreach (var shape in rootShapes)
            {
                if (shape.ShapeIdentifier == GfzShape.ShapeID.embed)
                    continue;

                if (shapeNode.EndcapModeIn == EndcapMode.Automatic)
                {
                    bool canBeContinuousFrom = CompareShapeIDs(shape, prevShapes);
                    isContinuousFrom &= canBeContinuousFrom;
                }
                if (shapeNode.EndcapModeIn == EndcapMode.Automatic)
                {
                    bool canBeContinuousTo = CompareShapeIDs(shape, nextShapes);
                    isContinuousTo &= canBeContinuousTo;
                }
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


        public static class General
        {
            public const float uvFlashSlipLight = 256;
            public const float uvFlashSlipDark = 512;
            public const float uvFlashRecover = -64;

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
            private static float[] GetDenormalizeLengthsWithPath(GfzPropertyEmbed embed, float repsLength, float[] length)
            {
                float segmentLength = embed.GetMaxTime();
                float embedLength = embed.GetRangeLength();
                float uvLength = embedLength / repsLength;
                float embedStart = embed.GetMaxTimeOffset() + embed.From * segmentLength;
                float uvStart = embedStart / repsLength;

                var newLengths = new float[length.Length];
                for (int i = 0; i < newLengths.Length; i++)
                    newLengths[i] = length[i] * uvLength + uvStart;

                return newLengths;
            }
            // TODO: uv setup is should be optionally ST or TS. Right now it is TS, unintuitive, requires swizzle
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
                var pathScaledLengths = GetDenormalizeLengthsWithPath(embed, uvFlashSlipLight, lengths);
                // Create UVs
                var uvsNormalized = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths);
                var uvs0 = ScaleByParentWidthAndCustomLength(uvsNormalized, parentMatrices, 1 / scaleW, scaleL);
                var uvs1 = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, pathScaledLengths, 0.5f);
                ApplyTex0(tristrips, uvs0);
                ApplyTex1(tristrips, uvs1);

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
                var pathScaledLengths = GetDenormalizeLengthsWithPath(embed, uvFlashSlipDark, lengths);
                // Create UVs
                var uvsNormalized = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, lengths);
                var uvs0 = ScaleByParentWidthAndCustomLength(uvsNormalized, parentMatrices, 1 / 64f, repetitionsAlongLength);
                var uvs1 = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, pathScaledLengths, 0.5f);
                ApplyTex0(tristrips, uvs0);
                ApplyTex1(tristrips, uvs1);

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
                var uvs0 = ScaleByParentWidthAndCustomLength(uvsNormalized, parentMatrices, 1 / scaleW, scaleL);
                Vector2 uvs1Offset = new Vector2(0.5f, 0.33f) * kEmbedFlashDirtReps;
                var uvs1 = ScaleUV(uvs0, -1f);
                uvs1 = OffsetUV(uvs1, uvs1Offset);
                ApplyTex0(tristrips, uvs0);
                ApplyTex1(tristrips, uvs1);

                return tristrips;
            }
            public static Tristrip[] CreateDirtAlpha(Matrix4x4[] matrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateHorizontalLineWithNormals(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);
                var uvs0 = ArrayUtility.DefaultArray2D(Vector2.zero, tristrips.Length, tristrips[0].VertexCount);
                ApplyTex0(tristrips, uvs0);

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
                ApplyTex0(tristrips, uvs0);

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
                ApplyTex0(tristrips, uvs0);
                ApplyTex0(tristrips, uvs1);

                return tristrips;
            }

            public static Tristrip[] CreateRecoverLightFloor(Matrix4x4[] matrices, GfzPropertyEmbed embed)
            {
                var tristrips = GenerateHorizontalLineWithNormals(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                // Scaling parameters
                float segmentLength = embed.GetRangeLength();
                float repetitionsAlongLength = math.ceil(segmentLength / kEmbedFlashHealReps);
                // Normalized values used to generate UVs
                GetNormalizedValues(embed, matrices.Length, out float[] halfWidths, out float[] offsets, out float[] lengths);
                var pathScaledLengths = GetDenormalizeLengthsWithPath(embed, 48, lengths);
                // Create UVs
                var uvs0 = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, pathScaledLengths, 0.5f); // flashing white stripe
                uvs0 = SwizzleUVs(uvs0);
                ApplyTex0(tristrips, uvs0);

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
                var pathScaledLengths = GetDenormalizeLengthsWithPath(embed, uvFlashRecover, lengths);
                // Create UVs
                var uvs0 = CreateTristripScaledUVs(tristrips, 2f, repetitionsAlongLength);
                var uvs1 = CreateTrackSpaceUVs(embed, tristrips, halfWidths, offsets, pathScaledLengths, 0.5f); // flashing white stripe
                uvs1 = SwizzleUVs(uvs1);
                ApplyTex0(tristrips, uvs0);
                ApplyTex1(tristrips, uvs1);

                return tristrips;
            }
            public static Tristrip[] CreateRecoverAlpha(Matrix4x4[] matrices, GfzPropertyEmbed embed)
            {
                var edgeLeft = TristripTemplates.edgeLeft + Vector3.up * kHealHeight;
                var edgeRight = TristripTemplates.edgeRight + Vector3.up * kHealHeight;
                var tristrips = GenerateHorizontalLineWithNormals(matrices, edgeLeft, edgeRight, Vector3.up, embed.WidthDivisions, true);

                // Scaling parameters
                float segmentLength = embed.GetRangeLength();
                float repetitionsAlongLength = math.ceil(segmentLength / kEmbedFlashSlipReps);

                var uvs0 = ArrayUtility.DefaultArray2D(Vector2.zero, tristrips.Length, tristrips[0].VertexCount);
                var uvs1 = CreateTristripScaledUVs(tristrips, 1f, repetitionsAlongLength);
                ApplyTex0(tristrips, uvs0);
                ApplyTex1(tristrips, uvs1);

                return tristrips;
            }

            public static Tristrip[] CreateTrim(Matrix4x4[] matrices, GfzPropertyEmbed embed, bool isGfzCoordinateSpace)
            {
                var allTristrips = new List<Tristrip>();

                float angleLeftRight = Mathf.Tan(kEmbedHeight / kTrimOffset) * Mathf.Rad2Deg;
                // Scaling parameters
                float segmentLength = embed.GetRangeLength();
                float repetitionsAlongLength = math.ceil(segmentLength / kTrimRepetitions); // TODO: parameter, not hard coded (or const instead?)

                if (embed.IncludeTrimLeft)
                {
                    var normal = Quaternion.Euler(0, 0, +angleLeftRight) * Vector3.up;
                    var leftTristrips = CreateTrimSide(matrices, embed, normal, -1, kTrimOffset, repetitionsAlongLength, isGfzCoordinateSpace);
                    allTristrips.AddRange(leftTristrips);
                }
                if (embed.IncludeTrimRight)
                {
                    var normal = Quaternion.Euler(0, 0, -angleLeftRight) * Vector3.up;
                    var rightTristrips = CreateTrimSide(matrices, embed, normal, +1, kTrimOffset, repetitionsAlongLength, isGfzCoordinateSpace);
                    allTristrips.AddRange(rightTristrips);
                }

                var trimEndcapTristrips = CreateTrimEndcaps(matrices, embed, kTrimOffset, kTrimRepetitions, isGfzCoordinateSpace);
                allTristrips.AddRange(trimEndcapTristrips);

                return allTristrips.ToArray();
            }
            private static Tristrip[] CreateTrimSide(Matrix4x4[] matrices, GfzPropertyEmbed node, Vector3 defaultNormal, float directionLR, float protrusion, float repetitionsAlongLength, bool isGfzCoord)
            {
                var offsetMatrices = GetNormalizedMatrixWithPositionOffset(matrices, directionLR);

                var outerEdge = new Vector3(protrusion * directionLR, 0, 0);
                var innerEdge = new Vector3(0, kEmbedHeight, 0);
                bool isBackFacing = directionLR > 0;
                var tristrips = GenerateHorizontalLineWithNormals(offsetMatrices, innerEdge, outerEdge, defaultNormal, 1, isBackFacing);

                var uvs0 = CreateTristripScaledUVs(tristrips, 1, repetitionsAlongLength);
                ApplyTex0(tristrips, uvs0);

                // Make first/last vert protrude outwards a bit, but only do so if we have start/end trim
                if (node.IncludeTrimStart)
                {
                    Vector3 back = isGfzCoord ? Vector3.forward : Vector3.back;
                    var mtx0 = matrices[0];
                    tristrips[0].positions[1] += mtx0.rotation * (back * Mathf.Abs(protrusion));
                }
                if (node.IncludeTrimEnd)
                {
                    Vector3 forward = isGfzCoord ? Vector3.back : Vector3.forward;
                    var mtx1 = matrices[matrices.Length - 1];
                    tristrips[0].positions[matrices.Length * 2 - 1] += mtx1.rotation * (forward * Mathf.Abs(protrusion));
                }

                return tristrips;
            }
            private static Tristrip[] CreateTrimEndcaps(Matrix4x4[] matrices, GfzPropertyEmbed embed, float protrusion, float texLength, bool isGfzCoord)
            {
                var allTristrips = new List<Tristrip>();
                Vector3 forward = isGfzCoord ? Vector3.back : Vector3.forward;
                Vector3 back = isGfzCoord ? Vector3.forward : Vector3.back;
                float angle = isGfzCoord ? -45 : 45;

                if (embed.IncludeTrimStart)
                {
                    int index0 = 0;
                    var mtx0 = matrices[index0];
                    var normal0 = Quaternion.Euler(+angle, 0, 0) * back;
                    var tristrips = Road.StandardTrapezoidEndCapUVTex0(mtx0, 0, -protrusion, kEmbedHeight, 0, normal0, Vector2.one);
                    // Hacky UV fix
                    float texReps = GetTexRepetitions(mtx0.lossyScale.x, texLength);
                    tristrips[0].tex0 = OffsetUV(tristrips[0].tex0, new Vector2(0, 0.5f));
                    tristrips[0].tex0 = ScaleUV(tristrips[0].tex0, new Vector2(1, texReps));
                    for (int i = 1; i < tristrips[0].positions.Length; i += 2)
                        tristrips[0].positions[i] += mtx0.rotation * back * Mathf.Abs(protrusion);
                    allTristrips.AddRange(tristrips);
                    AssignTristripMetadata(tristrips, false, false);
                }

                if (embed.IncludeTrimEnd)
                {
                    int index1 = matrices.Length - 1;
                    var mtx1 = matrices[index1];
                    var normal1 = Quaternion.Euler(-angle, 0, 0) * forward;
                    var tristrips = Road.StandardTrapezoidEndCapUVTex0(mtx1, 0, -protrusion, kEmbedHeight, 0, normal1, Vector2.one);
                    // Hacky UV fix
                    float texReps = GetTexRepetitions(mtx1.lossyScale.x, texLength);
                    tristrips[0].tex0 = OffsetUV(tristrips[0].tex0, new Vector2(0, 0.5f));
                    tristrips[0].tex0 = ScaleUV(tristrips[0].tex0, new Vector2(1, texReps));
                    for (int i = 1; i < tristrips[0].positions.Length; i += 2)
                        tristrips[0].positions[i] += mtx1.rotation * forward * Mathf.Abs(protrusion);
                    allTristrips.AddRange(tristrips);
                    AssignTristripMetadata(tristrips, true, false);
                }

                return allTristrips.ToArray();
            }
        }

        public static class Road
        {
            // Generic no UV
            public static Tristrip[] TopNoTex(Matrix4x4[] matrices, float[] sampleTimes, float inset)
            {
                var matricesInset = ModifyMatrixScales(matrices, new Vector3(inset * -2f, 0, 0));
                var vertexLine = CreateVertexLineFromTimes(sampleTimes, LineLeft, LineRight);
                var normalLine = ArrayUtility.DefaultArray(Vector3.up, vertexLine.Length);
                var tristrips = CreateTristrips(matricesInset, vertexLine, normalLine);
                AssignTristripMetadata(tristrips, true, false);
                return tristrips;
            }

            // Generic UV'ed
            public static Tristrip[] TopTex0(Matrix4x4[] matrices, int subdivWidth, float widthInset, float repWidth0, float repLength0)
            {
                var sampleTimes = CreateEvenlySpacedTimes(subdivWidth, 0, 1);
                var tristrips = TopNoTex(matrices, sampleTimes, widthInset);
                var tex0 = CreateUVsFromTimes(tristrips, sampleTimes, repWidth0, repLength0);
                ApplyTex0(tristrips, tex0);
                return tristrips;
            }
            public static Tristrip[] TopTex1(Matrix4x4[] matrices, int subdivWidth, float widthInset, float repWidth0, float repWidth1, float repLength0, float repLength1)
            {
                var sampleTimes = CreateEvenlySpacedTimes(subdivWidth, 0, 1);
                var tristrips = TopNoTex(matrices, sampleTimes, widthInset);
                var tex0 = CreateUVsFromTimes(tristrips, sampleTimes, repWidth0, repLength0);
                var tex1 = CreateUVsFromTimes(tristrips, sampleTimes, repWidth1, repLength1);
                ApplyTex0(tristrips, tex0);
                ApplyTex1(tristrips, tex1);
                return tristrips;
            }
            public static Tristrip[] TopTex2(Matrix4x4[] matrices, int subdivWidth, float widthInset, float repWidth0, float repWidth1, float repWidth2, float repLength0, float repLength1, float repLength2)
            {
                var sampleTimes = CreateEvenlySpacedTimes(subdivWidth, 0, 1);
                var tristrips = TopNoTex(matrices, sampleTimes, widthInset);
                var tex0 = CreateUVsFromTimes(tristrips, sampleTimes, repWidth0, repLength0);
                var tex1 = CreateUVsFromTimes(tristrips, sampleTimes, repWidth1, repLength1);
                var tex2 = CreateUVsFromTimes(tristrips, sampleTimes, repWidth2, repLength2);
                ApplyTex0(tristrips, tex0);
                ApplyTex1(tristrips, tex1);
                ApplyTex1(tristrips, tex2);
                return tristrips;
            }
            public static Tristrip[] BottomTex0(Matrix4x4[] matrices, int subdivWidth, float widthInset, float thickness, float segmentLength, float repWidth, float repLength)
            {
                var tristrips = StandardBottom(matrices, subdivWidth, thickness, widthInset);
                var uvs0 = CreateTristripScaledUVs(tristrips, repWidth, segmentLength, repLength);
                ApplyTex0(tristrips, uvs0);
                return tristrips;
            }
            public static Tristrip[] BottomTex1(Matrix4x4[] matrices, int subdivWidth, float widthInset, float thickness, float segmentLength, float repWidth0, float repWidth1, float repLength0, float repLength1)
            {
                var tristrips = StandardBottom(matrices, subdivWidth, thickness, widthInset);
                var uvs0 = CreateTristripScaledUVs(tristrips, repWidth0, segmentLength, repLength0);
                var uvs1 = CreateTristripScaledUVs(tristrips, repWidth1, segmentLength, repLength1);
                ApplyTex0(tristrips, uvs0);
                ApplyTex1(tristrips, uvs1);
                return tristrips;
            }
            public static Tristrip[] BottomTex2(Matrix4x4[] matrices, int subdivWidth, float widthInset, float thickness, float segmentLength, float repWidth0, float repWidth1, float repWidth2, float repLength0, float repLength1, float repLength2)
            {
                var tristrips = StandardBottom(matrices, subdivWidth, thickness, widthInset);
                var uvs0 = CreateTristripScaledUVs(tristrips, repWidth0, segmentLength, repLength0);
                var uvs1 = CreateTristripScaledUVs(tristrips, repWidth1, segmentLength, repLength1);
                var uvs2 = CreateTristripScaledUVs(tristrips, repWidth2, segmentLength, repLength2);
                ApplyTex0(tristrips, uvs0);
                ApplyTex1(tristrips, uvs1);
                ApplyTex2(tristrips, uvs2);
                return tristrips;
            }


            #region REVIEW AND GENERICIZE
            internal static Tristrip[] StandardTop(Matrix4x4[] matrices, int widthDivisions, float inset = 3.75f)
            {
                // Make road width equal to width minus the inset on both sides
                var matricesInset = ModifyMatrixScales(matrices, new Vector3(inset * -2f, 0, 0));
                var tristrips = GenerateHorizontalLineWithNormals(matricesInset, LineLeft, LineRight, Vector3.up, widthDivisions, true);
                return tristrips;
            }
            internal static Tristrip[] StandardBottom(Matrix4x4[] matrices, int widthDivisions, float thickness, float inset)
            {
                var matricesInset = ModifyMatrixScales(matrices, new Vector3(inset * -2f, 0, 0));
                var lineLeft = new Vector3(-0.5f, -thickness, 0); // bottom left
                var lineRight = new Vector3(+0.5f, -thickness, 0); // bottom right
                var tristrips = GenerateHorizontalLineWithNormals(matricesInset, lineLeft, lineRight, Vector3.down, widthDivisions, false);
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
                    //
                    tristripLeft.isBackFacing = false;
                    tristripRight.isBackFacing = true;
                }

                return tristrips;
            }
            public static Tristrip[] StandardCurbEndCap(Matrix4x4 matrix, float curbHeight, float curbSlantOuter, float curbSlantInner, float insetBottom, float thickness, Vector3 normal)
            {
                Vector3 OffsetTop = matrix.rotation * new Vector3(matrix.lossyScale.x * 0.5f, 0, 0);
                Vector3 OffsetBot = matrix.rotation * new Vector3(-insetBottom, -thickness, 0);
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
                tristrip.normals = ArrayUtility.DefaultArray(matrix.rotation * normal, tristrip.VertexCount);
                var tristrips = new Tristrip[] { tristrip };
                return tristrips;
            }
            public static Tristrip[] StandardTrapezoidEndCap(Matrix4x4 matrix, float insetTop, float insetBottom, float heightOffset, float thickness, Vector3 normal)
            {
                Vector3 OffsetTop = matrix.rotation * new Vector3(matrix.lossyScale.x * 0.5f, 0, 0);
                Vector3 OffsetBot = matrix.rotation * new Vector3(0, -thickness, 0);
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
                tristrip.normals = ArrayUtility.DefaultArray(matrix.rotation * normal, tristrip.VertexCount);
                tristrip.isBackFacing = Vector3.Dot(Vector3.back, normal) < 0; // TODO: THIS FAILS ON GFZ EXPORT

                var tristrips = new Tristrip[] { tristrip };
                return tristrips;
            }
            public static Tristrip[] StandardCurbEndCapUVTex0(Matrix4x4 matrix, float curbHeight, float curbSlantOuter, float curbSlantInner, float insetBottom, float thickness, Vector3 normal, Vector2 uv0Scale)
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
                var tristrips = StandardCurbEndCap(matrix, curbHeight, curbSlantOuter, curbSlantInner, insetBottom, thickness, normal);
                // Make sure assumptions are checked
                Assert.IsTrue(tristrips.Length == 1);
                Assert.IsTrue(tristrips[0].positions.Length == uvs0.Length);

                // Assign and return
                tristrips[0].tex0 = uvs0;
                return tristrips;
            }
            public static Tristrip[] StandardTrapezoidEndCapUVTex0(Matrix4x4 matrix, float insetTop, float insetBottom, float heightOffset, float thickness, Vector3 normal, Vector2 uv0Scale)
            {
                float scaleX = matrix.lossyScale.x;
                float upper = (scaleX - insetTop) / scaleX * 0.5f;
                float lower = (scaleX - insetBottom * 2) / scaleX * 0.5f;

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
            #endregion


            #region Styles
            public static class MuteCity
            {
                // TODO: de-hardcode these, put in road as param
                public const float kLengthTrim = TexStrideTrim;
                public const float kLengthRoadTop = TexStrideRoad;
                public const float kLengthRoadBottom = TexStrideRoad;
                public const float kLengthLaneDivider = TexStrideRoad;
                public const float kThickness = 1f;
                public const float kInsetTop = kCurbSlantInner;
                public const float kInsetBottom = 0f;
                public const float kRailMinHeight = 1.1667f;
                public const float kLaneDividerWidth = 0.75f;
                public const float kLaneDividerHeight = 0.04f;

                public static Tristrip[] Top(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                    => TopTex0(matrices, road.WidthDivisions, kInsetTop, 4, GetTexRepetitions(segmentLength, kLengthRoadTop));
                public static Tristrip[] Bottom(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                    => BottomTex0(matrices, road.WidthDivisions, 0, 1, segmentLength, 4, kLengthRoadTop);
                public static Tristrip[] TrimTex0(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength, bool isGfzCoordinate)
                {
                    var allTristrips = new List<Tristrip>();
                    float repetitions = math.ceil(segmentLength / kLengthTrim);

                    // Trims top
                    var matricesLeft = GetNormalizedMatrixWithPositionOffset(matrices, -1f);
                    var matricesRight = GetNormalizedMatrixWithPositionOffset(matrices, +1f);
                    var lineOuterEdge = new Vector3(+0.0f, kCurbHeight, 0); // left/right edge of track
                    var lineInnerLeft = new Vector3(+1.5f, kCurbHeight, 0); // left-inwards
                    var lineInnerRight = new Vector3(-1.5f, kCurbHeight, 0); // right-inwards
                    var tristripsLeft = GenerateHorizontalLineWithNormals(matricesLeft, lineOuterEdge, lineInnerLeft, Vector3.up, 1, true);
                    var tristripsRight = GenerateHorizontalLineWithNormals(matricesRight, lineOuterEdge, lineInnerRight, Vector3.up, 1, false);
                    var uvs0 = CreateTristripScaledUVs(tristripsLeft, 1, repetitions);
                    ApplyTex0(tristripsLeft, uvs0);
                    ApplyTex0(tristripsRight, uvs0);

                    // Trim sides
                    var sides = StandardSides(matrices, 0, 0, kCurbHeight, kThickness);
                    for (int i = 0; i < sides.Length; i++)
                        sides[i].tex0 = UvStripForward(sides[i], repetitions);

                    // EndCaps
                    {
                        Vector3 forward = isGfzCoordinate ? Vector3.back : Vector3.forward;
                        Vector3 back = isGfzCoordinate ? Vector3.forward : Vector3.back;

                        GetContinuity(road, out bool isContinuousFrom, out bool isContinuousTo);
                        if (!isContinuousFrom)
                        {
                            var matrix = matrices[0];
                            var uvRepsX = GetTexRepetitions(matrix.lossyScale.x, kLengthTrim);
                            var endcapFrom = StandardCurbEndCapUVTex0(matrix, kCurbHeight, kCurbSlantOuter, kCurbSlantInner, 0, kThickness, back, new Vector2(uvRepsX, 1));
                            AssignTristripMetadata(endcapFrom, false, false);
                            allTristrips.AddRange(endcapFrom);
                        }
                        if (!isContinuousTo)
                        {
                            var matrix = matrices[matrices.Length - 1];
                            var uvRepsX = GetTexRepetitions(matrix.lossyScale.x, kLengthTrim);
                            var endcapTo = StandardCurbEndCapUVTex0(matrix, kCurbHeight, kCurbSlantOuter, kCurbSlantInner, 0, kThickness, forward, new Vector2(uvRepsX, 1));
                            AssignTristripMetadata(endcapTo, true, false);
                            allTristrips.AddRange(endcapTo);
                        }
                    }

                    allTristrips.AddRange(tristripsLeft);
                    allTristrips.AddRange(tristripsRight);
                    allTristrips.AddRange(sides);

                    // Mute City offset
                    var allTristripsArray = allTristrips.ToArray();
                    foreach (var tristrip in allTristripsArray)
                        tristrip.tex0 = OffsetUVs(tristrip.tex0, new Vector2(0, 0.5f));

                    return allTristripsArray;
                }
                public static Tristrip[] EndCapsTex0(Matrix4x4[] matrices, GfzShapeRoad road, bool isGfzCoordinate)
                {
                    var allTristrips = new List<Tristrip>();

                    Vector3 forward = isGfzCoordinate ? Vector3.back : Vector3.forward;
                    Vector3 back = isGfzCoordinate ? Vector3.forward : Vector3.back;
                    GetContinuity(road, out bool isContinuousFrom, out bool isContinuousTo);

                    if (!isContinuousFrom)
                    {
                        var matrix = matrices[0];
                        var uvRepsX = GetTexRepetitions(matrix.lossyScale.x, kLengthTrim);
                        var endcapFrom = StandardCurbEndCapUVTex0(matrix, kCurbHeight, kCurbSlantOuter, kCurbSlantInner, 0, kThickness, back, new Vector2(uvRepsX, 1));
                        AssignTristripMetadata(endcapFrom, false, false);
                        allTristrips.AddRange(endcapFrom);
                    }

                    if (!isContinuousTo)
                    {
                        var matrix = matrices[matrices.Length - 1];
                        var uvRepsX = GetTexRepetitions(matrix.lossyScale.x, kLengthTrim);
                        var endcapTo = StandardCurbEndCapUVTex0(matrix, kCurbHeight, kCurbSlantOuter, kCurbSlantInner, 0, kThickness, forward, new Vector2(uvRepsX, 1));
                        AssignTristripMetadata(endcapTo, true, false);
                        allTristrips.AddRange(endcapTo);
                    }

                    return allTristrips.ToArray();
                }
                public static Tristrip[] TrimSidesTex0(Matrix4x4[] matrices, float insetTop, float insetBottom, float curbHeight, float thickness, float segmentLength, float repWidth, float repLength)
                {
                    // can optimized UV gen by using simpler gen code UVStripForward
                    var sides = StandardSides(matrices, insetTop, insetBottom, curbHeight, thickness);
                    var uvs0 = CreateTristripScaledUVs(sides, repWidth, segmentLength, repLength);
                    ApplyTex0(sides, uvs0);
                    return sides;
                }
                public static Tristrip[] TrimTopsTex0(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength, bool isGfzCoordinate)
                {
                    var matricesLeft = GetNormalizedMatrixWithPositionOffset(matrices, -1f);
                    var matricesRight = GetNormalizedMatrixWithPositionOffset(matrices, +1f);
                    var lineOuterEdge = new Vector3(+0.0f, kCurbHeight, 0); // left/right edge of track
                    var lineInnerLeft = new Vector3(+1.5f, kCurbHeight, 0); // left-inwards
                    var lineInnerRight = new Vector3(-1.5f, kCurbHeight, 0); // right-inwards
                    var tristripsLeft = GenerateHorizontalLineWithNormals(matricesLeft, lineOuterEdge, lineInnerLeft, Vector3.up, 1, true);
                    var tristripsRight = GenerateHorizontalLineWithNormals(matricesRight, lineOuterEdge, lineInnerRight, Vector3.up, 1, false);

                    var uvs0 = CreateTristripScaledUVs(tristripsLeft, 1, segmentLength, kLengthTrim);
                    ApplyTex0(tristripsLeft, uvs0);
                    ApplyTex0(tristripsRight, uvs0);

                    var tristrips = ConcatTristrips(tristripsLeft, tristripsRight);
                    return tristrips;
                }
                // use curb slant?
                public static Tristrip[] LightNoTex(Matrix4x4[] matrices)
                {
                    var matricesLeft = GetNormalizedMatrixWithPositionOffset(matrices, -1f);
                    var matricesRight = GetNormalizedMatrixWithPositionOffset(matrices, +1f);
                    var outerLeft = new Vector3(+kCurbSlantOuter, kCurbHeight, 0);
                    var outerRight = new Vector3(-kCurbSlantOuter, kCurbHeight, 0);
                    var innerLeft = new Vector3(+kCurbSlantInner, 0.0f, 0);
                    var innerRight = new Vector3(-kCurbSlantInner, 0.0f, 0);
                    var normalLeft = SurfaceNormalTOA(kCurbHeight, kCurbSlantInner - kCurbSlantOuter, Vector3.up, true);
                    var normalRight = SurfaceNormalTOA(kCurbHeight, kCurbSlantInner - kCurbSlantOuter, Vector3.up, false);
                    var tristripsLeft = GenerateHorizontalLineWithNormals(matricesLeft, outerLeft, innerLeft, normalLeft, 1, true);
                    var tristripsRight = GenerateHorizontalLineWithNormals(matricesRight, outerRight, innerRight, normalRight, 1, false);
                    var tristrips = ConcatTristrips(tristripsLeft, tristripsRight);
                    return tristrips;
                }
                public static Tristrip[] LightTex1(Matrix4x4[] matrices, float segmentLength)
                {
                    var lightTristrips = LightNoTex(matrices);
                    Assert.IsTrue(lightTristrips.Length == 2);

                    float repetitions = GetTexRepetitions(segmentLength, kLengthTrim);
                    int vertexCount = lightTristrips[0].VertexCount;
                    var uvs0 = UvStripForward(vertexCount, repetitions);
                    uvs0 = OffsetUVs(uvs0, new Vector2(0, 0.5f)); // offset so light part is not cut off at ends. !! has to match COM road !!
                    var uvs1 = ScaleUVs(uvs0, 2f); // trim light, these uvs are double on each side due to mirroring

                    foreach (var tri in lightTristrips)
                    {
                        tri.tex0 = uvs0;
                        tri.tex1 = uvs1;
                    }

                    return lightTristrips;
                }
                private static void GetMuteCityRailRanges(float railHeight, out float railLowerMin, out float railLowerMax, out float railUpperMin, out float railUpperMax)
                {
                    float trueRailHeight = railHeight - kRailMinHeight;
                    railLowerMin = kRailMinHeight; // trueRailHeight * 0.000f
                    railLowerMax = kRailMinHeight + trueRailHeight * 0.375f;
                    railUpperMin = kRailMinHeight + trueRailHeight * 0.625f;
                    railUpperMax = kRailMinHeight + trueRailHeight; // * 1.000f;
                }
                public static Tristrip[] RailTex2(Matrix4x4[] matrices, GfzShapeRoad road, bool isLeftSide)
                {
                    float side = isLeftSide ? -0.5f : 0.5f;
                    float railHeight = isLeftSide ? road.RailHeightLeft : road.RailHeightRight;

                    GetMuteCityRailRanges(railHeight, out float railLowerMin, out float railLowerMax, out float railUpperMin, out float railUpperMax);
                    var lowerEndpointA = new Vector3(side, railLowerMin, 0);
                    var lowerEndpointB = new Vector3(side, railLowerMax, 0);
                    var upperEndpointA = new Vector3(side, railUpperMin, 0);
                    var upperEndpointB = new Vector3(side, railUpperMax, 0);
                    var normal = isLeftSide ? Vector3.right : Vector3.left;
                    var lower = GenerateHorizontalLineWithNormals(matrices, lowerEndpointA, lowerEndpointB, normal, 1, true, true);
                    var upper = GenerateHorizontalLineWithNormals(matrices, upperEndpointA, upperEndpointB, normal, 1, true, true);

                    // hand tuned UVs just for MC rails
                    var uvsLower = UvStripSideways(lower[0], 1, 0, 0.35f);
                    var uvsUpper = UvStripSideways(upper[0], 1, 0.652f, 0.99f);
                    //
                    lower[0].tex0 = uvsLower;
                    lower[0].tex1 = uvsLower;
                    lower[0].tex2 = uvsLower;
                    //
                    upper[0].tex0 = uvsUpper;
                    upper[0].tex1 = uvsUpper;
                    upper[0].tex2 = uvsUpper;

                    var allTristrips = new List<Tristrip>();
                    allTristrips.AddRange(lower);
                    allTristrips.AddRange(upper);

                    return allTristrips.ToArray();
                }
                public static Tristrip[] RailsTex2(Matrix4x4[] matrices, GfzShapeRoad road)
                {
                    var allTristrips = new List<Tristrip>();

                    // rail left
                    if (road.RailHeightLeft > 0f)
                    {
                        var railTristripsLeft = RailTex2(matrices, road, true);
                        allTristrips.AddRange(railTristripsLeft);
                    }

                    // rail right
                    if (road.RailHeightRight > 0f)
                    {
                        var railTristripsLeft = RailTex2(matrices, road, false);
                        allTristrips.AddRange(railTristripsLeft);
                    }

                    return allTristrips.ToArray();
                }
                // Genericize a bit more
                public static Tristrip[] LaneDividersTex0(Matrix4x4[] matrices, GfzShapeRoad road, float length)
                {
                    // Position matrices on either edge
                    var matricesLeft = ModifyMatrixScaledPositions(matrices, Vector3.left * 0.5f);
                    var matricesRight = ModifyMatrixScaledPositions(matrices, Vector3.right * 0.5f);
                    matricesLeft = ModifyMatrixPositions(matricesLeft, Vector3.right * (kCurbSlantInner + 1f));
                    matricesRight = ModifyMatrixPositions(matricesRight, Vector3.left * (kCurbSlantInner + 1f));

                    var matricesNoScale = GetMatricesDefaultScale(matrices, Vector3.one * kLaneDividerWidth);
                    var matricesLeftNoScale = GetMatricesDefaultScale(matricesLeft, Vector3.one * kLaneDividerWidth);
                    var matricesRightNoScale = GetMatricesDefaultScale(matricesRight, Vector3.one * kLaneDividerWidth);

                    var allTristrips = new List<Tristrip>();
                    var leftTristrip = CreateLaneDividerTex0(matricesLeftNoScale, length);
                    var rightTristrip = CreateLaneDividerTex0(matricesRightNoScale, length);
                    allTristrips.Add(leftTristrip);
                    allTristrips.Add(rightTristrip);

                    if (road.HasLaneDividers)
                    {
                        var centerTristrip = CreateLaneDividerTex0(matricesNoScale, length);
                        allTristrips.Add(centerTristrip);
                    }

                    return allTristrips.ToArray();
                }
                private static Tristrip CreateLaneDividerTex0(Matrix4x4[] matrices, float length)
                {
                    var endpointA = new Vector3(-1.0f, kLaneDividerHeight, 0);
                    var endpointB = new Vector3(+1.0f, kLaneDividerHeight, 0);
                    var tristrips = GenerateHorizontalLineWithNormals(matrices, endpointA, endpointB, Vector3.up, 1, true);
                    var uvs0 = CreateTristripScaledUVs(tristrips, 1, length, kLengthLaneDivider);
                    ApplyTex0(tristrips, uvs0);
                    // only one element!
                    return tristrips[0];
                }

                public static Tristrip[][] Shape(Matrix4x4[] matrices, GfzShapeRoad road, float maxTime, bool isGfzCoordinateSpace)
                {
                    var tristrips = new Tristrip[][]
                    {
                        Top(matrices, road, maxTime),
                        Bottom(matrices, road, maxTime),
                        TrimTex0(matrices, road, maxTime, isGfzCoordinateSpace),
                        LightTex1(matrices, maxTime),
                        LaneDividersTex0(matrices, road, maxTime),
                        RailsTex2(matrices, road),
                    };
                    return tristrips;
                }
            }

            public static class MuteCityCOM
            {
                public const float RoadTexStride = 32f;
                public const float LaneDividerStride = RoadTexStride / 2f;

                public static Tristrip[] Top(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    // HARD CODED VALUES: 2, 8, 2
                    var repetitionsAlongLength0 = GetTexRepetitions(segmentLength, RoadTexStride);
                    var repetitionsAlongLength1 = GetTexRepetitionsRoundUp(segmentLength, TexStrideTrim, 2); // must be in pairs of 2 along length
                    var tristrips = TopTex1(matrices, road.WidthDivisions, kCurbInset, 8, 2, repetitionsAlongLength0, repetitionsAlongLength1);
                    //
                    foreach (var tristrip in tristrips)
                    {
                        var top_uvs1 = OffsetUV(tristrip.tex1, new Vector2(0, 2 * 0.5f));
                        tristrip.tex1 = top_uvs1;
                    }

                    return tristrips;
                }

                public static Tristrip[][] Shape(Matrix4x4[] matrices, GfzShapeRoad road, float maxTime, bool isGfzCoordinateSpace)
                {
                    var tristrips = new Tristrip[][]
                    {
                        Top(matrices, road, maxTime),
                        MuteCity.Bottom(matrices, road, maxTime),
                        MuteCity.TrimTex0(matrices, road, maxTime, isGfzCoordinateSpace),
                        MuteCity.LightTex1(matrices, maxTime),
                        MuteCity.RailsTex2(matrices, road),
                    };
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

                const float kEndCapTexStride = TexStrideRoad;
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

                    float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
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

                    float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
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
                    float hypoteneuse = math.sqrt(math.pow(kSideInset, kSideInset) + math.pow(kTrackThickness, kTrackThickness));
                    var normalLeft = SurfaceNormalTOA(kTrackThickness, hypoteneuse, Vector3.left, false);
                    var normalRight = SurfaceNormalTOA(kTrackThickness, hypoteneuse, Vector3.right, true);
                    OuterSpaceSlantedSide(matricesLeft, -1, kSideInset, kTrackThickness, normalLeft, sidesTristrips);
                    OuterSpaceSlantedSide(matricesRight, +1, kSideInset, kTrackThickness, normalRight, sidesTristrips);

                    var sidesTristripsArray = sidesTristrips.ToArray();
                    float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
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
                    float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
                    foreach (var tristrip in railTristrips)
                        tristrip.tex0 = UvStripForward(tristrip, repetitionsRoadTexture);

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
                    float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
                    foreach (var tristrip in railTristrips)
                        tristrip.tex0 = UvStripForward(tristrip, repetitionsRoadTexture);

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
                    float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
                    for (int i = 0; i < allTristrips.Count; i++)
                        allTristrips[i].tex0 = UvStripForward(allTristrips[i], repetitionsRoadTexture);

                    return allTristrips.ToArray();
                }

                public static Tristrip[] CurbAndLaneDividerFlat(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    var allTristrips = new List<Tristrip>();
                    allTristrips.AddRange(CurbFlat(matrices, road, segmentLength));
                    allTristrips.AddRange(LaneDividerTop(matrices, road, segmentLength));

                    // UVs are simple 0-1 along width, every desired length repeat
                    float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
                    for (int i = 0; i < allTristrips.Count; i++)
                    {
                        var uvs = UvStripForward(allTristrips[i], repetitionsRoadTexture);
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
                        var tristrips = StandardTrapezoidEndCapUVTex0(matrix, 0, kSideInset, kCurbHeight, kTrackThickness, Vector3.back, new Vector2(uvRepsX, kEndCapTexHeight));
                        AssignTristripMetadata(tristrips, false, false);
                        allTristrips.AddRange(tristrips);
                    }

                    if (!isContinuousTo)
                    {
                        var matrix = matrices[matrices.Length - 1];
                        var uvRepsX = Mathf.Ceil(matrix.lossyScale.x / kEndCapTexStride);
                        var tristrips = StandardTrapezoidEndCapUVTex0(matrix, 0, kSideInset, kCurbHeight, kTrackThickness, Vector3.forward, new Vector2(uvRepsX, kEndCapTexHeight));
                        AssignTristripMetadata(tristrips, true, false);
                        allTristrips.AddRange(tristrips);
                    }

                    return allTristrips.ToArray();
                }


                public static Tristrip[][] Shape(Matrix4x4[] matrices, GfzShapeRoad road, float segmentLength)
                {
                    var tristrips = new Tristrip[][]
                    {
                        Top(matrices, road, segmentLength),
                        BottomAndSides(matrices, road, segmentLength),
                        CurbAndLaneDividerFlat(matrices, road, segmentLength),
                        CurbAndLaneDividerSlants(matrices, road, segmentLength),
                        RailsAngle(matrices, road, segmentLength),
                        RailsLights(matrices, road, segmentLength),
                        EndCaps(matrices, road, segmentLength),
                    };
                    return tristrips;
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
            #endregion
        }

        public static class RoadModulated
        {
            public static float[] CreateEdgeSampleTimesAtMatrix(Matrix4x4 matrix, float insetCurbTop, float insetCurbBottom)
            {
                float width = matrix.lossyScale.x;
                float[] edgeTimes = new float[6];
                edgeTimes[0] = 0;
                edgeTimes[1] = insetCurbTop / width;
                edgeTimes[2] = insetCurbBottom / width;
                edgeTimes[3] = (width - insetCurbBottom) / width;
                edgeTimes[4] = (width - insetCurbTop) / width;
                edgeTimes[5] = 1f;
                return edgeTimes;
            }
            public static float[] CreateRoadSampleTimesWithInset(Matrix4x4 matrix, float[] sampleTimes, float insetCurbTop, float insetCurbBottom)
            {
                float[] edgeTimes = CreateEdgeSampleTimesAtMatrix(matrix, insetCurbTop, insetCurbBottom);
                int n = edgeTimes.Length / 2;
                float minTime = edgeTimes[n - 1];
                float maxTime = edgeTimes[edgeTimes.Length - n];
                float range = maxTime - minTime;
                // two times overlap (first and last), so remove from array copy
                float[] innerSampleTimes = new float[sampleTimes.Length - 2];
                for (int i = 0; i < innerSampleTimes.Length; i++)
                    innerSampleTimes[i] = sampleTimes[i + 1] * range + minTime;

                float[] times = new float[edgeTimes.Length + innerSampleTimes.Length];
                Array.Copy(edgeTimes, 0, times, 0, n);
                Array.Copy(edgeTimes, n, times, times.Length - n, n);
                Array.Copy(innerSampleTimes, 0, times, n, innerSampleTimes.Length);

                return times;
            }
            public static Vector3[] CreateModulatedVertexLine(GfzShapeRoadModulated road, int nVertices)
            {
                var vertices = CreateLineVertices(nVertices, LineLeft, LineRight);
                for (int i = 0; i < vertices.Length; i++)
                {
                    float time = i / (vertices.Length - 1f);
                    float heightOffset = road.WidthCurve.EvaluateNormalized(time);
                    vertices[i].y += heightOffset;
                }
                return vertices;
            }
            public static Vector3[][] CreateModulatedVertexLinesWithInset(Matrix4x4[] matrices, GfzShapeRoadModulated road, float[] sampleTimes, float insetCurbTop, float insetCurbBottom, float curbHeight)
            {
                Vector3[][] vertexLines = new Vector3[matrices.Length][];
                for (int matrixIndex = 0; matrixIndex < matrices.Length; matrixIndex++)
                {
                    float lengthTime = matrixIndex / (matrices.Length - 1f);
                    var matrix = matrices[matrixIndex];
                    var scale = matrix.lossyScale;
                    var allSampleTimes = CreateRoadSampleTimesWithInset(matrix, sampleTimes, insetCurbTop, insetCurbBottom);
                    // length curve and matrix scale.y combine to scale position.y
                    float scaleY = scale.y * road.LengthCurve.EvaluateNormalized(lengthTime);
                    vertexLines[matrixIndex] = new Vector3[allSampleTimes.Length];
                    for (int i = 0; i < allSampleTimes.Length; i++)
                    {
                        float widthTime = allSampleTimes[i];
                        float positionX = (allSampleTimes[i] - 0.5f) * scale.x;
                        float positionY = road.WidthCurve.EvaluateNormalized(widthTime);
                        Vector3 vertex = matrix.rotation * new Vector3(positionX, positionY * scaleY, 0f) + matrix.Position();
                        vertexLines[matrixIndex][i] = vertex;
                    }
                    Vector3 relativeUp = matrix.rotation * (curbHeight * Vector3.up);
                    vertexLines[matrixIndex][0] += relativeUp;
                    vertexLines[matrixIndex][1] += relativeUp;
                    vertexLines[matrixIndex][allSampleTimes.Length - 2] += relativeUp;
                    vertexLines[matrixIndex][allSampleTimes.Length - 1] += relativeUp;
                }
                return vertexLines;
            }
            public static Matrix4x4[] CreateModulatedMatrices(Matrix4x4[] matrices, GfzShapeRoadModulated road, float widthTime)
            {
                var modulatedMatrices = new Matrix4x4[matrices.Length];
                for (int i = 0; i < matrices.Length; i++)
                {
                    var matrix = matrices[i];
                    float lengthTime = i / (matrices.Length - 1f);
                    float scaleY = matrix.lossyScale.y;
                    float heightY = road.LengthCurve.EvaluateNormalized(lengthTime);
                    float heightX = road.WidthCurve.EvaluateNormalized(widthTime);
                    var position = matrix.Position() + matrix.rotation * new Vector3(0, scaleY * heightY * heightX, 0);
                    var scale = matrix.lossyScale;
                    scale.y = 1f;
                    modulatedMatrices[i] = Matrix4x4.TRS(position, matrix.rotation, scale);
                }
                return modulatedMatrices;
            }

            public static Tristrip[][] FullTopNoTex(Matrix4x4[] matrices, GfzShapeRoadModulated road, float insetCurbTop, float insetCurbBottom, float curbHeight, bool isGfzCoordinateSpace)
            {
                float[] innerSampleTimes = CreateEvenlySpacedTimes(road.SubdivisionsTop);
                // you can use this for endcaps by using v[0][] and v[n-1][] and one that is the bottom line
                var vertexLines = CreateModulatedVertexLinesWithInset(matrices, road, innerSampleTimes, insetCurbTop, insetCurbBottom, curbHeight);
                var tristrips = VertexLinesToTristrips(vertexLines);
                AssignTristripMetadata(tristrips, true, false);
                int n = tristrips.Length;

                var trimTop = ConcatTristrips(tristrips[0], tristrips[n - 1]);
                var trimSlope = ConcatTristrips(tristrips[1], tristrips[n - 2]);
                var top = tristrips[2..(n - 2)];

                SetNormalsFromTristripVerticesNoSmooth(trimTop, false, isGfzCoordinateSpace);
                SetNormalsFromTristripVerticesNoSmooth(trimSlope, false, isGfzCoordinateSpace);
                SetNormalsFromTristripVertices(top, false, false, isGfzCoordinateSpace);

                var tristripsArray = new Tristrip[][]
                {
                    top,
                    trimTop,
                    trimSlope,
                };
                return tristripsArray;
            }
            public static Tristrip[] BottomNoTex(Matrix4x4[] matrices, GfzShapeRoadModulated road, float thickness)
            {
                //var line = CreateEvenlySpacedTimes(road.SubdivisionsBottom);

                Vector3[][] vertexLines = new Vector3[matrices.Length][];
                for (int m = 0; m < matrices.Length; m++)
                {
                    float lengthTime = m / (matrices.Length - 1f);
                    var matrix = matrices[m];
                    var scale = matrix.lossyScale;
                    var scaleY = road.LengthCurve.EvaluateNormalized(lengthTime);
                    scale.y *= scaleY;
                    var modulatedMatrix = Matrix4x4.TRS(matrix.Position(), matrix.rotation, scale);
                    vertexLines[m] = CreateModulatedVertexLine(road, road.SubdivisionsBottom);
                    //vertexLines[m] = CreateModulatedVertexLinesWithInset(new Matrix4x4[] { matrix, new() }, road, line, 1.5f, 3.75f, 0f)[0];
                    for (int i = 0; i < vertexLines[m].Length; i++)
                    {
                        var vertex = vertexLines[m][i];
                        // There's a weird bug if I use the modulated matrix when scale.y is < -1
                        // the verts get all messed up (rotation invertes along y). This works around it.
                        var offset = matrix.rotation * (Vector3.down * thickness);
                        vertex = modulatedMatrix.MultiplyPoint(vertex) + offset;
                        vertexLines[m][i] = vertex;
                    }
                }

                var tristrips = VertexLinesToTristrips(vertexLines);
                AssignTristripMetadata(tristrips, false, false);
                return tristrips;
            }
            public static Tristrip SideNoTex(Matrix4x4[] matrices, float insetTop, float insetBottom, float height, float thickness, float side)
            {
                var matricesTop = ModifyMatrixScales(matrices, new Vector3(insetTop * -2f, 0, 0));
                var matricesBottom = ModifyMatrixScales(matrices, new Vector3(insetBottom * -2f, 0, 0));

                var upper = new Vector3(0.5f * side, height, 0);
                var lower = new Vector3(0.5f * side, -thickness, 0);
                var normal = side < 0 ? Vector3.left : Vector3.right;

                int vertexCount = matrices.Length * 2;
                var tristrip = new Tristrip();
                tristrip.positions = new Vector3[vertexCount];
                tristrip.normals = new Vector3[vertexCount];

                for (int i = 0; i < matrices.Length; i++)
                {
                    var mtxTop = matricesTop[i];
                    var mtxBot = matricesBottom[i];
                    int index0 = i * 2;
                    int index1 = index0 + 1;
                    tristrip.positions[index0] = mtxTop.MultiplyPoint(upper);
                    tristrip.positions[index1] = mtxBot.MultiplyPoint(lower);
                    //
                    var mtx = matrices[i];
                    var orientedNormal = mtx.rotation * normal;
                    tristrip.normals[index0] = orientedNormal;
                    tristrip.normals[index1] = orientedNormal;
                    //
                    tristrip.isBackFacing = side > 0;
                }

                return tristrip;
            }

            public static Tristrip[] SidesTex0(Matrix4x4[] matricesLeft, Matrix4x4[] matricesRight, float repsLength)
            {
                var sideL = SideNoTex(matricesLeft, 0, 0, 0.5f, 1f, -1f);
                var sideR = SideNoTex(matricesRight, 0, 0, 0.5f, 1f, +1f);
                var sideL_uv0 = CreateCustomUVs(new Tristrip[] { sideL }, repsLength, 0, 1);
                var sideR_uv0 = CreateCustomUVs(new Tristrip[] { sideR }, repsLength, 0, -1);
                sideL.tex0 = sideL_uv0[0];
                sideR.tex0 = sideR_uv0[0];
                var sides = new Tristrip[] { sideL, sideR };
                return sides;
            }
            public static Tristrip[] BottomTex0(Matrix4x4[] matrices, GfzShapeRoadModulated road, float[] times, float repsWidth, float repsLength, bool isGfzCoordinateSpace)
            {
                var bottom = BottomNoTex(matrices, road, 1f);
                var uvs0 = CreateUVsFromTimes(bottom, times, repsWidth, repsLength);
                ApplyTex0(bottom, uvs0);
                SetNormalsFromTristripVertices(bottom, true, false, isGfzCoordinateSpace);
                return bottom;
            }

            public static Tristrip EndcapNoTex(Matrix4x4 matrix, GfzShapeRoadModulated road, bool isStart, bool isGfzCoordinateSpace)
            {
                var sampleTimes = CreateEvenlySpacedTimes(road.SubdivisionsTop);
                var temp = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                var matrixTop = matrix;
                var matrixBot = Matrix4x4.TRS(matrix.Position() + (matrix.rotation * (Vector3.down * 1f)), matrix.rotation, matrix.lossyScale);
                var matricesTop = new Matrix4x4[] { matrixTop, temp };
                var matricesBot = new Matrix4x4[] { matrixBot, temp };
                if (!isStart)
                {
                    Array.Reverse(matricesTop);
                    Array.Reverse(matricesBot);
                }
                int index = isStart ? 0 : 1;
                var vertsTop = CreateModulatedVertexLinesWithInset(matricesTop, road, sampleTimes, 1.5f, 3.75f, 0.5f)[index];
                var vertsBot = CreateModulatedVertexLinesWithInset(matricesBot, road, sampleTimes, 1.5f, 3.75f, 0.0f)[index];
                var tristrip = InterleaveVertexLines(vertsTop, vertsBot);

                Vector3 normal = isGfzCoordinateSpace ^ isStart ? Vector3.back : Vector3.forward;
                normal = matrix.rotation * normal;
                var normals = ArrayUtility.DefaultArray(normal, tristrip.VertexCount);
                tristrip.normals = normals;

                tristrip.isBackFacing = isStart;
                return tristrip;
            }
            public static Tristrip[] EndcapsTex0(Matrix4x4[] matrices, GfzShapeRoadModulated road, bool isGfzCoordinateSpace)
            {
                var allTristrips = new List<Tristrip>();

                //
                const float kLengthTrim = 64f;

                GetContinuity(road, out bool isContinuousFrom, out bool isContinuousTo);
                if (!isContinuousFrom)
                {
                    var matrix = matrices[0];
                    var endcapFrom = EndcapNoTex(matrix, road, true, isGfzCoordinateSpace);
                    var uvRepsX = GetTexRepetitions(matrix.lossyScale.x, kLengthTrim);
                    endcapFrom.tex0 = UvStripForward(endcapFrom, uvRepsX);
                    allTristrips.Add(endcapFrom);
                }
                if (!isContinuousTo)
                {
                    var matrix = matrices[matrices.Length - 1];
                    var endcapTo = EndcapNoTex(matrix, road, false, isGfzCoordinateSpace);
                    var uvRepsX = GetTexRepetitions(matrix.lossyScale.x, kLengthTrim);
                    endcapTo.tex0 = UvStripForward(endcapTo, uvRepsX);
                    allTristrips.Add(endcapTo);
                }

                return allTristrips.ToArray();
            }




            public static class MuteCityCOM
            {
                internal static Tristrip[][] FullTop(Matrix4x4[] matrices, GfzShapeRoadModulated road, float segmentLength, bool isGfzCoordinateSpace)
                {
                    var tristripArrays = FullTopNoTex(matrices, road, kCurbSlantOuter, kCurbSlantInner, kCurbHeight, isGfzCoordinateSpace);
                    var top = tristripArrays[0];
                    var trimTop = tristripArrays[1];
                    var trimSlope = tristripArrays[2];

                    float repetitionsRoadTexture = GetTexRepetitions(segmentLength, TexStrideRoad);
                    float repetitionsSideTexture = GetTexRepetitionsRoundUp(segmentLength, TexStrideTrim, 2);

                    // in the future you can use more appropriate times
                    var times = CreateEvenlySpacedTimes(road.SubdivisionsTop);
                    var top_uvs0 = CreateUVsFromTimes(top, times, 8, repetitionsRoadTexture);
                    var top_uvs1 = CreateUVsFromTimes(top, times, 2, repetitionsSideTexture);
                    top_uvs1 = OffsetUV(top_uvs1, new Vector2(0, 1.0f));
                    ApplyTex0(top, top_uvs0);
                    ApplyTex1(top, top_uvs1);

                    var trimTop_uvs0 = CreateCustomUVs(trimTop, repetitionsSideTexture, 0, 1, 2);
                    trimTop_uvs0 = OffsetUV(trimTop_uvs0, new Vector2(0, 0.5f));
                    ApplyTex0(trimTop, trimTop_uvs0);

                    var trimSlope_uv0 = CreateCustomUVs(trimSlope, repetitionsSideTexture, 0, 1, 2);
                    var trimSlope_uv1 = CreateCustomUVs(trimSlope, repetitionsSideTexture, 0, 2, 4);
                    trimSlope_uv0 = OffsetUV(trimSlope_uv0, new Vector2(0, 0.5f));
                    trimSlope_uv1 = OffsetUV(trimSlope_uv1, new Vector2(0, 1.0f));
                    ApplyTex0(trimSlope, trimSlope_uv0);
                    ApplyTex1(trimSlope, trimSlope_uv1);

                    return tristripArrays;
                }
                internal static Tristrip[] Rails(Matrix4x4[] matricesLeft, Matrix4x4[] matricesRight, GfzShapeRoadModulated road)
                {
                    var rails = new List<Tristrip>();

                    // A gross hack, you should rafactor so you can use this class or break out important values to pass in
                    var tempRoad = new GfzShapeRoad();
                    tempRoad.railHeightLeft = road.OverrideRailHeights ? road.OverrideRailLeftHeight : 6;
                    tempRoad.railHeightRight = road.OverrideRailHeights ? road.OverrideRailRightHeight : 6;

                    if (road.HasRailLeft)
                    {
                        var railsL = Road.MuteCity.RailTex2(matricesLeft, tempRoad, true);
                        rails.AddRange(railsL);
                    }
                    if (road.HasRailRight)
                    {
                        var railsR = Road.MuteCity.RailTex2(matricesRight, tempRoad, false);
                        rails.AddRange(railsR);
                    }
                    return rails.ToArray();
                }

                public static Tristrip[][] Shape(Matrix4x4[] matrices, GfzShapeRoadModulated road, float segmentLength, bool isGfzCoordinateSpace)
                {
                    // Top and curbs
                    var roadTopTristripArrays = FullTop(matrices, road, segmentLength, isGfzCoordinateSpace);
                    var top = roadTopTristripArrays[0];
                    var trimTop = roadTopTristripArrays[1];
                    var trimSlope = roadTopTristripArrays[2];

                    // Get texture strides
                    float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
                    float repetitionsSideTexture = math.ceil(segmentLength / TexStrideRoad / 2);

                    // Bottom
                    // In the future, you can use more appropriate times
                    var times = CreateEvenlySpacedTimes(road.SubdivisionsBottom);
                    var bottom = BottomTex0(matrices, road, times, 4, repetitionsRoadTexture, isGfzCoordinateSpace);

                    //matrices = StripHeight(matrices);


                    // Sides and Rails
                    var matricesLeft = CreateModulatedMatrices(matrices, road, 0);
                    var matricesRight = CreateModulatedMatrices(matrices, road, 1);
                    var sides = SidesTex0(matricesLeft, matricesRight, repetitionsSideTexture);
                    foreach (var side in sides)
                        side.tex0 = OffsetUV(side.tex0, new Vector2(0, 0.5f));

                    var rails = Rails(matricesLeft, matricesRight, road);

                    var endcaps = EndcapsTex0(matrices, road, isGfzCoordinateSpace);
                    sides = ConcatTristrips(sides, endcaps);

                    var tristripsArray = new Tristrip[][]
                    {
                        top,
                        trimTop,
                        trimSlope,
                        bottom,
                        sides,
                        rails,
                    };
                    return tristripsArray;
                }
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

                float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
                var uvs0 = CreateTristripScaledUVs(tristrips, 8, repetitionsRoadTexture);
                for (int i = 0; i < tristrips.Length; i++)
                    tristrips[i].tex0 = uvs0[i];

                return tristrips;
            }
        }

        public static class Cylinder
        {
            // DEPRECATE and use TristripGen.CircleEndCap
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

                float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
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

                float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
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

                float repetitionsRoadTexture = math.ceil(segmentLength / TexStrideRoad);
                var uvs0 = CreateTristripScaledUVs(tristrips, texRepetitions, repetitionsRoadTexture);
                for (int i = 0; i < tristrips.Length; i++)
                    tristrips[i].tex0 = uvs0[i];

                return tristrips;
            }
        }

        public static class OpenPipe
        {
            public static Tristrip[] GenericOpenPipeNoTex(Matrix4x4[] matrices, GfzShapeOpenPipeCylinder open, bool isGfzCoordinateSpace)
            {
                int nTristrips = open.SubdivisionsPipe;
                var tristrips = GenerateOpenCircleWithNormals(matrices, open.OpennessCurveRenormalized, nTristrips, true, false, isGfzCoordinateSpace);
                AssignTristripMetadata(tristrips, true, false);
                return tristrips;
            }
            public static Tristrip[][] DebugOpenPipe(Matrix4x4[] matrices, GfzShapeOpenPipeCylinder open, bool isGfzCoordinateSpace)
            {
                var pipeTristrips = GenericOpenPipeNoTex(matrices, open, isGfzCoordinateSpace);
                var settings = GfzProjectWindow.GetSettings();

                float edge = 0.05f;
                float min = edge;
                float max = 1f - edge;
                int edgeIndexL = math.max((int)math.floor(pipeTristrips.Length * min), 1);
                int edgeIndexR = math.min((int)math.ceil(pipeTristrips.Length * max), pipeTristrips.Length - 1);

                var leftEdgeTristrips = pipeTristrips[..edgeIndexL];
                var rightEdgeTristrips = pipeTristrips[edgeIndexR..];
                var centerTristrips = pipeTristrips[edgeIndexL..edgeIndexR];

                ApplyColor0(leftEdgeTristrips, settings.DebugTrackLeft);
                ApplyColor0(rightEdgeTristrips, settings.DebugTrackRight);
                ApplyColor0(centerTristrips, settings.DebugTrackSurface);

                var tristripsCollection = new Tristrip[1][];
                tristripsCollection[0] = pipeTristrips;
                return tristripsCollection;
            }

            public static class MuteCityCOM
            {
                public static Tristrip[][] OpenPipe(Matrix4x4[] matrices, GfzShapeOpenPipeCylinder open, float segmentLength, bool isGfzCoordinateSpace)
                {
                    float repetitions = math.ceil(segmentLength / TexStrideRoad);
                    var tristrips = GenericOpenPipeNoTex(matrices, open, isGfzCoordinateSpace);

                    float edge = 0.05f;
                    float min = edge;
                    float max = 1f - edge;
                    int edgeIndexL = math.max((int)math.floor(tristrips.Length * min), 1);
                    int edgeIndexR = math.min((int)math.ceil(tristrips.Length * max), tristrips.Length - 1);

                    var leftEdgeTristrips = tristrips[..edgeIndexL];
                    var rightEdgeTristrips = tristrips[edgeIndexR..];
                    var edgeTristrips = ConcatTristrips(leftEdgeTristrips, rightEdgeTristrips);
                    var centerTristrips = tristrips[edgeIndexL..edgeIndexR];

                    ApplyTex0(leftEdgeTristrips, CreateTristripScaledUVs(leftEdgeTristrips, 1, repetitions));
                    ApplyTex0(rightEdgeTristrips, CreateTristripScaledUVs(rightEdgeTristrips, 1, repetitions));
                    ApplyTex1(edgeTristrips, ScaleUV(CopyTex0(edgeTristrips), 2));
                    ApplyTex0(centerTristrips, CreateTristripScaledUVs(tristrips, 8, repetitions)[edgeIndexL..edgeIndexR]);

                    var allTristrips = new Tristrip[][]
                    {
                        edgeTristrips,
                        centerTristrips,
                    };

                    foreach (var tristrip in tristrips)
                        tristrip.isDoubleSided = true;

                    return allTristrips;
                }
                public static GcmfTemplate[] OpenPipeMaterials()
                {
                    var materials = new GcmfTemplate[]
                    {
                        GcmfTemplates.MuteCity.RoadEmbelishments(),
                        GcmfTemplates.MuteCityCOM.RoadTopNoDividers(),
                    };
                    foreach (var mat in materials)
                        mat.Submesh.RenderFlags |= GameCube.GFZ.GMA.RenderFlags.doubleSidedFaces;

                    return materials;
                }
            }
        }

        public static class OpenCylinder
        {
            private static void AngleFromTo(GfzShapeOpenPipeCylinder open, bool isStart, bool isPipe, out float from, out float to)
            {
                // When pipe, 180+-180; when cylinder, 0+-180.
                int baseAngle = isPipe ? 180 : 0;
                int angleFrom = baseAngle + 180;
                int angleTo = baseAngle - 180;
                var curve = open.OpennessCurveRenormalized;
                float time = isStart ? 0 : curve.GetMaxTime();
                float ratio = math.clamp(curve.EvaluateNormalized(time), 0f, 1f);

                from = math.lerp(baseAngle, angleFrom, ratio);
                to = math.lerp(baseAngle, angleTo, ratio);
            }
            public static Tristrip OpenCircleEndCapTex0(Matrix4x4 matrix, int nTristrips, Vector3 normal, float angleFrom, float angleTo, bool isBackFacing)
            {
                var vertices = CreateCircleVertices(nTristrips, angleFrom, angleTo);

                var tristrip = new Tristrip();
                int vertexCount = vertices.Length + 1;
                tristrip.positions = new Vector3[vertexCount];
                tristrip.tex0 = ArrayUtility.DefaultArray(new Vector2(0.5f, 0.5f), vertexCount);
                tristrip.normals = ArrayUtility.DefaultArray(normal, vertexCount);

                for (int i = 0; i < vertexCount; i++)
                {
                    bool isEven = i % 2 == 0;
                    int halfI = i / 2;
                    int index = isEven
                        ? vertexCount - halfI - 2
                        : halfI;

                    Vector3 vertex = vertices[index];
                    Vector2 tex0 = new Vector2(vertex.x, vertex.y);
                    vertex = matrix.MultiplyPoint(vertex);
                    tristrip.positions[i] = vertex;
                    tristrip.tex0[i] += tex0;
                }

                // Make array to assign data
                var tristrips = new Tristrip[] { tristrip };
                AssignTristripMetadata(tristrips, isBackFacing, false);

                return tristrip;
            }

            public static Tristrip[] GenericOpenCylinderNoTex(Matrix4x4[] matrices, GfzShapeOpenPipeCylinder open, bool isGfzCoordinateSpace)
            {
                int nTristrips = open.SubdivisionsCylinder;
                var tristrips = GenerateOpenCircleWithNormals(matrices, open.OpennessCurveRenormalized, nTristrips, false, false, isGfzCoordinateSpace);
                AssignTristripMetadata(tristrips, false, false);
                return tristrips;
            }
            public static Tristrip[] GenericOpenCylinderBottomCapNoTex(Matrix4x4[] matrices, GfzShapeOpenPipeCylinder open, bool isGfzCoordinateSpace)
            {
                // By doing 1 strip we do the ends :ok_hand:
                // Inversion of space only affects the normals direction
                var tristrips = GenerateOpenCircleWithNormals(matrices, open.OpennessCurveRenormalized, 1, false, false, !isGfzCoordinateSpace);
                AssignTristripMetadata(tristrips, true, false);
                return tristrips;
            }

            public static Tristrip[] GenericOpenCylinderEndCapTex0(Matrix4x4[] matrices, GfzShapeOpenPipeCylinder open, bool isGfzCoordinateSpace)
            {
                var allTristrips = new List<Tristrip>();

                GetContinuity(open, out bool isContinuousFrom, out bool isContinuousTo);

                if (!isContinuousFrom)
                {
                    var matrix = matrices[0];
                    AngleFromTo(open, true, false, out float angleFrom, out float angleTo);
                    var tristrip = OpenCircleEndCapTex0(matrix, open.SubdivisionsCylinder, Vector3.back, angleFrom, angleTo, true);
                    tristrip.tex0 = ScaleUV(tristrip.tex0, (Vector2)matrix.lossyScale);
                    allTristrips.Add(tristrip);
                }

                if (!isContinuousTo)
                {
                    var matrix = matrices[matrices.Length - 1];
                    AngleFromTo(open, false, false, out float angleFrom, out float angleTo);
                    var tristrip = OpenCircleEndCapTex0(matrix, open.SubdivisionsCylinder, Vector3.forward, angleFrom, angleTo, false);
                    tristrip.tex0 = ScaleUV(tristrip.tex0, (Vector2)matrix.lossyScale);
                    allTristrips.Add(tristrip);
                }

                var tristrips = allTristrips.ToArray();
                //AssignTristripMetadata(tristrips, true, false);
                return tristrips;
            }

            public static Tristrip[][] DebugOpenCylinder(Matrix4x4[] matrices, GfzShapeOpenPipeCylinder open, bool isGfzCoordinateSpace)
            {
                var cylinderTristrips = GenericOpenCylinderNoTex(matrices, open, isGfzCoordinateSpace);
                var settings = GfzProjectWindow.GetSettings();

                float edge = 0.05f;
                float min = edge;
                float max = 1f - edge;
                int edgeIndexL = math.max((int)math.floor(cylinderTristrips.Length * min), 1);
                int edgeIndexR = math.min((int)math.ceil(cylinderTristrips.Length * max), cylinderTristrips.Length - 1);

                var leftEdgeTristrips = cylinderTristrips[..edgeIndexL];
                var rightEdgeTristrips = cylinderTristrips[edgeIndexR..];
                var centerTristrips = cylinderTristrips[edgeIndexL..edgeIndexR];
                var bottomTristrips = GenericOpenCylinderBottomCapNoTex(matrices, open, isGfzCoordinateSpace);
                var endcapTristrips = GenericOpenCylinderEndCapTex0(matrices, open, isGfzCoordinateSpace);

                ApplyColor0(leftEdgeTristrips, settings.DebugTrackLeft);
                ApplyColor0(rightEdgeTristrips, settings.DebugTrackRight);
                ApplyColor0(centerTristrips, settings.DebugTrackSurface);
                ApplyColor0(bottomTristrips, settings.DebugTrackUnderside);
                ApplyColor0(endcapTristrips, settings.DebugTrackUnderside);

                var tristripsCollection = new Tristrip[1][];
                tristripsCollection[0] = ConcatTristrips(cylinderTristrips, bottomTristrips, endcapTristrips);

                // Clear tex
                foreach (var tristrip in tristripsCollection[0])
                {
                    tristrip.tex0 = null;
                    tristrip.tex1 = null;
                    tristrip.tex2 = null;
                }

                return tristripsCollection;
            }

            public static class MuteCityCOM
            {
                public static Tristrip[][] OpenCylinder(Matrix4x4[] matrices, GfzShapeOpenPipeCylinder open, float segmentLength, bool isGfzCoordinateSpace)
                {
                    // TODO:
                    // hardcoded 8 for track tex width repetitions
                    // hardcoded 40 for encap size

                    float repetitions = math.ceil(segmentLength / TexStrideRoad);
                    var tristrips = GenericOpenCylinderNoTex(matrices, open, isGfzCoordinateSpace);

                    float edge = 0.05f; // 5%
                    float min = edge;
                    float max = 1f - edge; // 100% - edge%
                    int edgeIndexL = math.max((int)math.floor(tristrips.Length * min), 1);
                    int edgeIndexR = math.min((int)math.ceil(tristrips.Length * max), tristrips.Length - 1);

                    var leftEdgeTristrips = tristrips[..edgeIndexL];
                    var rightEdgeTristrips = tristrips[edgeIndexR..];
                    var edgeTristrips = ConcatTristrips(leftEdgeTristrips, rightEdgeTristrips);
                    var centerTristrips = tristrips[edgeIndexL..edgeIndexR];
                    var bottomTristrips = GenericOpenCylinderBottomCapNoTex(matrices, open, isGfzCoordinateSpace);
                    var endcapTristrips = GenericOpenCylinderEndCapTex0(matrices, open, isGfzCoordinateSpace);

                    ApplyTex0(leftEdgeTristrips, CreateTristripScaledUVs(leftEdgeTristrips, 1, repetitions));
                    ApplyTex0(rightEdgeTristrips, CreateTristripScaledUVs(rightEdgeTristrips, 1, repetitions));
                    ApplyTex1(edgeTristrips, ScaleUV(CopyTex0(edgeTristrips), 2));
                    ApplyTex0(centerTristrips, CreateTristripScaledUVs(tristrips, 8, repetitions)[edgeIndexL..edgeIndexR]);
                    ApplyTex0(bottomTristrips, ScaleUV(CreateTristripScaledUVs(bottomTristrips, 1, repetitions), new Vector2(2, 1)));
                    ApplyTex0(endcapTristrips, ScaleUV(CopyTex0(endcapTristrips), new Vector2(2 / 40f, 1 / 40f))); // play with this, play with end cap tex 0 scaling

                    var allTristrips = new Tristrip[][]
                    {
                        edgeTristrips,
                        centerTristrips,
                        bottomTristrips,
                        endcapTristrips,
                    };

                    return allTristrips;
                }
                public static GcmfTemplate[] OpenCylinderMaterials()
                {
                    var materials = new GcmfTemplate[]
                    {
                        GcmfTemplates.MuteCity.RoadEmbelishments(),
                        GcmfTemplates.MuteCityCOM.RoadTopNoDividers(),
                        GcmfTemplates.MuteCity.RoadBottom(),
                        GcmfTemplates.MuteCity.RoadBottom(),
                        //GcmfTemplates.Debug.CreateUnlitVertexColored(),
                    };
                    return materials;
                }
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