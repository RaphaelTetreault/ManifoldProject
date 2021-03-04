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
        [SerializeField]
        string name;

        // Generate some metadata to be used by some processes
        [SerializeField]
        public int id;

        //
        public Header header;
        public TrackNode[] trackNodes;
        public AICollisionPropertyTarget[] collisionPropertyAreas;
        public CollisionMeshTable collisionMeshTable;
        public TrackLength trackInformation;
        public GameObject[] gameObjects;
        public List<TrackTransform> trackTransforms = new List<TrackTransform>();

        public string FileName
        {
            get => name;
            set => name = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            BinaryIoUtility.PushEndianess(false);

            Debug.Log(FileName);

            // Store the stage index, can solve venue and course name from this
            id = int.Parse(System.Text.RegularExpressions.Regex.Match(FileName, @"\d+").Value);

            // Read COLI_COURSE## file header
            reader.ReadX(ref header, true);

            // 0x08 and 0x0C: Track Nodes
            reader.JumpToAddress(header.trackNodes);
            reader.ReadX(ref trackNodes, header.trackNodes.length, true);

            // 0x10 and 0x14: Collision Effect Area
            reader.JumpToAddress(header.collisionEffectsAreas);
            reader.ReadX(ref collisionPropertyAreas, header.collisionEffectsAreas.length, true);

            // 0x1C 
            reader.JumpToAddress(header.unkPtr_0x1C);
            reader.ReadX(ref collisionMeshTable, true);

            // 0x48 and 0x??: Game Objects
            reader.JumpToAddress(header.gameObjectPtr);
            reader.ReadX(ref gameObjects, header.gameObjectCount, true);

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
