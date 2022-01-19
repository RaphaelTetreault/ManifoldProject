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
    /// The data structure representing a scene in F-Zero AX/GX. It contains all the necessary
    /// data for object placement, object animations, fog, collision data for track and surfaces,
    /// track metadata, and triggers for mission, visual effects, etc.
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
        [UnityEngine.SerializeField] private int fileSize;
        [UnityEngine.SerializeField] private Venue venue;
        [UnityEngine.SerializeField] private string courseName;
        [UnityEngine.SerializeField] private string author;
        [UnityEngine.SerializeField] private bool serializeVerbose = true;
        [UnityEngine.SerializeField] private SerializeFormat format;


        // FIELDS
        // Note: order of folowing structures is same as they appear in binary
        public Range unkRange0x00;
        public ArrayPointer trackNodesPtr;
        public ArrayPointer surfaceAttributeAreasPtr;
        // 2022-01-14: this is a bool, the game loads the following structure regardless
        // (because it'd crash anways because of it when unloading)
        public Bool32 staticColliderMeshesActive = Bool32.True;
        public Pointer staticColliderMeshesPtr;
        public Pointer zeroes0x20Ptr; // GX: 0xE8, AX: 0xE4
        public Pointer trackMinHeightPtr; // GX: 0xFC, AX: 0xF8
        public byte[] zeroes0x28 = new byte[kSizeOfZeroes0x28]; //
        public int dynamicSceneObjectCount;
        public int unk_sceneObjectCount1;
        public int unk_sceneObjectCount2; // GX exclusive
        public Pointer dynamicSceneObjectsPtr;
        public Bool32 unkBool32_0x58 = Bool32.True;
        public ArrayPointer unknownSolsTriggersPtr;
        public ArrayPointer templateSceneObjectsPtr;
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
        public ArrayPointer courseMetadataTriggersPtr;
        public ArrayPointer arcadeCheckpointTriggersPtr;
        public ArrayPointer storyObjectTriggersPtr;
        public Pointer trackCheckpointMatrixPtr;
        public MatrixBoundsXZ trackNodeBoundsXZ;
        public byte[] zeroes0xD8 = new byte[kSizeOfZeroes0xD8]; // 

        // REFERENCE FIELDS
        public TrackNode[] trackNodes;// = new TrackNode[0];
        public SurfaceAttributeArea[] surfaceAttributeAreas = SurfaceAttributeArea.DefaultArray();
        public StaticColliderMap colliderMap;
        public byte[] zeroes0x20 = new byte[kSizeOfZeroes0x20];
        public TrackMinHeight trackMinHeight;// = new TrackMinHeight(); // has default constructor
        public SceneObjectDynamic[] dynamicSceneObjects;// = new SceneObjectDynamic[0];
        public SceneObjectTemplate[] templateSceneObjects;// = new SceneObjectTemplate[0];
        public SceneObjectStatic[] staticSceneObjects;// = new SceneObjectStatic[0];
        public UnknownCollider[] unknownSolsTriggers;// = new UnknownSolsTrigger[0];
        public FogCurves fogCurves;
        public Fog fog;
        public TrackLength trackLength;
        public UnknownTrigger[] unknownTriggers;// = new UnknownTrigger[0];
        public VisualEffectTrigger[] visualEffectTriggers;// = new VisualEffectTrigger[0];
        public CourseMetadataTrigger[] courseMetadataTriggers;// = new CourseMetadataTrigger[0];
        public ArcadeCheckpointTrigger[] arcadeCheckpointTriggers;// = new ArcadeCheckpointTrigger[0];
        public StoryObjectTrigger[] storyObjectTriggers;// = new StoryObjectTrigger[0];
        public TrackCheckpointMatrix trackCheckpointMatrix;
        // FIELDS (that require extra processing)
        // Shared references
        public TrackSegment[] allTrackSegments;// = new TrackSegment[0];
        public TrackSegment[] rootTrackSegments;// = new TrackSegment[0];
        public CString[] sceneObjectNames;// = new CString[0];
        public SceneObject[] sceneObjects;// = new SceneObject[0];

        // TODO: basic analytics suggest these types never share pointers.
        // NOTE: instances are also shared
        //public ColliderGeometry[] sceneColliderGeometries = new ColliderGeometry[0];
        // NOTE: these ones MIGHT be? TODO: validate with analytics?
        //public TextureMetadata[] textureMetadata = new TextureMetadata[0];
        //public AnimationClip[] animationClips = new AnimationClip[0];
        //public SkeletalAnimator[] skeletalAnimators = new SkeletalAnimator[0];


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

        public bool SerializeVerbose
        {
            get => serializeVerbose;
            set => serializeVerbose = value;
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
            reader.ReadX(ref trackNodes, trackNodesPtr.Length, true);

            // 0x10 and 0x14: Track Effect Attribute Areas
            reader.JumpToAddress(surfaceAttributeAreasPtr);
            reader.ReadX(ref surfaceAttributeAreas, surfaceAttributeAreasPtr.Length, true);

            // 0x1C 
            // Format is deserialized in DeserializeSelf(reader);
            // The structure's size differs between AX and GX. Format defines which it uses.
            colliderMap = new StaticColliderMap(Format);
            reader.JumpToAddress(staticColliderMeshesPtr);
            reader.ReadX(ref colliderMap, false);

            // 0x20
            reader.JumpToAddress(zeroes0x20Ptr);
            reader.ReadX(ref zeroes0x20, kSizeOfZeroes0x20);

            // 0x24
            reader.JumpToAddress(trackMinHeightPtr);
            reader.ReadX(ref trackMinHeight, true);

            // 0x48 (count total), 0x4C, 0x50, 0x54 (pointer address)
            reader.JumpToAddress(dynamicSceneObjectsPtr);
            reader.ReadX(ref dynamicSceneObjects, dynamicSceneObjectCount, true);

            // 0x5C and 0x60 SOLS values
            reader.JumpToAddress(unknownSolsTriggersPtr);
            reader.ReadX(ref unknownSolsTriggers, unknownSolsTriggersPtr.Length, true);

            // 0x64 and 0x68
            reader.JumpToAddress(templateSceneObjectsPtr);
            reader.ReadX(ref templateSceneObjects, templateSceneObjectsPtr.Length, true);

            // 0x6C and 0x70
            reader.JumpToAddress(staticSceneObjectsPtr);
            reader.ReadX(ref staticSceneObjects, staticSceneObjectsPtr.Length, true);

            // 0x80
            // Data is optional
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

            // TEMP
            // For some reason, this structure points back to these
            colliderMap.UnknownColliders = unknownSolsTriggers;
            colliderMap.staticSceneObjects = staticSceneObjects;
            Assert.IsTrue(colliderMap.unknownSolsTriggersPtr == unknownSolsTriggersPtr);
            Assert.IsTrue(colliderMap.staticSceneObjectsPtr == staticSceneObjectsPtr);


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
                var templateSceneObjectsDict = new Dictionary<Pointer, SceneObjectTemplate>();
                var sceneObjectNamesDict = new Dictionary<Pointer, CString>();

                // Get all unique instances of SceneObjectTemplates
                // NOTE: instances can share the same name/model but have different properties.
                foreach (var staticSceneObject in staticSceneObjects)
                {
                    GetSerializable(reader, staticSceneObject.templateSceneObjectPtr, ref staticSceneObject.templateSceneObject, templateSceneObjectsDict);
                }
                foreach (var dynamicSceneObject in dynamicSceneObjects)
                {
                    GetSerializable(reader, dynamicSceneObject.templateSceneObjectPtr, ref dynamicSceneObject.templateSceneObject, templateSceneObjectsDict);
                }
                templateSceneObjects = templateSceneObjectsDict.Values.ToArray();

                // Copy over the instances into it's own array
                sceneObjects = new SceneObject[templateSceneObjects.Length];
                for (int i = 0; i < sceneObjects.Length; i++)
                {
                    sceneObjects[i] = templateSceneObjects[i].sceneObject;
                }

                // Get all unique instances of SceneObjectTemplates' names
                // NOTE: since SceneObjectTemplates instances can use the same name/model, there is occasionally a few duplicate names.
                foreach (var templateSceneObject in templateSceneObjects)
                {
                    GetSerializable(reader, templateSceneObject.sceneObject.namePtr, ref templateSceneObject.sceneObject.name, sceneObjectNamesDict);
                }
                sceneObjectNames = sceneObjectNamesDict.Values.ToArray();
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

            BinaryIoUtility.PopEndianess();
        }

        public void Serialize(BinaryWriter writer)
        {
            //// DEBUG
            //var addressables = GetAllAddressables();
            //foreach (var addressable in addressables)
            //{
            //    if (addressable == null)
            //        continue;

            //    addressable.AddressRange = new AddressRange()
            //    {
            //        startAddress = int.MaxValue,
            //        endAddress = int.MaxValue,
            //    };
            //}


            BinaryIoUtility.PushEndianess(false);

            // Write header. At first, pointers will be null or broken.
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
                        writer.WriteX(allTrackCurves, false);

                        // Construct list of all /animation curves/ (breakout from track structure)
                        var listAnimationCurves = new List<AnimationCurve>();
                        foreach (var trackAnimationCurve in allTrackCurves)
                            foreach (var animationCurve in trackAnimationCurve.animationCurves)
                                listAnimationCurves.Add(animationCurve);
                        var allAnimationCurves = listAnimationCurves.ToArray();
                        //
                        writer.InlineDesc(serializeVerbose, allTrackCurves.GetBasePointer(), allAnimationCurves);
                        writer.WriteX(allAnimationCurves, false);
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
                writer.InlineDesc(serializeVerbose, 0x1C, colliderMap);
                writer.WriteX(colliderMap);
                var scmPtr = colliderMap.GetPointer();

                // Write collider bounds (applies to to non-tri/quad collision, too)
                writer.InlineDesc(serializeVerbose, colliderMap.unkData);
                writer.WriteX(colliderMap.unkData);

                // COLLIDER TRIS
                {
                    var colliderTris = colliderMap.colliderTris;
                    // Write tri data and comment
                    if (!colliderTris.IsNullOrEmpty())
                        writer.InlineDesc(serializeVerbose, scmPtr, colliderTris);
                    writer.WriteX(colliderTris, false);
                    WriteStaticColliderMeshMatrices(writer, scmPtr, "ColiTri", colliderMap.triMeshMatrices);
                }

                // COLLIDER QUADS
                {
                    var colliderQuads = colliderMap.colliderQuads;
                    // Write quad data and comment
                    if (!colliderQuads.IsNullOrEmpty())
                        writer.InlineDesc(serializeVerbose, scmPtr, colliderQuads);
                    writer.WriteX(colliderQuads, false);
                    WriteStaticColliderMeshMatrices(writer, scmPtr, "ColiQuad", colliderMap.quadMeshMatrices);
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
            // SCENE OBJECT NAMES
            // No direct pointer. Names are aligned to 4 bytes.
            writer.CommentAlign(serializeVerbose);
            writer.CommentNewLine(serializeVerbose, '-');
            writer.Comment("ScnObjectNames[]", serializeVerbose, ' ');
            writer.CommentNewLine(serializeVerbose, '-');
            foreach (var sceneObjectName in sceneObjectNames)
            {
                writer.WriteX(sceneObjectName);
                writer.AlignTo(4);
            }

            // SCENE OBJECTS
            writer.InlineDesc(serializeVerbose, sceneObjects);
            writer.WriteX(sceneObjects, false);

            // SCENE OBJECT TEMPLATES
            // Grab sub-structures of SceneObjectTemplate
            var colliderGeomertries = new List<ColliderGeometry>();
            foreach (var templateSceneObject in templateSceneObjects)
            {
                if (templateSceneObject.colliderGeometry != null)
                {
                    colliderGeomertries.Add(templateSceneObject.colliderGeometry);
                }
            }
            // Scene Object Template
            writer.InlineDesc(serializeVerbose, 0x68 + offset, templateSceneObjects);
            writer.WriteX(templateSceneObjects, false);
            // Collider Geometry
            writer.InlineDesc(serializeVerbose, colliderGeomertries);
            foreach (var colliderGeometry in colliderGeomertries)
                writer.WriteX(colliderGeometry);

            // STATIC SCENE OBJECTS
            if (!staticSceneObjects.IsNullOrEmpty())
                writer.InlineDesc(serializeVerbose, 0x70 + offset, staticSceneObjects);
            writer.WriteX(staticSceneObjects, false);

            // DYNAMIC SCENE OBJECTS
            writer.InlineDesc(serializeVerbose, 0x54 + offset, dynamicSceneObjects);
            writer.WriteX(dynamicSceneObjects, false);

            // Grab all of the data needed to serialize. By doing this, we can
            // create linear blocks of data for each type. It simplifies the
            // process since we can check for nulls in one loop, and serialize
            // all the values in their own mini loops.
            var animationClips = new List<AnimationClip>();
            var animationClipCurves = new List<AnimationClipCurve>();
            var textureMetadatas = new List<TextureMetadata>();
            var textureMetadataFields = new List<TextureMetadataField>();
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
                if (dynamicSceneObject.textureMetadata != null)
                {
                    textureMetadatas.Add(dynamicSceneObject.textureMetadata);
                    foreach (var field in dynamicSceneObject.textureMetadata.fields)
                    {
                        if (field != null)
                        {
                            textureMetadataFields.Add(field);
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
            writer.InlineDesc(serializeVerbose, animationClips.ToArray());
            foreach (var animationClip in animationClips)
                writer.WriteX(animationClip);
            // Animation Clip Curves
            writer.InlineDesc(serializeVerbose, animationClipCurves.ToArray());
            foreach (var animationClipCurve in animationClipCurves)
                writer.WriteX(animationClipCurve);
            // 2022-01-18: add serilization for animation data!
            writer.InlineComment(serializeVerbose, nameof(AnimationClip), "AnimClipCurve", $"{nameof(AnimationCurve)}[]");
            foreach (var animationClipCurve in animationClipCurves)
                if (animationClipCurve.animationCurve != null)
                    writer.WriteX(animationClipCurve.animationCurve);


            // Texture metadata
            writer.InlineDesc(serializeVerbose, textureMetadatas.ToArray());
            foreach (var textureMetadata in textureMetadatas)
                writer.WriteX(textureMetadata);
            writer.InlineDesc(serializeVerbose, textureMetadataFields.ToArray());
            foreach (var textureMetadataField in textureMetadataFields)
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
            //}

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
                        // breaking the rules again. Should inlining be allowed for these ptr types?
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
                var referers = new List<ISerializedBinaryAddressableReferer>();

                // Track Nodes and dependencies
                referers.AddRange(trackNodes);
                referers.AddRange(allTrackSegments);
                foreach (var trackSegment in allTrackSegments)
                    referers.Add(trackSegment.trackCurves);
                // The checkpoint table
                referers.Add(trackCheckpointMatrix);

                // Static Collider Meshes and dependencies
                referers.Add(colliderMap);
                referers.AddRange(colliderMap.triMeshMatrices);
                referers.AddRange(colliderMap.quadMeshMatrices);

                // OBJECTS
                // Scene Objects
                referers.AddRange(sceneObjects);
                // Scene Object Templates
                referers.AddRange(templateSceneObjects);
                referers.AddRange(colliderGeomertries);
                // Scene Object Statics
                referers.AddRange(staticSceneObjects);
                // Scene Object Dynamics
                referers.AddRange(dynamicSceneObjects);
                referers.AddRange(textureMetadatas);
                referers.AddRange(skeletalAnimators);
                referers.AddRange(animationClipCurves);

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

            //
            fileSize = (int)writer.BaseStream.Length;

            BinaryIoUtility.PopEndianess();

            // DEBUG
            //foreach (var addressable in addressables)
            //{
            //    if (addressable == null)
            //        continue;
            //    var range = addressable.AddressRange;
            //    if (range.startAddress == int.MaxValue || range.endAddress == int.MaxValue)
            //    {
            //        DebugConsole.Log(addressable.GetType().FullName);
            //    }
            //}
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

            list.AddRange(surfaceAttributeAreas);

            // Static Collider Meshes
            list.Add(colliderMap);
            list.AddRange(colliderMap.colliderTris);
            list.AddRange(colliderMap.colliderQuads);
            foreach (var matrix in colliderMap.triMeshMatrices)
            {
                list.Add(matrix);
                list.AddRange(matrix.indexLists);
            }
            foreach (var matrix in colliderMap.quadMeshMatrices)
            {
                list.Add(matrix);
                list.AddRange(matrix.indexLists);
            }
            list.Add(colliderMap.meshBounds);
            list.Add(colliderMap.unkData);

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
                list.Add(dynamicSceneObject.textureMetadata);
                if (dynamicSceneObject.textureMetadata != null)
                    list.AddRange(dynamicSceneObject.textureMetadata.fields);
                list.Add(dynamicSceneObject.skeletalAnimator);
                if (dynamicSceneObject.skeletalAnimator != null)
                    list.Add(dynamicSceneObject.skeletalAnimator.properties);
                list.Add(dynamicSceneObject.transformMatrix3x4);
                // Elsewhere: Scene Object Templates
            }

            list.AddRange(unknownSolsTriggers);

            foreach (var template in templateSceneObjects)
            {
                list.Add(template);
                list.Add(template.colliderGeometry);
                list.Add(template.sceneObject);
                list.Add(template.sceneObject.name);
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
            list.AddRange(courseMetadataTriggers);
            list.AddRange(arcadeCheckpointTriggers);

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

            list.Add(trackCheckpointMatrix);
            list.AddRange(trackCheckpointMatrix.indexLists);

            return list.ToArray();
        }


        public void DeserializeHeader(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // Deserialize main structure
                reader.ReadX(ref unkRange0x00, true);
                reader.ReadX(ref trackNodesPtr);
                reader.ReadX(ref surfaceAttributeAreasPtr);
                reader.ReadX(ref staticColliderMeshesActive);
                reader.ReadX(ref staticColliderMeshesPtr);
                reader.ReadX(ref zeroes0x20Ptr);
                reader.ReadX(ref trackMinHeightPtr);
                ValidateFileFormatPointers(); // VALIDATE
                reader.ReadX(ref zeroes0x28, kSizeOfZeroes0x28);
                reader.ReadX(ref dynamicSceneObjectCount);
                reader.ReadX(ref unk_sceneObjectCount1);
                if (isFileGX) reader.ReadX(ref unk_sceneObjectCount2);
                reader.ReadX(ref dynamicSceneObjectsPtr);
                reader.ReadX(ref unkBool32_0x58);
                reader.ReadX(ref unknownSolsTriggersPtr);
                reader.ReadX(ref templateSceneObjectsPtr);
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
                reader.ReadX(ref courseMetadataTriggersPtr);
                reader.ReadX(ref arcadeCheckpointTriggersPtr);
                reader.ReadX(ref storyObjectTriggersPtr);
                reader.ReadX(ref trackCheckpointMatrixPtr);
                reader.ReadX(ref trackNodeBoundsXZ, true);
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
                staticColliderMeshesPtr = colliderMap.GetPointer();
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
                templateSceneObjectsPtr = templateSceneObjects.GetArrayPointer();
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
                writer.WriteX(surfaceAttributeAreasPtr);
                writer.WriteX(staticColliderMeshesActive);
                writer.WriteX(staticColliderMeshesPtr);
                writer.WriteX(zeroes0x20Ptr);
                writer.WriteX(trackMinHeightPtr);
                writer.WriteX(new byte[kSizeOfZeroes0x28], false); // write const zeros
                writer.WriteX(dynamicSceneObjectCount);
                writer.WriteX(unk_sceneObjectCount1);
                if (Format == SerializeFormat.GX)
                    writer.WriteX(unk_sceneObjectCount2);
                writer.WriteX(dynamicSceneObjectsPtr);
                writer.WriteX(unkBool32_0x58);
                writer.WriteX(unknownSolsTriggersPtr);
                writer.WriteX(templateSceneObjectsPtr);
                writer.WriteX(staticSceneObjectsPtr);
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
                writer.WriteX(trackNodeBoundsXZ);
                writer.WriteX(new byte[kSizeOfZeroes0xD8], false); // write const zeros
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
            Assert.IsTrue(trackNodesPtr.IsNotNullPointer);
            Assert.IsTrue(surfaceAttributeAreasPtr.IsNotNullPointer);
            Assert.IsTrue(trackMinHeightPtr.IsNotNullPointer);
            Assert.IsTrue(staticColliderMeshesPtr.IsNotNullPointer);
            Assert.IsTrue(zeroes0x20Ptr.IsNotNullPointer);
            Assert.IsTrue(trackMinHeightPtr.IsNotNullPointer);
            Assert.IsTrue(templateSceneObjectsPtr.IsNotNullPointer);
            Assert.IsTrue(fogPtr.IsNotNullPointer);
            Assert.IsTrue(trackLengthPtr.IsNotNullPointer);
            Assert.IsTrue(trackCheckpointMatrixPtr.IsNotNullPointer);

            // Ensure existing structures pointers were resolved correctly
            Assert.ReferencePointer(trackNodes, trackNodesPtr);
            Assert.ReferencePointer(surfaceAttributeAreas, surfaceAttributeAreasPtr);
            Assert.ReferencePointer(trackMinHeight, trackMinHeightPtr);
            Assert.ReferencePointer(dynamicSceneObjects, new ArrayPointer(dynamicSceneObjectCount, dynamicSceneObjectsPtr));
            Assert.ReferencePointer(templateSceneObjects, templateSceneObjectsPtr);
            Assert.ReferencePointer(staticSceneObjects, staticSceneObjectsPtr);
            if (unknownSolsTriggers.Length > 0)
                Assert.ReferencePointer(unknownSolsTriggers, unknownSolsTriggersPtr);
            Assert.ReferencePointer(fogCurves, fogCurvesPtr);
            Assert.ReferencePointer(fog, fogPtr);
            Assert.ReferencePointer(trackLength, trackLengthPtr);
            if (unknownTriggers.Length > 0)
                Assert.ReferencePointer(unknownTriggers, unknownTriggersPtr);
            if (courseMetadataTriggers.Length > 0)
                Assert.ReferencePointer(courseMetadataTriggers, courseMetadataTriggersPtr);
            if (arcadeCheckpointTriggers.Length > 0)
                Assert.ReferencePointer(arcadeCheckpointTriggers, arcadeCheckpointTriggersPtr);
            if (storyObjectTriggers.Length > 0)
                Assert.ReferencePointer(storyObjectTriggers, storyObjectTriggersPtr);
            Assert.ReferencePointer(trackCheckpointMatrix, trackCheckpointMatrixPtr);
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
            if (!ptr.IsNotNullPointer)
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
                reader.ReadX(ref reference, true);
                dict.Add(ptr, reference);
            }
        }
    }
}
