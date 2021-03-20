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
        public SurfaceAttributeArea[] surfaceAttributeAreas;
        public SurfaceAttributesMeshTable surfaceAttributesMeshTable;
        public UnknownObjectAttributes[] collisionObjectReferences;
        public UnknownObjectAttributes2[] unk_collisionObjectReferences;
        public SceneObject[] sceneObjects;
        public TrackLength trackLength;
        public TrackNode[] trackNodes;
        public List<TrackTransform> trackTransforms = new List<TrackTransform>();
        public int[] unknownData_0x20;
        public float unknownFloat_0x24;
        public UnknownTrigger2[] unknownTrigger2s;
        public ColiUnknownStruct3 unknownStruct3;
        public ColiUnknownStruct5 unknownStruct5_0x84;
        public UnknownTrigger1[] unknownTriggers_0x94;
        public VisualEffectTrigger[] effectTriggers;
        public CourseMetadataTrigger[] courseMetadataTriggers;
        public ArcadeCheckpointTrigger[] arcadeCheckpointTriggers;
        public StoryObjectTrigger[] storyObjectTriggers;
        public TrackIndexesTable trackIndexTable;

        // to find
        // * minimap rotation.............

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

            // 0x10 and 0x14: Track Effect Attribute Areas
            reader.JumpToAddress(header.surfaceAttributeAreasPtr);
            reader.ReadX(ref surfaceAttributeAreas, header.surfaceAttributeAreasPtr.length, true);

            // 0x1C 
            reader.JumpToAddress(header.surfaceAttributesMeshTablePtr);
            reader.ReadX(ref surfaceAttributesMeshTable, true);

            // 0x20
            reader.JumpToAddress(header.unkPtr_0x20);
            reader.ReadX(ref unknownData_0x20, 5);

            // 0x24
            reader.JumpToAddress(header.unkPtr_0x24);
            reader.ReadX(ref unknownFloat_0x24);

            // 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address): Scene Objects
            reader.JumpToAddress(header.sceneObjectsPtr);
            reader.ReadX(ref sceneObjects, header.sceneObjectCount, true);

            // 0x5C and 0x60 SOLS values
            reader.JumpToAddress(header.unkArrayPtr_0x5C);
            reader.ReadX(ref unknownTrigger2s, header.unkArrayPtr_0x5C.length, true);

            // 0x64 and 0x68
            reader.JumpToAddress(header.collisionObjectReferences);
            reader.ReadX(ref collisionObjectReferences, header.collisionObjectReferences.length, true);

            // 0x6C and 0x70
            // This one is weird. Pointers which lead to an array which reference collisionObjectReferences.
            // The count is different, so perhaps leads to certain properties on those objects.
            reader.JumpToAddress(header.unk_collisionObjectReferences);
            reader.ReadX(ref unk_collisionObjectReferences, header.unk_collisionObjectReferences.length, true);

            // 0x80
            if (header.unkPtr_0x80.IsNotNullPointer)
            {
                reader.JumpToAddress(header.unkPtr_0x80);
                reader.ReadX(ref unknownStruct3, true);
            }

            // 0x84
            reader.JumpToAddress(header.unkPtr_0x84);
            reader.ReadX(ref unknownStruct5_0x84, true);

            // 0x90 
            reader.JumpToAddress(header.trackLengthPtr);
            reader.ReadX(ref trackLength, true);

            // 0x94
            reader.JumpToAddress(header.unkArrayPtr_0x94);
            reader.ReadX(ref unknownTriggers_0x94, header.unkArrayPtr_0x94.length, true);

            // 0x9C
            reader.JumpToAddress(header.unkArrayPtr_0x9C);
            reader.ReadX(ref effectTriggers, header.unkArrayPtr_0x9C.length, true);

            // 0xA4
            reader.JumpToAddress(header.pathObjects);
            reader.ReadX(ref courseMetadataTriggers, header.pathObjects.length, true);

            // 0xAC
            reader.JumpToAddress(header.arcadeCheckpoint);
            reader.ReadX(ref arcadeCheckpointTriggers, header.arcadeCheckpoint.length, true);

            // 0xB4
            reader.JumpToAddress(header.storyObjects);
            reader.ReadX(ref storyObjectTriggers, header.storyObjects.length, true);

            // 0xBC
            reader.JumpToAddress(header.trackIndexTable);
            reader.ReadX(ref trackIndexTable, true);

            // Let's build the transform after-the-fact
            // We want to remove duplicate references to the same instance.
            var usedKeys = new List<int>();
            foreach (var node in trackNodes)
            {
                var transformAddress = node.transformPtr.address;
                if (!usedKeys.Contains(transformAddress))
                {
                    usedKeys.Add(transformAddress);

                    reader.JumpToAddress(node.transformPtr);
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
