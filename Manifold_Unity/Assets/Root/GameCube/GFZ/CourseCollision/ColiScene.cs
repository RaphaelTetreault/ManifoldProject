using Manifold.IO;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ColiScene : IBinarySerializable, IFile
    {
        private const int unknownData_0x20_count = 5;

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
        public int[] unknownData_0x20 = new int[unknownData_0x20_count];
        public float unknownFloat_0x24;
        public SceneObject[] sceneObjects = new SceneObject[0];
        public UnknownObjectAttributes[] collisionObjectReferences = new UnknownObjectAttributes[0];
        public UnknownObjectAttributes2[] unk_collisionObjectReferences = new UnknownObjectAttributes2[0];
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
            reader.ReadX(ref unknownData_0x20, 5);

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
            reader.JumpToAddress(header.collisionObjectReferencePtrs);
            reader.ReadX(ref collisionObjectReferences, header.collisionObjectReferencePtrs.Length, true);

            // 0x6C and 0x70
            // This one is weird. Pointers which lead to an array which reference collisionObjectReferences.
            // The count is different, so perhaps leads to certain properties on those objects.
            reader.JumpToAddress(header.unk_collisionObjectReferencePtrs);
            reader.ReadX(ref unk_collisionObjectReferences, header.unk_collisionObjectReferencePtrs.Length, true);

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

            // 0x94
            reader.JumpToAddress(header.unknownTrigger1sPtr);
            reader.ReadX(ref unknownTrigger1s, header.unknownTrigger1sPtr.Length, true);

            // 0x9C
            reader.JumpToAddress(header.visualEffectTriggersPtr);
            reader.ReadX(ref visualEffectTriggers, header.visualEffectTriggersPtr.Length, true);

            // 0xA4
            reader.JumpToAddress(header.courseMetadataTriggersPtr);
            reader.ReadX(ref courseMetadataTriggers, header.courseMetadataTriggersPtr.Length, true);

            // 0xAC
            reader.JumpToAddress(header.arcadeCheckpointTriggersPtr);
            reader.ReadX(ref arcadeCheckpointTriggers, header.arcadeCheckpointTriggersPtr.Length, true);

            // 0xB4
            reader.JumpToAddress(header.storyObjectTriggersPtr);
            reader.ReadX(ref storyObjectTriggers, header.storyObjectTriggersPtr.Length, true);

            // 0xBC
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

            // Read COLI_COURSE## file header

            // Write empty header
            header = new Header();
            writer.WriteX(header);

            // TODO order serialization by pointer size based on real files, 
            //      maintain isFileAX/GX compatibility

            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x08, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x0C, ColiCourseUtility.SerializeVerbose);
            //header.trackNodesPtr = trackNodes.SerializeReferences(writer).GetArrayPointer();

            writer.CommentTypeDesc(surfaceAttributeAreas, new Pointer(0x14), ColiCourseUtility.SerializeVerbose);
            header.surfaceAttributeAreasPtr = surfaceAttributeAreas.SerializeWithReferences(writer).GetArrayPointer();

            writer.CommentTypeDesc(surfaceAttributeMeshTable, new Pointer(0x1C), ColiCourseUtility.SerializeVerbose);
            header.surfaceAttributeMeshTablePtr = surfaceAttributeMeshTable.SerializeWithReference(writer).GetPointer();


            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x20, ColiCourseUtility.SerializeVerbose);
            //// SHOULD NOT BE HARD CODED
            //writer.Comment(nameof(unknownData_0x20), ColiCourseUtility.SerializeVerbose);
            //header.unknownData_0x20_Ptr = new Pointer() { address = (int)writer.BaseStream.Position };
            //writer.WriteX(new byte[5 * 4], false);


            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x24, ColiCourseUtility.SerializeVerbose);
            //// SHOULD NOT BE HARD CODED
            //writer.Comment(nameof(unknownFloat_0x24), ColiCourseUtility.SerializeVerbose);
            //header.unknownFloat_0x24_Ptr = new Pointer() { address = (int)writer.BaseStream.Position };
            //writer.WriteX(0f);


            ////// 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address): Scene Objects
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x48, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x4C, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x50, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x54, ColiCourseUtility.SerializeVerbose);
            //var sceneObjectsPtrs = sceneObjects.SerializeReferences(writer).GetArrayPointer();
            //header.sceneObjectCount = sceneObjectsPtrs.Length;
            //header.unk_sceneObjectCount1 = 0; // still don't know what this is for
            //header.unk_sceneObjectCount2 = 0; // still don't know what this is for
            //header.sceneObjectsPtr = new Pointer() { address = sceneObjectsPtrs.Address };

            ////// 0x5C and 0x60 SOLS values
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x5C, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x60, ColiCourseUtility.SerializeVerbose);
            //header.unknownSolsTriggerPtrs = unknownSolsTriggers.SerializeReferences(writer).GetArrayPointer();

            ////// 0x64 and 0x68
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x64, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x68, ColiCourseUtility.SerializeVerbose);
            //header.collisionObjectReferencePtrs = collisionObjectReferences.SerializeReferences(writer).GetArrayPointer();

            ////// 0x6C and 0x70
            ////// This one is weird. Pointers which lead to an array which reference collisionObjectReferences.
            ////// The count is different, so perhaps leads to certain properties on those objects.
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x6C, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x70, ColiCourseUtility.SerializeVerbose);
            //header.unk_collisionObjectReferencePtrs = unk_collisionObjectReferences.SerializeReferences(writer).GetArrayPointer();

            //// 0x74, 0x78: unused in header

            ////// 0x80
            //// can be null, write empty?
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x80, ColiCourseUtility.SerializeVerbose);
            //header.unknownStageData2Ptr = unknownStageData2.SerializeReference(writer).GetPointer();

            ////// 0x84
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x84, ColiCourseUtility.SerializeVerbose);
            //header.unknownStageData1Ptr = unknownStageData1.SerializeReference(writer).GetPointer();

            //// 0x88, 0x8C: unused in header

            //// 0x90 - Track Length
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x90, ColiCourseUtility.SerializeVerbose);
            //header.trackLengthPtr = trackLength.SerializeReference(writer).GetPointer();

            ////// 0x94
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x94, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x98, ColiCourseUtility.SerializeVerbose);
            //header.unknownTrigger1sPtr = unknownTrigger1s.SerializeReferences(writer).GetArrayPointer();

            ////// 0x9C
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x9C, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0xA0, ColiCourseUtility.SerializeVerbose);
            //header.visualEffectTriggersPtr = visualEffectTriggers.SerializeReferences(writer).GetArrayPointer();

            ////// 0xA4
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0xA4, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0xA8, ColiCourseUtility.SerializeVerbose);
            //header.courseMetadataTriggersPtr = courseMetadataTriggers.SerializeReferences(writer).GetArrayPointer();

            ////// 0xAC
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0xAC, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0xB0, ColiCourseUtility.SerializeVerbose);
            //header.arcadeCheckpointTriggersPtr = arcadeCheckpointTriggers.SerializeReferences(writer).GetArrayPointer();

            ////// 0xB4
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0xB4, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0xB8, ColiCourseUtility.SerializeVerbose);
            //header.storyObjectTriggersPtr = storyObjectTriggers.SerializeReferences(writer).GetArrayPointer();

            ////// 0xBC
            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0xBC, ColiCourseUtility.SerializeVerbose);
            //header.trackIndexTable = trackIndexTable.SerializeReference(writer).GetPointer();

            // Overwrite header with pointers resolved
            writer.SeekStart();
            writer.WriteX(header);

            BinaryIoUtility.PopEndianess();
        }
    }
}
