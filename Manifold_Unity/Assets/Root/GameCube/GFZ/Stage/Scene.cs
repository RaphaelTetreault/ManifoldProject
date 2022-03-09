using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// The data structure representing a scene in F-Zero AX/GX. It contains all the necessary
    /// data for object placement, object animations, fog, collision data for track and surfaces,
    /// track metadata, and triggers for mission, visual effects, etc.
    /// </summary>
    [Serializable]
    public class Scene :
        IBinaryAddressable,
        IBinarySerializable,
        IFileType,
        IHasReference,
        ITextPrintable
    {
        // CONSTANTS
        public const int kSizeOfZeroes0x20 = 0x14; // 20
        public const int kSizeOfZeroes0x28 = 0x20; // 32
        public const int kSizeOfZeroes0xD8 = 0x10; // 16
        public const int kAxConstPtr0x20 = 0xE4;
        public const int kAxConstPtr0x24 = 0xF8;
        public const int kGxConstPtr0x20 = 0xE8;
        public const int kGxConstPtr0x24 = 0xFC;


        // FIELDS
        private ViewRange unkRange0x00;
        private ArrayPointer trackNodesPtr;
        private ArrayPointer embeddedTrackPropertyAreasPtr;
        // 2022-01-14: this is a bool, the game loads the following structure regardless
        // (because it'd crash anways because of it when unloading)
        private Bool32 staticColliderMeshManagerActive = Bool32.True;
        private Pointer staticColliderMeshManagerPtr;
        private Pointer zeroes0x20Ptr; // GX: 0xE8, AX: 0xE4
        private Pointer trackMinHeightPtr; // GX: 0xFC, AX: 0xF8
        private byte[] zeroes0x28 = new byte[kSizeOfZeroes0x28];
        private int dynamicSceneObjectCount;
        private int unk_sceneObjectCount1;
        private int unk_sceneObjectCount2; // GX exclusive
        private Pointer dynamicSceneObjectsPtr;
        private Bool32 unkBool32_0x58 = Bool32.True;
        private ArrayPointer unknownCollidersPtr;
        private ArrayPointer sceneObjectsPtr;
        private ArrayPointer staticSceneObjectsPtr;
        private int zero0x74; // Ptr? Array Ptr length?
        private int zero0x78; // Ptr? Array Ptr address?
        private CircuitType circuitType = CircuitType.ClosedCircuit;
        private Pointer fogCurvesPtr;
        private Pointer fogPtr;
        private int zero0x88; // Ptr? Array Ptr length?
        private int zero0x8C; // Ptr? Array Ptr address?
        private Pointer trackLengthPtr;
        private ArrayPointer unknownTriggersPtr;
        private ArrayPointer visualEffectTriggersPtr;
        private ArrayPointer miscellaneousTriggersPtr;
        private ArrayPointer timeExtensionTriggersPtr;
        private ArrayPointer storyObjectTriggersPtr;
        private Pointer checkpointGridPtr;
        private GridXZ checkpointGridXZ;
        private byte[] zeroes0xD8 = new byte[kSizeOfZeroes0xD8];
        // REFERENCE FIELDS
        public TrackNode[] trackNodes;
        public EmbeddedTrackPropertyArea[] embeddedPropertyAreas;
        public StaticColliderMeshManager staticColliderMeshManager;
        public byte[] zeroes0x20 = new byte[kSizeOfZeroes0x20];
        public TrackMinHeight trackMinHeight;
        public SceneObjectDynamic[] dynamicSceneObjects;
        public SceneObject[] sceneObjects;
        public SceneObjectStatic[] staticSceneObjects;
        public UnknownCollider[] unknownColliders;
        public FogCurves fogCurves;
        public Fog fog;
        public TrackLength trackLength;
        public CullOverrideTrigger[] cullOverrideTriggers;
        public VisualEffectTrigger[] visualEffectTriggers;
        public MiscellaneousTrigger[] miscellaneousTriggers;
        public TimeExtensionTrigger[] timeExtensionTriggers;
        public StoryObjectTrigger[] storyObjectTriggers;
        public TrackCheckpointGrid trackCheckpointGrid;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }

        /// <summary>
        /// The course's author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The course's name.
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// The file extension for Scene (COLI_COURSE##). There is none.
        /// </summary>
        public string FileExtension => "";

        /// <summary>
        /// The Scene's file name. 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// How large the file is in bytes.
        /// </summary>
        public int FileSize { get; private set; }

        /// <summary>
        /// The serialization format to use on Serialize().
        /// </summary>
        public SerializeFormat Format { get; set; }

        /// <summary>
        /// The course index as indicated by the file name COLI_COURSE## where ## is the index.
        /// </summary>
        public int CourseIndex { get; private set; }

        /// <summary>
        /// Returns true if this Scene is in F-Zero AX format.
        /// </summary>
        public bool IsFileAX { get; private set; }

        /// <summary>
        /// Returns true if this Scene is in F-Zero GX format.
        /// </summary>
        public bool IsFileGX { get; private set; }

        /// <summary>
        /// Returns true if file is properly tagged as either AX or GX (mutually exclusive).
        /// </summary>
        public bool IsValidFile => IsFileAX ^ IsFileGX;

        /// <summary>
        /// If set to true, serialization prints text inlined into the resulting binary output file.
        /// </summary>
        public bool SerializeVerbose { get; set; }
        
        /// <summary>
        /// The venue for this course.
        /// </summary>
        public Venue Venue { get; set; }

        /// <summary>
        /// Gets the venue's name
        /// </summary>
        public string VenueName => EnumExtensions.GetDescription(Venue);

        /// <summary>
        /// An array of all the track segments in this scene.
        /// </summary>
        public TrackSegment[] AllTrackSegments { get; set; }

        /// <summary>
        /// An array of the only the root track segments in this scene. 
        /// </summary>
        public TrackSegment[] RootTrackSegments { get; set; }
        
        /// <summary>
        /// An array of all the scene object names.
        /// </summary>
        public ShiftJisCString[] SceneObjectNames { get; set; }
        
        /// <summary>
        /// An array of all the scene objecy LODs.
        /// </summary>
        public SceneObjectLOD[] SceneObjectLODs { get; set; }


        public ViewRange UnkRange0x00 { get => unkRange0x00; set => unkRange0x00 = value; }
        public ArrayPointer TrackNodesPtr { get => trackNodesPtr; set => trackNodesPtr = value; }
        public ArrayPointer EmbeddedTrackPropertyAreasPtr { get => embeddedTrackPropertyAreasPtr; set => embeddedTrackPropertyAreasPtr = value; }
        public Bool32 StaticColliderMeshManagerActive { get => staticColliderMeshManagerActive; set => staticColliderMeshManagerActive = value; }
        public Pointer StaticColliderMeshManagerPtr { get => staticColliderMeshManagerPtr; set => staticColliderMeshManagerPtr = value; }
        public Pointer TrackMinHeightPtr { get => trackMinHeightPtr; set => trackMinHeightPtr = value; }
        public int DynamicSceneObjectCount { get => dynamicSceneObjectCount; set => dynamicSceneObjectCount = value; }
        public int Unk_sceneObjectCount1 { get => unk_sceneObjectCount1; set => unk_sceneObjectCount1 = value; }
        public int Unk_sceneObjectCount2 { get => unk_sceneObjectCount2; set => unk_sceneObjectCount2 = value; }
        public Pointer DynamicSceneObjectsPtr { get => dynamicSceneObjectsPtr; set => dynamicSceneObjectsPtr = value; }
        public Bool32 UnkBool32_0x58 { get => unkBool32_0x58; set => unkBool32_0x58 = value; }
        public ArrayPointer UnknownCollidersPtr { get => unknownCollidersPtr; set => unknownCollidersPtr = value; }
        public ArrayPointer SceneObjectsPtr { get => sceneObjectsPtr; set => sceneObjectsPtr = value; }
        public ArrayPointer StaticSceneObjectsPtr { get => staticSceneObjectsPtr; set => staticSceneObjectsPtr = value; }
        public CircuitType CircuitType { get => circuitType; set => circuitType = value; }
        public Pointer FogCurvesPtr { get => fogCurvesPtr; set => fogCurvesPtr = value; }
        public Pointer FogPtr { get => fogPtr; set => fogPtr = value; }
        public Pointer TrackLengthPtr { get => trackLengthPtr; set => trackLengthPtr = value; }
        public ArrayPointer UnknownTriggersPtr { get => unknownTriggersPtr; set => unknownTriggersPtr = value; }
        public ArrayPointer VisualEffectTriggersPtr { get => visualEffectTriggersPtr; set => visualEffectTriggersPtr = value; }
        public ArrayPointer MiscellaneousTriggersPtr { get => miscellaneousTriggersPtr; set => miscellaneousTriggersPtr = value; }
        public ArrayPointer TimeExtensionTriggersPtr { get => timeExtensionTriggersPtr; set => timeExtensionTriggersPtr = value; }
        public ArrayPointer StoryObjectTriggersPtr { get => storyObjectTriggersPtr; set => storyObjectTriggersPtr = value; }
        public Pointer CheckpointGridPtr { get => checkpointGridPtr; set => checkpointGridPtr = value; }
        public GridXZ CheckpointGridXZ { get => checkpointGridXZ; set => checkpointGridXZ = value; }



        public static bool IsAX(Pointer ptr0x20, Pointer ptr0x24)
        {
            bool isAx0x20 = ptr0x20.Address == kAxConstPtr0x20;
            bool isAx0x24 = ptr0x24.Address == kAxConstPtr0x24;
            bool isAX = isAx0x20 & isAx0x24;
            return isAX;
        }

        public static bool IsGX(Pointer ptr0x20, Pointer ptr0x24)
        {
            bool isGx0x20 = ptr0x20.Address == kGxConstPtr0x20;
            bool isGx0x24 = ptr0x24.Address == kGxConstPtr0x24;
            bool isGX = isGx0x20 & isGx0x24;
            return isGX;
        }

        public void ValidateFileFormatPointers()
        {
            IsFileAX = IsAX(zeroes0x20Ptr, TrackMinHeightPtr);
            IsFileGX = IsGX(zeroes0x20Ptr, TrackMinHeightPtr);
            Assert.IsTrue(IsValidFile);
        }

        public void Deserialize(BinaryReader reader)
        {
            BinaryIoUtility.PushEndianness(Endianness.BigEndian);

            // CAPTURE METADATA
            FileSize = (int)reader.BaseStream.Length;
            // Store the stage index, can solve venue and course name from this using hashes
            var matchDigits = Regex.Match(FileName, Const.Regex.MatchIntegers);
            CourseIndex = int.Parse(matchDigits.Value);

            // Read COLI_COURSE## file header
            DeserializeHeader(reader);

            // DESERIALIZE REFERENCE TYPES
            // All types below are beyond the inital header and have pointers to them
            // specified in the header. While deserialization could be in any order, 
            // the following is order as they appear in the header.

            // 0x08 and 0x0C: Track Nodes
            reader.JumpToAddress(TrackNodesPtr);
            reader.ReadX(ref trackNodes, TrackNodesPtr.Length);

            // 0x10 and 0x14: Track Effect Attribute Areas
            reader.JumpToAddress(EmbeddedTrackPropertyAreasPtr);
            reader.ReadX(ref embeddedPropertyAreas, EmbeddedTrackPropertyAreasPtr.Length);

            // 0x1C 
            reader.JumpToAddress(StaticColliderMeshManagerPtr);
            // The structure's size differs between AX and GX. Format defines which it uses.
            staticColliderMeshManager = new StaticColliderMeshManager(Format);
            staticColliderMeshManager.Deserialize(reader);

            // 0x20
            reader.JumpToAddress(zeroes0x20Ptr);
            reader.ReadX(ref zeroes0x20, kSizeOfZeroes0x20);

            // 0x24
            reader.JumpToAddress(TrackMinHeightPtr);
            reader.ReadX(ref trackMinHeight);

            // 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address)
            reader.JumpToAddress(DynamicSceneObjectsPtr);
            reader.ReadX(ref dynamicSceneObjects, DynamicSceneObjectCount);

            // 0x5C and 0x60 SOLS values
            reader.JumpToAddress(UnknownCollidersPtr);
            reader.ReadX(ref unknownColliders, UnknownCollidersPtr.Length);

            // 0x64 and 0x68
            reader.JumpToAddress(SceneObjectsPtr);
            reader.ReadX(ref sceneObjects, SceneObjectsPtr.Length);

            // 0x6C and 0x70
            reader.JumpToAddress(StaticSceneObjectsPtr);
            reader.ReadX(ref staticSceneObjects, StaticSceneObjectsPtr.Length);

            // 0x80
            // Data is optional
            if (FogCurvesPtr.IsNotNull)
            {
                reader.JumpToAddress(FogCurvesPtr);
                reader.ReadX(ref fogCurves);
            }

            // 0x84
            reader.JumpToAddress(FogPtr);
            reader.ReadX(ref fog);

            // 0x90 
            reader.JumpToAddress(TrackLengthPtr);
            reader.ReadX(ref trackLength);

            // 0x94 and 0x98
            reader.JumpToAddress(UnknownTriggersPtr);
            reader.ReadX(ref cullOverrideTriggers, UnknownTriggersPtr.Length);

            // 0x9C and 0xA0
            reader.JumpToAddress(VisualEffectTriggersPtr);
            reader.ReadX(ref visualEffectTriggers, VisualEffectTriggersPtr.Length);

            // 0xA4 and 0xA8
            reader.JumpToAddress(MiscellaneousTriggersPtr);
            reader.ReadX(ref miscellaneousTriggers, MiscellaneousTriggersPtr.Length);

            // 0xAC and 0xB0
            reader.JumpToAddress(TimeExtensionTriggersPtr);
            reader.ReadX(ref timeExtensionTriggers, TimeExtensionTriggersPtr.Length);

            // 0xB4 and 0xB8
            reader.JumpToAddress(StoryObjectTriggersPtr);
            reader.ReadX(ref storyObjectTriggers, StoryObjectTriggersPtr.Length);

            // 0xBC and 0xC0
            reader.JumpToAddress(CheckpointGridPtr);
            reader.ReadX(ref trackCheckpointGrid);

            // StaticColliderMeshManager shares references to these types. Rather than deserialize
            // twice, simple rebind the shared link here. Assertion proves this is always ther case.
            staticColliderMeshManager.UnknownColliders = unknownColliders;
            staticColliderMeshManager.StaticSceneObjects = staticSceneObjects;
            Assert.IsTrue(staticColliderMeshManager.UnknownCollidersPtr == UnknownCollidersPtr);
            Assert.IsTrue(staticColliderMeshManager.StaticSceneObjectsPtr == StaticSceneObjectsPtr);


            // UNMANGLE SHARED REFERENCES
            {
                /*/
                    Problem
                    
                    Due to how the scene data is configured, there are a few data types which share
                    references. This is smart and efficient for memory, but it means simple/traditional
                    deserialization fails. The way most all data types are deserialized in this project
                    is done in linear fashion. X1 deserializes itself. If X1 references Y, it deserializes Y.
                    The issue is that if we have an X2 and it also references Y, what happens is that both
                    X1 and X2 create their own instance of Y. The issue surfaces once the data goes to
                    serialize and X1 and X2 serialize their own version of the data.

                    The solution here is to let X1 and X2 deserialize separate instances of Y. We can then
                    use the pointer address of those references and create a dictionary indexed by the pointer
                    address of the data. If we come across another structure that has the same pointer as
                    a previous structure, we can assign the same reference to it. This relinks shared data
                    types in memory here in C# land.
                /*/

                // Keep a dictionary of each shared reference type
                var sceneObjectsDict = new Dictionary<Pointer, SceneObject>();
                var sceneObjectNamesDict = new Dictionary<Pointer, ShiftJisCString>();

                // Get all unique instances of SceneObjects
                // NOTE: instances can share the same name/model but have different properties.
                foreach (var staticSceneObject in staticSceneObjects)
                {
                    var sceneObject = GetSharedSerializable(reader, staticSceneObject.SceneObjectPtr, sceneObjectsDict);
                    staticSceneObject.SceneObject = sceneObject;
                }
                foreach (var dynamicSceneObject in dynamicSceneObjects)
                {
                    var sceneObject = GetSharedSerializable(reader, dynamicSceneObject.SceneObjectPtr, sceneObjectsDict);
                    dynamicSceneObject.SceneObject = sceneObject;
                }
                foreach (var unknownCollider in unknownColliders)
                {
                    var sceneObject = GetSharedSerializable(reader, unknownCollider.SceneObjectPtr, sceneObjectsDict);
                    unknownCollider.SceneObject = sceneObject;
                }
                // Save, order by address
                sceneObjects = sceneObjectsDict.Values.ToArray();
                sceneObjects = sceneObjects.OrderBy(x => x.AddressRange.StartAddress).ToArray();

                // TEMP DEBUGGING
                // Copy over the LOD instances into it's own array
                {
                    var sceneObjectLODs = new List<SceneObjectLOD>();
                    for (int i = 0; i < sceneObjects.Length; i++)
                    {
                        var sceneObject = sceneObjects[i];
                        for (int j = 0; j < sceneObject.LODs.Length; j++)
                        {
                            sceneObjectLODs.Add(sceneObject.LODs[j]);
                        }
                    }
                    this.SceneObjectLODs = sceneObjectLODs.OrderBy(x => x.AddressRange.StartAddress).ToArray();
                }

                // Get all unique name instances
                // NOTE: since instances can use the same name/model, there is occasionally a few duplicate names.
                foreach (var sceneObject in sceneObjects)
                {
                    foreach (var lod in sceneObject.LODs)
                    {
                        var name = GetSharedSerializable(reader, lod.LodNamePtr, sceneObjectNamesDict);
                        lod.Name = name;
                    }
                }
                // Save, order by name (alphabetical)
                SceneObjectNames = sceneObjectNamesDict.Values.ToArray();
                SceneObjectNames = SceneObjectNames.OrderBy(x => x.value).ToArray();
            }

            // DESERIALIZE TRACK SEGMENTS
            {
                /*/
                Unlike any other data type in the scene structure, TrackSegments are referenced
                by many other instances. A single track can have about a dozen track segments
                total, with perhaps a single root segment, referenced by hundreds TrackNodes.
                If we were to let each TrackNode deserialize it's own recursive tree, it would
                take quite a long time for redundant data.

                The approach taken here is to not let the data type itself deserialize the
                TrackSegment. Instead, it deserializes the pointers only. We can then, after
                deserializing all TrackNodes, find all unique instances and recursively
                deserialize them only once, sharing the C# reference afterwards.
                /*/

                // ROOT TRACK SEGMENTS
                // These root segments are the ones pointed at by all nodes.
                var rootTrackSegmentDict = new Dictionary<Pointer, TrackSegment>();
                foreach (var trackNode in trackNodes)
                {
                    var segment = GetSharedSerializable(reader, trackNode.SegmentPtr, rootTrackSegmentDict);
                    trackNode.Segment = segment;
                }
                RootTrackSegments = rootTrackSegmentDict.Values.ToArray();

                // ALL TRACK SEGMENTS
                // Use helper function to collect all TrackSegments
                var allTrackSegmentsList = new List<TrackSegment>();
                foreach (var rootSegment in RootTrackSegments)
                {
                    var rootSegmentHierarchy = rootSegment.GetGraphSerializableOrder();
                    allTrackSegmentsList.AddRange(rootSegmentHierarchy);
                }
                AllTrackSegments = allTrackSegmentsList.ToArray();
            }

            BinaryIoUtility.PopEndianness();
        }


        public void Serialize(BinaryWriter writer)
        {
            BinaryIoUtility.PushEndianness(Endianness.BigEndian);

            // Write header. At first, pointers will be null or broken.
            SerializeHeader(writer);

            // MAINTAIN FILE IDENTIFICATION COMPATIBILITY
            {
                // 0x20
                // Resulting pointer should be 0xE4 or 0xE8 for AX or GX, respectively.
                zeroes0x20 = new byte[kSizeOfZeroes0x20];
                zeroes0x20Ptr = writer.GetPositionAsPointer();
                writer.WriteX(zeroes0x20); // TODO: HARD-CODED

                // 0x24
                // Resulting pointer should be 0xF8 or 0xFC for AX or GX, respectively.
                writer.WriteX(trackMinHeight);
                TrackMinHeightPtr = trackMinHeight.GetPointer();

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
            writer.CommentLineWide("Verbose:", SerializeVerbose, true);
            writer.CommentLineWide("Universal:", false, true);
            writer.CommentNewLine(true, '-');
            writer.Comment("File name:", true);
            writer.Comment(FileName, true);
            writer.CommentNewLine(true, ' ');
            writer.Comment("Course Name:", true);
            writer.Comment(VenueName, true);
            writer.Comment(CourseName, true);
            writer.CommentNewLine(true, ' ');
            writer.Comment("Stage Author(s):", true);
            writer.Comment(Author, true);
            writer.CommentNewLine(true, '-');


            // TRACK DATA
            {
                // TODO: consider re-serializing min height

                // Print track length
                writer.InlineDesc(SerializeVerbose, 0x90 + offset, trackLength);
                writer.CommentLineWide("Length:", trackLength.Value.ToString("0.00"), SerializeVerbose);
                writer.CommentNewLine(SerializeVerbose, '-');
                writer.WriteX(trackLength);

                // The actual track data
                {
                    // Write each tracknode - stitch of trackpoint and tracksegment
                    writer.InlineDesc(SerializeVerbose, 0x0C, trackNodes);
                    writer.WriteX(trackNodes);
                    var trackNodesPtr = trackNodes.GetBasePointer();

                    // TRACK CHECKPOINTS
                    {
                        // NOTICE:
                        // All Checkpoints for each TrackNode must be sequential (branches 0-4).
                        // Sequential order in ROM is required for array pointer deserialization.
                        var typeTemp = new Checkpoint[0]; // TODO: remove need for this
                        writer.InlineDesc(SerializeVerbose, trackNodesPtr, typeTemp);
                        foreach (var trackNode in trackNodes)
                            writer.WriteX(trackNode.Checkpoints);
                    }

                    // TRACK SEGMENTS
                    {
                        writer.InlineDesc(SerializeVerbose, trackNodesPtr, AllTrackSegments);
                        writer.WriteX(AllTrackSegments);
                    }

                    // TRACK ANIMATION CURVES
                    {
                        // Construct list of all track curves (sets of 9 ptrs)
                        var listTrackCurves = new List<AnimationCurveTRS>();
                        foreach (var trackSegment in AllTrackSegments)
                            listTrackCurves.Add(trackSegment.AnimationCurveTRS);
                        var allTrackCurves = listTrackCurves.ToArray();
                        // Write anim curve ptrs
                        writer.InlineDesc(SerializeVerbose, AllTrackSegments.GetBasePointer(), allTrackCurves);
                        writer.WriteX(allTrackCurves);

                        // Construct list of all /animation curves/ (breakout from track structure)
                        var listAnimationCurves = new List<AnimationCurve>();
                        foreach (var trackAnimationCurve in allTrackCurves)
                            foreach (var animationCurve in trackAnimationCurve.AnimationCurves)
                                listAnimationCurves.Add(animationCurve);
                        var allAnimationCurves = listAnimationCurves.ToArray();
                        //
                        writer.InlineDesc(SerializeVerbose, allTrackCurves.GetBasePointer(), allAnimationCurves);
                        writer.WriteX(allAnimationCurves);
                    }

                    // TODO: better type comment
                    writer.InlineDesc(SerializeVerbose, new TrackCorner());
                    foreach (var trackSegment in AllTrackSegments)
                    {
                        var corner = trackSegment.TrackCorner;
                        if (corner != null)
                        {
                            writer.WriteX(corner);
                        }
                    }
                }


                // Write track checkpoint indexers
                writer.InlineDesc(SerializeVerbose, 0xBC + offset, trackCheckpointGrid);
                writer.WriteX(trackCheckpointGrid);
                var trackIndexListPtr = trackCheckpointGrid.GetPointer();
                // Only write if it has indexes
                writer.InlineDesc(SerializeVerbose, trackIndexListPtr, trackCheckpointGrid.IndexLists);
                foreach (var trackIndexList in trackCheckpointGrid.IndexLists)
                {
                    writer.WriteX(trackIndexList);
                }

                //
                writer.InlineDesc(SerializeVerbose, 0x14, embeddedPropertyAreas);
                writer.WriteX(embeddedPropertyAreas);
            }

            // STATIC COLLIDER MESHES
            {
                // STATIC COLLIDER MESHES
                // Write main structure
                writer.InlineDesc(SerializeVerbose, 0x1C, staticColliderMeshManager);
                writer.WriteX(staticColliderMeshManager);
                var scmPtr = staticColliderMeshManager.GetPointer();

                // Write collider bounds (applies to to non-tri/quad collision, too)
                writer.InlineDesc(SerializeVerbose, staticColliderMeshManager.BoundingSphere);
                writer.WriteX(staticColliderMeshManager.BoundingSphere);

                // COLLIDER TRIS
                {
                    var colliderTris = staticColliderMeshManager.ColliderTris;
                    // Write tri data and comment
                    if (!colliderTris.IsNullOrEmpty())
                        writer.InlineDesc(SerializeVerbose, scmPtr, colliderTris);
                    writer.WriteX(colliderTris);
                    WriteStaticColliderMeshMatrices(writer, scmPtr, "ColiTri", staticColliderMeshManager.TriMeshGrids);
                }

                // COLLIDER QUADS
                {
                    var colliderQuads = staticColliderMeshManager.ColliderQuads;
                    // Write quad data and comment
                    if (!colliderQuads.IsNullOrEmpty())
                        writer.InlineDesc(SerializeVerbose, scmPtr, colliderQuads);
                    writer.WriteX(colliderQuads);
                    WriteStaticColliderMeshMatrices(writer, scmPtr, "ColiQuad", staticColliderMeshManager.QuadMeshGrids);
                }
            }

            // FOG
            {
                // Stage always has fog parameters
                writer.InlineDesc(SerializeVerbose, 0x84 + offset, fog);
                writer.WriteX(fog);

                // ... but does not always have associated curves
                if (fogCurves != null)
                {
                    // TODO: assert venue vs fog curves?

                    // Write FogCurves pointers to animation curves...
                    writer.InlineDesc(SerializeVerbose, 0x80 + offset, fogCurves);
                    writer.WriteX(fogCurves);
                    var fogCurvesPtr = fogCurves.GetPointer();
                    // ... then write the animation data associated with fog curves
                    writer.InlineDesc(SerializeVerbose, fogCurvesPtr, fogCurves.animationCurves);
                    foreach (var curve in fogCurves.animationCurves)
                        writer.WriteX(curve);
                }
            }

            // SCENE OBJECTS

            // SCENE OBJECT NAMES
            // No direct pointer. Names are aligned to 4 bytes.
            writer.CommentAlign(SerializeVerbose);
            writer.CommentNewLine(SerializeVerbose, '-');
            writer.Comment("ScnObjectNames[]", SerializeVerbose, ' ');
            writer.CommentNewLine(SerializeVerbose, '-');
            foreach (var sceneObjectName in SceneObjectNames)
            {
                writer.WriteX(sceneObjectName);
                //writer.AlignTo(4);
            }

            // SCENE OBJECTS
            writer.InlineComment(SerializeVerbose, nameof(SceneObjectLOD));
            writer.WriteX(SceneObjectLODs);

            // SCENE OBJECT TEMPLATES
            writer.InlineDesc(SerializeVerbose, 0x68 + offset, sceneObjects); // <<<<
            writer.WriteX(sceneObjects);

            // STATIC SCENE OBJECTS
            if (!staticSceneObjects.IsNullOrEmpty())
            {
                writer.InlineDesc(SerializeVerbose, 0x70 + offset, staticSceneObjects); // <<<<
                writer.WriteX(staticSceneObjects);
            }

            // DYNAMIC SCENE OBJECTS
            writer.InlineDesc(SerializeVerbose, 0x54 + offset, dynamicSceneObjects);
            writer.WriteX(dynamicSceneObjects);

            // Scene Object Collider Geo
            //{
            // Grab sub-structures of SceneObjectTemplate
            var colliderGeometries = new List<ColliderMesh>();
            var colliderGeoTris = new List<ColliderTriangle>();
            var colliderGeoQuads = new List<ColliderQuad>();
            foreach (var sceneObject in sceneObjects)
            {
                var colliderGeo = sceneObject.ColliderMesh;
                if (colliderGeo != null)
                {
                    colliderGeometries.Add(colliderGeo);

                    if (colliderGeo.TrisPtr.Length > 0)
                        colliderGeoTris.AddRange(colliderGeo.Tris);

                    if (colliderGeo.QuadsPtr.Length > 0)
                        colliderGeoQuads.AddRange(colliderGeo.Quads);
                }
            }
            // Collider Geometry
            writer.InlineComment(SerializeVerbose, nameof(ColliderMesh));
            foreach (var colliderGeometry in colliderGeometries)
                writer.WriteX(colliderGeometry);
            //
            writer.InlineComment(SerializeVerbose, nameof(ColliderMesh), nameof(ColliderTriangle));
            foreach (var tri in colliderGeoTris)
                writer.WriteX(tri);
            writer.InlineComment(SerializeVerbose, nameof(ColliderMesh), nameof(ColliderQuad));
            foreach (var quad in colliderGeoQuads)
                writer.WriteX(quad);


            // Grab all of the data needed to serialize. By doing this, we can
            // create linear blocks of data for each type. It simplifies the
            // process since we can check for nulls in one loop, and serialize
            // all the values in their own mini loops.
            var animationClips = new List<AnimationClip>();
            var animationClipCurves = new List<AnimationClipCurve>();
            var textureScrolls = new List<TextureScroll>();
            var textureScrollFields = new List<TextureScrollField>();
            var skeletalAnimators = new List<SkeletalAnimator>();
            var skeletalProperties = new List<SkeletalProperties>();
            var transformMatrices = new List<TransformMatrix3x4>();

            // Collect data from SceneObjectDynamics
            foreach (var dynamicSceneObject in dynamicSceneObjects)
            {
                // Animation Data
                if (dynamicSceneObject.AnimationClip != null)
                {
                    animationClips.Add(dynamicSceneObject.AnimationClip);
                    // Serialize individual animation clip curves
                    foreach (var animationClipCurve in dynamicSceneObject.AnimationClip.Curves)
                        animationClipCurves.Add(animationClipCurve);
                }

                // Texture Metadata
                if (dynamicSceneObject.TextureScroll != null)
                {
                    textureScrolls.Add(dynamicSceneObject.TextureScroll);
                    foreach (var field in dynamicSceneObject.TextureScroll.Fields)
                    {
                        if (field != null)
                        {
                            textureScrollFields.Add(field);
                        }
                    }
                }

                // Skeletal Animator
                if (dynamicSceneObject.SkeletalAnimator != null)
                {
                    skeletalAnimators.Add(dynamicSceneObject.SkeletalAnimator);
                    skeletalProperties.Add(dynamicSceneObject.SkeletalAnimator.Properties);
                }

                // Transforms
                if (dynamicSceneObject.TransformMatrix3x4 != null)
                {
                    transformMatrices.Add(dynamicSceneObject.TransformMatrix3x4);
                }
            }

            // Animation clips
            //writer.InlineDesc(serializeVerbose, animationClips.ToArray());
            writer.InlineComment(SerializeVerbose, nameof(AnimationClip) + "[]");
            foreach (var animationClip in animationClips)
                writer.WriteX(animationClip);
            // Animation clips' animation curves
            // 2022-01-18: add serilization for animation data!
            writer.InlineComment(SerializeVerbose, nameof(AnimationClip), "AnimClipCurve", $"{nameof(AnimationCurve)}[]");
            foreach (var animationClipCurve in animationClipCurves)
                if (animationClipCurve.AnimationCurve != null)
                    writer.WriteX(animationClipCurve.AnimationCurve);

            // Texture metadata
            writer.InlineDesc(SerializeVerbose, textureScrolls.ToArray());
            foreach (var textureMetadata in textureScrolls)
                writer.WriteX(textureMetadata);
            writer.InlineDesc(SerializeVerbose, textureScrollFields.ToArray());
            foreach (var textureMetadataField in textureScrollFields)
                writer.WriteX(textureMetadataField);

            // Skeletal animator
            writer.InlineDesc(SerializeVerbose, skeletalAnimators.ToArray());
            foreach (var skeletalAnimator in skeletalAnimators)
                writer.WriteX(skeletalAnimator);
            writer.InlineDesc(SerializeVerbose, skeletalProperties.ToArray());
            foreach (var skeletalProperty in skeletalProperties)
                writer.WriteX(skeletalProperty);

            // Transforms for dynamic scene objects
            writer.InlineDesc(SerializeVerbose, transformMatrices.ToArray());
            foreach (var transformMatrix3x4 in transformMatrices)
                writer.WriteX(transformMatrix3x4);


            // TRIGGERS
            {
                // TIME EXTENSION TRIGGERS
                if (!timeExtensionTriggers.IsNullOrEmpty())
                    writer.InlineDesc(SerializeVerbose, 0xB0 + offset, timeExtensionTriggers);
                writer.WriteX(timeExtensionTriggers);

                // COURSE METADATA TRIGGERS
                if (!miscellaneousTriggers.IsNullOrEmpty())
                    writer.InlineDesc(SerializeVerbose, 0xA8 + offset, miscellaneousTriggers);
                writer.WriteX(miscellaneousTriggers);

                // STORY OBJECT TRIGGERS
                if (!storyObjectTriggers.IsNullOrEmpty())
                    writer.InlineDesc(SerializeVerbose, 0xB8 + offset, storyObjectTriggers);
                writer.WriteX(storyObjectTriggers);
                //
                // Get better comment? Get proper pointers?
                if (!storyObjectTriggers.IsNullOrEmpty())
                    writer.InlineDesc(SerializeVerbose, new StoryObjectPath());
                foreach (var storyObjectTrigger in storyObjectTriggers)
                {
                    // Optional data
                    var storyObjectPath = storyObjectTrigger.StoryObjectPath;
                    if (storyObjectPath != null)
                    {
                        // Uncomment if you want super-inline -- maybe have that as option?
                        //writer.InlineDesc(serializeVerbose, storyObjectTrigger.GetPointer(), storyObjectTrigger);
                        writer.WriteX(storyObjectPath);

                        // Assert is true. This data is always here if existing
                        Assert.IsTrue(storyObjectPath.AnimationCurve != null);
                        // breaking the rules again. Should inlining be allowed for these ptr types?
                        writer.WriteX(storyObjectPath.AnimationCurve);
                    }
                }

                // UNKNOWN TRIGGERS
                if (!cullOverrideTriggers.IsNullOrEmpty())
                {
                    writer.InlineDesc(SerializeVerbose, 0x94 + offset, cullOverrideTriggers);
                    writer.WriteX(cullOverrideTriggers);
                }

                // VISUAL EFFECT TRIGGERS
                if (!visualEffectTriggers.IsNullOrEmpty())
                {
                    writer.InlineDesc(SerializeVerbose, 0x9C + offset, visualEffectTriggers);
                    writer.WriteX(visualEffectTriggers);
                }

                // UNKNOWN COLLIDERS (SOLS ONLY)
                if (!unknownColliders.IsNullOrEmpty())
                {
                    writer.InlineDesc(SerializeVerbose, 0x60 + offset, unknownColliders);
                    writer.WriteX(unknownColliders);
                }
            }

            // DEBUG
            // Assuming the writer for this stream is the type specified below,
            // It will error if we write to the same address twice. This is useful
            // for finding bugs where writer is not reset to correct position.
            if (writer.GetType() == typeof(AddressLogBinaryWriter))
            {
                ((AddressLogBinaryWriter)writer).MemoryLogActive = false;
            }

            // GET ALL REFERERS, RE-SERIALIZE FOR POINTERS
            {
                // Get a reference to EVERY object in file that has a pointer to an object
                var hasReferences = new List<IHasReference>();

                // Track Nodes and dependencies
                hasReferences.AddRange(trackNodes);
                hasReferences.AddRange(AllTrackSegments);
                foreach (var trackSegment in AllTrackSegments)
                    hasReferences.Add(trackSegment.AnimationCurveTRS);
                // The checkpoint table
                hasReferences.Add(trackCheckpointGrid);

                // Static Collider Meshes and dependencies
                hasReferences.Add(staticColliderMeshManager);
                hasReferences.AddRange(staticColliderMeshManager.TriMeshGrids);
                hasReferences.AddRange(staticColliderMeshManager.QuadMeshGrids);
                hasReferences.AddRange(unknownColliders);

                // OBJECTS
                // Scene Objects
                hasReferences.AddRange(SceneObjectLODs);
                hasReferences.AddRange(sceneObjects);
                hasReferences.AddRange(colliderGeometries);
                // Scene Object Statics
                if (staticSceneObjects != null)
                    hasReferences.AddRange(staticSceneObjects);
                // Scene Object Dynamics
                hasReferences.AddRange(dynamicSceneObjects);
                hasReferences.AddRange(textureScrolls);
                hasReferences.AddRange(skeletalAnimators);
                hasReferences.AddRange(animationClips);
                hasReferences.AddRange(animationClipCurves);

                // FOG
                // The structure points to 6 anim curves
                hasReferences.Add(fogCurves);

                // The story mode checkpoints
                foreach (var storyObjectTrigger in storyObjectTriggers)
                {
                    hasReferences.Add(storyObjectTrigger);
                    hasReferences.Add(storyObjectTrigger.StoryObjectPath);
                }

                // RE-SERIALIZE
                // Patch pointers by re-writing structure in same place as previously serialized
                foreach (var hasReference in hasReferences)
                {
                    var pointer = hasReference.GetPointer();
                    if (pointer.IsNotNull)
                    {
                        writer.JumpToAddress(pointer);
                        hasReference.Serialize(writer);
                        // Run assertions on referer to ensure pointer requirements are met
                        hasReference.ValidateReferences();
                    }
                }
            } // end patching pointers

            // RE-WRITE ColiScene HEADER TO RESERIALIZE POINTERS
            // Rewrite main block pointers
            writer.JumpToAddress(0);
            SerializeHeader(writer);
            // Validate this structure before finishing.
            ValidateReferences();

            //
            FileSize = (int)writer.BaseStream.Length;

            BinaryIoUtility.PopEndianness();
        }

        /// <summary>
        /// Writes out a StaticColliderMeshMatrix[] with loads of comments.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="refPtr"></param>
        /// <param name="id"></param>
        /// <param name="staticColliderMeshMatrix"></param>
        public void WriteStaticColliderMeshMatrices(BinaryWriter writer, Pointer refPtr, string id, StaticColliderMeshGrid[] staticColliderMeshMatrix)
        {
            // GX COUNT: 14. "Pointer table" of 256 ptrs PER item.
            var nMatrices = staticColliderMeshMatrix.Length;
            const int w = 2; // number width format

            for (int i = 0; i < nMatrices; i++)
            {
                var matrix = staticColliderMeshMatrix[i];
                var type = (StaticColliderMeshProperty)(i);

                // Cannot be null
                Assert.IsTrue(matrix != null);

                // Write extra-helpful comment.
                writer.CommentAlign(SerializeVerbose, ' ');
                writer.CommentNewLine(SerializeVerbose, '-');
                writer.CommentType(matrix, SerializeVerbose);
                writer.CommentPtr(refPtr, SerializeVerbose);
                writer.CommentNewLine(SerializeVerbose, '-');
                writer.CommentLineWide("Owner:", id, SerializeVerbose);
                writer.CommentLineWide("Index:", $"[{i,w}/{nMatrices}]", SerializeVerbose);
                writer.CommentLineWide("Type:", type, SerializeVerbose);
                writer.CommentLineWide("Mtx:", $"x:{matrix.SubdivisionsX,2}, z:{matrix.SubdivisionsZ,2}", SerializeVerbose);
                writer.CommentLineWide("MtxCnt:", matrix.Count, SerializeVerbose);
                writer.CommentNewLine(SerializeVerbose, '-');
                //
                writer.WriteX(matrix);
                var qmiPtr = matrix.GetPointer(); // quad mesh indices? rename as is generic function between quad/tri

                if (matrix.HasIndexes)
                {
                    // 256 lists
                    writer.CommentAlign(SerializeVerbose, ' ');
                    writer.CommentNewLine(SerializeVerbose, '-');
                    writer.Comment($"{nameof(IndexList)}[{i,w}/{nMatrices}]", SerializeVerbose);
                    writer.CommentPtr(qmiPtr, SerializeVerbose);
                    writer.CommentLineWide("Type:", type, SerializeVerbose);
                    writer.CommentLineWide("Owner:", id, SerializeVerbose);
                    writer.CommentNewLine(SerializeVerbose, '-');
                    for (int index = 0; index < matrix.IndexLists.Length; index++)
                        if (matrix.IndexLists[index].Length > 0)
                            writer.CommentIdx(index, SerializeVerbose);
                    writer.CommentNewLine(SerializeVerbose, '-');
                    for (int index = 0; index < matrix.IndexLists.Length; index++)
                    {
                        var quadIndexList = matrix.IndexLists[index];
                        writer.WriteX(quadIndexList);
                    }
                }
            }
        }

        /// <summary>
        /// Returns an array of all IBinaryAddressables (possibly with nulls) in this ColiScene.
        /// Useful to check if values are written to disk if addresses are set to consts beforehand.
        /// </summary>
        /// <returns></returns>
        public IBinaryAddressable[] GetAllAddressables()
        {
            var list = new List<IBinaryAddressable>();

            foreach (var trackNode in trackNodes)
            {
                list.Add(trackNode);
                list.AddRange(trackNode.Checkpoints);
                list.Add(trackNode.Segment);
                list.Add(trackNode.Segment.AnimationCurveTRS);
                list.AddRange(trackNode.Segment.AnimationCurveTRS.AnimationCurves);
                foreach (var anim in trackNode.Segment.AnimationCurveTRS.AnimationCurves) // null?
                    list.AddRange(anim.KeyableAttributes);
                list.Add(trackNode.Segment.TrackCorner);
                if (trackNode.Segment.TrackCorner != null)
                    list.Add(trackNode.Segment.TrackCorner.Transform);
            }

            list.AddRange(embeddedPropertyAreas);

            // Static Collider Meshes
            list.Add(staticColliderMeshManager);
            list.AddRange(staticColliderMeshManager.ColliderTris);
            list.AddRange(staticColliderMeshManager.ColliderQuads);
            foreach (var matrix in staticColliderMeshManager.TriMeshGrids)
            {
                list.Add(matrix);
                list.AddRange(matrix.IndexLists);
            }
            foreach (var matrix in staticColliderMeshManager.QuadMeshGrids)
            {
                list.Add(matrix);
                list.AddRange(matrix.IndexLists);
            }
            list.Add(staticColliderMeshManager.MeshGridXZ);
            //list.Add(staticColliderMeshManager.BoundingSphere);

            list.Add(trackMinHeight);

            foreach (var dynamicSceneObject in dynamicSceneObjects)
            {
                list.Add(dynamicSceneObject);
                list.Add(dynamicSceneObject.AnimationClip);
                if (dynamicSceneObject.AnimationClip != null)
                {
                    foreach (var animClipCurve in dynamicSceneObject.AnimationClip.Curves)
                    {
                        list.Add(animClipCurve);
                        list.Add(animClipCurve.AnimationCurve);
                        if (animClipCurve.AnimationCurve != null)
                            list.AddRange(animClipCurve.AnimationCurve.KeyableAttributes);
                    }
                }
                list.Add(dynamicSceneObject.TextureScroll);
                if (dynamicSceneObject.TextureScroll != null)
                    list.AddRange(dynamicSceneObject.TextureScroll.Fields);
                list.Add(dynamicSceneObject.SkeletalAnimator);
                if (dynamicSceneObject.SkeletalAnimator != null)
                    list.Add(dynamicSceneObject.SkeletalAnimator.Properties);
                list.Add(dynamicSceneObject.TransformMatrix3x4);
                // Elsewhere: Scene Object Templates
            }

            list.AddRange(unknownColliders);

            foreach (var template in sceneObjects)
            {
                list.Add(template);
                list.Add(template.ColliderMesh);
                list.AddRange(template.LODs);
                list.Add(template.PrimaryLOD.Name);
            }

            list.Add(fog);
            list.Add(fogCurves);
            if (fogCurves != null)
            {
                foreach (var curve in fogCurves.animationCurves)
                {
                    list.Add(curve);
                    list.AddRange(curve.KeyableAttributes);
                }
            }

            list.Add(trackLength);
            list.AddRange(cullOverrideTriggers);
            list.AddRange(visualEffectTriggers);
            list.AddRange(miscellaneousTriggers);
            list.AddRange(timeExtensionTriggers);

            foreach (var trigger in storyObjectTriggers)
            {
                list.Add(trigger);
                list.Add(trigger.StoryObjectPath);
                if (trigger.StoryObjectPath != null)
                {
                    list.Add(trigger.StoryObjectPath.AnimationCurve);
                    list.AddRange(trigger.StoryObjectPath.AnimationCurve.KeyableAttributes);
                }
            }

            list.Add(trackCheckpointGrid);
            list.AddRange(trackCheckpointGrid.IndexLists);

            return list.ToArray();
        }


        public void DeserializeHeader(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // Deserialize main structure
                reader.ReadX(ref unkRange0x00);
                reader.ReadX(ref trackNodesPtr);
                reader.ReadX(ref embeddedTrackPropertyAreasPtr);
                reader.ReadX(ref staticColliderMeshManagerActive);
                reader.ReadX(ref staticColliderMeshManagerPtr);
                reader.ReadX(ref zeroes0x20Ptr);
                reader.ReadX(ref trackMinHeightPtr);
                ValidateFileFormatPointers(); // VALIDATE
                reader.ReadX(ref zeroes0x28, kSizeOfZeroes0x28);
                reader.ReadX(ref dynamicSceneObjectCount);
                reader.ReadX(ref unk_sceneObjectCount1);
                if (IsFileGX) reader.ReadX(ref unk_sceneObjectCount2);
                reader.ReadX(ref dynamicSceneObjectsPtr);
                reader.ReadX(ref unkBool32_0x58);
                reader.ReadX(ref unknownCollidersPtr);
                reader.ReadX(ref sceneObjectsPtr);
                reader.ReadX(ref staticSceneObjectsPtr);
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
                reader.ReadX(ref miscellaneousTriggersPtr);
                reader.ReadX(ref timeExtensionTriggersPtr);
                reader.ReadX(ref storyObjectTriggersPtr);
                reader.ReadX(ref checkpointGridPtr);
                reader.ReadX(ref checkpointGridXZ);
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
                IsFileAX = Format == SerializeFormat.AX;
                IsFileGX = Format == SerializeFormat.GX;
                Assert.IsFalse(Format == SerializeFormat.InvalidFormat);

                // UPDATE POINTERS AND COUNTS
                // Track and stage data
                StaticColliderMeshManagerPtr = staticColliderMeshManager.GetPointer();
                EmbeddedTrackPropertyAreasPtr = embeddedPropertyAreas.GetArrayPointer();
                CheckpointGridPtr = trackCheckpointGrid.GetPointer();
                TrackLengthPtr = trackLength.GetPointer();
                TrackMinHeightPtr = trackMinHeight.GetPointer();
                TrackNodesPtr = trackNodes.GetArrayPointer();
                FogPtr = fog.GetPointer();
                FogCurvesPtr = fogCurves.GetPointer();
                // TRIGGERS
                TimeExtensionTriggersPtr = timeExtensionTriggers.GetArrayPointer();
                MiscellaneousTriggersPtr = miscellaneousTriggers.GetArrayPointer();
                StoryObjectTriggersPtr = storyObjectTriggers.GetArrayPointer();
                UnknownCollidersPtr = unknownColliders.GetArrayPointer();
                UnknownTriggersPtr = cullOverrideTriggers.GetArrayPointer();
                VisualEffectTriggersPtr = visualEffectTriggers.GetArrayPointer();
                // SCENE OBJECTS
                // References
                SceneObjectsPtr = sceneObjects.GetArrayPointer();
                StaticSceneObjectsPtr = staticSceneObjects.GetArrayPointer();
                // Main list
                DynamicSceneObjectCount = dynamicSceneObjects.Length;
                // 2021/01/12: test to see if this makes objects appear
                // It does and doesn't. Objects with animation data seem to play, so may have something to do with that.
                Unk_sceneObjectCount1 = dynamicSceneObjects.Length;
                Unk_sceneObjectCount2 = dynamicSceneObjects.Length;
                DynamicSceneObjectsPtr = dynamicSceneObjects.GetArrayPointer().Pointer;
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unkRange0x00);
                writer.WriteX(trackNodesPtr);
                writer.WriteX(embeddedTrackPropertyAreasPtr);
                writer.WriteX(staticColliderMeshManagerActive);
                writer.WriteX(staticColliderMeshManagerPtr);
                writer.WriteX(zeroes0x20Ptr);
                writer.WriteX(trackMinHeightPtr);
                writer.WriteX(new byte[kSizeOfZeroes0x28]); // write const zeros
                writer.WriteX(dynamicSceneObjectCount);
                writer.WriteX(unk_sceneObjectCount1);
                if (Format == SerializeFormat.GX)
                    writer.WriteX(unk_sceneObjectCount2);
                writer.WriteX(dynamicSceneObjectsPtr);
                writer.WriteX(unkBool32_0x58);
                writer.WriteX(unknownCollidersPtr);
                writer.WriteX(sceneObjectsPtr);
                writer.WriteX(staticSceneObjectsPtr);
                writer.WriteX(new ArrayPointer()); // const unused
                writer.WriteX(circuitType);
                writer.WriteX(fogCurvesPtr);
                writer.WriteX(fogPtr);
                writer.WriteX(new ArrayPointer()); // const unused
                writer.WriteX(trackLengthPtr);
                writer.WriteX(unknownTriggersPtr);
                writer.WriteX(visualEffectTriggersPtr);
                writer.WriteX(miscellaneousTriggersPtr);
                writer.WriteX(timeExtensionTriggersPtr);
                writer.WriteX(storyObjectTriggersPtr);
                writer.WriteX(checkpointGridPtr);
                writer.WriteX(checkpointGridXZ);
                writer.WriteX(new byte[kSizeOfZeroes0xD8]); // write const zeros
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // "Constants"
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

            // Structures that always exist
            Assert.IsTrue(TrackNodesPtr.IsNotNull);
            Assert.IsTrue(EmbeddedTrackPropertyAreasPtr.IsNotNull);
            Assert.IsTrue(TrackMinHeightPtr.IsNotNull);
            Assert.IsTrue(StaticColliderMeshManagerPtr.IsNotNull);
            Assert.IsTrue(zeroes0x20Ptr.IsNotNull);
            Assert.IsTrue(TrackMinHeightPtr.IsNotNull);
            if (sceneObjects.Length > 0)
                Assert.IsTrue(SceneObjectsPtr.IsNotNull);
            Assert.IsTrue(FogPtr.IsNotNull);
            Assert.IsTrue(TrackLengthPtr.IsNotNull);
            Assert.IsTrue(CheckpointGridPtr.IsNotNull);

            // Ensure existing structures pointers were resolved correctly
            Assert.ReferencePointer(trackNodes, TrackNodesPtr);
            Assert.ReferencePointer(embeddedPropertyAreas, EmbeddedTrackPropertyAreasPtr);
            Assert.ReferencePointer(trackMinHeight, TrackMinHeightPtr);
            if (dynamicSceneObjects.Length > 0)
                Assert.ReferencePointer(dynamicSceneObjects, new ArrayPointer(DynamicSceneObjectCount, DynamicSceneObjectsPtr));
            if (sceneObjects.Length > 0)
                Assert.ReferencePointer(sceneObjects, SceneObjectsPtr);
            Assert.ReferencePointer(staticSceneObjects, StaticSceneObjectsPtr);
            if (unknownColliders.Length > 0)
                Assert.ReferencePointer(unknownColliders, UnknownCollidersPtr);
            Assert.ReferencePointer(fogCurves, FogCurvesPtr);
            Assert.ReferencePointer(fog, FogPtr);
            Assert.ReferencePointer(trackLength, TrackLengthPtr);
            if (cullOverrideTriggers.Length > 0)
                Assert.ReferencePointer(cullOverrideTriggers, UnknownTriggersPtr);
            if (miscellaneousTriggers.Length > 0)
                Assert.ReferencePointer(miscellaneousTriggers, MiscellaneousTriggersPtr);
            if (timeExtensionTriggers.Length > 0)
                Assert.ReferencePointer(timeExtensionTriggers, TimeExtensionTriggersPtr);
            if (storyObjectTriggers.Length > 0)
                Assert.ReferencePointer(storyObjectTriggers, StoryObjectTriggersPtr);
            Assert.ReferencePointer(trackCheckpointGrid, CheckpointGridPtr);
        }

        /// <summary>
        /// Returns the BinarySerializable at the specified <paramref name="pointer"/>. If the instance's pointer is not in the
        /// <paramref name="dictionary"/>, a new instance will be deserialized from the <paramref name="reader"/> at the
        /// <paramref name="pointer"/> address and added to the <paramref name="dictionary"/>. If the <paramref name="pointer"/>
        /// exists in the <paramref name="dictionary"/>, that instance will be returned without deserializing a new
        /// instance from the <paramref name="reader"/>.
        /// </summary>
        /// <typeparam name="TBinarySerializable">The type of BinarySerializable</typeparam>
        /// <param name="reader">The binary reader to deserialize from, if necessary.</param>
        /// <param name="pointer">The pointer of an instance to either deserialize or retrieve from <paramref name="dictionary"/></param>
        /// <param name="dictionary">The dictionary correlating <paramref name="pointer"/> to instances.</param>
        public static TBinarySerializable GetSharedSerializable<TBinarySerializable>(BinaryReader reader, Pointer pointer, Dictionary<Pointer, TBinarySerializable> dictionary)
            where TBinarySerializable : class, IBinarySerializable, new()
        {
            // If ptr is null, set reference to null, return
            if (pointer.IsNull)
                return null;

            // If we have have this reference, return it
            if (dictionary.ContainsKey(pointer))
            {
                return dictionary[pointer];
            }
            // If we don't have this reference, deserialize it, store in dict, return it
            else
            {
                reader.JumpToAddress(pointer);
                var binarySerializable = new TBinarySerializable();
                binarySerializable.Deserialize(reader);
                dictionary.Add(pointer, binarySerializable);
                return binarySerializable;
            }
        }


        public string PrintSingleLine()
        {
            return $"{nameof(Scene)}({Venue} [{CourseName}] by {Author}, {nameof(CourseIndex)}({CourseIndex}), {Format})";
        }

        //// FIELDS (that require extra processing)
        //// Shared references
        //public TrackSegment[] allTrackSegments;
        //public TrackSegment[] rootTrackSegments;
        //public ShiftJisCString[] sceneObjectNames;
        //public SceneObjectLOD[] sceneObjectLODs; // debug

        public void PrintMultiLine(StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            const int lengthDivider = 128;

            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(FileSize)}: {FileSize:n0}, {FileSize:x8}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(SerializeFormat)}: {Format}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(IsValidFile)}: {IsValidFile}");
            builder.AppendLine();

            builder.AppendRepeat('-', lengthDivider);
            builder.AppendLineIndented(indent, indentLevel, $"MISC DATA");
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, UnkRange0x00);
            builder.AppendLineIndented(indent, indentLevel, fog);
            builder.AppendLineIndented(indent, indentLevel, fogCurves);
            builder.AppendLine();
            indentLevel--;

            builder.AppendRepeat('-', lengthDivider);
            builder.AppendLineIndented(indent, indentLevel, $"TRIGGERS");
            {
                indentLevel++;
                builder.AppendLine();

                foreach (var cullOverrideTrigger in cullOverrideTriggers)
                    builder.AppendLineIndented(indent, indentLevel, cullOverrideTrigger);
                builder.AppendLine();

                foreach (var visualEffectTrigger in visualEffectTriggers)
                    builder.AppendLineIndented(indent, indentLevel, visualEffectTrigger);
                builder.AppendLine();

                foreach (var miscellaneousTrigger in miscellaneousTriggers)
                    builder.AppendLineIndented(indent, indentLevel, miscellaneousTrigger);
                builder.AppendLine();

                foreach (var timeExtensionTrigger in timeExtensionTriggers)
                    builder.AppendLineIndented(indent, indentLevel, timeExtensionTrigger);
                builder.AppendLine();

                foreach (var storyObjectTrigger in storyObjectTriggers)
                    builder.AppendLineIndented(indent, indentLevel, storyObjectTrigger);
                builder.AppendLine();

                indentLevel--;
            }

            builder.AppendRepeat('-', lengthDivider);
            builder.AppendLineIndented(indent, indentLevel, $"TRACK DATA");
            {
                indentLevel++;
                builder.AppendLine();

                builder.AppendLineIndented(indent, indentLevel, trackLength);
                builder.AppendLineIndented(indent, indentLevel, trackMinHeight);
                builder.AppendLine();

                // Track Segments
                foreach (var rootTrackSegment in RootTrackSegments)
                {
                    var trackSegmentHierarchy = rootTrackSegment.GetGraphHierarchyOrder();
                    foreach (var trackSegment in trackSegmentHierarchy)
                    {
                        var depth = trackSegment.Depth;
                        builder.AppendLineIndented(indent, indentLevel, trackSegment);
                    }
                }
                builder.AppendLine();

                // Track segment animation curves
                foreach (var rootTrackSegment in RootTrackSegments)
                {
                    var trackSegmentHierarchy = rootTrackSegment.GetGraphHierarchyOrder();
                    foreach (var trackSegment in trackSegmentHierarchy)
                    {
                        var depth = trackSegment.Depth;
                        builder.AppendLineIndented(indent, indentLevel, trackSegment.AnimationCurveTRS);
                    }
                }
                builder.AppendLine();

                // Track Nodes
                int formatWidth = trackNodes.LengthToFormat();
                builder.AppendLineIndented(indent, indentLevel, $"{nameof(trackNodes)} [{trackNodes.Length}]");
                for (int i = 0; i < trackNodes.Length; i++)
                    builder.AppendLineIndented(indent, indentLevel, $"[{i.PadLeft(formatWidth)}] {trackNodes[i]}");
                builder.AppendLine();

                // Checkpoints
                builder.AppendLineIndented(indent, indentLevel, CheckpointGridXZ);
                builder.AppendLineIndented(indent, indentLevel, trackCheckpointGrid);
                builder.AppendLine();

                indentLevel--;
            }

            //
            builder.AppendRepeat('-', lengthDivider);
            builder.AppendLineIndented(indent, indentLevel, $"COLLIDER DATA");
            {
                indentLevel++;
                builder.AppendLineIndented(indent, indentLevel, staticColliderMeshManager);
                indentLevel--;
            }

            builder.AppendRepeat('-', lengthDivider);
            builder.AppendLineIndented(indent, indentLevel, $"SCENE OBJECTS");
            {
                indentLevel++;
                foreach (var name in SceneObjectNames)
                    builder.AppendLineIndented(indent, indentLevel + 1, name);
                builder.AppendLine();

                foreach (var sceneObject in sceneObjects)
                    builder.AppendLineIndented(indent, indentLevel + 1, sceneObject);
                builder.AppendLine();

                foreach (var staticSceneObject in staticSceneObjects)
                    builder.AppendLineIndented(indent, indentLevel + 1, staticSceneObject);
                builder.AppendLine();

                foreach (var dynamicSceneObject in dynamicSceneObjects)
                    builder.AppendLineIndented(indent, indentLevel + 1, dynamicSceneObject);
                builder.AppendLine();

                builder.AppendLineIndented(indent, indentLevel, staticColliderMeshManager);
                indentLevel--;
            }
        }

        public override string ToString() => PrintSingleLine();

    }
}
