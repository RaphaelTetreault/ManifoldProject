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
    public class ColiScene :
        IBinaryAddressable,
        IBinarySerializable,
        IFile,
        ISerializedBinaryAddressableReferer
    {

        // To find
        // * minimap rotation
        // OF NOTE:
        // some of this stuff might be in the REL file.

        // TODO: move where most appropriate
        public enum SerializeFormat
        {
            InvalidFormat,
            AX,
            GX,
        }

        // CONSTANTS
        public const int kCountZeroes0x20 = 0x14; // 20
        public const int kSizeOfZero0x28 = 0x20; // 32
        public const int kSizeOfZero0xD8 = 0x10; // 16
        public const int kAxConst0x20 = 0xE4;
        public const int kAxConst0x24 = 0xF8;
        public const int kGxConst0x20 = 0xE8;
        public const int kGxConst0x24 = 0xFC;

        // METADATA
        [UnityEngine.SerializeField] private bool isFileAX;
        [UnityEngine.SerializeField] private bool isFileGX;
        [UnityEngine.SerializeField] private AddressRange addressRange;
        [UnityEngine.SerializeField] private int id;
        [UnityEngine.SerializeField] private string name;


        // FIELDS
        public UnknownFloatPair unk_0x00;
        public ArrayPointer trackNodesPtr;
        public ArrayPointer surfaceAttributeAreasPtr;
        public BoostPadsActive boostPadsActive;
        public Pointer staticColliderMeshTablePtr;
        public Pointer zeroes0x20Ptr; // GX: 0xE8, AX: 0xE4
        public Pointer trackMinHeightPtr; // GX: 0xFC, AX: 0xF8
        public byte[] zeroes0x28;
        public int sceneObjectCount;
        public int unk_sceneObjectCount1;
        public int unk_sceneObjectCount2; // GX exclusive
        public Pointer sceneObjectsPtr;
        public Bool32 unkBool32_0x58;
        public ArrayPointer unknownSolsTriggerPtrs;
        public ArrayPointer sceneInstancesListPtrs;
        public ArrayPointer sceneOriginObjectsListPtrs;
        public int zero0x74;
        public int zero0x78;
        //public ArrayPointer unused_0x74_0x78;
        public CircuitType circuitType;
        public Pointer unknownStageData2Ptr;
        public Pointer unknownStageData1Ptr;
        public int zero0x88;
        public int zero0x8C;
        //public ArrayPointer unused_0x88_0x8C;
        public Pointer trackLengthPtr;
        public ArrayPointer unknownTrigger1sPtr;
        public ArrayPointer visualEffectTriggersPtr;
        public ArrayPointer courseMetadataTriggersPtr;
        public ArrayPointer arcadeCheckpointTriggersPtr;
        public ArrayPointer storyObjectTriggersPtr;
        public Pointer trackCheckpointTable8x8Ptr;
        public Bounds courseBounds;
        public byte[] zeroes0xD8;
        // REFERENCE FIELDS
        public TrackNode[] trackNodes = new TrackNode[0];
        public SurfaceAttributeArea[] surfaceAttributeAreas = new SurfaceAttributeArea[0];
        public StaticColliderMeshes staticColliderMeshes = new StaticColliderMeshes();
        public byte[] zeroes0x20 = new byte[kCountZeroes0x20];
        public TrackMinHeight trackMinHeight;
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
        public TrackCheckpointTable8x8 trackCheckpointTable8x8 = new TrackCheckpointTable8x8();
        // FIELDS (that require extra processing)
        public TrackTransform[] allTrackTransforms;
        public TrackTransform[] rootTrackTransforms;
        public CString[] nameTable;
        public SceneObjectReference[] objectReferenceTable;



        // PROPERTIES
        public bool IsFileAX => isFileAX;
        public bool IsFileGX => isFileGX;
        /// <summary>
        /// Returns true if file is tagged either AX or GX, but not both
        /// </summary>
        public bool IsValidFile => isFileAX ^ isFileGX;
        public SerializeFormat Format { get; set; }

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }
        public string FileName
        {
            get => name;
            set => name = value;
        }

        public int ID => id;



        public static bool IsAX(Pointer ptr0x20, Pointer ptr0x24)
        {
            bool isAx0x20 = ptr0x20.address == kAxConst0x20;
            bool isAx0x24 = ptr0x24.address == kAxConst0x24;
            bool isAX = isAx0x20 & isAx0x24;
            return isAX;
        }

        public static bool IsGX(Pointer ptr0x20, Pointer ptr0x24)
        {
            bool isGx0x20 = ptr0x20.address == kGxConst0x20;
            bool isGx0x24 = ptr0x24.address == kGxConst0x24;
            bool isGX = isGx0x20 & isGx0x24;
            return isGX;
        }

        public void ValidateFileFormatPointers()
        {
            isFileAX = IsAX(zeroes0x20Ptr, trackMinHeightPtr);
            isFileGX = IsGX(zeroes0x20Ptr, trackMinHeightPtr);
            Assert.IsTrue(IsValidFile);
        }


        public void Deserialize(BinaryReader reader)
        {
            BinaryIoUtility.PushEndianess(false);
            DebugConsole.Log(FileName);

            // Store the stage index, can solve venue and course name from this
            var matchDigits = Regex.Match(FileName, Const.Regex.MatchIntegers);
            id = int.Parse(matchDigits.Value);

            // Read COLI_COURSE## file header
            DeserializeSelf(reader);

            // 0x08 and 0x0C: Track Nodes
            reader.JumpToAddress(trackNodesPtr);
            reader.ReadX(ref trackNodes, trackNodesPtr.Length, true);

            // 0x10 and 0x14: Track Effect Attribute Areas
            reader.JumpToAddress(surfaceAttributeAreasPtr);
            reader.ReadX(ref surfaceAttributeAreas, surfaceAttributeAreasPtr.Length, true);

            // 0x1C 
            reader.JumpToAddress(staticColliderMeshTablePtr);
            reader.ReadX(ref staticColliderMeshes, true);

            // 0x20
            reader.JumpToAddress(zeroes0x20Ptr);
            reader.ReadX(ref zeroes0x20, kCountZeroes0x20);

            // 0x24
            reader.JumpToAddress(trackMinHeightPtr);
            reader.ReadX(ref trackMinHeight, true);

            // 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address): Scene Objects
            reader.JumpToAddress(sceneObjectsPtr);
            reader.ReadX(ref sceneObjects, sceneObjectCount, true);

            // 0x5C and 0x60 SOLS values
            reader.JumpToAddress(unknownSolsTriggerPtrs);
            reader.ReadX(ref unknownSolsTriggers, unknownSolsTriggerPtrs.Length, true);

            // 0x64 and 0x68
            reader.JumpToAddress(sceneInstancesListPtrs);
            reader.ReadX(ref sceneInstancesList, sceneInstancesListPtrs.Length, true);

            // 0x6C and 0x70
            reader.JumpToAddress(sceneOriginObjectsListPtrs);
            reader.ReadX(ref sceneOriginObjectsList, sceneOriginObjectsListPtrs.Length, true);

            // 0x80
            if (unknownStageData2Ptr.IsNotNullPointer)
            {
                reader.JumpToAddress(unknownStageData2Ptr);
                reader.ReadX(ref unknownStageData2, true);
            }

            // 0x84
            reader.JumpToAddress(unknownStageData1Ptr);
            reader.ReadX(ref unknownStageData1, true);

            // 0x90 
            reader.JumpToAddress(trackLengthPtr);
            reader.ReadX(ref trackLength, true);

            // 0x94 and 0x98
            reader.JumpToAddress(unknownTrigger1sPtr);
            reader.ReadX(ref unknownTrigger1s, unknownTrigger1sPtr.Length, true);

            // 0x9C and 0xA0
            reader.JumpToAddress(visualEffectTriggersPtr);
            reader.ReadX(ref visualEffectTriggers, visualEffectTriggersPtr.Length, true);

            // 0xA4 and 0xA8
            reader.JumpToAddress(courseMetadataTriggersPtr);
            reader.ReadX(ref courseMetadataTriggers, courseMetadataTriggersPtr.Length, true);

            // 0xAC and 0xB0
            reader.JumpToAddress(arcadeCheckpointTriggersPtr);
            reader.ReadX(ref arcadeCheckpointTriggers, arcadeCheckpointTriggersPtr.Length, true);

            // 0xB4 and 0xB8
            reader.JumpToAddress(storyObjectTriggersPtr);
            reader.ReadX(ref storyObjectTriggers, storyObjectTriggersPtr.Length, true);

            // 0xBC and 0xC0
            reader.JumpToAddress(trackCheckpointTable8x8Ptr);
            reader.ReadX(ref trackCheckpointTable8x8, true);

            // DESERIALIZE UNIQUE TRACK TRANSFORMS
            var trackTransformPtrs = new List<Pointer>();
            foreach (var node in trackNodes)
            {
                var pointer = node.AddressRange.GetPointer();
                if (trackTransformPtrs.Contains(pointer))
                    trackTransformPtrs.Add(pointer);
            }
            rootTrackTransforms = new TrackTransform[trackTransformPtrs.Count];
            for (int i = 0; i < rootTrackTransforms.Length; i++)
            {
                var pointer = trackTransformPtrs[i];
                reader.JumpToAddress(pointer);
                rootTrackTransforms[i].Deserialize(reader);
            }
            // Track Transforms

            BinaryIoUtility.PopEndianess();
        }

        public void Serialize(BinaryWriter writer)
        {
            BinaryIoUtility.PushEndianess(false);
            DebugConsole.Log(FileName);

            // Write header
            SerializeSelf(writer);

            // MAINTAIN FILE IDENTIFICATION COMPATIBILITY
            {
                // 0x20
                // Resulting pointer should be 0xE4 or 0xE8 for AX or GX, respectively.
                zeroes0x20Ptr = writer.GetPositionAsPointer();
                writer.WriteX(new byte[kCountZeroes0x20], false); // TODO: HARD-CODED

                // 0x24
                // Resulting pointer should be 0xF8 or 0xFC for AX or GX, respectively.
                writer.WriteX(trackMinHeight);
                trackMinHeightPtr = trackMinHeight.GetPointer();

                // The pointers written by the last 2 calls should create a valid AX or GX file header.
                // If not, an assert will trigger.
                ValidateFileFormatPointers();
            }


            // Write credit and useful debugging info
            writer.CommentDateAndCredits(true);
            writer.Comment("File Information", true);
            writer.CommentLineWide("Format:", Format, true);
            writer.CommentLineWide("Universal:", false, true);
            writer.CommentNewLine(true, '-');

            //writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x08, ColiCourseUtility.SerializeVerbose);
            //writer.CommentPointer(0x0C, ColiCourseUtility.SerializeVerbose);
            //header.trackNodesPtr = trackNodes.SerializeReferences(writer).GetArrayPointer();

            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x14, surfaceAttributeAreas);
            writer.WriteX(surfaceAttributeAreas, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x1C, staticColliderMeshes);
            writer.WriteX(staticColliderMeshes);

            // SCENE OBJECTS
            // TEST. Next: try reserializing references with ISerailizedBinaryAddressableReferer.
            var names = new CString[]
            {
                new CString{ value = "TEST_STRING" },
                new CString{ value = "ORIGIN_OBJECT" }
            };
            var sceneObjectReferences = new SceneObjectReference[]
            {
                new SceneObjectReference() { name = names[0] },
                new SceneObjectReference() { name = names[1] },
            };
            sceneInstancesList = new SceneInstanceReference[]
            {
                new SceneInstanceReference() { objectReference = sceneObjectReferences[0] },
                new SceneInstanceReference() { objectReference = sceneObjectReferences[1] },
            };
            sceneOriginObjectsList = new SceneOriginObjects[]
            {
                new SceneOriginObjects() { instanceReference = sceneInstancesList[1] }
            };
            sceneObjects = new SceneObject[]
            {
                new SceneObject() {instanceReference = sceneInstancesList[0] }
            };

            // Write in reverse order. If references are correct, pointers will be correct when serializing.
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, -1, nameTable);
            writer.WriteX(nameTable, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, -1, objectReferenceTable);
            writer.WriteX(objectReferenceTable, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, -1, sceneInstancesList);
            writer.WriteX(sceneInstancesList, false);

            // Offset pointer address if AX file
            int offset = IsFileAX ? -4 : 0;

            // 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address): Scene Objects;
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x54 + offset, sceneObjects);
            writer.WriteX(sceneObjects, false);
            // 
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x70 + offset, sceneOriginObjectsList);
            writer.WriteX(sceneOriginObjectsList, false);

            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x60 + offset, unknownSolsTriggers);
            writer.WriteX(unknownSolsTriggers, false);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x68 + offset, sceneInstancesList);
            writer.WriteX(sceneInstancesList, false);
            //writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x70 + offset, sceneOriginObjectsList);
            //writer.WriteX(sceneOriginObjectsList, false);
            // 0x74, 0x78: unused in header
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x80 + offset, unknownStageData2);
            writer.WriteX(unknownStageData2);
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x84 + offset, unknownStageData1);
            writer.WriteX(unknownStageData1);
            // 0x88, 0x8C: unused in header
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0x90 + offset, trackLength);
            writer.CommentLineWide("Length:", trackLength.value.ToString("0.00"), true); // print length
            writer.CommentNewLine(true, '-');
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
            writer.InlineDesc(ColiCourseUtility.SerializeVerbose, 0xBC + offset, trackCheckpointTable8x8);
            writer.WriteX(trackCheckpointTable8x8);


            // Non pointer data (aside from SceneObject counts)
            unk_0x00 = new UnknownFloatPair();
            boostPadsActive = BoostPadsActive.Enabled;
            unkBool32_0x58 = Bool32.True;
            circuitType = CircuitType.ClosedCircuit;
            courseBounds = new Bounds();

            // GET ALL REFERERS, RE-SERIALIZE FOR POINTERS
            {
                // Get a reference to EVERY object in file that has a pointer to an object
                var referers = new List<ISerializedBinaryAddressableReferer>();

                // The file header has many dependencies
                referers.Add(this);

                // Track Nodes and dependencies
                referers.AddRange(trackNodes);
                referers.AddRange(allTrackTransforms);
                foreach (var trackTransform in allTrackTransforms)
                    referers.Add(trackTransform.trackTopology);

                // Static Collider Meshes and dependencies
                referers.Add(staticColliderMeshes);
                referers.AddRange(staticColliderMeshes.triMeshIndexTables);
                referers.AddRange(staticColliderMeshes.quadMeshIndexTables);

                // OBJECTS
                // The structure which points to the object name
                referers.AddRange(sceneObjectReferences);
                // The structure which points to the above and collider geometry
                referers.AddRange(sceneInstancesList);
                foreach (var instance in sceneInstancesList)
                    referers.Add(instance.colliderGeometry);
                // The list which points to objects placed at the origin
                referers.AddRange(sceneOriginObjectsList);
                // The scene objects
                referers.AddRange(sceneObjects);
                foreach (var obj in sceneObjects)
                {
                    referers.Add(obj.animation);
                    foreach (var animationCurvePlus in obj.animation.animationCurvePluses)
                    {
                        referers.Add(animationCurvePlus);
                    }
                    referers.Add(obj.unk1);
                    referers.Add(obj.skeletalAnimator);
                }

                // The structure that points to 6 anim curves
                referers.Add(unknownStageData2);
                // The story mode checkpoints
                foreach (var storyObjectTrigger in storyObjectTriggers)
                {
                    referers.Add(storyObjectTrigger);
                    referers.Add(storyObjectTrigger.storyObjectPath);
                }
                // The checkpoint table
                referers.Add(trackCheckpointTable8x8);

                // Patch pointers by re-writing structure in same place as previously serialized
                foreach (var referer in referers)
                {
                    var pointer = referer.GetPointer();
                    writer.JumpToAddress(pointer);
                    referer.Serialize(writer);
                    // Run assertions on referer to ensure pointer requirements are met
                    referer.ValidateReferences();
                }
            } // end patching pointers

            BinaryIoUtility.PopEndianess();
        }

        public T[] RemoveDuplicateInstances<T>(T[] array)
            where T : IBinaryAddressable
        {
            var uniqueList = new List<T>();
            var uniqueAddressses = new List<int>();
            foreach (var item in array)
            {
                var address = item.AddressRange.GetPointer().address;
                if (!uniqueAddressses.Contains(address))
                {
                    // Keep track of current address
                    uniqueAddressses.Add(address);
                    uniqueList.Add(item);
                }
            }
            return uniqueList.ToArray();
        }



        public void DeserializeSelf(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // Deserialize main structure
                reader.ReadX(ref unk_0x00, true);
                reader.ReadX(ref trackNodesPtr);
                reader.ReadX(ref surfaceAttributeAreasPtr);
                reader.ReadX(ref boostPadsActive);
                reader.ReadX(ref staticColliderMeshTablePtr);
                reader.ReadX(ref zeroes0x20Ptr);
                reader.ReadX(ref trackMinHeightPtr);
                ValidateFileFormatPointers(); // VALIDATE
                reader.ReadX(ref zeroes0x28, kSizeOfZero0x28);
                reader.ReadX(ref sceneObjectCount);
                reader.ReadX(ref unk_sceneObjectCount1);
                if (isFileGX) reader.ReadX(ref unk_sceneObjectCount2);
                reader.ReadX(ref sceneObjectsPtr);
                reader.ReadX(ref unkBool32_0x58);
                reader.ReadX(ref unknownSolsTriggerPtrs);
                reader.ReadX(ref sceneInstancesListPtrs);
                reader.ReadX(ref sceneOriginObjectsListPtrs);
                reader.ReadX(ref zero0x74);
                reader.ReadX(ref zero0x78);
                reader.ReadX(ref circuitType);
                reader.ReadX(ref unknownStageData2Ptr);
                reader.ReadX(ref unknownStageData1Ptr);
                reader.ReadX(ref zero0x88);
                reader.ReadX(ref zero0x8C);
                reader.ReadX(ref trackLengthPtr);
                reader.ReadX(ref unknownTrigger1sPtr);
                reader.ReadX(ref visualEffectTriggersPtr);
                reader.ReadX(ref courseMetadataTriggersPtr);
                reader.ReadX(ref arcadeCheckpointTriggersPtr);
                reader.ReadX(ref storyObjectTriggersPtr);
                reader.ReadX(ref trackCheckpointTable8x8Ptr);
                reader.ReadX(ref courseBounds, true);
                reader.ReadX(ref zeroes0xD8, kSizeOfZero0xD8);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero0x74 == 0);
                Assert.IsTrue(zero0x78 == 0);
                Assert.IsTrue(zero0x88 == 0);
                Assert.IsTrue(zero0x8C == 0);

                for (int i = 0; i < zeroes0x28.Length; i++)
                    Assert.IsTrue(zeroes0x28[i] == 0);

                for (int i = 0; i < zeroes0xD8.Length; i++)
                    Assert.IsTrue(zeroes0xD8[i] == 0);

                // Record some metadata
                Format = IsFileAX ? SerializeFormat.AX : SerializeFormat.GX;
            }
        }

        public void SerializeSelf(BinaryWriter writer)
        {
            {
                // Refresh metadata
                isFileAX = Format == SerializeFormat.AX;
                isFileGX = Format == SerializeFormat.GX;

                // TODO: below. Can't assert, this is called twice.
                // Two pointers should already be serialized...
                // Might be worth putting the zeroes in some class, then
                // both could just .GetPointer() and there would be no
                // issues around when this function is called.
                //Assert.IsTrue(zeroes0x20Ptr.IsNotNullPointer);
                //Assert.IsTrue(trackMinHeightPtr.IsNotNullPointer);

                // UPDATE POINTERS AND COUNTS
                // Track and stage data
                staticColliderMeshTablePtr = staticColliderMeshes.GetPointer();
                surfaceAttributeAreasPtr = surfaceAttributeAreas.GetArrayPointer();
                trackCheckpointTable8x8Ptr = trackCheckpointTable8x8.GetPointer();
                trackLengthPtr = trackLength.GetPointer();
                trackMinHeightPtr = trackMinHeight.GetPointer();
                trackNodesPtr = trackNodes.GetArrayPointer();
                unknownStageData1Ptr = unknownStageData1.GetPointer();
                unknownStageData2Ptr = unknownStageData2.GetPointer();
                // TRIGGERS
                arcadeCheckpointTriggersPtr = arcadeCheckpointTriggers.GetArrayPointer();
                courseMetadataTriggersPtr = courseMetadataTriggers.GetArrayPointer();
                storyObjectTriggersPtr = storyObjectTriggers.GetArrayPointer();
                unknownSolsTriggerPtrs = unknownSolsTriggers.GetArrayPointer();
                unknownTrigger1sPtr = unknownTrigger1s.GetArrayPointer();
                visualEffectTriggersPtr = visualEffectTriggers.GetArrayPointer();
                // SCENE OBJECTS
                // References
                sceneInstancesListPtrs = sceneInstancesList.GetArrayPointer();
                sceneOriginObjectsListPtrs = sceneOriginObjectsList.GetArrayPointer();
                // Main list
                sceneObjectCount = sceneObjects.Length;
                unk_sceneObjectCount1 = 0; // still don't know what this is for
                unk_sceneObjectCount2 = 0; // still don't know what this is for
                sceneObjectsPtr = sceneObjects.GetArrayPointer().Pointer;
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(trackNodesPtr);
                writer.WriteX(surfaceAttributeAreasPtr);
                writer.WriteX(boostPadsActive);
                writer.WriteX(staticColliderMeshTablePtr);
                writer.WriteX(zeroes0x20Ptr);
                writer.WriteX(trackMinHeightPtr);
                writer.WriteX(new byte[kSizeOfZero0x28], false); // write const zeros
                writer.WriteX(sceneObjectCount);
                writer.WriteX(unk_sceneObjectCount1);
                if (Format == SerializeFormat.GX)
                    writer.WriteX(unk_sceneObjectCount2);
                writer.WriteX(sceneObjectsPtr);
                writer.WriteX(unkBool32_0x58);
                writer.WriteX(unknownSolsTriggerPtrs);
                writer.WriteX(sceneInstancesListPtrs);
                writer.WriteX(sceneOriginObjectsListPtrs);
                writer.WriteX(new ArrayPointer()); // const unused
                writer.WriteX(circuitType);
                writer.WriteX(unknownStageData2Ptr);
                writer.WriteX(unknownStageData1Ptr);
                writer.WriteX(new ArrayPointer()); // const unused
                writer.WriteX(trackLengthPtr);
                writer.WriteX(unknownTrigger1sPtr);
                writer.WriteX(visualEffectTriggersPtr);
                writer.WriteX(courseMetadataTriggersPtr);
                writer.WriteX(arcadeCheckpointTriggersPtr);
                writer.WriteX(storyObjectTriggersPtr);
                writer.WriteX(trackCheckpointTable8x8Ptr);
                writer.WriteX(courseBounds);
                writer.WriteX(new byte[kSizeOfZero0xD8], false); // write const zeros
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            Assert.IsTrue(zero0x74 == 0);
            Assert.IsTrue(zero0x78 == 0);
            Assert.IsTrue(zero0x88 == 0);
            Assert.IsTrue(zero0x8C == 0);

            for (int i = 0; i < zeroes0x28.Length; i++)
                Assert.IsTrue(zeroes0x28[i] == 0);

            for (int i = 0; i < zeroes0xD8.Length; i++)
                Assert.IsTrue(zeroes0xD8[i] == 0);

            // Assert all pointers which are never null in game files
            Assert.IsTrue(trackNodesPtr.IsNotNullPointer);
            Assert.IsTrue(surfaceAttributeAreasPtr.IsNotNullPointer);
            Assert.IsTrue(staticColliderMeshTablePtr.IsNotNullPointer);
            Assert.IsTrue(zeroes0x20Ptr.IsNotNullPointer);
            Assert.IsTrue(trackMinHeightPtr.IsNotNullPointer);
            Assert.IsTrue(sceneObjectsPtr.IsNotNullPointer);
            Assert.IsTrue(sceneInstancesListPtrs.IsNotNullPointer);
            Assert.IsTrue(sceneOriginObjectsListPtrs.IsNotNullPointer);
            Assert.IsTrue(unknownStageData1Ptr.IsNotNullPointer);
            Assert.IsTrue(trackCheckpointTable8x8Ptr.IsNotNullPointer);

            // Assert pointers which can be null IF the reference type is not null
            if (unknownSolsTriggers != null)
                Assert.IsTrue(unknownSolsTriggerPtrs.IsNotNullPointer);
            if (unknownStageData2 != null)
                Assert.IsTrue(unknownStageData2Ptr.IsNotNullPointer);
            if (unknownStageData1 != null)
                Assert.IsTrue(unknownTrigger1sPtr.IsNotNullPointer);
            if (visualEffectTriggers != null)
                Assert.IsTrue(visualEffectTriggersPtr.IsNotNullPointer);
            if (courseMetadataTriggers != null)
                Assert.IsTrue(courseMetadataTriggersPtr.IsNotNullPointer);
            if (arcadeCheckpointTriggers != null)
                Assert.IsTrue(arcadeCheckpointTriggersPtr.IsNotNullPointer);
            if (storyObjectTriggers != null)
                Assert.IsTrue(storyObjectTriggersPtr.IsNotNullPointer);
        }
    }
}
