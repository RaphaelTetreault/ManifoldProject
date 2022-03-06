using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Mathematics;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// The data structure representing a scene in F-Zero AX/GX. It contains all the necessary
    /// data for object placement, object animations, fog, collision data for track and surfaces,
    /// track metadata, and triggers for mission, visual effects, etc.
    /// </summary>
    [Serializable]
    public class ColiScene :
        IBinaryAddressable,
        IBinarySerializable,
        IFile,
        IHasReference
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
        private bool isFileAX;
        private bool isFileGX;
        private AddressRange addressRange;
        private int id;
        private string fileName;
        private int fileSize;
        private Venue venue;
        private string courseName;
        private string author;
        private bool serializeVerbose = true;
        private SerializeFormat format;


        // FIELDS
        // Note: order of folowing structures is same as they appear in binary
        public ViewRange unkRange0x00;
        public ArrayPointer trackNodesPtr;
        public ArrayPointer embeddedTrackPropertyAreasPtr;
        // 2022-01-14: this is a bool, the game loads the following structure regardless
        // (because it'd crash anways because of it when unloading)
        public Bool32 staticColliderMeshManagerActive = Bool32.True;
        public Pointer staticColliderMeshManagerPtr;
        public Pointer zeroes0x20Ptr; // GX: 0xE8, AX: 0xE4
        public Pointer trackMinHeightPtr; // GX: 0xFC, AX: 0xF8
        public byte[] zeroes0x28 = new byte[kSizeOfZeroes0x28];
        public int dynamicSceneObjectCount;
        public int unk_sceneObjectCount1;
        public int unk_sceneObjectCount2; // GX exclusive
        public Pointer dynamicSceneObjectsPtr;
        public Bool32 unkBool32_0x58 = Bool32.True;
        public ArrayPointer unknownCollidersPtr;
        public ArrayPointer sceneObjectsPtr;
        public ArrayPointer staticSceneObjectsPtr;
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
        public ArrayPointer miscellaneousTriggersPtr;
        public ArrayPointer timeExtensionTriggersPtr;
        public ArrayPointer storyObjectTriggersPtr;
        public Pointer checkpointGridPtr;
        public GridXZ checkpointGridXZ;
        public byte[] zeroes0xD8 = new byte[kSizeOfZeroes0xD8];

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
        public CullOverrideTrigger[] unknownTriggers;
        public VisualEffectTrigger[] visualEffectTriggers;
        public MiscellaneousTrigger[] miscellaneousTriggers;
        public TimeExtensionTrigger[] timeExtensionTriggers;
        public StoryObjectTrigger[] storyObjectTriggers;
        public TrackCheckpointGrid trackCheckpointGrid;
        // FIELDS (that require extra processing)
        // Shared references
        public TrackSegment[] allTrackSegments;
        public TrackSegment[] rootTrackSegments;
        public ShiftJisCString[] sceneObjectNames;
        public SceneObjectLOD[] sceneObjectLODs;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }

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

        public bool SerializeVerbose
        {
            get => serializeVerbose;
            set => serializeVerbose = value;
        }

        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }

        public int FileSize
        {
            get => fileSize;
            set => fileSize = value;
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

        public void InitAllTypes()
        {
            checkpointGridXZ = new GridXZ();

            // REFERENCE FIELDS
            trackNodes = new TrackNode[0];
            embeddedPropertyAreas = EmbeddedTrackPropertyArea.DefaultArray();
            staticColliderMeshManager = new StaticColliderMeshManager(SerializeFormat.InvalidFormat);

            trackMinHeight = new TrackMinHeight(); // has default constructor
            dynamicSceneObjects = new SceneObjectDynamic[0];
            sceneObjects = new SceneObject[0];
            staticSceneObjects = new SceneObjectStatic[0];
            unknownColliders = new UnknownCollider[0];
            fogCurves = new FogCurves();
            fog = new Fog();
            trackLength = new TrackLength();
            unknownTriggers = new CullOverrideTrigger[0];
            visualEffectTriggers = new VisualEffectTrigger[0];
            miscellaneousTriggers = new MiscellaneousTrigger[0];
            timeExtensionTriggers = new TimeExtensionTrigger[0];
            storyObjectTriggers = new StoryObjectTrigger[0];
            trackCheckpointGrid = new TrackCheckpointGrid();
            // FIELDS (that require extra processing)
            // Shared references
            allTrackSegments = new TrackSegment[0];
            rootTrackSegments = new TrackSegment[0];
            sceneObjectNames = new ShiftJisCString[0];
            sceneObjectLODs = new SceneObjectLOD[0];
        }

        public void ValidateFileFormatPointers()
        {
            isFileAX = IsAX(zeroes0x20Ptr, trackMinHeightPtr);
            isFileGX = IsGX(zeroes0x20Ptr, trackMinHeightPtr);
            Assert.IsTrue(IsValidFile);
        }


        public void Deserialize(BinaryReader reader)
        {
            BinaryIoUtility.PushEndianness(Endianness.BigEndian);

            // CAPTURE METADATA
            fileSize = (int)reader.BaseStream.Length;
            // Store the stage index, can solve venue and course name from this using hashes
            var matchDigits = Regex.Match(FileName, Const.Regex.MatchIntegers);
            id = int.Parse(matchDigits.Value);

            // Read COLI_COURSE## file header
            DeserializeHeader(reader);

            // DESERIALIZE REFERENCE TYPES
            // All types below are beyond the inital header and have pointers to them
            // specified in the header. While deserialization could be in any order, 
            // the following is order as they appear in the header.

            // 0x08 and 0x0C: Track Nodes
            reader.JumpToAddress(trackNodesPtr);
            reader.ReadX(ref trackNodes, trackNodesPtr.Length);

            // 0x10 and 0x14: Track Effect Attribute Areas
            reader.JumpToAddress(embeddedTrackPropertyAreasPtr);
            reader.ReadX(ref embeddedPropertyAreas, embeddedTrackPropertyAreasPtr.Length);

            // 0x1C 
            reader.JumpToAddress(staticColliderMeshManagerPtr);
            // The structure's size differs between AX and GX. Format defines which it uses.
            staticColliderMeshManager = new StaticColliderMeshManager(Format);
            staticColliderMeshManager.Deserialize(reader);

            // 0x20
            reader.JumpToAddress(zeroes0x20Ptr);
            reader.ReadX(ref zeroes0x20, kSizeOfZeroes0x20);

            // 0x24
            reader.JumpToAddress(trackMinHeightPtr);
            reader.ReadX(ref trackMinHeight);

            // 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address)
            reader.JumpToAddress(dynamicSceneObjectsPtr);
            reader.ReadX(ref dynamicSceneObjects, dynamicSceneObjectCount);

            // 0x5C and 0x60 SOLS values
            reader.JumpToAddress(unknownCollidersPtr);
            reader.ReadX(ref unknownColliders, unknownCollidersPtr.Length);

            // 0x64 and 0x68
            reader.JumpToAddress(sceneObjectsPtr);
            reader.ReadX(ref sceneObjects, sceneObjectsPtr.Length);

            // 0x6C and 0x70
            reader.JumpToAddress(staticSceneObjectsPtr);
            reader.ReadX(ref staticSceneObjects, staticSceneObjectsPtr.Length);

            // 0x80
            // Data is optional
            if (fogCurvesPtr.IsNotNull)
            {
                reader.JumpToAddress(fogCurvesPtr);
                reader.ReadX(ref fogCurves);
            }

            // 0x84
            reader.JumpToAddress(fogPtr);
            reader.ReadX(ref fog);

            // 0x90 
            reader.JumpToAddress(trackLengthPtr);
            reader.ReadX(ref trackLength);

            // 0x94 and 0x98
            reader.JumpToAddress(unknownTriggersPtr);
            reader.ReadX(ref unknownTriggers, unknownTriggersPtr.Length);

            // 0x9C and 0xA0
            reader.JumpToAddress(visualEffectTriggersPtr);
            reader.ReadX(ref visualEffectTriggers, visualEffectTriggersPtr.Length);

            // 0xA4 and 0xA8
            reader.JumpToAddress(miscellaneousTriggersPtr);
            reader.ReadX(ref miscellaneousTriggers, miscellaneousTriggersPtr.Length);

            // 0xAC and 0xB0
            reader.JumpToAddress(timeExtensionTriggersPtr);
            reader.ReadX(ref timeExtensionTriggers, timeExtensionTriggersPtr.Length);

            // 0xB4 and 0xB8
            reader.JumpToAddress(storyObjectTriggersPtr);
            reader.ReadX(ref storyObjectTriggers, storyObjectTriggersPtr.Length);

            // 0xBC and 0xC0
            reader.JumpToAddress(checkpointGridPtr);
            reader.ReadX(ref trackCheckpointGrid);

            // TEMP
            // For some reason, this structure points back to these
            staticColliderMeshManager.unknownColliders = unknownColliders;
            staticColliderMeshManager.staticSceneObjects = staticSceneObjects;
            Assert.IsTrue(staticColliderMeshManager.unknownCollidersPtr == unknownCollidersPtr);
            Assert.IsTrue(staticColliderMeshManager.staticSceneObjectsPtr == staticSceneObjectsPtr);


            // UNMANGLE SHARED REFERENCES
            {
                /*/
                    Problem
                    
                    Due to how the scene data is configured, there are a few data types which share
                    references. This is smart and efficient for memory, but it means simple/traditional
                    deserialization fails. The way most all data types are deserializedd in this project
                    is in linear fashion. X1 deserializes itself. If X1 references Y, it deserializes Y.
                    The issue is that if we have an X2 and it also references Y, what happens is that both
                    X1 and X2 create their own instance of Y. The issue surfaces once the data goes to
                    serialize and X1 and X2 seriaslize their own version of the data.

                    The solution here is to let X1 and X2 deserialize separate instances of Y. We can then
                    use the pointer address of those references and create a dictionary indexed by the pointer
                    address of the data. If we come across another structure that has the same pointer as
                    a previous structure, we can assign the same reference to it. This relinks shared data
                    types in memory.
                /*/

                // Keep a dictionary of each shared reference type
                var sceneObjectsDict = new Dictionary<Pointer, SceneObject>();
                var sceneObjectNamesDict = new Dictionary<Pointer, ShiftJisCString>();

                // Get all unique instances of SceneObjectTemplates
                // NOTE: instances can share the same name/model but have different properties.
                foreach (var staticSceneObject in staticSceneObjects)
                {
                    GetSerializable(reader, staticSceneObject.sceneObjectPtr, ref staticSceneObject.sceneObject, sceneObjectsDict);
                }
                foreach (var dynamicSceneObject in dynamicSceneObjects)
                {
                    GetSerializable(reader, dynamicSceneObject.sceneObjectPtr, ref dynamicSceneObject.sceneObject, sceneObjectsDict);
                }
                foreach (var unknownCollider in unknownColliders)
                {
                    GetSerializable(reader, unknownCollider.templateSceneObjectPtr, ref unknownCollider.sceneObject, sceneObjectsDict);
                }
                // Save, order by address
                sceneObjects = sceneObjectsDict.Values.ToArray();
                sceneObjects = sceneObjects.OrderBy(x => x.AddressRange.StartAddress).ToArray();

                // Copy over the instances into it's own array
                var sceneObjectLODs = new List<SceneObjectLOD>();
                for (int i = 0; i < this.sceneObjects.Length; i++)
                {
                    var template = this.sceneObjects[i];
                    for (int j = 0; j < template.lods.Length; j++)
                    {
                        sceneObjectLODs.Add(template.lods[j]);
                    }
                }
                this.sceneObjectLODs = sceneObjectLODs.OrderBy(x => x.AddressRange.StartAddress).ToArray();

                // Get all unique instances of SceneObjectTemplates' names
                // NOTE: since SceneObjectTemplates instances can use the same name/model, there is occasionally a few duplicate names.
                foreach (var templateSceneObject in this.sceneObjects)
                {
                    foreach (var so in templateSceneObject.lods)
                    {
                        GetSerializable(reader, so.lodNamePtr, ref so.name, sceneObjectNamesDict);

                    }
                    //GetSerializable(reader, templateSceneObject.PrimarySceneObject.namePtr, ref templateSceneObject.PrimarySceneObject.name, sceneObjectNamesDict);
                }
                // Save, order by name (alphabetical)
                sceneObjectNames = sceneObjectNamesDict.Values.ToArray();
                sceneObjectNames = sceneObjectNames.OrderBy(x => x.value).ToArray();
            }

            // DESERIALIZE TRACK SEGMENTS
            {
                /*/
                Unlike any other data type in the scene structure, TrackSegments are very much
                shared across it's data. A single track can have about a dozen track segments
                total, with perhaps a single root segment, referenced by hundreds TrackNodes.
                If we were to let each TrackNode deserialize it's own recursive tree, it would
                take quite a long time for redundant data.

                The approach taken here is to not let the data type itself deserialize the
                TrackSegment. Instead, it deserializes the pointers only. We can then, after
                deserializing all TrackNodes, find all unique instances and recursively
                deserialize them only once, sharing the C# reference afterwards.
                /*/

                // Read all ROOT track segments
                // These root segments are the ones pointed at by all nodes.
                var rootTrackSegmentDict = new Dictionary<Pointer, TrackSegment>();
                foreach (var trackNode in trackNodes)
                {
                    GetSerializable(reader, trackNode.segmentPtr, ref trackNode.segment, rootTrackSegmentDict);
                }
                rootTrackSegments = rootTrackSegmentDict.Values.ToArray();

                // 2021/09/22: test tagging for serialization order
                foreach (var rootTrackSegment in rootTrackSegments)
                    rootTrackSegment.isRoot = true;

                // Read ALL track segments
                // We want to have all segments. We must recursively follow the hierarchy tree.
                var allTrackSegments = new Dictionary<Pointer, TrackSegment>();
                // Start by iterating over each root node in graph
                foreach (var rootSegment in rootTrackSegments)
                {
                    // Read out children of root, then continue deserializing children recursively
                    // Deserialization method respects ArayPointer order in list.
                    ReadTrackSegmentsRecursive(reader, allTrackSegments, rootSegment);
                }
                this.allTrackSegments = allTrackSegments.Values.ToArray();
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
                    writer.WriteX(trackNodes);
                    var trackNodesPtr = trackNodes.GetBasePointer();

                    // TRACK CHECKPOINTS
                    {
                        // NOTICE:
                        // Ensure sequential order in ROM for array pointer deserialization

                        var all = new List<Checkpoint>();
                        foreach (var trackNode in trackNodes)
                            foreach (var trackCheckpoint in trackNode.checkpoints)
                                all.Add(trackCheckpoint);
                        var array = all.ToArray();

                        writer.InlineDesc(serializeVerbose, trackNodesPtr, array);
                        writer.WriteX(array);
                    }

                    // TRACK SEGMENTS
                    {
                        writer.InlineDesc(serializeVerbose, trackNodesPtr, allTrackSegments);
                        //writer.WriteX(allTrackSegments, false);

                        TrackSegment root = null;
                        for (int i = 0; i < allTrackSegments.Length; i++)
                        {
                            var trackSegment = allTrackSegments[i];
                            if (trackSegment.isRoot)
                            {
                                // If we are holding onto a reference, write
                                if (root != null)
                                    writer.WriteX(root);

                                // record next root
                                root = trackSegment;
                                continue;
                            }
                            else
                            {
                                writer.WriteX(trackSegment);
                            }
                        }
                        if (root != null)
                        {
                            writer.WriteX(root);
                        }


                        // Manually refresh pointers due to recursive format.
                        foreach (var trackSegment in allTrackSegments)
                            trackSegment.SetChildPointers(allTrackSegments);
                    }

                    // TRACK ANIMATION CURVES
                    {
                        // Construct list of all track curves (sets of 9 ptrs)
                        var listTrackCurves = new List<TrackCurves>();
                        foreach (var trackSegment in allTrackSegments)
                            listTrackCurves.Add(trackSegment.trackCurves);
                        var allTrackCurves = listTrackCurves.ToArray();
                        // Write anim curve ptrs
                        writer.InlineDesc(serializeVerbose, allTrackSegments.GetBasePointer(), allTrackCurves);
                        writer.WriteX(allTrackCurves);

                        // Construct list of all /animation curves/ (breakout from track structure)
                        var listAnimationCurves = new List<AnimationCurve>();
                        foreach (var trackAnimationCurve in allTrackCurves)
                            foreach (var animationCurve in trackAnimationCurve.animationCurves)
                                listAnimationCurves.Add(animationCurve);
                        var allAnimationCurves = listAnimationCurves.ToArray();
                        //
                        writer.InlineDesc(serializeVerbose, allTrackCurves.GetBasePointer(), allAnimationCurves);
                        writer.WriteX(allAnimationCurves);
                    }

                    // TODO: better type comment
                    writer.InlineDesc(serializeVerbose, new TrackCorner());
                    foreach (var trackSegment in allTrackSegments)
                    {
                        var corner = trackSegment.trackCorner;
                        if (corner != null)
                        {
                            writer.WriteX(corner);
                        }
                    }
                }


                // Write track checkpoint indexers
                writer.InlineDesc(serializeVerbose, 0xBC + offset, trackCheckpointGrid);
                writer.WriteX(trackCheckpointGrid);
                var trackIndexListPtr = trackCheckpointGrid.GetPointer();
                // Only write if it has indexes
                writer.InlineDesc(serializeVerbose, trackIndexListPtr, trackCheckpointGrid.indexLists);
                foreach (var trackIndexList in trackCheckpointGrid.indexLists)
                {
                    writer.WriteX(trackIndexList);
                }

                //
                writer.InlineDesc(serializeVerbose, 0x14, embeddedPropertyAreas);
                writer.WriteX(embeddedPropertyAreas);
            }

            // STATIC COLLIDER MESHES
            {
                // STATIC COLLIDER MESHES
                // Write main structure
                writer.InlineDesc(serializeVerbose, 0x1C, staticColliderMeshManager);
                writer.WriteX(staticColliderMeshManager);
                var scmPtr = staticColliderMeshManager.GetPointer();

                // Write collider bounds (applies to to non-tri/quad collision, too)
                writer.InlineDesc(serializeVerbose, staticColliderMeshManager.boundingSphere);
                writer.WriteX(staticColliderMeshManager.boundingSphere);

                // COLLIDER TRIS
                {
                    var colliderTris = staticColliderMeshManager.colliderTris;
                    // Write tri data and comment
                    if (!colliderTris.IsNullOrEmpty())
                        writer.InlineDesc(serializeVerbose, scmPtr, colliderTris);
                    writer.WriteX(colliderTris);
                    WriteStaticColliderMeshMatrices(writer, scmPtr, "ColiTri", staticColliderMeshManager.triMeshGrids);
                }

                // COLLIDER QUADS
                {
                    var colliderQuads = staticColliderMeshManager.colliderQuads;
                    // Write quad data and comment
                    if (!colliderQuads.IsNullOrEmpty())
                        writer.InlineDesc(serializeVerbose, scmPtr, colliderQuads);
                    writer.WriteX(colliderQuads);
                    WriteStaticColliderMeshMatrices(writer, scmPtr, "ColiQuad", staticColliderMeshManager.quadMeshGrids);
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
            //{
            //// SCENE OBJECT NAMES
            //// No direct pointer. Names are aligned to 4 bytes.
            //writer.CommentAlign(serializeVerbose);
            //writer.CommentNewLine(serializeVerbose, '-');
            //writer.Comment("ScnObjectNames[]", serializeVerbose, ' ');
            //writer.CommentNewLine(serializeVerbose, '-');
            //foreach (var sceneObjectName in sceneObjectNames)
            //{
            //    writer.WriteX(sceneObjectName);
            //    //writer.AlignTo(4);
            //}

            // SCENE OBJECTS
            writer.InlineComment(serializeVerbose, nameof(SceneObjectLOD));
            writer.WriteX(sceneObjectLODs);

            // SCENE OBJECT TEMPLATES
            writer.InlineDesc(serializeVerbose, 0x68 + offset, sceneObjects); // <<<<
            writer.WriteX(sceneObjects);

            // STATIC SCENE OBJECTS
            if (!staticSceneObjects.IsNullOrEmpty())
            {
                writer.InlineDesc(serializeVerbose, 0x70 + offset, staticSceneObjects); // <<<<
                writer.WriteX(staticSceneObjects);
            }

            // DYNAMIC SCENE OBJECTS
            writer.InlineDesc(serializeVerbose, 0x54 + offset, dynamicSceneObjects);
            writer.WriteX(dynamicSceneObjects);

            // Scene Object Collider Geo
            //{
            // Grab sub-structures of SceneObjectTemplate
            var colliderGeometries = new List<ColliderMesh>();
            var colliderGeoTris = new List<ColliderTriangle>();
            var colliderGeoQuads = new List<ColliderQuad>();
            foreach (var sceneObject in sceneObjects)
            {
                var colliderGeo = sceneObject.colliderMesh;
                if (colliderGeo != null)
                {
                    colliderGeometries.Add(colliderGeo);

                    if (colliderGeo.triCount > 0)
                        colliderGeoTris.AddRange(colliderGeo.tris);

                    if (colliderGeo.quadCount > 0)
                        colliderGeoQuads.AddRange(colliderGeo.quads);
                }
            }
            // Collider Geometry
            writer.InlineComment(serializeVerbose, nameof(ColliderMesh));
            foreach (var colliderGeometry in colliderGeometries)
                writer.WriteX(colliderGeometry);
            //
            writer.InlineComment(serializeVerbose, nameof(ColliderMesh), nameof(ColliderTriangle));
            foreach (var tri in colliderGeoTris)
                writer.WriteX(tri);
            writer.InlineComment(serializeVerbose, nameof(ColliderMesh), nameof(ColliderQuad));
            foreach (var quad in colliderGeoQuads)
                writer.WriteX(quad);
            //}

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
                if (dynamicSceneObject.animationClip != null)
                {
                    animationClips.Add(dynamicSceneObject.animationClip);
                    // Serialize individual animation clip curves
                    foreach (var animationClipCurve in dynamicSceneObject.animationClip.curves)
                        animationClipCurves.Add(animationClipCurve);
                }

                // Texture Metadata
                if (dynamicSceneObject.textureScroll != null)
                {
                    textureScrolls.Add(dynamicSceneObject.textureScroll);
                    foreach (var field in dynamicSceneObject.textureScroll.fields)
                    {
                        if (field != null)
                        {
                            textureScrollFields.Add(field);
                        }
                    }
                }

                // Skeletal Animator
                if (dynamicSceneObject.skeletalAnimator != null)
                {
                    skeletalAnimators.Add(dynamicSceneObject.skeletalAnimator);
                    skeletalProperties.Add(dynamicSceneObject.skeletalAnimator.properties);
                }

                // Transforms
                if (dynamicSceneObject.transformMatrix3x4 != null)
                {
                    transformMatrices.Add(dynamicSceneObject.transformMatrix3x4);
                }
            }

            // Animation clips
            //writer.InlineDesc(serializeVerbose, animationClips.ToArray());
            writer.InlineComment(serializeVerbose, nameof(AnimationClip) + "[]");
            foreach (var animationClip in animationClips)
                writer.WriteX(animationClip);
            // Animation clips' animation curves
            // 2022-01-18: add serilization for animation data!
            writer.InlineComment(serializeVerbose, nameof(AnimationClip), "AnimClipCurve", $"{nameof(AnimationCurve)}[]");
            foreach (var animationClipCurve in animationClipCurves)
                if (animationClipCurve.animationCurve != null)
                    writer.WriteX(animationClipCurve.animationCurve);

            // Texture metadata
            writer.InlineDesc(serializeVerbose, textureScrolls.ToArray());
            foreach (var textureMetadata in textureScrolls)
                writer.WriteX(textureMetadata);
            writer.InlineDesc(serializeVerbose, textureScrollFields.ToArray());
            foreach (var textureMetadataField in textureScrollFields)
                writer.WriteX(textureMetadataField);

            // Skeletal animator
            writer.InlineDesc(serializeVerbose, skeletalAnimators.ToArray());
            foreach (var skeletalAnimator in skeletalAnimators)
                writer.WriteX(skeletalAnimator);
            writer.InlineDesc(serializeVerbose, skeletalProperties.ToArray());
            foreach (var skeletalProperty in skeletalProperties)
                writer.WriteX(skeletalProperty);

            // Transforms for dynamic scene objects
            writer.InlineDesc(serializeVerbose, transformMatrices.ToArray());
            foreach (var transformMatrix3x4 in transformMatrices)
                writer.WriteX(transformMatrix3x4);

            //writer.InlineDesc(serializeVerbose, 0x54 + offset, dynamicSceneObjects);
            //writer.WriteX(dynamicSceneObjects, false);
            //}

            // TRIGGERS
            {
                // ARCADE CHECKPOINT TRIGGERS
                if (!timeExtensionTriggers.IsNullOrEmpty())
                    writer.InlineDesc(serializeVerbose, 0xB0 + offset, timeExtensionTriggers);
                writer.WriteX(timeExtensionTriggers);

                // COURSE METADATA TRIGGERS
                if (!miscellaneousTriggers.IsNullOrEmpty())
                    writer.InlineDesc(serializeVerbose, 0xA8 + offset, miscellaneousTriggers);
                writer.WriteX(miscellaneousTriggers);

                // STORY OBJECT TRIGGERS
                if (!storyObjectTriggers.IsNullOrEmpty())
                    writer.InlineDesc(serializeVerbose, 0xB8 + offset, storyObjectTriggers);
                writer.WriteX(storyObjectTriggers);
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
                        // breaking the rules again. Should inlining be allowed for these ptr types?
                        writer.WriteX(storyObjectPath.animationCurve);
                    }
                }

                // UNKNOWN TRIGGERS
                if (!unknownTriggers.IsNullOrEmpty())
                {
                    writer.InlineDesc(serializeVerbose, 0x94 + offset, unknownTriggers);
                    writer.WriteX(unknownTriggers);
                }

                // VISUAL EFFECT TRIGGERS
                if (!visualEffectTriggers.IsNullOrEmpty())
                {
                    writer.InlineDesc(serializeVerbose, 0x9C + offset, visualEffectTriggers);
                    writer.WriteX(visualEffectTriggers);
                }

                // UNKNOWN COLLIDERS (SOLS ONLY)
                if (!unknownColliders.IsNullOrEmpty())
                {
                    writer.InlineDesc(serializeVerbose, 0x60 + offset, unknownColliders);
                    writer.WriteX(unknownColliders);
                }
            }

            // SCENE OBJECT NAMES
            // No direct pointer. Names are aligned to 4 bytes.
            writer.CommentAlign(serializeVerbose);
            writer.CommentNewLine(serializeVerbose, '-');
            writer.Comment("ScnObjectNames[]", serializeVerbose, ' ');
            writer.CommentNewLine(serializeVerbose, '-');
            foreach (var sceneObjectName in sceneObjectNames)
            {
                writer.WriteX(sceneObjectName);
                //writer.AlignTo(4);
            }

            // DEBUG
            // Assuming the writer for this stream is the type specified below,
            // It will error if we write to the same address twice. this is useful
            // for finding bugs where 
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
                hasReferences.AddRange(allTrackSegments);
                foreach (var trackSegment in allTrackSegments)
                    hasReferences.Add(trackSegment.trackCurves);
                // The checkpoint table
                hasReferences.Add(trackCheckpointGrid);

                // Static Collider Meshes and dependencies
                hasReferences.Add(staticColliderMeshManager);
                hasReferences.AddRange(staticColliderMeshManager.triMeshGrids);
                hasReferences.AddRange(staticColliderMeshManager.quadMeshGrids);
                hasReferences.AddRange(unknownColliders);

                // OBJECTS
                // Scene Objects
                hasReferences.AddRange(sceneObjectLODs);
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
                    hasReferences.Add(storyObjectTrigger.storyObjectPath);
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
            fileSize = (int)writer.BaseStream.Length;

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
                writer.CommentAlign(serializeVerbose, ' ');
                writer.CommentNewLine(serializeVerbose, '-');
                writer.CommentType(matrix, serializeVerbose);
                writer.CommentPtr(refPtr, serializeVerbose);
                writer.CommentNewLine(serializeVerbose, '-');
                writer.CommentLineWide("Owner:", id, serializeVerbose);
                writer.CommentLineWide("Index:", $"[{i,w}/{nMatrices}]", serializeVerbose);
                writer.CommentLineWide("Type:", type, serializeVerbose);
                writer.CommentLineWide("Mtx:", $"x:{matrix.SubdivisionsX,2}, z:{matrix.SubdivisionsZ,2}", serializeVerbose);
                writer.CommentLineWide("MtxCnt:", matrix.Count, serializeVerbose);
                writer.CommentNewLine(serializeVerbose, '-');
                //
                writer.WriteX(matrix);
                var qmiPtr = matrix.GetPointer(); // quad mesh indices? rename as is generic function between quad/tri

                if (matrix.HasIndexes)
                {
                    // 256 lists
                    writer.CommentAlign(serializeVerbose, ' ');
                    writer.CommentNewLine(serializeVerbose, '-');
                    writer.Comment($"{nameof(IndexList)}[{i,w}/{nMatrices}]", serializeVerbose);
                    writer.CommentPtr(qmiPtr, serializeVerbose);
                    writer.CommentLineWide("Type:", type, serializeVerbose);
                    writer.CommentLineWide("Owner:", id, serializeVerbose);
                    writer.CommentNewLine(serializeVerbose, '-');
                    for (int index = 0; index < matrix.indexLists.Length; index++)
                        if (matrix.indexLists[index].Length > 0)
                            writer.CommentIdx(index, serializeVerbose);
                    writer.CommentNewLine(serializeVerbose, '-');
                    for (int index = 0; index < matrix.indexLists.Length; index++)
                    {
                        var quadIndexList = matrix.indexLists[index];
                        writer.WriteX(quadIndexList);
                    }
                }
            }
        }


        public void ReadTrackSegmentsRecursive(BinaryReader reader, Dictionary<Pointer, TrackSegment> allTrackSegments, TrackSegment parent, bool isFirstCall = true)
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
                ReadTrackSegmentsRecursive(reader, allTrackSegments, child, false);
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
                list.AddRange(trackNode.checkpoints);
                list.Add(trackNode.segment);
                list.Add(trackNode.segment.trackCurves);
                list.AddRange(trackNode.segment.trackCurves.animationCurves);
                foreach (var anim in trackNode.segment.trackCurves.animationCurves) // null?
                    list.AddRange(anim.keyableAttributes);
                list.Add(trackNode.segment.trackCorner);
                if (trackNode.segment.trackCorner != null)
                    list.Add(trackNode.segment.trackCorner.matrix3x4);
            }

            list.AddRange(embeddedPropertyAreas);

            // Static Collider Meshes
            list.Add(staticColliderMeshManager);
            list.AddRange(staticColliderMeshManager.colliderTris);
            list.AddRange(staticColliderMeshManager.colliderQuads);
            foreach (var matrix in staticColliderMeshManager.triMeshGrids)
            {
                list.Add(matrix);
                list.AddRange(matrix.indexLists);
            }
            foreach (var matrix in staticColliderMeshManager.quadMeshGrids)
            {
                list.Add(matrix);
                list.AddRange(matrix.indexLists);
            }
            list.Add(staticColliderMeshManager.meshGridXZ);
            list.Add(staticColliderMeshManager.boundingSphere);

            list.Add(trackMinHeight);

            foreach (var dynamicSceneObject in dynamicSceneObjects)
            {
                list.Add(dynamicSceneObject);
                list.Add(dynamicSceneObject.animationClip);
                if (dynamicSceneObject.animationClip != null)
                {
                    foreach (var animClipCurve in dynamicSceneObject.animationClip.curves)
                    {
                        list.Add(animClipCurve);
                        list.Add(animClipCurve.animationCurve);
                        if (animClipCurve.animationCurve != null)
                            list.AddRange(animClipCurve.animationCurve.keyableAttributes);
                    }
                }
                list.Add(dynamicSceneObject.textureScroll);
                if (dynamicSceneObject.textureScroll != null)
                    list.AddRange(dynamicSceneObject.textureScroll.fields);
                list.Add(dynamicSceneObject.skeletalAnimator);
                if (dynamicSceneObject.skeletalAnimator != null)
                    list.Add(dynamicSceneObject.skeletalAnimator.properties);
                list.Add(dynamicSceneObject.transformMatrix3x4);
                // Elsewhere: Scene Object Templates
            }

            list.AddRange(unknownColliders);

            foreach (var template in sceneObjects)
            {
                list.Add(template);
                list.Add(template.colliderMesh);
                list.AddRange(template.lods);
                list.Add(template.PrimaryLOD.name);
            }

            list.Add(fog);
            list.Add(fogCurves);
            if (fogCurves != null)
            {
                foreach (var curve in fogCurves.animationCurves)
                {
                    list.Add(curve);
                    list.AddRange(curve.keyableAttributes);
                }
            }

            list.Add(trackLength);
            list.AddRange(unknownTriggers);
            list.AddRange(visualEffectTriggers);
            list.AddRange(miscellaneousTriggers);
            list.AddRange(timeExtensionTriggers);

            foreach (var trigger in storyObjectTriggers)
            {
                list.Add(trigger);
                list.Add(trigger.storyObjectPath);
                if (trigger.storyObjectPath != null)
                {
                    list.Add(trigger.storyObjectPath.animationCurve);
                    list.AddRange(trigger.storyObjectPath.animationCurve.keyableAttributes);
                }
            }

            list.Add(trackCheckpointGrid);
            list.AddRange(trackCheckpointGrid.indexLists);

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
                if (isFileGX) reader.ReadX(ref unk_sceneObjectCount2);
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
                isFileAX = Format == SerializeFormat.AX;
                isFileGX = Format == SerializeFormat.GX;
                Assert.IsFalse(Format == SerializeFormat.InvalidFormat);

                // UPDATE POINTERS AND COUNTS
                // Track and stage data
                staticColliderMeshManagerPtr = staticColliderMeshManager.GetPointer();
                embeddedTrackPropertyAreasPtr = embeddedPropertyAreas.GetArrayPointer();
                checkpointGridPtr = trackCheckpointGrid.GetPointer();
                trackLengthPtr = trackLength.GetPointer();
                trackMinHeightPtr = trackMinHeight.GetPointer();
                trackNodesPtr = trackNodes.GetArrayPointer();
                fogPtr = fog.GetPointer();
                fogCurvesPtr = fogCurves.GetPointer();
                // TRIGGERS
                timeExtensionTriggersPtr = timeExtensionTriggers.GetArrayPointer();
                miscellaneousTriggersPtr = miscellaneousTriggers.GetArrayPointer();
                storyObjectTriggersPtr = storyObjectTriggers.GetArrayPointer();
                unknownCollidersPtr = unknownColliders.GetArrayPointer();
                unknownTriggersPtr = unknownTriggers.GetArrayPointer();
                visualEffectTriggersPtr = visualEffectTriggers.GetArrayPointer();
                // SCENE OBJECTS
                // References
                sceneObjectsPtr = sceneObjects.GetArrayPointer();
                staticSceneObjectsPtr = staticSceneObjects.GetArrayPointer();
                // Main list
                dynamicSceneObjectCount = dynamicSceneObjects.Length;
                // 2021/01/12: test to see if this makes objects appear
                // It does and doesn't. Objects with animation data seem to play, so may have something to do with that.
                unk_sceneObjectCount1 = dynamicSceneObjects.Length;
                unk_sceneObjectCount2 = dynamicSceneObjects.Length;
                dynamicSceneObjectsPtr = dynamicSceneObjects.GetArrayPointer().Pointer;
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
            Assert.IsTrue(trackNodesPtr.IsNotNull);
            Assert.IsTrue(embeddedTrackPropertyAreasPtr.IsNotNull);
            Assert.IsTrue(trackMinHeightPtr.IsNotNull);
            Assert.IsTrue(staticColliderMeshManagerPtr.IsNotNull);
            Assert.IsTrue(zeroes0x20Ptr.IsNotNull);
            Assert.IsTrue(trackMinHeightPtr.IsNotNull);
            if (sceneObjects.Length > 0)
                Assert.IsTrue(sceneObjectsPtr.IsNotNull);
            Assert.IsTrue(fogPtr.IsNotNull);
            Assert.IsTrue(trackLengthPtr.IsNotNull);
            Assert.IsTrue(checkpointGridPtr.IsNotNull);

            // Ensure existing structures pointers were resolved correctly
            Assert.ReferencePointer(trackNodes, trackNodesPtr);
            Assert.ReferencePointer(embeddedPropertyAreas, embeddedTrackPropertyAreasPtr);
            Assert.ReferencePointer(trackMinHeight, trackMinHeightPtr);
            if (dynamicSceneObjects.Length > 0)
                Assert.ReferencePointer(dynamicSceneObjects, new ArrayPointer(dynamicSceneObjectCount, dynamicSceneObjectsPtr));
            if (sceneObjects.Length > 0)
                Assert.ReferencePointer(sceneObjects, sceneObjectsPtr);
            Assert.ReferencePointer(staticSceneObjects, staticSceneObjectsPtr);
            if (unknownColliders.Length > 0)
                Assert.ReferencePointer(unknownColliders, unknownCollidersPtr);
            Assert.ReferencePointer(fogCurves, fogCurvesPtr);
            Assert.ReferencePointer(fog, fogPtr);
            Assert.ReferencePointer(trackLength, trackLengthPtr);
            if (unknownTriggers.Length > 0)
                Assert.ReferencePointer(unknownTriggers, unknownTriggersPtr);
            if (miscellaneousTriggers.Length > 0)
                Assert.ReferencePointer(miscellaneousTriggers, miscellaneousTriggersPtr);
            if (timeExtensionTriggers.Length > 0)
                Assert.ReferencePointer(timeExtensionTriggers, timeExtensionTriggersPtr);
            if (storyObjectTriggers.Length > 0)
                Assert.ReferencePointer(storyObjectTriggers, storyObjectTriggersPtr);
            Assert.ReferencePointer(trackCheckpointGrid, checkpointGridPtr);
        }

        /// <summary>
        /// Adds serialized reference to Dictionary<> if not present, otherwise skips adding it. 'ref' is used so that
        /// the calling function can access the reference if found within the dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="ptr"></param>
        /// <param name="reference"></param>
        /// <param name="dict"></param>
        public static void GetSerializable<T>(BinaryReader reader, Pointer ptr, ref T reference, Dictionary<Pointer, T> dict)
            where T : class, IBinarySerializable, new()
        {
            // If ptr is null, set reference to null, return
            if (!ptr.IsNotNull)
            {
                reference = null;
                return;
            }

            // If we have have this reference, return it
            if (dict.ContainsKey(ptr))
            {
                reference = dict[ptr];
                //DebugConsole.Log($"REMOVING: {ptr} of {typeof(T).Name} ({reference})");
                //DebugConsole.Log($"REMOVING: {typeof(T).Name}");
                //DebugConsole.Log($"REMOVING: {ptr} of {typeof(T).Name} ({reference}) macthes ref {dict[ptr]}");
            }
            // If we don't have this reference, deserialize it, store in dict, return it
            else
            {
                reader.JumpToAddress(ptr);
                reader.ReadX(ref reference);
                dict.Add(ptr, reference);
            }
        }
    }
}
