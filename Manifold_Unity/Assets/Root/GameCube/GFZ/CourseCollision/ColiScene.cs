using Manifold.IO;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ColiScene : IBinarySerializable, IFile
    {
        // metadata
        [SerializeField]
        private int id;
        [SerializeField]
        private string name;

        //
        public Header header;
        //
        public AICollisionPropertyTarget[] collisionPropertyAreas;
        public CollisionMeshTable collisionMeshTable;
        public SceneObject[] sceneObjects;
        public TrackLength trackInformation;
        public TrackNode[] trackNodes;
        public List<TrackTransform> trackTransforms = new List<TrackTransform>();
        public int[] unknownData_0x20;
        public float unknownFloat_0x24;
        public ColiUnknownStruct2[] unknownStruct2s;


        public string FileName
        {
            get => name;
            set => name = value;
        }

        public int ID => id;


        public void Deserialize(BinaryReader reader)
        {
            BinaryIoUtility.PushEndianess(false);

            Debug.Log(FileName);

            // Store the stage index, can solve venue and course name from this
            var matchDigits = System.Text.RegularExpressions.Regex.Match(FileName, @"\d+");
            id = int.Parse(matchDigits.Value);

            // Read COLI_COURSE## file header
            reader.ReadX(ref header, true);

            // 0x08 and 0x0C: Track Nodes
            reader.JumpToAddress(header.trackNodesPtr);
            reader.ReadX(ref trackNodes, header.trackNodesPtr.length, true);

            // 0x10 and 0x14: Collision Effect Area
            reader.JumpToAddress(header.collisionEffectsAreasPtr);
            reader.ReadX(ref collisionPropertyAreas, header.collisionEffectsAreasPtr.length, true);

            // 0x1C 
            reader.JumpToAddress(header.collisionMeshTablePtr);
            reader.ReadX(ref collisionMeshTable, true);

            // 0x20
            reader.JumpToAddress(header.unkPtr_0x20);
            reader.ReadX(ref unknownData_0x20, 5);

            // 0x24
            reader.JumpToAddress(header.unkPtr_0x24);
            reader.ReadX(ref unknownFloat_0x24);
            //Debug.Log($"{FileName}:{unknownFloat_0x24}");

            // 0x48 and 0x??: Game Objects
            reader.JumpToAddress(header.gameObjectPtr);
            reader.ReadX(ref sceneObjects, header.gameObjectCount, true);

            // 0x5c SOLS values
            reader.JumpToAddress(header.unkArrayPtr_0x5C);
            reader.ReadX(ref unknownStruct2s, header.unkArrayPtr_0x5C.length, true);

            // 0x90 - Track Transforms
            reader.JumpToAddress(header.trackInfo);
            reader.ReadX(ref trackInformation, true);

            // Let's build the transform after-the-fact
            // We want to remove duplicate references to the same instance.
            var usedKeys = new List<int>();
            foreach (var node in trackNodes)
            {
                var transformAbsPtr = node.trackTransformAbsPtr;
                if (!usedKeys.Contains(transformAbsPtr))
                {
                    usedKeys.Add(transformAbsPtr);

                    reader.BaseStream.Seek(transformAbsPtr, SeekOrigin.Begin);
                    var value = new TrackTransform();
                    value.Deserialize(reader);
                    trackTransforms.Add(value);
                }
            }

            BinaryIoUtility.PopEndianess();
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
