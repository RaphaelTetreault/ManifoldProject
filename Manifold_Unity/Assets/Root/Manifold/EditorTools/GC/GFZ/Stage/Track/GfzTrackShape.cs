//using GameCube.GFZ.Stage;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.Rendering;

//namespace Manifold.EditorTools.GC.GFZ.Stage.Track
//{
//    [RequireComponent(typeof(MeshFilter))]
//    [RequireComponent(typeof(MeshRenderer))]
//    public abstract class GfzTrackShape : MonoBehaviour
//    {
//        [field: Header("Segment Base")]
//        [field: SerializeField] public GfzTrackSegment Segment { get; private set; }


//        public AnimationCurveTRS AnimationCurveTRS => Segment.AnimationCurveTRS;



//        /// <summary>
//        /// Generates the entire mesh for this segment.
//        /// </summary>
//        /// <returns></returns>
//        public abstract Mesh[] GenerateMeshes();

//        /// <summary>
//        /// Generates the entire TrackSegment tree for this segment.
//        /// </summary>
//        /// <returns></returns>
//        public abstract TrackSegment GenerateTrackSegment();

//        /// <summary>
//        /// Acquires GfzTrackEmbededProperty from this GameObject instance.
//        /// </summary>
//        /// <returns></returns>
//        public virtual GfzTrackEmbeddedProperty GetEmbeddedProperties()
//        {
//            var embeddedProperties = GetComponents<GfzTrackEmbeddedProperty>();
//            IO.Assert.IsTrue(embeddedProperties.Length == 1);
//            return embeddedProperties[0];
//        }


//        protected virtual void OnValidate()
//        {
//            if (Segment == null)
//            {
//                Segment = GetComponent<GfzTrackSegment>();
//            }
//        }

//        public Mesh TristripsToMesh(Tristrip[] tristrips)
//        {
//            var mesh = new Mesh();
//            var submeshes = SubmeshesFromTristrips(mesh, tristrips);
//            mesh.SetSubMeshes(submeshes);
//            return mesh;
//        }

//        public static SubMeshDescriptor[] SubmeshesFromTristrips(Mesh mesh, Tristrip[] tristrips)
//        {
//            var submeshes = new SubMeshDescriptor[tristrips.Length];
//            for (int i = 0; i < submeshes.Length; i++)
//            {
//                var submesh = new SubMeshDescriptor();

//                var tristrip = tristrips[i];
//                var vertices = tristrip.positions;
//                var normals = tristrip.normals;
//                var triangles = tristrip.GetIndices(); // not offset

//                // Build submesh
//                submesh.baseVertex = mesh.vertexCount;
//                submesh.firstVertex = mesh.vertexCount;
//                submesh.indexCount = triangles.Length;
//                submesh.indexStart = mesh.triangles.Length;
//                submesh.topology = MeshTopology.Triangles;
//                submesh.vertexCount = vertices.Length;

//                // Append to mesh
//                var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
//                var normalsConcat = mesh.normals.Concat(normals).ToArray();
//                //var uv1Concat = mesh.uv.Concat(uv1).ToArray();
//                //var uv2Concat = mesh.uv2.Concat(uv2).ToArray();
//                //var uv3Concat = mesh.uv3.Concat(uv3).ToArray();
//                var colorsConcat = mesh.colors32.Concat(tristrip.color0).ToArray();
//                //if (list.nbt != null)
//                //    mesh.tangents = list.nbt;
//                var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

//                // Assign values to mesh
//                mesh.vertices = verticesConcat;
//                mesh.normals = normalsConcat;
//                //mesh.uv = uv1Concat;
//                //mesh.uv2 = uv2Concat;
//                //mesh.uv3 = uv3Concat;
//                mesh.colors32 = colorsConcat;
//                mesh.triangles = trianglesConcat;

//                submeshes[i] = submesh;
//            }
//            return submeshes;
//        }


//    }
//}
