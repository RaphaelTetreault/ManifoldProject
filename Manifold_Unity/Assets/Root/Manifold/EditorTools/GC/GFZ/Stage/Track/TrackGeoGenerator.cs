using GameCube.GX;
using GameCube.GFZ.GMA;
using GameCube.GFZ.LZ;
using GameCube.GFZ.Stage;
using Manifold;
using Manifold.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class TrackMesh
    {
        public Tristrip[] TrackTop { get; set; }
        public Tristrip[] TrackBottom { get; set; }
        public Tristrip[] TrackLeft { get; set; }
        public Tristrip[] TrackRight { get; set; }
        public Tristrip[] RailLeft { get; set; }
        public Tristrip[] RailRight { get; set; }

        public Tristrip[] GetAllTristrips()
        {
            var temp = new List<Tristrip>();
            temp.AddRange(TrackTop);
            temp.AddRange(TrackBottom);
            temp.AddRange(TrackLeft);
            temp.AddRange(TrackRight);
            temp.AddRange(RailLeft);
            temp.AddRange(RailRight);
            return temp.ToArray();
        }
    }

    public class Tristrip
    {
        public Vector3[] positions { get; set; }
        public Vector3[] normals { get; set; }
        public Vector3[] binormals { get; set; }
        public Vector3[] tangents { get; set; }
        public Vector2[] uv0 { get; set; }
        public Vector2[] uv1 { get; set; }
        public Vector2[] uv2 { get; set; }
        public Color[] color0 { get; set; }
        public Color[] color1 { get; set; }

        public int TrianglesCount => ((positions.Length / 2) - 1) * 6;

        public int[] GetIndices()
        {
            int nTriangles = TrianglesCount;
            int[] indexes = new int[nTriangles];

            // Process 1 triangle at a time
            int vertexBaseIndex = 0;
            for (int i = 0; i < indexes.Length; i += 6)
            {
                indexes[i + 0] = vertexBaseIndex + 0;
                indexes[i + 1] = vertexBaseIndex + 1;
                indexes[i + 2] = vertexBaseIndex + 2;
                vertexBaseIndex++;
                indexes[i + 3] = vertexBaseIndex + 0;
                indexes[i + 4] = vertexBaseIndex + 2;
                indexes[i + 5] = vertexBaseIndex + 1;
                vertexBaseIndex++;
            }
            return indexes;
        }
    }


    public static class TrackGeoGenerator
    {
        public static Matrix4x4[] GenerateMtxIntervals(GfzTrackSegment trackSegment, float maxStep)
        {
            var trueMts = trackSegment.AnimationCurveTRS.CreateDeepCopy();
            trueMts.Rotation.z = trueMts.Rotation.z.GetInverted();

            var segmentLength = trueMts.GetMaxTime();
            float step = segmentLength / maxStep;
            int totalIterations = (int)math.ceil(step);

            var staticMatrix = trackSegment.transform.localToWorldMatrix;
            var matrices = new Matrix4x4[totalIterations + 1];

            for (int i = 0; i <= totalIterations; i++)
            {
                double percentage = i / (double)totalIterations;
                double sampleTime = percentage * segmentLength;
                var animationMatrix = trueMts.EvaluateMatrix(sampleTime);
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
                // so we can copy them to both tristrips, just offset (n+0.last, n+1.first).
                for (int t = 0; t < lastTristripIndex; t++)
                {
                    var vertex = matrix.MultiplyPoint(vertices[t + 1]);
                    tristrips[t + 0].positions[index1] = vertex;
                    tristrips[t + 1].positions[index0] = vertex;
                }
            }
            return tristrips;
        }

        // This shouldn't really be a function, too hard-coded
        public static Tristrip[] CreateTrackTemp(GfzTrackSegment trackSegment, int nTristrips, float maxStep)
        {
            // Sample left and right vertices
            var endpointA = new Vector3(-0.5f, 0, 0);
            var endpointB = new Vector3(+0.5f, 0, 0);
            var vertices = new Vector3[nTristrips + 1];
            for (int i = 0; i < vertices.Length; i++)
            {
                float percentage = i / (float)nTristrips;
                vertices[i] = Vector3.Lerp(endpointA, endpointB, percentage);
            }

            var matrices = GenerateMtxIntervals(trackSegment, maxStep);
            var tristrips = GenerateTristrips(matrices, vertices);

            foreach (var tristrip in tristrips)
            {
                tristrip.normals = new Vector3[tristrip.positions.Length];
                for (int i = 0; i < tristrip.normals.Length; i++)
                    tristrip.normals[i] = new Vector3(0, 1, 0);

                tristrip.uv0 = new Vector2[tristrip.positions.Length];
                for (int i = 0; i < tristrip.normals.Length; i++)
                {
                    tristrip.normals[i] = new Vector2(i*0.5f, i*0.1f);
                }
            }

            return tristrips;
        }


        [MenuItem("Manifold/HEY! TEST GMA")]
        public static void TestGmaExport3()
        {
            // TODO: get GfzTrack, use it to get children
            var track = GameObject.FindObjectOfType<GfzTrack>(false);
            track.FindChildSegments();

            int debugIndex = 0;
            var models = new List<Model>();
            foreach (var trackSegment in track.AllRootSegmentShapes)
            {
                // Make the vertex data
                var trackMeshTristrips = CreateTrackTemp(trackSegment.Segment, 4, 10f);
                // convert to GameCube format
                var dlists = TristripsToDisplayLists(trackMeshTristrips, GameCube.GFZ.GfzGX.VAT);

                // Compute bounding sphere
                var allVertices = new List<Vector3>();
                foreach (var tristrip in trackMeshTristrips)
                    allVertices.AddRange(tristrip.positions);
                var boundingSphere = CreateFromPoints(allVertices);

                var railTemplate = GfzAssetTemplates.MeshTemplates.MuteCity.CreateRail();
                var railGcmf = railTemplate.Gcmf;
                railGcmf.BoundingSphere = boundingSphere;
                railGcmf.Submeshes[0].PrimaryDisplayListsTranslucid = dlists;
                railGcmf.Submeshes[0].VertexAttributes = dlists[0].Attributes; // hacky
                railGcmf.Submeshes[0].UnkAlphaOptions.Origin = boundingSphere.origin;
                railGcmf.PatchTevLayerIndexes();

                models.Add(new Model($"TRACK TEST {debugIndex++}", railGcmf));
            }

            // Create single GMA for model, comprised on many GCMFs (display lists and materials)
            var gma = new Gma();
            gma.Models = models.ToArray();

            var settings = GfzProjectWindow.GetSettings();
            var dest = settings.FileOutput + "common.gma";
            BinarySerializableIO.SaveFile(gma, dest);
            LzUtility.CompressAvLzToDisk(dest, GameCube.AmusementVision.GxGame.FZeroGX, true);
            OSUtility.OpenDirectory(dest);
        }


        public static void TestGmaExport2()
        {
            // TODO: get GfzTrack, use it to get children
            var track = GameObject.FindObjectOfType<GfzTrack>(false);
            track.FindChildSegments();

            // TODO: iterate over all segments, not just the first
            var trackSegment = track.StartSegmentShape.Segment;

            // Make the vertex data
            var trackMeshTristrips = CreateTrackTemp(trackSegment, 4, 10f);
            // convert to GameCube format
            var dlists = TristripsToDisplayLists(trackMeshTristrips, GameCube.GFZ.GfzGX.VAT);

            var submeshes = new Submesh[]
            {
                new Submesh()
                {
                    RenderFlags = RenderFlags.doubleSidedFaces,
                    VertexAttributes = dlists[0].Attributes, // hacky

                    Material = new GameCube.GFZ.GMA.Material()
                    {
                        MaterialColor = new GXColor(0xFFFFFFFF),
                        AmbientColor = new GXColor(0xFFFFFFFF),
                        SpecularColor = new GXColor(0x00000000),
                        // ??? THIS IS OPTIONAL?
                        MaterialDestination = 0,
                    },

                    // Set DLs
                    PrimaryDisplayListsOpaque = dlists,
                },
            };

            // Compute bounding sphere
            var allVertices = new List<Vector3>();
            foreach (var tristrip in trackMeshTristrips)
                allVertices.AddRange(tristrip.positions);
            var boundingSphere = CreateFromPoints(allVertices);

            // Create single GMA for model, comprised on many GCMFs (display lists and materials)
            var gma = new Gma();
            gma.Models = new Model[]
            {
                new Model()
                {
                    Name = "my cool model name",
                    Gcmf = new Gcmf()
                    {
                        // attributes = default
                        Attributes = (GcmfAttributes)0,
                        BoundingSphere = boundingSphere,
                        //
                        TextureConfigsCount = 0,
                        OpaqueMaterialCount = (ushort)submeshes.Length,
                        TranslucidMaterialCount = 0,
                        BoneCount = 0,
                        //
                        TevLayers = new TevLayer[0], // this is what you get from templating materials...
                        Submeshes = submeshes,
                    },
                },
            };

            var settings = GfzProjectWindow.GetSettings();
            var dest = settings.FileOutput + "common.gma";
            BinarySerializableIO.SaveFile(gma, dest);
            LzUtility.CompressAvLzToDisk(dest, GameCube.AmusementVision.GxGame.FZeroGX, true);
            OSUtility.OpenDirectory(dest);
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

                // TODO: set to something real
                dlist.clr0 = GetColor(tristrip.positions.Length, new GXColor(255, 255, 255, 255));

                // This is actually bad when there are more than one width divisions - the strip
                // does "wrap" correctly, right?
                Copy(tristrip.positions, ref dlist.pos);
                Copy(tristrip.normals, ref dlist.nrm);
                Copy(tristrip.uv0, ref dlist.tex0);

                // Set attributes based on provided data
                dlist.Attributes = dlist.ComponentsToGXAttributes();
                dlist.VertexCount = checked((ushort)tristrip.positions.Length);

                displayLists[i] = dlist;
            }
            return displayLists;
        }

        private static void Copy(Vector3[] vector3s, ref float3[] float3s)
        {
            float3s = new float3[vector3s.Length];
            for (int i = 0; i < float3s.Length; i++)
                float3s[i] = vector3s[i];
        }
        private static void Copy(Vector2[] vector2s, ref float2[] float2s)
        {
            float2s = new float2[vector2s.Length];
            for (int i = 0; i < float2s.Length; i++)
                float2s[i] = vector2s[i];
        }


        private static GXColor[] GetColor(int length, GXColor color)
        {
            // TODO: set to something real
            var colors = new GXColor[length];
            for (int i = 0; i < length; i++)
                colors[i] = color;
            return colors;
        }


        // http://www.technologicalutopia.com/sourcecode/xnageometry/boundingsphere.cs.htm
        public static GameCube.GFZ.BoundingSphere CreateFromPoints(IEnumerable<Vector3> points)
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
