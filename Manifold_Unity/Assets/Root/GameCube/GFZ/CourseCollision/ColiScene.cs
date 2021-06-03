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

            // Write empty header
            header = new Header();
            writer.WriteX(header);

            // MAINTAIN FILE IDENTIFICATION COMPATIBILITY
            {
                // Resulting pointer should be 0xE4 or 0xE8 for AX or GX, respectively.
                header.unknownData_0x20_Ptr = writer.GetPositionAsPointer();
                writer.WriteX(new byte[unknownData_0x20_count], false); // TODO: HARD-CODED
                
                // Resulting pointer should be 0xF8 or 0xFC for AX or GX, respectively.
                header.unknownFloat_0x24_Ptr = writer.GetPositionAsPointer();
                writer.WriteX(0f); // TODO: HARD-CODED
                
                // The pointers written by the last 2 calls should create a valid AX or GX file header.
                Assert.IsTrue(header.IsValidFile);
            }
            // Offset pointer address if AX file
            int offset = header.IsFileAX ? -4 : 0;

            // Write info
            writer.CommentDateAndCredits(ColiCourseUtility.SerializeVerbose);

            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x08, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x0C, ColiCourseUtility.SerializeVerbose);
            //header.trackNodesPtr = trackNodes.SerializeReferences(writer).GetArrayPointer();

            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x14 + offset, surfaceAttributeAreas);
            writer.WriteX(surfaceAttributeAreas, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x1C + offset, surfaceAttributeMeshTable);
            writer.WriteX(surfaceAttributeMeshTable);

            //// scene objects
            //// 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address): Scene Objects;
            //writer.CommentTypeDesc(sceneObjects, 0x54, ColiCourseUtility.SerializeVerbose);
            //var sceneObjectsPtrs = sceneObjects.SerializeWithReferences(writer).GetArrayPointer();
            //header.sceneObjectCount = sceneObjectsPtrs.Length;
            //header.unk_sceneObjectCount1 = 0; // still don't know what this is for
            //header.unk_sceneObjectCount2 = 0; // still don't know what this is for
            //header.sceneObjectsPtr = writer.GetPositionAsPointer();

            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x60 + offset, unknownSolsTriggers);
            writer.WriteX(unknownSolsTriggers, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x68 + offset, sceneInstancesList);
            writer.WriteX(sceneInstancesList, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x70 + offset, sceneOriginObjectsList);
            writer.WriteX(sceneOriginObjectsList, false);
            // 0x74, 0x78: unused in header
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x80 + offset, unknownStageData2);
            header.unknownStageData2Ptr = unknownStageData2.SerializeWithReference(writer).GetPointer();
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x84 + offset, unknownStageData1);
            header.unknownStageData1Ptr = unknownStageData1.SerializeWithReference(writer).GetPointer();
            // 0x88, 0x8C: unused in header
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x90 + offset, trackLength);
            writer.WriteX(trackLength);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x94 + offset, unknownTrigger1s);
            writer.WriteX(unknownTrigger1s, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x9C + offset, visualEffectTriggers);
            writer.WriteX(visualEffectTriggers, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0xA8 + offset, courseMetadataTriggers);
            writer.WriteX(courseMetadataTriggers, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0xB0 + offset, arcadeCheckpointTriggers);
            writer.WriteX(arcadeCheckpointTriggers, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0xB8 + offset, storyObjectTriggers);
            writer.WriteX(storyObjectTriggers, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0xBC + offset, trackIndexTable);
            writer.WriteX(trackIndexTable);

            // Get pointers from serialized values
            header.surfaceAttributeAreasPtr = surfaceAttributeAreas.GetArrayPointer();
            header.surfaceAttributeMeshTablePtr = surfaceAttributeMeshTable.GetPointer();
            header.unknownSolsTriggerPtrs = unknownSolsTriggers.GetArrayPointer();
            header.sceneInstancesListPtrs = sceneInstancesList.GetArrayPointer();
            header.sceneOriginObjectsListPtrs = sceneOriginObjectsList.GetArrayPointer();
            header.trackLengthPtr = trackLength.GetPointer();
            header.unknownTrigger1sPtr = unknownTrigger1s.GetArrayPointer();
            header.visualEffectTriggersPtr = visualEffectTriggers.GetArrayPointer();
            header.courseMetadataTriggersPtr = courseMetadataTriggers.GetArrayPointer();
            header.arcadeCheckpointTriggersPtr = arcadeCheckpointTriggers.GetArrayPointer();
            header.storyObjectTriggersPtr = storyObjectTriggers.GetArrayPointer();
            header.trackIndexTable = trackIndexTable.GetPointer();
            // Non pointer data (aside from SceneObject counts)
            header.unk_0x00 = new UnknownFloatPair();
            header.boostPadsActive = BoostPadsActive.Enabled;
            header.unkBool32_0x58 = Bool32.True;
            header.circuitType = CircuitType.ClosedCircuit;
            header.unknownStructure1_0xC0 = new ColiUnknownStruct1();

            // Overwrite header with pointers resolved
            writer.SeekStart();
            writer.WriteX(header);

            BinaryIoUtility.PopEndianess();
        }

    }
}
