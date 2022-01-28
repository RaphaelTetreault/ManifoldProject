//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using GameCube.GFZ.CourseCollision;

//namespace Manifold.EditorTools.GC.GFZ.CourseCollision
//{
//    public class DisplayTrackTransform : MonoBehaviour, IColiCourseDisplayable
//    {
//        [SerializeField]
//        private ColiSceneSobj sceneSobj;
//        public float size = 1f;

//        public Color[] gizmosColor = new Color[]
//        {
//            new Color32 (255,   0,   0, 128),
//            new Color32 (255, 255,   0, 128),
//            new Color32 (  0, 255,   0, 128),
//            new Color32 (  0, 255, 255, 128),
//            new Color32 (  0,   0, 255, 128),
//            new Color32 (255,   0, 255, 128),
//            new Color32 (  0,   0,   0, 128),
//        };


//        public ColiSceneSobj SceneSobj
//        {
//            get => sceneSobj;
//            set => sceneSobj = value;
//        }



//        private void OnDrawGizmos()
//        {
//            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

//            foreach (var trackTransform in sceneSobj.Value.rootTrackSegments)
//            {
//                Draw(trackTransform, mesh, 0);
//            }
//        }

//        public void Draw(TrackSegment trackTransform, Mesh mesh, int depth)
//        {
//            // Draw self
//            var worldMatrix = trackTransform.worldMatrix;
//            var position = worldMatrix.Position();
//            var rotation = worldMatrix.RotationBad();
//            var scale = worldMatrix.Scale();
//            //
//            var colorIndex = trackTransform.depth;
//            Gizmos.color = gizmosColor[colorIndex];
//            Gizmos.DrawMesh(mesh, 0, position, rotation, Vector3.one * size);
//            Gizmos.DrawWireMesh(mesh, 0, position, rotation, scale);

//            //
//            foreach (var child in trackTransform.children)
//            {
//                Draw(child, mesh, depth + 1);
//            }
//        }
//    }
//}