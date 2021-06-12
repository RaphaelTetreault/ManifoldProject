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

        // TODO: move enum where most appropriate
        public enum SerializeFormat
        {
            InvalidFormat,
            AX,
            GX,
        }

        // CONSTANTS
        public const int kSizeOfZeroes0x20 = 0x14; // 20
        public const int kSizeOfZeroes0x28 = 0x20; // 32
        public const int kSizeOfZeroes0xD8 = 0x10; // 16
        public const int kAxConstPtr0x20 = 0xE4;
        public const int kAxConstPtr0x24 = 0xF8;
        public const int kGxConstPtr0x20 = 0xE8;
        public const int kGxConstPtr0x24 = 0xFC;

        // METADATA
        [UnityEngine.SerializeField] private bool isFileAX;
        [UnityEngine.SerializeField] private bool isFileGX;
        [UnityEngine.SerializeField] private AddressRange addressRange;
        [UnityEngine.SerializeField] private int id;
        [UnityEngine.SerializeField] private string name;
        [UnityEngine.SerializeField] private bool serializeVerbose = true;


        // FIELDS
        public Range unkRange0x00;
        public ArrayPointer trackNodesPtr;
        public ArrayPointer surfaceAttributeAreasPtr;
        public BoostPlatesActive boostPlatesActive;
        public Pointer staticColliderMeshesPtr;
        public Pointer zeroes0x20Ptr; // GX: 0xE8, AX: 0xE4
        public Pointer trackMinHeightPtr; // GX: 0xFC, AX: 0xF8
        public byte[] zeroes0x28;
        public int sceneObjectCount;
        public int unk_sceneObjectCount1;
        public int unk_sceneObjectCount2; // GX exclusive
        public Pointer sceneObjectsPtr;
        public Bool32 unkBool32_0x58;
        public ArrayPointer unknownSolsTriggersPtr;
        public ArrayPointer sceneInstancesPtr;
        public ArrayPointer sceneOriginObjectsPtr;
        public int zero0x74; // Ptr? Array Ptr length?
        public int zero0x78; // Ptr? Array Ptr address?
        public CircuitType circuitType;
        public Pointer fogCurvesPtr;
        public Pointer fogPtr;
        public int zero0x88; // Ptr? Array Ptr length?
        public int zero0x8C; // Ptr? Array Ptr address?
        public Pointer trackLengthPtr;
        public ArrayPointer unknownTriggersPtr;
        public ArrayPointer visualEffectTriggersPtr;
        public ArrayPointer courseMetadataTriggersPtr;
        public ArrayPointer arcadeCheckpointTriggersPtr;
        public ArrayPointer storyObjectTriggersPtr;
        public Pointer trackCheckpointMatrixPtr;
        public BoundsXZ courseBoundsXZ;
        public byte[] zeroes0xD8;

        // REFERENCE FIELDS
        public TrackNode[] trackNodes = new TrackNode[0];
        public SurfaceAttributeArea[] surfaceAttributeAreas = new SurfaceAttributeArea[0];
        public StaticColliderMeshes staticColliderMeshes;
        public byte[] zeroes0x20 = new byte[kSizeOfZeroes0x20];
        public TrackMinHeight trackMinHeight;
        public SceneObject[] sceneObjects = new SceneObject[0];
        public SceneInstanceReference[] sceneInstancesList = new SceneInstanceReference[0];
        public SceneOriginObjects[] sceneOriginObjects = new SceneOriginObjects[0];
        public UnknownSolsTrigger[] unknownSolsTriggers = new UnknownSolsTrigger[0];
        public FogCurves fogAnimationCurves;
        public Fog fog;
        public TrackLength trackLength;
        public UnknownTrigger[] unknownTriggers = new UnknownTrigger[0];
        public VisualEffectTrigger[] visualEffectTriggers = new VisualEffectTrigger[0];
        public CourseMetadataTrigger[] courseMetadataTriggers = new CourseMetadataTrigger[0];
        public ArcadeCheckpointTrigger[] arcadeCheckpointTriggers = new ArcadeCheckpointTrigger[0];
        public StoryObjectTrigger[] storyObjectTriggers = new StoryObjectTrigger[0];
        public TrackCheckpointMatrix8x8 trackCheckpointMatrix;
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
            bool isAx0x20 = ptr0x20.address == kAxConstPtr0x20;
            bool isAx0x24 = ptr0x24.address == kAxConstPtr0x24;
            bool isAX = isAx0x20 & isAx0x24;
            return isAX;
        }

        public static bool IsGX(Pointer ptr0x20, Pointer ptr0x24)
        {
            bool isGx0x20 = ptr0x20.address == kGxConstPtr0x20;
            bool isGx0x24 = ptr0x24.address == kGxConstPtr0x24;
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
            reader.JumpToAddress(staticColliderMeshesPtr);
            reader.ReadX(ref staticColliderMeshes, true);

            // 0x20
            reader.JumpToAddress(zeroes0x20Ptr);
            reader.ReadX(ref zeroes0x20, kSizeOfZeroes0x20);

            // 0x24
            reader.JumpToAddress(trackMinHeightPtr);
            reader.ReadX(ref trackMinHeight, true);

            // 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address): Scene Objects
            reader.JumpToAddress(sceneObjectsPtr);
            reader.ReadX(ref sceneObjects, sceneObjectCount, true);

            // 0x5C and 0x60 SOLS values
            reader.JumpToAddress(unknownSolsTriggersPtr);
            reader.ReadX(ref unknownSolsTriggers, unknownSolsTriggersPtr.Length, true);

            // 0x64 and 0x68
            reader.JumpToAddress(sceneInstancesPtr);
            reader.ReadX(ref sceneInstancesList, sceneInstancesPtr.Length, true);

            // 0x6C and 0x70
            reader.JumpToAddress(sceneOriginObjectsPtr);
            reader.ReadX(ref sceneOriginObjects, sceneOriginObjectsPtr.Length, true);

            // 0x80
            if (fogCurvesPtr.IsNotNullPointer)
            {
                reader.JumpToAddress(fogCurvesPtr);
                reader.ReadX(ref fogAnimationCurves, true);
            }

            // 0x84
            reader.JumpToAddress(fogPtr);
            reader.ReadX(ref fog, true);

            // 0x90 
            reader.JumpToAddress(trackLengthPtr);
            reader.ReadX(ref trackLength, true);

            // 0x94 and 0x98
            reader.JumpToAddress(unknownTriggersPtr);
            reader.ReadX(ref unknownTriggers, unknownTriggersPtr.Length, true);

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
            reader.JumpToAddress(trackCheckpointMatrixPtr);
            reader.ReadX(ref trackCheckpointMatrix, true);

            // DESERIALIZE UNIQUE TRACK TRANSFORMS
            var trackTransformPtrs = new List<Pointer>();
            foreach (var node in trackNodes)
            {
                var pointer = node.transformPtr;
                if (!trackTransformPtrs.Contains(pointer))
                    trackTransformPtrs.Add(pointer);
            }
            rootTrackTransforms = new TrackTransform[trackTransformPtrs.Count];
            for (int i = 0; i < rootTrackTransforms.Length; i++)
            {
                var pointer = trackTransformPtrs[i];
                reader.JumpToAddress(pointer);
                reader.ReadX(ref rootTrackTransforms[i], true);
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
                writer.WriteX(new byte[kSizeOfZeroes0x20], false); // TODO: HARD-CODED

                // 0x24
                // Resulting pointer should be 0xF8 or 0xFC for AX or GX, respectively.
                writer.WriteX(trackMinHeight);
                trackMinHeightPtr = trackMinHeight.GetPointer();

                // The pointers written by the last 2 calls should create a valid AX or GX file header.
                // If not, an assert will trigger.
                ValidateFileFormatPointers();
            }

            // Offset pointer address if AX file. Applies to pointers at address 0x
            int offset = IsFileAX ? -4 : 0;

            // Write credit and useful debugging info
            writer.CommentDateAndCredits(true);
            writer.Comment("File Information", true);
            writer.CommentLineWide("Format:", Format, true);
            writer.CommentLineWide("Verbose:", serializeVerbose, true);
            writer.CommentLineWide("Universal:", false, true);
            writer.CommentNewLine(true, '-');

            //writer.CommentNewLine(serializeVerbose);
            //writer.CommentPointer(0x08, serializeVerbose);
            //writer.CommentPointer(0x0C, serializeVerbose);
            //trackNodesPtr = trackNodes.SerializeReferences(writer).GetArrayPointer();
            writer.InlineDesc(serializeVerbose, 0x0C, trackNodes);
            writer.WriteX(trackNodes, false);

            writer.InlineDesc(serializeVerbose, 0x14, surfaceAttributeAreas);
            writer.WriteX(surfaceAttributeAreas, false);

            writer.InlineDesc(serializeVerbose, 0x1C, staticColliderMeshes);
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
            sceneOriginObjects = new SceneOriginObjects[]
            {
                new SceneOriginObjects() { instanceReference = sceneInstancesList[1] }
            };
            sceneObjects = new SceneObject[]
            {
                new SceneObject() {instanceReference = sceneInstancesList[0] }
            };

            // Write in reverse order. If references are correct, pointers will be correct when serializing.
            // No direct pointer
            writer.InlineDesc(serializeVerbose, -1, nameTable);
            writer.WriteX(nameTable, false);

            // No direct pointer
            writer.InlineDesc(serializeVerbose, -1, objectReferenceTable);
            writer.WriteX(objectReferenceTable, false);

            // 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address): Scene Objects;
            writer.InlineDesc(serializeVerbose, 0x54 + offset, sceneObjects);
            writer.WriteX(sceneObjects, false);
            // 
            writer.InlineDesc(serializeVerbose, 0x70 + offset, sceneOriginObjects);
            writer.WriteX(sceneOriginObjects, false);

            writer.InlineDesc(serializeVerbose, 0x60 + offset, unknownSolsTriggers);
            writer.WriteX(unknownSolsTriggers, false);

            writer.InlineDesc(serializeVerbose, 0x68 + offset, sceneInstancesList);
            writer.WriteX(sceneInstancesList, false);

            writer.InlineDesc(serializeVerbose, 0x70 + offset, sceneOriginObjects);
            writer.WriteX(sceneOriginObjects, false);

            //0x74, 0x78: unused

            writer.InlineDesc(serializeVerbose, 0x80 + offset, fogAnimationCurves);
            writer.WriteX(fogAnimationCurves);

            writer.InlineDesc(serializeVerbose, 0x84 + offset, fog);
            writer.WriteX(fog);

            // 0x88, 0x8C: unused

            writer.InlineDesc(serializeVerbose, 0x90 + offset, trackLength);
            writer.CommentLineWide("Length:", trackLength.value.ToString("0.00"), true); // print length
            writer.CommentNewLine(true, '-');
            writer.WriteX(trackLength);

            writer.InlineDesc(serializeVerbose, 0x94 + offset, unknownTriggers);
            writer.WriteX(unknownTriggers, false);

            writer.InlineDesc(serializeVerbose, 0x9C + offset, visualEffectTriggers);
            writer.WriteX(visualEffectTriggers, false);

            writer.InlineDesc(serializeVerbose, 0xA8 + offset, courseMetadataTriggers);
            writer.WriteX(courseMetadataTriggers, false);

            writer.InlineDesc(serializeVerbose, 0xB0 + offset, arcadeCheckpointTriggers);
            writer.WriteX(arcadeCheckpointTriggers, false);

            writer.InlineDesc(serializeVerbose, 0xB8 + offset, storyObjectTriggers);
            writer.WriteX(storyObjectTriggers, false);

            writer.InlineDesc(serializeVerbose, 0xBC + offset, trackCheckpointMatrix);
            writer.WriteX(trackCheckpointMatrix);

            // Non pointer data (aside from SceneObject counts)
            unkRange0x00 = new Range(0f, 10000f); // default for test stages
            boostPlatesActive = BoostPlatesActive.Enabled;
            unkBool32_0x58 = Bool32.True;
            circuitType = CircuitType.ClosedCircuit;
            courseBoundsXZ = new BoundsXZ();


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
                referers.AddRange(staticColliderMeshes.triMeshIndexMatrices);
                referers.AddRange(staticColliderMeshes.quadMeshIndexMatrices);

                // OBJECTS
                // The structure which points to the object name
                referers.AddRange(sceneObjectReferences);
                // The structure which points to the above and collider geometry
                referers.AddRange(sceneInstancesList);
                foreach (var instance in sceneInstancesList)
                    referers.Add(instance.colliderGeometry);
                // The list which points to objects placed at the origin
                referers.AddRange(sceneOriginObjects);
                // The scene objects
                referers.AddRange(sceneObjects);
                foreach (var obj in sceneObjects)
                {
                    //referers.Add(obj.animation);
                    foreach (var animationCurvePlus in obj.animation.animationCurvePluses)
                    {
                        referers.Add(animationCurvePlus);
                    }
                    referers.Add(obj.unk1);
                    referers.Add(obj.skeletalAnimator);
                }

                // The structure that points to 6 anim curves
                referers.Add(fogAnimationCurves);
                // The story mode checkpoints
                foreach (var storyObjectTrigger in storyObjectTriggers)
                {
                    referers.Add(storyObjectTrigger);
                    referers.Add(storyObjectTrigger.storyObjectPath);
                }
                // The checkpoint table
                referers.Add(trackCheckpointMatrix);

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
                reader.ReadX(ref unkRange0x00, true);
                reader.ReadX(ref trackNodesPtr);
                reader.ReadX(ref surfaceAttributeAreasPtr);
                reader.ReadX(ref boostPlatesActive);
                reader.ReadX(ref staticColliderMeshesPtr);
                reader.ReadX(ref zeroes0x20Ptr);
                reader.ReadX(ref trackMinHeightPtr);
                ValidateFileFormatPointers(); // VALIDATE
                reader.ReadX(ref zeroes0x28, kSizeOfZeroes0x28);
                reader.ReadX(ref sceneObjectCount);
                reader.ReadX(ref unk_sceneObjectCount1);
                if (isFileGX) reader.ReadX(ref unk_sceneObjectCount2);
                reader.ReadX(ref sceneObjectsPtr);
                reader.ReadX(ref unkBool32_0x58);
                reader.ReadX(ref unknownSolsTriggersPtr);
                reader.ReadX(ref sceneInstancesPtr);
                reader.ReadX(ref sceneOriginObjectsPtr);
                reader.ReadX(ref zero0x74);
                reader.ReadX(ref zero0x78);
                reader.ReadX(ref circuitType);
                reader.ReadX(ref fogCurvesPtr);
                reader.ReadX(ref fogPtr);
                reader.ReadX(ref zero0x88);
                reader.ReadX(ref zero0x8C);
                reader.ReadX(ref trackLengthPtr);
                reader.ReadX(ref unknownTriggersPtr);
                reader.ReadX(ref visualEffectTriggersPtr);
                reader.ReadX(ref courseMetadataTriggersPtr);
                reader.ReadX(ref arcadeCheckpointTriggersPtr);
                reader.ReadX(ref storyObjectTriggersPtr);
                reader.ReadX(ref trackCheckpointMatrixPtr);
                reader.ReadX(ref courseBoundsXZ, true);
                reader.ReadX(ref zeroes0xD8, kSizeOfZeroes0xD8);
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
                staticColliderMeshesPtr = staticColliderMeshes.GetPointer();
                surfaceAttributeAreasPtr = surfaceAttributeAreas.GetArrayPointer();
                trackCheckpointMatrixPtr = trackCheckpointMatrix.GetPointer();
                trackLengthPtr = trackLength.GetPointer();
                trackMinHeightPtr = trackMinHeight.GetPointer();
                trackNodesPtr = trackNodes.GetArrayPointer();
                fogPtr = fog.GetPointer();
                fogCurvesPtr = fogAnimationCurves.GetPointer();
                // TRIGGERS
                arcadeCheckpointTriggersPtr = arcadeCheckpointTriggers.GetArrayPointer();
                courseMetadataTriggersPtr = courseMetadataTriggers.GetArrayPointer();
                storyObjectTriggersPtr = storyObjectTriggers.GetArrayPointer();
                unknownSolsTriggersPtr = unknownSolsTriggers.GetArrayPointer();
                unknownTriggersPtr = unknownTriggers.GetArrayPointer();
                visualEffectTriggersPtr = visualEffectTriggers.GetArrayPointer();
                // SCENE OBJECTS
                // References
                sceneInstancesPtr = sceneInstancesList.GetArrayPointer();
                sceneOriginObjectsPtr = sceneOriginObjects.GetArrayPointer();
                // Main list
                sceneObjectCount = sceneObjects.Length;
                unk_sceneObjectCount1 = 0; // still don't know what this is for
                unk_sceneObjectCount2 = 0; // still don't know what this is for
                sceneObjectsPtr = sceneObjects.GetArrayPointer().Pointer;
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unkRange0x00);
                writer.WriteX(trackNodesPtr);
                writer.WriteX(surfaceAttributeAreasPtr);
                writer.WriteX(boostPlatesActive);
                writer.WriteX(staticColliderMeshesPtr);
                writer.WriteX(zeroes0x20Ptr);
                writer.WriteX(trackMinHeightPtr);
                writer.WriteX(new byte[kSizeOfZeroes0x28], false); // write const zeros
                writer.WriteX(sceneObjectCount);
                writer.WriteX(unk_sceneObjectCount1);
                if (Format == SerializeFormat.GX)
                    writer.WriteX(unk_sceneObjectCount2);
                writer.WriteX(sceneObjectsPtr);
                writer.WriteX(unkBool32_0x58);
                writer.WriteX(unknownSolsTriggersPtr);
                writer.WriteX(sceneInstancesPtr);
                writer.WriteX(sceneOriginObjectsPtr);
                writer.WriteX(new ArrayPointer()); // const unused
                writer.WriteX(circuitType);
                writer.WriteX(fogCurvesPtr);
                writer.WriteX(fogPtr);
                writer.WriteX(new ArrayPointer()); // const unused
                writer.WriteX(trackLengthPtr);
                writer.WriteX(unknownTriggersPtr);
                writer.WriteX(visualEffectTriggersPtr);
                writer.WriteX(courseMetadataTriggersPtr);
                writer.WriteX(arcadeCheckpointTriggersPtr);
                writer.WriteX(storyObjectTriggersPtr);
                writer.WriteX(trackCheckpointMatrixPtr);
                writer.WriteX(courseBoundsXZ);
                writer.WriteX(new byte[kSizeOfZeroes0xD8], false); // write const zeros
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
            Assert.IsTrue(staticColliderMeshesPtr.IsNotNullPointer);
            Assert.IsTrue(zeroes0x20Ptr.IsNotNullPointer);
            Assert.IsTrue(trackMinHeightPtr.IsNotNullPointer);
            Assert.IsTrue(sceneObjectsPtr.IsNotNullPointer);
            Assert.IsTrue(sceneInstancesPtr.IsNotNullPointer);
            Assert.IsTrue(sceneOriginObjectsPtr.IsNotNullPointer);
            Assert.IsTrue(fogPtr.IsNotNullPointer);
            Assert.IsTrue(trackCheckpointMatrixPtr.IsNotNullPointer);

            // Assert pointers which can be null IF the reference type is not null
            if (unknownSolsTriggers != null)
                Assert.IsTrue(unknownSolsTriggersPtr.IsNotNullPointer);
            if (fogAnimationCurves != null)
                Assert.IsTrue(fogCurvesPtr.IsNotNullPointer);
            if (fog != null)
                Assert.IsTrue(unknownTriggersPtr.IsNotNullPointer);
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
