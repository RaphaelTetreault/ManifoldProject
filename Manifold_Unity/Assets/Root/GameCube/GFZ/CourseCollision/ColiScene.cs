using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Mathematics;

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
        [UnityEngine.SerializeField] private string fileName;
        [UnityEngine.SerializeField] private Venue venue;
        [UnityEngine.SerializeField] private string courseName;
        [UnityEngine.SerializeField] private string author;
        [UnityEngine.SerializeField] private bool serializeVerbose = true;
        [UnityEngine.SerializeField] private SerializeFormat format;


        // FIELDS
        public Range unkRange0x00;
        public ArrayPointer trackNodesPtr;
        public ArrayPointer surfaceAttributeAreasPtr;
        public BoostPlatesActive boostPlatesActive = BoostPlatesActive.Enabled;
        public Pointer staticColliderMeshesPtr;
        public Pointer zeroes0x20Ptr; // GX: 0xE8, AX: 0xE4
        public Pointer trackMinHeightPtr; // GX: 0xFC, AX: 0xF8
        public byte[] zeroes0x28 = new byte[kSizeOfZeroes0x28]; //
        public int sceneObjectCount;
        public int unk_sceneObjectCount1;
        public int unk_sceneObjectCount2; // GX exclusive
        public Pointer sceneObjectsPtr;
        public Bool32 unkBool32_0x58 = Bool32.True;
        public ArrayPointer unknownSolsTriggersPtr;
        public ArrayPointer sceneInstancesPtr;
        public ArrayPointer sceneOriginObjectsPtr;
        public int zero0x74; // Ptr? Array Ptr length?
        public int zero0x78; // Ptr? Array Ptr address?
        public CircuitType circuitType = CircuitType.ClosedCircuit;
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
        public byte[] zeroes0xD8 = new byte[kSizeOfZeroes0xD8]; // 

        // REFERENCE FIELDS
        public TrackNode[] trackNodes = new TrackNode[0];
        public SurfaceAttributeArea[] surfaceAttributeAreas = SurfaceAttributeArea.DefaultArray();
        public StaticColliderMeshes staticColliderMeshes;
        public byte[] zeroes0x20 = new byte[kSizeOfZeroes0x20];
        public TrackMinHeight trackMinHeight = new TrackMinHeight(); // has default constructor
        public SceneObject[] sceneObjects = new SceneObject[0];
        public SceneInstanceReference[] sceneInstances = new SceneInstanceReference[0];
        public SceneOriginObject[] sceneOriginObjects = new SceneOriginObject[0];
        public UnknownSolsTrigger[] unknownSolsTriggers = new UnknownSolsTrigger[0];
        public FogCurves fogCurves;
        public Fog fog;
        public TrackLength trackLength;
        public UnknownTrigger[] unknownTriggers = new UnknownTrigger[0];
        public VisualEffectTrigger[] visualEffectTriggers = new VisualEffectTrigger[0];
        public CourseMetadataTrigger[] courseMetadataTriggers = new CourseMetadataTrigger[0];
        public ArcadeCheckpointTrigger[] arcadeCheckpointTriggers = new ArcadeCheckpointTrigger[0];
        public StoryObjectTrigger[] storyObjectTriggers = new StoryObjectTrigger[0];
        public TrackCheckpointMatrix trackCheckpointMatrix;
        // FIELDS (that require extra processing)
        // Shared references
        public TrackSegment[] allTrackSegments = new TrackSegment[0];
        public TrackSegment[] rootTrackSegments = new TrackSegment[0];
        public CString[] objectNames = new CString[0];
        public SceneObjectReference[] sceneObjectReferences = new SceneObjectReference[0];
        // NOTE: instances are also shared
        public ColliderGeometry[] sceneColliderGeometries = new ColliderGeometry[0];
        // NOTE: these ones MIGHT be? TODO: validate with analytics?
        public UnknownSceneObjectData[] unkSceneObjData = new UnknownSceneObjectData[0];
        public AnimationClip[] animationClips = new AnimationClip[0];
        public SkeletalAnimator[] skeletalAnimators = new SkeletalAnimator[0];


        // PROPERTIES
        public bool IsFileAX => isFileAX;
        public bool IsFileGX => isFileGX;
        /// <summary>
        /// Returns true if file is tagged either AX or GX, but not both
        /// </summary>
        public bool IsValidFile => isFileAX ^ isFileGX;
        public SerializeFormat Format
        {
            get => format;
            set => format = value;
        }

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }

        public string CourseName
        {
            get => courseName;
            set => courseName = value;
        }

        public string VenueName
        {
            get => EnumExtensions.GetDescription(venue);
        }

        public Venue Venue
        {
            get => venue;
            set => venue = value;
        }
        public string Author
        {
            get => author;
            set => author = value;
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
            DeserializeHeader(reader);

            // 0x08 and 0x0C: Track Nodes
            reader.JumpToAddress(trackNodesPtr);
            reader.ReadX(ref trackNodes, trackNodesPtr.Length, true);

            // 0x10 and 0x14: Track Effect Attribute Areas
            reader.JumpToAddress(surfaceAttributeAreasPtr);
            reader.ReadX(ref surfaceAttributeAreas, surfaceAttributeAreasPtr.Length, true);

            // 0x1C 
            // Format is deserialized in DeserializeSelf(reader);
            staticColliderMeshes = new StaticColliderMeshes(Format);
            reader.JumpToAddress(staticColliderMeshesPtr);
            reader.ReadX(ref staticColliderMeshes, false);

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
            reader.ReadX(ref sceneInstances, sceneInstancesPtr.Length, true);

            // 0x6C and 0x70
            reader.JumpToAddress(sceneOriginObjectsPtr);
            reader.ReadX(ref sceneOriginObjects, sceneOriginObjectsPtr.Length, true);

            // 0x80
            if (fogCurvesPtr.IsNotNullPointer)
            {
                reader.JumpToAddress(fogCurvesPtr);
                reader.ReadX(ref fogCurves, true);
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


            // DESERIALIZE SCENE OBJECT SUB-TYPES WHILE MAINTAINING REFERENCE
            {
                // Keep a dictionary of each shared reference type
                var instanceRefsDict = new Dictionary<Pointer, SceneInstanceReference>();
                var objectRefsDict = new Dictionary<Pointer, SceneObjectReference>();
                var colliderGeoDict = new Dictionary<Pointer, ColliderGeometry>();
                var objNamesDict = new Dictionary<Pointer, CString>();
                //
                var unkDict = new Dictionary<Pointer, UnknownSceneObjectData>();
                var skeletalAnimatorDict = new Dictionary<Pointer, SkeletalAnimator>();
                var animationClipDict = new Dictionary<Pointer, AnimationClip>();

                // PHASE 1
                // Deserialize instances as unique
                foreach (var sceneObject in sceneObjects)
                {
                    GetSerializable(reader, sceneObject.instanceReferencePtr, ref sceneObject.instanceReference, instanceRefsDict);
                    // 3 types below are being tested. Not 100% sure if multiple ptr use - but possible
                    GetSerializable(reader, sceneObject.unkPtr_0x34, ref sceneObject.unk1, unkDict);
                    GetSerializable(reader, sceneObject.skeletalAnimatorPtr, ref sceneObject.skeletalAnimator, skeletalAnimatorDict);
                    GetSerializable(reader, sceneObject.animationPtr, ref sceneObject.animation, animationClipDict);
                }
                foreach (var sceneOriginObject in sceneOriginObjects)
                {
                    GetSerializable(reader, sceneOriginObject.sceneInstanceReferencePtr, ref sceneOriginObject.instanceReference, instanceRefsDict);
                }

                // PHASE 2
                // Deserialize object references / collider geo as unique
                foreach (var instance in instanceRefsDict.Values)
                {
                    GetSerializable(reader, instance.objectReferencePtr, ref instance.objectReference, objectRefsDict);
                    GetSerializable(reader, instance.colliderGeometryPtr, ref instance.colliderGeometry, colliderGeoDict);
                }

                // PHASE 3
                // Serialize object names as unique
                foreach (var sceneObjectReference in objectRefsDict.Values)
                {
                    GetSerializable(reader, sceneObjectReference.namePtr, ref sceneObjectReference.name, objNamesDict);
                }

                // Assign lists to local arrays for easy access and debugging.
                sceneInstances = instanceRefsDict.Values.ToArray();
                sceneObjectReferences = objectRefsDict.Values.ToArray();
                sceneColliderGeometries = colliderGeoDict.Values.ToArray();
                objectNames = objNamesDict.Values.ToArray();
                //
                unkSceneObjData = unkDict.Values.ToArray();
                animationClips = animationClipDict.Values.ToArray();
                skeletalAnimators = skeletalAnimatorDict.Values.ToArray();
            }

            // 
            {
                // Read all ROOT track segments
                var rootTrackSegmentDict = new Dictionary<Pointer, TrackSegment>();
                foreach (var trackNode in trackNodes)
                {
                    GetSerializable(reader, trackNode.segmentPtr, ref trackNode.segment, rootTrackSegmentDict);
                }
                rootTrackSegments = rootTrackSegmentDict.Values.ToArray();

                // Read ALL track segments
                // Master list of all TrackSegments
                var allTrackSegments = new Dictionary<Pointer, TrackSegment>();
                // Start by iterating over each root node in graph
                foreach (var rootSegment in rootTrackSegments)
                {
                    // Read out children fo root, then continue deserializing children recursively
                    // Deserialization method respects ArayPointer order in list.
                    ReadTrackSegmentsRecursive(reader, allTrackSegments, rootSegment);
                }
                this.allTrackSegments = allTrackSegments.Values.ToArray();
            }

            BinaryIoUtility.PopEndianess();
        }

        public void Serialize(BinaryWriter writer)
        {
            BinaryIoUtility.PushEndianess(false);

            // TEMP HACK 
            serializeVerbose = true;

            // Write header
            SerializeHeader(writer);

            // MAINTAIN FILE IDENTIFICATION COMPATIBILITY
            {
                // 0x20
                // Resulting pointer should be 0xE4 or 0xE8 for AX or GX, respectively.
                zeroes0x20 = new byte[kSizeOfZeroes0x20];
                zeroes0x20Ptr = writer.GetPositionAsPointer();
                writer.WriteX(zeroes0x20, false); // TODO: HARD-CODED

                // 0x24
                // Resulting pointer should be 0xF8 or 0xFC for AX or GX, respectively.
                writer.WriteX(trackMinHeight);
                trackMinHeightPtr = trackMinHeight.GetPointer();

                // The pointers written by the last 2 calls should create a valid AX or GX file header.
                // If not, an assert will trigger. Compatibility is important for re-importing scene.
                ValidateFileFormatPointers();
            }
            // Offset pointer address if AX file. Applies to pointers from 0x54 onwards
            int offset = IsFileAX ? -4 : 0;

            // CREDIT / DEBUG INFO / METADATA
            // Write credit and useful debugging info
            writer.CommentDateAndCredits(true);
            writer.Comment("File Information", true);
            writer.CommentLineWide("Format:", Format, true);
            writer.CommentLineWide("Verbose:", serializeVerbose, true);
            writer.CommentLineWide("Universal:", false, true);
            writer.CommentNewLine(true, '-');
            writer.Comment("File name:", true);
            writer.Comment(FileName, true);
            writer.CommentNewLine(true, ' ');
            writer.Comment("Course Name:", true);
            writer.Comment(VenueName, true);
            writer.Comment(courseName, true);
            writer.CommentNewLine(true, ' ');
            writer.Comment("Stage Author(s):", true);
            writer.Comment(author, true);
            writer.CommentNewLine(true, '-');


            // TRACK DATA
            {
                // TODO: consider re-serializing min height
                // Print track length
                writer.InlineDesc(serializeVerbose, 0x90 + offset, trackLength);
                writer.CommentLineWide("Length:", trackLength.value.ToString("0.00"), serializeVerbose);
                writer.CommentNewLine(serializeVerbose, '-');
                writer.WriteX(trackLength);

                // The actual track data
                {
                    // Write each tracknode - stitch of trackpoint and tracksegment
                    writer.InlineDesc(serializeVerbose, 0x0C, trackNodes);
                    writer.WriteX(trackNodes, false);
                    var trackNodesPtr = trackNodes.GetBasePointer();

                    // TRACK CHECKPOINTS
                    {
                        // NOTICE:
                        // Ensure sequential order in ROM for array pointer deserialization

                        var all = new List<TrackCheckpoint>();
                        foreach (var trackNode in trackNodes)
                            foreach (var trackCheckpoint in trackNode.checkpoints)
                                all.Add(trackCheckpoint);
                        var array = all.ToArray();

                        writer.InlineDesc(serializeVerbose, trackNodesPtr, array);
                        writer.WriteX(array, false);
                    }

                    // TRACK SEGMENTS
                    {
                        writer.InlineDesc(serializeVerbose, trackNodesPtr, allTrackSegments);
                        writer.WriteX(allTrackSegments, false);
                        // Manually refresh pointers due to recursive format.
                        foreach (var trackSegment in allTrackSegments)
                            trackSegment.SetChildPointers(allTrackSegments);
                    }

                    // TRACK ANIMATION CURVES
                    {
                        //
                        var allX = new List<TopologyParameters>();
                        foreach (var trackSegment in allTrackSegments)
                            allX.Add(trackSegment.trackAnimationCurves);
                        var allTrackAnimationCurves = allX.ToArray();
                        // Write anim curve ptrs
                        writer.InlineDesc(serializeVerbose, allTrackSegments.GetBasePointer(), allTrackAnimationCurves);
                        writer.WriteX(allTrackAnimationCurves, false);
                        
                        //
                        var listTrackAnimationCurves = new List<AnimationCurve>();
                        foreach (var trackAnimationCurve in allTrackAnimationCurves)
                            foreach (var x in trackAnimationCurve.animationCurves)
                                listTrackAnimationCurves.Add(x);
                        var allAnimationCurves = listTrackAnimationCurves.ToArray();
                        //
                        writer.InlineDesc(serializeVerbose, allTrackAnimationCurves.GetBasePointer(), allAnimationCurves);
                        writer.WriteX(allAnimationCurves, false);

                        //// TOPO
                        //foreach (var trackSegment in allTrackSegments)
                        //{
                        //    var topology = trackSegment.trackAnimationCurves;
                        //    writer.WriteX(topology);
                        //}
                    }

                    // TODO: better type comment
                    writer.InlineDesc(serializeVerbose, new TrackCornerTopology());
                    foreach (var trackSegment in allTrackSegments)
                    {
                        var corner = trackSegment.hairpinCornerTopology;
                        if (corner != null)
                        {
                            writer.WriteX(corner);
                        }
                    }
                }


                // Write track checkpoint indexers
                writer.InlineDesc(serializeVerbose, 0xBC + offset, trackCheckpointMatrix);
                writer.WriteX(trackCheckpointMatrix);
                var trackIndexListPtr = trackCheckpointMatrix.GetPointer();
                // Only write if it has indexes
                writer.InlineDesc(serializeVerbose, trackIndexListPtr, trackCheckpointMatrix.indexLists);
                foreach (var trackIndexList in trackCheckpointMatrix.indexLists)
                {
                    writer.WriteX(trackIndexList);
                }

                //
                writer.InlineDesc(serializeVerbose, 0x14, surfaceAttributeAreas);
                writer.WriteX(surfaceAttributeAreas, false);
            }

            // STATIC COLLIDER MESHES
            {
                // STATIC COLLIDER MESHES
                // Write main structure
                writer.InlineDesc(serializeVerbose, 0x1C, staticColliderMeshes);
                writer.WriteX(staticColliderMeshes);
                var scmPtr = staticColliderMeshes.GetPointer();

                // COLLIDER TRIS
                {
                    var colliderTris = staticColliderMeshes.colliderTriangles;
                    // Write tri data and comment
                    if (!colliderTris.IsNullOrEmpty())
                        writer.InlineDesc(serializeVerbose, scmPtr, colliderTris);
                    writer.WriteX(colliderTris, false);
                    WriteStaticColliderMeshMatrices(writer, scmPtr, "ColiTri", staticColliderMeshes.triMeshIndexMatrices);
                }

                // COLLIDER QUADS
                {
                    var colliderQuads = staticColliderMeshes.colliderQuads;
                    // Write quad data and comment
                    if (!colliderQuads.IsNullOrEmpty())
                        writer.InlineDesc(serializeVerbose, scmPtr, colliderQuads);
                    writer.WriteX(colliderQuads, false);
                    WriteStaticColliderMeshMatrices(writer, scmPtr, "ColiQuad", staticColliderMeshes.quadMeshIndexMatrices);
                }

            }

            // FOG
            {
                // Stage always has fog parameters
                writer.InlineDesc(serializeVerbose, 0x84 + offset, fog);
                writer.WriteX(fog);

                // ... but does not always have associated curves
                if (fogCurves != null)
                {
                    // TODO: assert venue vs fog curves?

                    // Write FogCurves pointers to animation curves...
                    writer.InlineDesc(serializeVerbose, 0x80 + offset, fogCurves);
                    writer.WriteX(fogCurves);
                    var fogCurvesPtr = fogCurves.GetPointer();
                    // ... then write the animation data associated with fog curves
                    writer.InlineDesc(serializeVerbose, fogCurvesPtr, fogCurves.animationCurves);
                    foreach (var curve in fogCurves.animationCurves)
                        writer.WriteX(curve);
                }
            }

            // SCENE OBJECTS
            {
                // SCENE OBJECTS
                // 0x48 (count total), 0x4C, 0x50 (GX exclusive count), 0x54 (pointer address)
                writer.InlineDesc(serializeVerbose, 0x54 + offset, sceneObjects);
                writer.WriteX(sceneObjects, false);

                // SCENE ORIGIN OBJECTS
                if (!sceneOriginObjects.IsNullOrEmpty())
                    writer.InlineDesc(serializeVerbose, 0x70 + offset, sceneOriginObjects);
                writer.WriteX(sceneOriginObjects, false);

                // SCENE INSTANCES
                writer.InlineDesc(serializeVerbose, 0x68 + offset, sceneInstances);
                writer.WriteX(sceneInstances, false);

                // SCENE OBJECT REFERENCES
                writer.InlineDesc(serializeVerbose, sceneObjectReferences);
                writer.WriteX(sceneObjectReferences, false);

                // SCENE OBJECT NAMES
                // No direct pointer. Names are aligned to 4 bytes.
                writer.CommentAlign(serializeVerbose);
                writer.CommentNewLine(serializeVerbose, '-');
                writer.Comment("ObjectNames[]", serializeVerbose, ' ');
                writer.CommentNewLine(serializeVerbose, '-');
                //
                foreach (var objectName in objectNames)
                {
                    writer.WriteX(objectName);
                    writer.AlignTo(4);
                }

                // SCENE OBJECTS TRANSFORMS
                // TODO: better type comments
                writer.InlineDesc(serializeVerbose, new TransformMatrix3x4());
                foreach (var sceneObject in sceneObjects)
                {
                    var sceneObjectMatrix = sceneObject.transformMatrix3x4;
                    if (sceneObjectMatrix != null)
                    {
                        writer.WriteX(sceneObjectMatrix);
                    }
                }

                // SCENE OBJECTS - unknown data
                writer.InlineDesc(serializeVerbose, unkSceneObjData);
                foreach (var unk1 in unkSceneObjData)
                {
                    if (unk1 == null)
                        continue;

                    writer.WriteX(unk1);
                    // TODO: don't inline?
                    foreach (var unk2 in unk1.unk)
                        if (unk2 != null)
                            writer.WriteX(unk2);
                }

                // SCENE OBJECTS - 
                writer.InlineDesc(serializeVerbose, skeletalAnimators);
                foreach (var skeletalAnimator in skeletalAnimators)
                {
                    if (skeletalAnimator == null)
                        continue;

                    writer.WriteX(skeletalAnimator);
                    // TODO: don't inline?
                    writer.WriteX(skeletalAnimator.properties);
                }

                writer.InlineDesc(serializeVerbose, animationClips);
                foreach (var animationClip in animationClips)
                {
                    if (animationClip == null)
                        continue;

                    writer.WriteX(animationClip);
                    // TODO: don't inline?
                    foreach (var curvePlus in animationClip.animationCurvePluses)
                        if (curvePlus != null)
                            writer.WriteX(curvePlus);
                }
            }

            // TRIGGERS
            {
                // ARCADE CHECKPOINT TRIGGERS
                if (!arcadeCheckpointTriggers.IsNullOrEmpty())
                    writer.InlineDesc(serializeVerbose, 0xB0 + offset, arcadeCheckpointTriggers);
                writer.WriteX(arcadeCheckpointTriggers, false);

                // COURSE METADATA TRIGGERS
                if (!courseMetadataTriggers.IsNullOrEmpty())
                    writer.InlineDesc(serializeVerbose, 0xA8 + offset, courseMetadataTriggers);
                writer.WriteX(courseMetadataTriggers, false);

                // STORY OBJECT TRIGGERS
                if (!storyObjectTriggers.IsNullOrEmpty())
                    writer.InlineDesc(serializeVerbose, 0xB8 + offset, storyObjectTriggers);
                writer.WriteX(storyObjectTriggers, false);
                //
                // Get better comment? Get proper pointers?
                if (!storyObjectTriggers.IsNullOrEmpty())
                    writer.InlineDesc(serializeVerbose, new StoryObjectPath());
                foreach (var storyObjectTrigger in storyObjectTriggers)
                {
                    // Optional data
                    var storyObjectPath = storyObjectTrigger.storyObjectPath;
                    if (storyObjectPath != null)
                    {
                        // Uncomment if you want super-inline -- maybe have that as option?
                        //writer.InlineDesc(serializeVerbose, storyObjectTrigger.GetPointer(), storyObjectTrigger);
                        writer.WriteX(storyObjectPath);

                        // Assert is true. This data is always here if existing
                        Assert.IsTrue(storyObjectPath.animationCurve != null);
                        // breaking the rules again. Should inlining be allows for these ptr types?
                        writer.WriteX(storyObjectPath.animationCurve);
                    }
                }

                // UNKNOWN SOLS TRIGGERS
                if (!unknownSolsTriggers.IsNullOrEmpty())
                    writer.InlineDesc(serializeVerbose, 0x60 + offset, unknownSolsTriggers);
                writer.WriteX(unknownSolsTriggers, false);

                // UNKNOWN TRIGGERS
                if (!unknownTriggers.IsNullOrEmpty())
                    writer.InlineDesc(serializeVerbose, 0x94 + offset, unknownTriggers);
                writer.WriteX(unknownTriggers, false);

                // VISUAL EFFECT TRIGGERS
                if (!visualEffectTriggers.IsNullOrEmpty())
                    writer.InlineDesc(serializeVerbose, 0x9C + offset, visualEffectTriggers);
                writer.WriteX(visualEffectTriggers, false);
            }

            // GET ALL REFERERS, RE-SERIALIZE FOR POINTERS
            {
                // Get a reference to EVERY object in file that has a pointer to an object
                var referers = new List<ISerializedBinaryAddressableReferer>();

                // Track Nodes and dependencies
                referers.AddRange(trackNodes);
                referers.AddRange(allTrackSegments);
                foreach (var trackSegment in allTrackSegments)
                    referers.Add(trackSegment.trackAnimationCurves);
                // The checkpoint table
                referers.Add(trackCheckpointMatrix);

                // Static Collider Meshes and dependencies
                referers.Add(staticColliderMeshes);
                referers.AddRange(staticColliderMeshes.triMeshIndexMatrices);
                referers.AddRange(staticColliderMeshes.quadMeshIndexMatrices);

                // OBJECTS
                referers.AddRange(sceneObjectReferences);
                referers.AddRange(sceneInstances);
                referers.AddRange(sceneColliderGeometries);
                referers.AddRange(sceneOriginObjects);
                referers.AddRange(sceneObjects);
                //
                referers.AddRange(unkSceneObjData);
                referers.AddRange(skeletalAnimators);
                foreach (var anim in animationClips)
                    if (!anim.animationCurvePluses.IsNullOrEmpty())
                        referers.AddRange(anim.animationCurvePluses);

                // FOG
                // The structure points to 6 anim curves
                referers.Add(fogCurves);

                // The story mode checkpoints
                foreach (var storyObjectTrigger in storyObjectTriggers)
                {
                    referers.Add(storyObjectTrigger);
                    referers.Add(storyObjectTrigger.storyObjectPath);
                }

                // RE-SERIALIZE
                // Patch pointers by re-writing structure in same place as previously serialized
                foreach (var referer in referers)
                {
                    var pointer = referer.GetPointer();
                    if (pointer.IsNotNullPointer)
                    {
                        writer.JumpToAddress(pointer);
                        referer.Serialize(writer);
                        // Run assertions on referer to ensure pointer requirements are met
                        referer.ValidateReferences();
                    }
                }
            } // end patching pointers

            // RE-WRITE ColiScene HEADER TO RESERIALIZE POINTERS
            // Rewrite main block pointers
            writer.JumpToAddress(0);
            SerializeHeader(writer);
            // Validate this structure before finishing.
            ValidateReferences();

            BinaryIoUtility.PopEndianess();
        }

        /// <summary>
        /// Writes out a StaticColliderMeshMatrix[] with loads of comments.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="refPtr"></param>
        /// <param name="id"></param>
        /// <param name="staticColliderMeshMatrix"></param>
        public void WriteStaticColliderMeshMatrices(BinaryWriter writer, Pointer refPtr, string id, StaticColliderMeshMatrix[] staticColliderMeshMatrix)
        {
            // GX COUNT: 14. "Pointer table" of 256 ptrs PER item.
            var nMatrices = staticColliderMeshMatrix.Length;
            const int w = 2; //

            for (int i = 0; i < nMatrices; i++)
            {
                var indexMatrix = staticColliderMeshMatrix[i];
                var type = (StaticColliderMeshProperty)(i);

                // Cannot be null
                Assert.IsTrue(indexMatrix != null);

                // Write extra-helpful comment.
                writer.CommentAlign(serializeVerbose, ' ');
                writer.CommentNewLine(serializeVerbose, '-');
                writer.CommentType(indexMatrix, serializeVerbose);
                writer.CommentPtr(refPtr, serializeVerbose);
                writer.CommentNewLine(serializeVerbose, '-');
                writer.CommentLineWide("Owner:", id, serializeVerbose);
                writer.CommentLineWide("Index:", $"[{i,w}/{nMatrices}]", serializeVerbose);
                writer.CommentLineWide("Type:", type, serializeVerbose);
                writer.CommentLineWide("Mtx:", $"x:{indexMatrix.SubdivisionsX,2}, z:{indexMatrix.SubdivisionsZ,2}", serializeVerbose);
                writer.CommentLineWide("MtxCnt:", indexMatrix.Count, serializeVerbose);
                writer.CommentNewLine(serializeVerbose, '-');
                //
                writer.WriteX(indexMatrix);
                var qmiPtr = indexMatrix.GetPointer();

                if (indexMatrix.HasIndexes)
                {
                    // 256 lists
                    writer.CommentAlign(serializeVerbose, ' ');
                    writer.CommentNewLine(serializeVerbose, '-');
                    writer.Comment($"{nameof(IndexList)}[{i,w}/{nMatrices}]", serializeVerbose);
                    writer.CommentPtr(qmiPtr, serializeVerbose);
                    writer.CommentLineWide("Type:", type, serializeVerbose);
                    writer.CommentLineWide("Owner:", id, serializeVerbose);
                    writer.CommentNewLine(serializeVerbose, '-');
                    for (int index = 0; index < indexMatrix.indexLists.Length; index++)
                        if (indexMatrix.indexLists[index].Length > 0)
                            writer.CommentIdx(index, serializeVerbose);
                    writer.CommentNewLine(serializeVerbose, '-');
                    for (int index = 0; index < indexMatrix.indexLists.Length; index++)
                    {
                        var quadIndexList = indexMatrix.indexLists[index];
                        writer.WriteX(quadIndexList);
                    }
                }
            }
        }

        public void ReadTrackSegmentsRecursive(BinaryReader reader, Dictionary<Pointer, TrackSegment> allTrackSegments, TrackSegment parent)
        {
            // Add parent node to master list
            var parentPtr = parent.GetPointer();
            if (!allTrackSegments.ContainsKey(parentPtr))
                allTrackSegments.Add(parentPtr, parent);

            // Deserialize children (if any)
            var children = parent.GetChildren(reader);
            var requiresDeserialization = new List<TrackSegment>();

            // Get the index of where these children WILL be in list
            var childIndexes = new int[children.Length];
            for (int i = 0; i < childIndexes.Length; i++)
            {
                var child = children[i];
                var childPtr = child.GetPointer();

                var listContainsChild = allTrackSegments.ContainsKey(childPtr);
                if (listContainsChild)
                {
                    // Child is in master list, get index
                    for (int j = 0; j < allTrackSegments.Count; j++)
                    {
                        if (allTrackSegments[j].GetPointer() == childPtr)
                        {
                            childIndexes[i] = j;
                            break;
                        }
                    }
                }
                else
                {
                    // Add child to master list
                    // child index is COUNT (oob), add to list (is now in array bounds)
                    childIndexes[i] = allTrackSegments.Count;
                    allTrackSegments.Add(childPtr, child);
                    requiresDeserialization.Add(child);
                }
            }

            // Set indexes of children to 
            parent.childIndexes = childIndexes;

            // Read children recursively (if any).
            foreach (var child in requiresDeserialization)
            {
                ReadTrackSegmentsRecursive(reader, allTrackSegments, child);
            }
        }


        public void DeserializeHeader(BinaryReader reader)
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

        public void SerializeHeader(BinaryWriter writer)
        {
            {
                // Refresh metadata
                isFileAX = Format == SerializeFormat.AX;
                isFileGX = Format == SerializeFormat.GX;
                Assert.IsFalse(Format == SerializeFormat.InvalidFormat);

                // UPDATE POINTERS AND COUNTS
                // Track and stage data
                staticColliderMeshesPtr = staticColliderMeshes.GetPointer();
                surfaceAttributeAreasPtr = surfaceAttributeAreas.GetArrayPointer();
                trackCheckpointMatrixPtr = trackCheckpointMatrix.GetPointer();
                trackLengthPtr = trackLength.GetPointer();
                trackMinHeightPtr = trackMinHeight.GetPointer();
                trackNodesPtr = trackNodes.GetArrayPointer();
                fogPtr = fog.GetPointer();
                fogCurvesPtr = fogCurves.GetPointer();
                // TRIGGERS
                arcadeCheckpointTriggersPtr = arcadeCheckpointTriggers.GetArrayPointer();
                courseMetadataTriggersPtr = courseMetadataTriggers.GetArrayPointer();
                storyObjectTriggersPtr = storyObjectTriggers.GetArrayPointer();
                unknownSolsTriggersPtr = unknownSolsTriggers.GetArrayPointer();
                unknownTriggersPtr = unknownTriggers.GetArrayPointer();
                visualEffectTriggersPtr = visualEffectTriggers.GetArrayPointer();
                // SCENE OBJECTS
                // References
                sceneInstancesPtr = sceneInstances.GetArrayPointer();
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

            for (int i = 0; i < zeroes0x20.Length; i++)
                Assert.IsTrue(zeroes0x20[i] == 0);
            for (int i = 0; i < zeroes0x28.Length; i++)
                Assert.IsTrue(zeroes0x28[i] == 0);
            for (int i = 0; i < zeroes0xD8.Length; i++)
                Assert.IsTrue(zeroes0xD8[i] == 0);

            // Assert all pointers which are never null in game files
            ////Assert.IsTrue(trackNodesPtr.IsNotNullPointer);
            Assert.IsTrue(surfaceAttributeAreasPtr.IsNotNullPointer);
            Assert.IsTrue(staticColliderMeshesPtr.IsNotNullPointer);
            Assert.IsTrue(zeroes0x20Ptr.IsNotNullPointer);
            Assert.IsTrue(trackMinHeightPtr.IsNotNullPointer);
            ////Assert.IsTrue(sceneObjectsPtr.IsNotNullPointer);
            ////Assert.IsTrue(sceneInstancesPtr.IsNotNullPointer);
            Assert.IsTrue(fogPtr.IsNotNullPointer);
            Assert.IsTrue(trackCheckpointMatrixPtr.IsNotNullPointer);

            if (!sceneOriginObjects.IsNullOrEmpty())
                Assert.IsTrue(sceneOriginObjectsPtr.IsNotNullPointer);

            // Assert pointers which can be null IF the reference type is not null
            if (fogCurves != null)
                Assert.IsTrue(fogCurvesPtr.IsNotNullPointer);
            // triggers
            if (!unknownSolsTriggers.IsNullOrEmpty())
                Assert.IsTrue(unknownSolsTriggersPtr.IsNotNullPointer);
            if (!unknownTriggers.IsNullOrEmpty())
                Assert.IsTrue(unknownTriggersPtr.IsNotNullPointer);
            if (!visualEffectTriggers.IsNullOrEmpty())
                Assert.IsTrue(visualEffectTriggersPtr.IsNotNullPointer);
            if (!courseMetadataTriggers.IsNullOrEmpty())
                Assert.IsTrue(courseMetadataTriggersPtr.IsNotNullPointer);
            if (!arcadeCheckpointTriggers.IsNullOrEmpty())
                Assert.IsTrue(arcadeCheckpointTriggersPtr.IsNotNullPointer);
            if (!storyObjectTriggers.IsNullOrEmpty())
                Assert.IsTrue(storyObjectTriggersPtr.IsNotNullPointer);
        }

        public static void GetSerializable<T>(BinaryReader reader, Pointer ptr, ref T reference, Dictionary<Pointer, T> dict)
            where T : class, IBinarySerializable, new()
        {
            // If ptr is null, set reference to null, return
            if (!ptr.IsNotNullPointer)
            {
                reference = null;
                return;
            }

            // If we have have this reference, return it
            if (dict.ContainsKey(ptr))
            {
                reference = dict[ptr];
            }
            // If we don't have this reference, deserialize it, store in dict, return it
            else
            {
                reader.JumpToAddress(ptr);
                reader.ReadX(ref reference, true);
                dict.Add(ptr, reference);
            }
        }

    }
}
