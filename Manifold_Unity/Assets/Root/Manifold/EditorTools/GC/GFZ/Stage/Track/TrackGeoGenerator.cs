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

            var temp = new TempMesh()
            {
                vertexes = vertexes,
                normals = normals,
                indexes = indexes,
            };

            return temp;
        }

        public static void TestGmaExport()
        {
            var trackSegment = GameObject.FindObjectOfType<GfzTrackSegment>(false);
            var tempMesh = GenerateTopSurface(trackSegment, 10f, 4);

            // TODO easy way to pop in a model and get out this crap? -- note on materials, too.

            var gma = new Gma();
            gma.Models = new Model[]
            {
                new Model()
                {
                    Name = "my cool model name",
                    Gcmf = new Gcmf()
                    {
                        // attributes = default
                        TextureConfigs = new TextureConfig[0], // this is what you get from templating materials... + v
                        Submeshes = new Submesh[]
                        {
                            new Submesh()
                            {
                                Material = new GameCube.GFZ.GMA.Material(), // along with this... + ^
                                PrimaryDisplayListDescriptor = new DisplayListDescriptor(),
                                PrimaryDisplayListsOpaque = new GameCube.GX.DisplayList[]
                                {
                                    new GameCube.GX.DisplayList(GameCube.GX.GXAttributes.GX_VA_POS, GameCube.GFZ.GfzGX.VAT),
                                },
                            },
                        },
                        BoundingSphere = new GameCube.GFZ.BoundingSphere(),
                    },
                },
            };
        }

        [MenuItem("Manifold/HEY! TEST GMA")]
        public static void TestGmaExport2()
        {
            var trackSegment = GameObject.FindObjectOfType<GfzTrackSegment>(false);
            var tempMesh = GenerateTopSurface(trackSegment, 10f, 1);
            var dlists = TempMeshToDisplayList(tempMesh, (GXAttributes)0, GameCube.GFZ.GfzGX.VAT);
            // TODO easy way to pop in a model and get out this crap? -- note on materials, too.

            var submeshes = new Submesh[]
            {
                new Submesh()
                {
                    Material = new GameCube.GFZ.GMA.Material()
                    {
                        Unk0x02 = MatFlags0x02.unlit | MatFlags0x02.doubleSidedFaces,
                        MaterialColor = new GXColor(0xFFFFFFFF),
                        AmbientColor = new GXColor(0xFFFFFFFF),
                        SpecularColor = new GXColor(0x00000000),

                        // Hacky, find elegant solution - out param?
                        VertexAttributes = dlists[0].Attributes,
                    },

                    // THIS information is auto-calculated
                    //PrimaryDisplayListDescriptor = new DisplayListDescriptor(),

                    // Set DLs
                    PrimaryDisplayListsOpaque = dlists,
                },
            };

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
                        BoundingSphere = CreateFromPoints(tempMesh.vertexes),
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

        private static DisplayList[] TempMeshToDisplayList(TempMesh tempMesh, GXAttributes attributes, VertexAttributeTable vat)
        {
            var dlist = new DisplayList(attributes, vat);
            dlist.GxCommand = new DisplayCommand()
            {
                Primitive = Primitive.GX_TRIANGLESTRIP,
                VertexFormat = VertexFormat.GX_VTXFMT0,
            };

            dlist.clr0 = new GXColor[tempMesh.vertexes.Length];
            for (int i = 0; i < dlist.clr0.Length; i++)
                dlist.clr0[i] = new GXColor(255, 128, 255, 255);

            // This is actually bad when there are more than one width divisions - the strip
            // does "wrap" correctly, right?
            Copy(tempMesh.vertexes, ref dlist.pos);
            Copy(tempMesh.normals, ref dlist.nrm);

            //
            dlist.Attributes = dlist.ComponentsToGXAttributes();
            dlist.VertexCount = checked((ushort)tempMesh.vertexes.Length);

            return new DisplayList[] { dlist };
        }

        private static void Copy(Vector3[] vector3s, ref float3[] float3s)
        {
            float3s = new float3[vector3s.Length];
            for (int i = 0; i < float3s.Length; i++)
                float3s[i] = vector3s[i];
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
