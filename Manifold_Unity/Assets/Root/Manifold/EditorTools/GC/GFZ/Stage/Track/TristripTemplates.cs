//using GameCube.GX;
//using GameCube.GFZ.GMA;
//using GameCube.GFZ.LZ;
//using GameCube.GFZ.Stage;
//using Manifold;
//using Manifold.IO;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;
//using Unity.Mathematics;

//namespace Manifold.EditorTools.GC.GFZ.Stage.Track
//{
//    public class TristripTemplates
//    {
//        public static Tristrip[] CreateTempTrackRoad(GfzTrackSegmentNode node, int nTristrips, float maxStep, bool useGfzCoordSpace)
//        {
//            var allTriStrips = new List<Tristrip>();
//            var hacTRS = node.CreateHierarchichalAnimationCurveTRS(useGfzCoordSpace);
//            var segmentLength = hacTRS.GetSegmentLength();
//            var matrices = GenerateMatrixIntervals(hacTRS, maxStep);

//            // track top
//            {
//                var endpointA = new Vector3(-0.5f, 0, 0);
//                var endpointB = new Vector3(+0.5f, 0, 0);
//                //var color0 = new Color32(128, 128, 128, 255);
//                var color0 = new Color32(112, 112, 112, 255);
//                var normal = Vector3.up;
//                var trackTopTristrips = CreateTristrips(matrices, endpointA, endpointB, nTristrips, color0, normal, 3, true);
//                allTriStrips.AddRange(trackTopTristrips);
//            }

//            // track bottom
//            {
//                var endpointA = new Vector3(-0.5f, -2.0f, 0);
//                var endpointB = new Vector3(+0.5f, -2.0f, 0);
//                var color0 = new Color32(48, 48, 48, 255);
//                var normal = Vector3.down;
//                var trackBottomTristrips = CreateTristrips(matrices, endpointA, endpointB, nTristrips, color0, normal, 3, false);
//                allTriStrips.AddRange(trackBottomTristrips);
//            }

//            // track left
//            {
//                var endpointA = new Vector3(-0.5f, +0.0f, 0);
//                var endpointB = new Vector3(-0.5f, -2.0f, 0);
//                //var color0 = new Color32(127, 255, 127, 255); // green
//                var color0 = new Color32(112, 112, 112, 255);
//                var normal = Vector3.left;
//                var trackLeftTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, normal, 0, false);
//                allTriStrips.AddRange(trackLeftTristrips);
//            }

//            // track right
//            {
//                var endpointA = new Vector3(+0.5f, +0.0f, 0);
//                var endpointB = new Vector3(+0.5f, -2.0f, 0);
//                //var color0 = new Color32(255, 127, 127, 255); // red
//                var color0 = new Color32(112, 112, 112, 255);
//                var normal = Vector3.right;
//                var trackRightTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, normal, 0, true);
//                allTriStrips.AddRange(trackRightTristrips);
//            }

//            var isTypeofRail = node is IRailSegment;
//            if (isTypeofRail)
//            {
//                var rails = node as IRailSegment;

//                // rail left
//                if (rails.RailHeightLeft > 0f)
//                {
//                    var endpointA = new Vector3(-0.5f, +0.0f, 0);
//                    var endpointB = new Vector3(-0.5f, rails.RailHeightLeft, 0);
//                    //var color0 = new Color32(0, 255, 0, 255); // green
//                    var color0 = new Color32(255, 255, 255, 255);
//                    var trackRightTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, null, 0, false);
//                    allTriStrips.AddRange(trackRightTristrips);
//                }

//                // rail right
//                if (rails.RailHeightRight > 0f)
//                {
//                    var endpointA = new Vector3(+0.5f, +0.0f, 0);
//                    var endpointB = new Vector3(+0.5f, rails.RailHeightRight, 0);
//                    //var color0 = new Color32(255, 0, 0, 255); // red
//                    var color0 = new Color32(255, 255, 255, 255);
//                    var trackRightTristrips = CreateTristrips(matrices, endpointA, endpointB, 1, color0, null, 0, true);
//                    allTriStrips.AddRange(trackRightTristrips);
//                }
//            }

//            //
//            var intervals = (int)math.ceil(segmentLength / 100.0); // per ~100 meters
//            var intervalsInverse = 1.0 / intervals;
//            // width dividers
//            for (int i = 0; i < intervals; i++)
//            {
//                var time = (i * intervalsInverse) * segmentLength;
//                var matrix0 = hacTRS.EvaluateAnimationMatrices(time);
//                var matrix1 = hacTRS.EvaluateAnimationMatrices(time + 3f);// 3 units forward
//                var matrices01 = new Matrix4x4[] { matrix0, matrix1 };

//                var endpointA = new Vector3(-0.5f, +0.10f, 0);
//                var endpointB = new Vector3(+0.5f, +0.10f, 0);
//                var color0 = new Color32(96, 96, 96, 255); // dark grey
//                var normal = Vector3.up;
//                var widthDividers = CreateTristrips(matrices01, endpointA, endpointB, nTristrips, color0, normal, 0, true);
//                allTriStrips.AddRange(widthDividers);
//            }

//            // REMOVE SCALE.X
//            var matricesNoScale = GetMatricesDefaultScale(matrices, Vector3.one);
//            // center line
//            {
//                var endpointA = new Vector3(-0.5f, +0.15f, 0);
//                var endpointB = new Vector3(+0.5f, +0.15f, 0);
//                //var color0 = new Color32(127, 255, 255, 255); // cyan
//                var color0 = new Color32(255, 255, 255, 255); // cyan
//                var normal = Vector3.up;
//                var trackLaneDivider = CreateTristrips(matricesNoScale, endpointA, endpointB, 1, color0, normal, 0, true);
//                allTriStrips.AddRange(trackLaneDivider);
//            }

//            return allTriStrips.ToArray();
//        }

//    }
//}
