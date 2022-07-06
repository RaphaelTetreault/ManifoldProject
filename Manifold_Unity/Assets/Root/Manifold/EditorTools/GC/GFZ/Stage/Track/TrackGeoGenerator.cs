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
        public Tristrip[] TrackTop { get; private set; }
        public Tristrip[] TrackBottom { get; private set; }
        public Tristrip[] TrackLeft { get; private set; }
        public Tristrip[] TrackRight { get; private set; }
        public Tristrip[] RailLeft { get; private set; }
        public Tristrip[] RailRight { get; private set; }

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

    public class TrackMeshComponent
    {
        public Vector3[] vertexes { get; set; }
        public Vector3[] normals { get; set; }
        public Vector3[] binormals { get; set; }
        public Vector3[] tangents { get; set; }
        public Vector2[] uv0 { get; set; }
        public Vector2[] uv1 { get; set; }
        public Vector2[] uv2 { get; set; }
        public Color[] color0 { get; set; }
        public Color[] color1 { get; set; }
        public int[][] tristrips { get; set; }
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
    }

    public struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
    }

    public static class TrackGeoGenerator
    {
        private static readonly Vector3 left = new Vector3(-1, 0, 0);
        private static readonly Vector3 right = new Vector3(1, 0, 0);

        public static TrackMesh GenerateTopSurface(GfzTrackSegment trackSegment, float minDistance, int widthSamples = 4)
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

            throw new System.NotImplementedException();

            //var temp = new TrackMesh()
            //{
            //    vertexes = vertexes,
            //    normals = normals,
            //    indexes = indexes,
            //};

            //return temp;
        }

        public static Tristrip GenerateTristrip(GfzTrackSegment trackSegment, Vertex a, Vertex b, float minDistance, bool doScaleX)
        {
            var mtx = trackSegment.transform.localToWorldMatrix;
            //var maxTime = trackSegment.AnimationCurveTRS.GetMaxTime();
            var segmentLength = trackSegment.AnimationCurveTRS.GetDistanceBetweenRepeated(0, 1);
            float step = segmentLength / minDistance;
            int lengthSamples = (int)math.ceil(step);

            var nverts = (lengthSamples + 1) * 2;
            var tristrip = new Tristrip();
            tristrip.positions = new Vector3[nverts];
            tristrip.normals = new Vector3[nverts];

            for (int l = 0; l <= lengthSamples; l++)
            {
                double sampleTime = l / (double)lengthSamples;
                var animMtx = trackSegment.AnimationCurveTRS.EvaluateMatrix(sampleTime);
                var sampleMtx = mtx * animMtx;

                var position = sampleMtx.Position();
                var rotation = sampleMtx.Rotation();
                var scale = sampleMtx.Scale();
                var width = doScaleX ? scale.x : 1f;

                int index0 = l * 2;
                int index1 = index0 + 1;
                var p0 = (rotation * (a.position * width)) + position;
                var p1 = (rotation * (b.position * width)) + position;
                tristrip.positions[index0] = p0;
                tristrip.positions[index1] = p1;
                tristrip.normals[index0] = rotation * a.normal;
                tristrip.normals[index1] = rotation * b.normal;
            }

            return tristrip;
        }

        public static Tristrip[] GenerateTristrips(GfzTrackSegment trackSegment, float minDistance, Vertex[] v)
        {
            var mtx = trackSegment.transform.localToWorldMatrix;
            //var maxTime = trackSegment.AnimationCurveTRS.GetMaxTime();
            var segmentLength = trackSegment.AnimationCurveTRS.GetDistanceBetweenRepeated(0, 1);
            float step = segmentLength / minDistance;
            int lengthSamples = (int)math.ceil(step);

            var nverts = (lengthSamples + 1) * v.Length;
            Tristrip[] tristrips = new Tristrip[v.Length];
            foreach (var tristrip in tristrips)
            {
                tristrip.positions = new Vector3[nverts];
                tristrip.normals = new Vector3[nverts];
            }

            for (int z = 0; z <= lengthSamples; z++)
            {
                double samplePoint = z / (double)lengthSamples;
                var animMtx = trackSegment.AnimationCurveTRS.EvaluateMatrix(samplePoint);
                var sampleMtx = mtx * animMtx;

                var position = sampleMtx.Position();
                var rotation = sampleMtx.Rotation();
                var scale = sampleMtx.Scale();

                var width = scale.x;
                var halfWidth = width / 2;
                var beginLeft = position + rotation * (left * halfWidth);
                var directionRight = rotation * right;

                for (int x = 0; x < tristrips.Length; x++)
                {
                    var percent = x / (float)tristrips.Length;
                    var offsetRight = directionRight * percent * width;
                    var vertexPos = beginLeft + offsetRight;

                    int index0 = z * 2;
                    int index1 = index0 + 1;
                    tristrips[x + 0].positions[index0] = vertexPos;
                    tristrips[x + 1].positions[index1] = vertexPos;
                    throw new System.NotImplementedException();
                }

            }
            return tristrips;
        }



        [MenuItem("Manifold/HEY! TEST GMA")]
        public static void TestGmaExport2()
        {
            // TODO: get GfzTrack, use it to get children
            var track = GameObject.FindObjectOfType<GfzTrack>(false);
            track.FindChildSegments();

            // TODO: iterate over all segments, not just the first
            var trackSegment = track.StartSegmentShape.Segment;
            //var tristrips = GenerateTristrips(trackSegment, 10f, null);
            var trackMesh = new TrackMesh();
            var dlists = TrackMeshComponentToDisplayLists(trackMesh.TrackTop, (AttributeFlags)0, GameCube.GFZ.GfzGX.VAT);
            // TODO easy way to pop in a model and get out this bleep? -- note on materials, too.

            var submeshes = new Submesh[]
            {
                new Submesh()
                {
                    RenderFlags = RenderFlags.unlit | RenderFlags.doubleSidedFaces,
                    VertexAttributes = dlists[0].Attributes, // hacky

                    Material = new GameCube.GFZ.GMA.Material()
                    {
                        MaterialColor = new GXColor(0xFFFFFFFF),
                        AmbientColor = new GXColor(0xFFFFFFFF),
                        SpecularColor = new GXColor(0x00000000),
                        // ??? THIS IS OPTIONAL?
                        DisplayListFlags = 0,
                    },

                    // Set DLs
                    PrimaryDisplayListsOpaque = dlists,
                },
            };

            // Compute bounding sphere
            var allVertices = new List<Vector3>();
            foreach (var component in trackMesh.GetAllTristrips())
                allVertices.AddRange(component.positions);
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
                        TextureCount = 0,
                        OpaqueMaterialCount = (ushort)submeshes.Length,
                        TranslucidMaterialCount = 0,
                        BoneCount = 0,
                        //
                        TextureConfigs = new TextureConfig[0], // this is what you get from templating materials...
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

        private static DisplayList[] TrackMeshComponentToDisplayLists(Tristrip[] tristrips, AttributeFlags attributes, VertexAttributeTable vat)
        {
            var displayLists = new DisplayList[tristrips.Length];
            for (int i = 0; i < displayLists.Length; i++)
            {
                var tristrip = tristrips[i];

                // Initialize display list
                var dlist = new DisplayList(attributes, vat);
                dlist.GxCommand = new DisplayCommand()
                {
                    Primitive = Primitive.GX_TRIANGLESTRIP,
                    VertexFormat = VertexFormat.GX_VTXFMT0,
                };

                // TODO: set to something real
                dlist.clr0 = GetColor(tristrip.positions.Length, new GXColor(128, 128, 128, 255));

                // This is actually bad when there are more than one width divisions - the strip
                // does "wrap" correctly, right?
                Copy(tristrip.positions, ref dlist.pos);
                Copy(tristrip.normals, ref dlist.nrm);

                // Set attributes based on provided data
                dlist.Attributes = dlist.ComponentsToGXAttributes();
                dlist.VertexCount = checked((ushort)tristrip.positions.Length);
            }
            return displayLists;
        }

        private static void Copy(Vector3[] vector3s, ref float3[] float3s)
        {
            float3s = new float3[vector3s.Length];
            for (int i = 0; i < float3s.Length; i++)
                float3s[i] = vector3s[i];
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
