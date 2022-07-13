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
    public static class TrackGeoGenerator
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
                var matrix = hacTRS.EvaluateHierarchyMatrix(sampleTime);
                matrices[i] = matrix;
                // debug
                var position = matrices[i].Position();
                var rotation = matrices[i].Rotation().eulerAngles;
                var scale = matrices[i].Scale();
            }
            return matrices;
        }

        public static Matrix4x4[] GenerateMatrixIntervals(AnimationCurveTRS animationCurveTRS, Matrix4x4 staticMatrix, float maxStep)
        {
            var segmentLength = animationCurveTRS.GetMaxTime();
            float step = segmentLength / maxStep;
            int totalIterations = (int)math.ceil(step);

            var matrices = new Matrix4x4[totalIterations + 1];

            for (int i = 0; i <= totalIterations; i++)
            {
                double percentage = i / (double)totalIterations;
                double sampleTime = percentage * segmentLength;
                var animationMatrix = animationCurveTRS.EvaluateMatrix(sampleTime);
                matrices[i] = staticMatrix * animationMatrix;
            }
            return matrices;
        }

        public static Tristrip[] GenerateTristrips(Matrix4x4[] matrices, Vector3[] vertices)
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
                // And then everything else in-between. The vertex between 2 tristrips is the same,
                // so we can copy them to both tristrips, just offset (n+0.prev, n+1.next).
                for (int t = 0; t < lastTristripIndex; t++)
                {
                    var vertex = matrix.MultiplyPoint(vertices[t + 1]);
                    tristrips[t + 0].positions[index1] = vertex;
                    tristrips[t + 1].positions[index0] = vertex;
                }
            }
            return tristrips;
        }

        public static Tristrip[] CreateAllTemp(GfzTrackSegmentNode node, int nTristrips, float maxStep, bool useGfzCoordSpace)
        {
            var allTriStrips = new List<Tristrip>();
            var hacTRS = node.CreateHierarchichalAnimationCurveTRS(useGfzCoordSpace);
            var segmentLength = hacTRS.GetSegmentLength();
            var matrices = GenerateMatrixIntervals(hacTRS, maxStep);

            // track top
            {
                var endpointA = new Vector3(-0.5f, 0, 0);
                var endpointB = new Vector3(+0.5f, 0, 0);
                var color0 = new Color32(128, 128, 128, 255);
                var normal = Vector3.up;
                var trackTopTristrips = CreateTristrips(matrices, endpointA, endpointB, nTristrips, color0, normal, 3, true);
                allTriStrips.AddRange(trackTopTristrips);
            }

            // track bottom
            {
                var endpointA = new Vector3(-0.5f, -2.0f, 0);
                var endpointB = new Vector3(+0.5f, -2.0f, 0);
                var color0 = new Color32(48, 48, 48, 255);
                var normal = Vector3.down;
                var trackBottomTristrips = CreateTristrips(matrices, endpointA, endpointB, nTristrips, color0, normal, 3, false);
                allTriStrips.AddRange(trackBottomTristrips);
            }

            // track left
            {
                var endpointA = new Vector3(-0.5f, +0.0f, 0);
                var endpointB = new Vector3(-0.5f, -2.0f, 0);
                var color0 = new Color32(127, 255, 127, 255); // green
                var normal = Vector3.left;
                var trackLeftTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, normal, 0, false);
                allTriStrips.AddRange(trackLeftTristrips);
            }

            // track right
            {
                var endpointA = new Vector3(+0.5f, +0.0f, 0);
                var endpointB = new Vector3(+0.5f, -2.0f, 0);
                var color0 = new Color32(255, 127, 127, 255); // red
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
                    var color0 = new Color32(0, 255, 0, 255); // green
                    var trackRightTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, null, 0, false);
                    allTriStrips.AddRange(trackRightTristrips);
                }

                // rail right
                if (rails.RailHeightRight > 0f)
                {
                    var endpointA = new Vector3(+0.5f, +0.0f, 0);
                    var endpointB = new Vector3(+0.5f, rails.RailHeightRight, 0);
                    var color0 = new Color32(255, 0, 0, 255); // red
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

                var endpointA = new Vector3(-0.5f, +0.10f, 0);
                var endpointB = new Vector3(+0.5f, +0.10f, 0);
                var color0 = new Color32(96, 96, 96, 255); // dark grey
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
                var color0 = new Color32(127, 255, 255, 255); // cyan
                var normal = Vector3.up;
                var trackLaneDivider = CreateTristrips(matricesNoScale, endpointA, endpointB, 1, color0, normal, 0, true);
                allTriStrips.AddRange(trackLaneDivider);
            }

            return allTriStrips.ToArray();
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


        public static Tristrip[] CreateTristrips(Matrix4x4[] matrices, Vector3 endpointA, Vector3 endpointB, int nTristrips, Color32 color0, Vector3? defaultNormal, int uvs, bool reverse)
        {
            // Sample left and right vertices
            var vertices = new Vector3[nTristrips + 1];
            for (int i = 0; i < vertices.Length; i++)
            {
                float percentage = i / (float)nTristrips;
                vertices[i] = Vector3.Lerp(endpointA, endpointB, percentage);
            }

            // Make tristrips
            var tristrips = GenerateTristrips(matrices, vertices);

            // Assign data
            // CLR0
            foreach (var tristrip in tristrips)
            {
                // COLOR
                var nVertices = tristrip.positions.Length;
                tristrip.color0 = new Color32[nVertices];
                for (int i = 0; i < nVertices; i++)
                {
                    tristrip.color0[i] = color0;
                }
                // NORMALS
                if (defaultNormal is not null)
                    tristrip.normals = CreateNormals(matrices, (Vector3)defaultNormal);

                // UVs
                //if (uvs > 0)
                //    tristrip.uv0 = CreateUVs(matrices);
                //if (uvs > 1)
                //    tristrip.uv1 = CreateUVs(matrices);
                //if (uvs > 2)
                //    tristrip.uv2 = CreateUVs(matrices);

                tristrip.reverse = reverse;
            }

            return tristrips;
        }

        // MAJOR ASSUMPTION: num normals = matrices * 2 since tristrips
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
        public static Vector2[] CreateUVs(Matrix4x4[] matrices)
        {
            int baseIndex = 0;
            var uvs = new Vector2[matrices.Length * 2];
            for (int i = 0; i < matrices.Length; i++)
            {
                int index = i * 2;
                uvs[index + 0] = new Vector2(0, baseIndex);
                uvs[index + 1] = new Vector2(1, baseIndex);
                baseIndex++;
                baseIndex %= 2;
            }
            return uvs;
        }


        [MenuItem("Manifold/Scene/Export scene track model GMA (as common.gma)")]
        public static void TestGmaExport4()
        {
            var settings = GfzProjectWindow.GetSettings();
            var filePath = settings.FileOutput + $"common.gma";
            var gma = CreateTrackModelsGma("Track Segment");
            BinarySerializableIO.SaveFile(gma, filePath);
            LzUtility.CompressAvLzToDisk(filePath, GameCube.AmusementVision.GxGame.FZeroGX, true);
            OSUtility.OpenDirectory(filePath);
        }

        public static Gma CreateTrackModelsGma(string modelName)
        {
            // TODO: get GfzTrack, use it to get children
            var track = GameObject.FindObjectOfType<GfzTrack>(false);
            track.FindChildSegments();

            int debugIndex = 0;
            var models = new List<Model>();
            foreach (var trackSegment in track.AllRoots)
            {
                // Make the vertex data
                var trackMeshTristrips = CreateAllTemp(trackSegment, 4, 10f, true);
                // convert to GameCube format
                var dlists = TristripsToDisplayLists(trackMeshTristrips, GameCube.GFZ.GfzGX.VAT);

                // Compute bounding sphere
                var allVertices = new List<Vector3>();
                foreach (var tristrip in trackMeshTristrips)
                    allVertices.AddRange(tristrip.positions);
                var boundingSphere = CreateBoundingSphereFromPoints(allVertices);

                var template = GfzAssetTemplates.MeshTemplates.DebugTemplates.CreateLitVertexColored();
                var gcmf = template.Gcmf;
                gcmf.BoundingSphere = boundingSphere;
                gcmf.Submeshes[0].PrimaryDisplayListsTranslucid = dlists;
                gcmf.Submeshes[0].VertexAttributes = dlists[0].Attributes; // hacky
                gcmf.Submeshes[0].UnkAlphaOptions.Origin = boundingSphere.origin;
                gcmf.PatchTevLayerIndexes();

                models.Add(new Model($"{modelName} {debugIndex++}", gcmf));
            }

            // Create single GMA for model, comprised on many GCMFs (display lists and materials)
            var gma = new Gma();
            gma.Models = models.ToArray();

            return gma;
        }

        private static DisplayList[] TristripsToDisplayLists(Tristrip[] tristrips, VertexAttributeTable vat)
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
                Copy(tristrip.uv0, ref dlist.tex0);
                Copy(tristrip.uv1, ref dlist.tex1);
                Copy(tristrip.uv2, ref dlist.tex2);
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

        // http://www.technologicalutopia.com/sourcecode/xnageometry/boundingsphere.cs.htm
        public static GameCube.GFZ.BoundingSphere CreateBoundingSphereFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
                throw new System.ArgumentNullException("points");

            float radius = 0;
            Vector3 center = new Vector3();
            // First, we'll find the center of gravity for the point 'cloud'.
            int num_points = 0; // The number of points (there MUST be a better way to get this instead of counting the number of points one by one?)

            foreach (Vector3 v in points)
            {
                center += v;    // If we actually knew the number of points, we'd get better accuracy by adding v / num_points.
                ++num_points;
            }

            center /= num_points;

            // Calculate the radius of the needed sphere (it equals the distance between the center and the point further away).
            foreach (Vector3 v in points)
            {
                float distance = (v - center).magnitude;

                if (distance > radius)
                    radius = distance;
            }

            return new GameCube.GFZ.BoundingSphere(center, radius);
        }

    }
}
