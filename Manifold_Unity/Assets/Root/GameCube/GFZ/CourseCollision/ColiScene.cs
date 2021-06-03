using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ColiScene : IBinarySerializable, IFile
    {
        private const int unknownData_0x20_count = 0x14; // 20

        // metadata
        [UnityEngine.SerializeField]
        private int id;
        [UnityEngine.SerializeField]
        private string name;

        //
        public Header header;
        //
        public TrackNode[] trackNodes = new TrackNode[0];
        public List<TrackTransform> trackTransforms = new List<TrackTransform>();
        public SurfaceAttributeArea[] surfaceAttributeAreas = new SurfaceAttributeArea[0];
        public StaticMeshTable surfaceAttributeMeshTable = new StaticMeshTable();
        public byte[] unknownData_0x20 = new byte[unknownData_0x20_count];
        public float unknownFloat_0x24;
        public SceneObject[] sceneObjects = new SceneObject[0];
        public SceneInstanceReference[] sceneInstancesList = new SceneInstanceReference[0];
        public SceneOriginObjects[] sceneOriginObjectsList = new SceneOriginObjects[0];
        public UnknownSolsTrigger[] unknownSolsTriggers = new UnknownSolsTrigger[0];
        public UnknownStageData2 unknownStageData2 = new UnknownStageData2();
        public UnknownStageData1 unknownStageData1 = new UnknownStageData1();
        public TrackLength trackLength;
        public UnknownTrigger1[] unknownTrigger1s = new UnknownTrigger1[0]; // "green"
        public VisualEffectTrigger[] visualEffectTriggers = new VisualEffectTrigger[0];
        public CourseMetadataTrigger[] courseMetadataTriggers = new CourseMetadataTrigger[0];
        public ArcadeCheckpointTrigger[] arcadeCheckpointTriggers = new ArcadeCheckpointTrigger[0];
        public StoryObjectTrigger[] storyObjectTriggers = new StoryObjectTrigger[0];
        public TrackIndexTable trackIndexTable = new TrackIndexTable();

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
            DebugConsole.Log(FileName);

            // Store the stage index, can solve venue and course name from this
            var matchDigits = Regex.Match(FileName, Const.Regex.MatchIntegers);
            id = int.Parse(matchDigits.Value);

            // Read COLI_COURSE## file header
            reader.ReadX(ref header, true);

            // 0x08 and 0x0C: Track Nodes
            reader.JumpToAddress(header.trackNodesPtr);
            reader.ReadX(ref trackNodes, header.trackNodesPtr.Length, true);

            // 0x10 and 0x14: Track Effect Attribute Areas
            reader.JumpToAddress(header.surfaceAttributeAreasPtr);
            reader.ReadX(ref surfaceAttributeAreas, header.surfaceAttributeAreasPtr.Length, true);

            // 0x1C 
            reader.JumpToAddress(header.surfaceAttributeMeshTablePtr);
            reader.ReadX(ref surfaceAttributeMeshTable, true);

            // 0x20
            reader.JumpToAddress(header.unknownData_0x20_Ptr);
            reader.ReadX(ref unknownData_0x20, unknownData_0x20_count);

            // 0x24
            reader.JumpToAddress(header.unknownFloat_0x24_Ptr);
            reader.ReadX(ref unknownFloat_0x24);

            // 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address): Scene Objects
            reader.JumpToAddress(header.sceneObjectsPtr);
            reader.ReadX(ref sceneObjects, header.sceneObjectCount, true);

            // 0x5C and 0x60 SOLS values
            reader.JumpToAddress(header.unknownSolsTriggerPtrs);
            reader.ReadX(ref unknownSolsTriggers, header.unknownSolsTriggerPtrs.Length, true);

            // 0x64 and 0x68
            reader.JumpToAddress(header.sceneInstancesListPtrs);
            reader.ReadX(ref sceneInstancesList, header.sceneInstancesListPtrs.Length, true);

            // 0x6C and 0x70
            reader.JumpToAddress(header.sceneOriginObjectsListPtrs);
            reader.ReadX(ref sceneOriginObjectsList, header.sceneOriginObjectsListPtrs.Length, true);

            // 0x80
            if (header.unknownStageData2Ptr.IsNotNullPointer)
            {
                reader.JumpToAddress(header.unknownStageData2Ptr);
                reader.ReadX(ref unknownStageData2, true);
            }

            // 0x84
            reader.JumpToAddress(header.unknownStageData1Ptr);
            reader.ReadX(ref unknownStageData1, true);

            // 0x90 
            reader.JumpToAddress(header.trackLengthPtr);
            reader.ReadX(ref trackLength, true);

            // 0x94 and 0x98
            reader.JumpToAddress(header.unknownTrigger1sPtr);
            reader.ReadX(ref unknownTrigger1s, header.unknownTrigger1sPtr.Length, true);

            // 0x9C and 0xA0
            reader.JumpToAddress(header.visualEffectTriggersPtr);
            reader.ReadX(ref visualEffectTriggers, header.visualEffectTriggersPtr.Length, true);

            // 0xA4 and 0xA8
            reader.JumpToAddress(header.courseMetadataTriggersPtr);
            reader.ReadX(ref courseMetadataTriggers, header.courseMetadataTriggersPtr.Length, true);

            // 0xAC and 0xB0
            reader.JumpToAddress(header.arcadeCheckpointTriggersPtr);
            reader.ReadX(ref arcadeCheckpointTriggers, header.arcadeCheckpointTriggersPtr.Length, true);

            // 0xB4 and 0xB8
            reader.JumpToAddress(header.storyObjectTriggersPtr);
            reader.ReadX(ref storyObjectTriggers, header.storyObjectTriggersPtr.Length, true);

            // 0xBC and 0xC0
            reader.JumpToAddress(header.trackIndexTable);
            reader.ReadX(ref trackIndexTable, true);

            // Clear
            trackTransforms = new List<TrackTransform>();
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
            BinaryIoUtility.PushEndianess(false);
            DebugConsole.Log(FileName);

            // KINDA A HACK
            // Store all names for things in this table with ref to pointer,
            // serialize everything at the end.
            //ColiCourseUtility.stringTable = new List<ColiCourseUtility.StringTableEntry>();

            // Write empty header
            header = new Header();
            // Non pointer data (aside from SceneObject counts)
            header.unk_0x00 = new UnknownFloatPair();
            header.boostPadsActive = BoostPadsActive.Enabled;
            header.unkBool32_0x58 = Bool32.True;
            header.circuitType = CircuitType.ClosedCircuit;
            header.unknownStructure1_0xC0 = new ColiUnknownStruct1();


            writer.WriteX(header);

            // Write info
            writer.CommentDateAndCredits(ColiCourseUtility.SerializeVerbose);

            // TODO order serialization by pointer size based on real files, 
            //      maintain isFileAX/GX compatibility

            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x08, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x0C, ColiCourseUtility.SerializeVerbose);
            //header.trackNodesPtr = trackNodes.SerializeReferences(writer).GetArrayPointer();

            writer.CommentTypeDesc(surfaceAttributeAreas, 0x14, ColiCourseUtility.SerializeVerbose);
            header.surfaceAttributeAreasPtr = surfaceAttributeAreas.SerializeWithReferences(writer).GetArrayPointer();

            writer.CommentTypeDesc(surfaceAttributeMeshTable, 0x1C, ColiCourseUtility.SerializeVerbose);
            header.surfaceAttributeMeshTablePtr = surfaceAttributeMeshTable.SerializeWithReference(writer).GetPointer();

            writer.CommentTypeDesc(unknownData_0x20, 0x20, ColiCourseUtility.SerializeVerbose);
            // SHOULD NOT BE HARD-CODED?
            header.unknownData_0x20_Ptr = writer.GetPositionAsPointer();
            writer.WriteX(new byte[unknownData_0x20_count], false);

            writer.CommentTypeDesc(unknownFloat_0x24, 0x24, ColiCourseUtility.SerializeVerbose);
            // SHOULD NOT BE HARD CODED
            header.unknownFloat_0x24_Ptr = writer.GetPositionAsPointer();
            writer.WriteX(0f);

            //// scene objects
            //// 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address): Scene Objects;
            //writer.CommentTypeDesc(sceneObjects, 0x54, ColiCourseUtility.SerializeVerbose);
            //var sceneObjectsPtrs = sceneObjects.SerializeWithReferences(writer).GetArrayPointer();
            //header.sceneObjectCount = sceneObjectsPtrs.Length;
            //header.unk_sceneObjectCount1 = 0; // still don't know what this is for
            //header.unk_sceneObjectCount2 = 0; // still don't know what this is for
            //header.sceneObjectsPtr = writer.GetPositionAsPointer();

            // 0x5C and 0x60 SOLS values
            writer.CommentTypeDesc(unknownSolsTriggers, 0x60, ColiCourseUtility.SerializeVerbose);
            header.unknownSolsTriggerPtrs = unknownSolsTriggers.SerializeWithReferences(writer).GetArrayPointer();

            // 0x64 and 0x68
            writer.CommentTypeDesc(sceneInstancesList, 0x68, ColiCourseUtility.SerializeVerbose);
            header.sceneInstancesListPtrs = sceneInstancesList.SerializeWithReferences(writer).GetArrayPointer();


            // 0x6C and 0x70
            // This one is weird. Pointers which lead to an array which reference collisionObjectReferences.
            // The count is different, so perhaps leads to certain properties on those objects.
            writer.CommentTypeDesc(sceneOriginObjectsList, 0x70, ColiCourseUtility.SerializeVerbose);
            header.sceneOriginObjectsListPtrs = sceneOriginObjectsList.SerializeWithReferences(writer).GetArrayPointer();

            // 0x74, 0x78: unused in header

            // 0x80
            writer.CommentTypeDesc(unknownStageData2, 0x80, ColiCourseUtility.SerializeVerbose);
            header.unknownStageData2Ptr = unknownStageData2.SerializeWithReference(writer).GetPointer();

            // 0x84;
            writer.CommentTypeDesc(unknownStageData1, 0x84, ColiCourseUtility.SerializeVerbose);
            header.unknownStageData1Ptr = unknownStageData1.SerializeWithReference(writer).GetPointer();

            // 0x88, 0x8C: unused in header

            // 0x90 - Track Length
            writer.CommentTypeDesc(trackLength, 0x90, ColiCourseUtility.SerializeVerbose);
            header.trackLengthPtr = trackLength.SerializeWithReference(writer).GetPointer();

            // 0x94 and 0x98
            writer.CommentTypeDesc(unknownTrigger1s, 0x94, ColiCourseUtility.SerializeVerbose);
            header.unknownTrigger1sPtr = unknownTrigger1s.SerializeWithReferences(writer).GetArrayPointer();

            // 0x9C
            writer.CommentTypeDesc(visualEffectTriggers, 0x9C, ColiCourseUtility.SerializeVerbose);
            header.visualEffectTriggersPtr = visualEffectTriggers.SerializeWithReferences(writer).GetArrayPointer();

            // 0xA4 and 0xA8
            writer.CommentTypeDesc(courseMetadataTriggers, 0xA8, ColiCourseUtility.SerializeVerbose);
            header.courseMetadataTriggersPtr = courseMetadataTriggers.SerializeWithReferences(writer).GetArrayPointer();

            // 0xAC and 0xB0
            writer.CommentTypeDesc(arcadeCheckpointTriggers, 0xB0, ColiCourseUtility.SerializeVerbose);
            header.arcadeCheckpointTriggersPtr = arcadeCheckpointTriggers.SerializeWithReferences(writer).GetArrayPointer();

            // 0xB4 and 0xB8
            writer.CommentTypeDesc(storyObjectTriggers, 0xB8, ColiCourseUtility.SerializeVerbose);
            header.storyObjectTriggersPtr = storyObjectTriggers.SerializeWithReferences(writer).GetArrayPointer();

            // 0xBC
            writer.CommentTypeDesc(trackIndexTable, 0xBC, ColiCourseUtility.SerializeVerbose);
            header.trackIndexTable = trackIndexTable.SerializeWithReference(writer).GetPointer();

            //// Write string table
            //var objectEntries =
            //    from entry in ColiCourseUtility.stringTable
            //    where entry.type == ColiCourseUtility.StringTableEntry.Type.objectName
            //    select entry;
            //WriteEntries(writer, "Object Names", objectEntries);

            //var colliderEntries =
            //    from entry in ColiCourseUtility.stringTable
            //    where entry.type == ColiCourseUtility.StringTableEntry.Type.objectName
            //    select entry;
            //WriteEntries(writer, "Collider Names", colliderEntries);

            //var unkObjAttrs =
            //    from entry in ColiCourseUtility.stringTable
            //    where entry.type == ColiCourseUtility.StringTableEntry.Type.unknownObjectsAttributes
            //    select entry;
            //WriteEntries(writer, "Unk Obj Attrs", unkObjAttrs);

            // Overwrite header with pointers resolved
            writer.SeekStart();
            writer.WriteX(header);

            BinaryIoUtility.PopEndianess();
        }


        //public void WriteEntries(BinaryWriter writer, string name, IEnumerable<ColiCourseUtility.StringTableEntry> stringTableEntries)
        //{
        //    writer.CommentNewLine(ColiCourseUtility.SerializeVerbose, '-');
        //    writer.Comment(name, ColiCourseUtility.SerializeVerbose);
        //    writer.CommentCnt(stringTableEntries.Count(), ColiCourseUtility.SerializeVerbose);
        //    writer.CommentNewLine(ColiCourseUtility.SerializeVerbose, '-');

        //    foreach (var entry in stringTableEntries)
        //    {
        //        writer.SeekEnd(); // unnecesasry?
        //        var ptr = writer.GetPositionAsPointer();
        //        //writer.WriteXCString(entry.value, Encoding.ASCII);
        //        writer.JumpToAddress(entry.pointer);
        //        writer.WriteX(ptr);
        //        writer.SeekEnd();
        //    }
        //}

    }
}
