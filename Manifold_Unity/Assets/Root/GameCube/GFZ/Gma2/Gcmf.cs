using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameCube.GFZ.Gma2
{
    public class Gcmf :
        IBinaryAddressable,
        IBinarySerializable
    {
        // CONSTANTS
        /// <summary>
        /// Equivilent to ASCII/Shift-JIS "GCMF"
        /// </summary>
        public const uint kMagic = 0x47434D46;
        public const int kZeroes0x24 = 4;

        // FIELDS
        private uint magic;
        private GcmfAttributes attributes;
        private BoundingSphere boundingSphere;
        private ushort textureCount;
        private ushort materialCount;
        private ushort translucidMaterialCount;
        private byte transformMatrixCount;
        private byte zero0x1F;
        private uint gcmfTexturesSize;
        private byte[] zeroes0x24;
        private TransformMatrixIndexes8 transformMatrixIndexes8;
        // Must enforce GX FIFO. TODO: get specifics for each
        private TextureConfig[] textureConfigs;
        private TransformMatrix3x4[] bones; // no fifo between
        private SkinnedVertexDescriptor skinnedVertexDescriptor;
        private Submesh[] submeshes;
        private SkinnedVertexA[] skinnedVerticesA;
        private SkinnedVertexB[] skinnedVerticesB;
        private SkinBoneBinding[] skinBoneBindings;
        private ushort[] unkBoneMatrixIndices;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public bool IsSkinnedModel
        {
            get => attributes.HasFlag(GcmfAttributes.isSkinModel);
        }
        public bool IsPhysicsDrivenModel
        {
            get => attributes.HasFlag(GcmfAttributes.isEffectiveModel);
        }
        public bool IsStitchingModel
        {
            get => attributes.HasFlag(GcmfAttributes.isStitchingModel);
        }
        public bool Is16bitModel
        {
            get => attributes.HasFlag(GcmfAttributes.is16Bit);
        }

        public int TotalSubmeshCount
        {
            get => materialCount + translucidMaterialCount;
        }
        public Pointer SkinnedDataBasePtr { get; private set; }

        public GcmfAttributes Attributes { get => attributes; set => attributes = value; }
        public BoundingSphere BoundingSphere { get => boundingSphere; set => boundingSphere = value; }
        public ushort TextureCount { get => textureCount; set => textureCount = value; }
        public ushort MaterialCount { get => materialCount; set => materialCount = value; }
        public ushort TranslucidMaterialCount { get => translucidMaterialCount; set => translucidMaterialCount = value; }
        public byte TransformMatrixCount { get => transformMatrixCount; set => transformMatrixCount = value; }
        public byte Zero0x1F { get => zero0x1F; set => zero0x1F = value; }
        public uint GcmfTexturesSize { get => gcmfTexturesSize; set => gcmfTexturesSize = value; }
        public TransformMatrixIndexes8 TransformMatrixIndexes8 { get => transformMatrixIndexes8; set => transformMatrixIndexes8 = value; }
        public TransformMatrix3x4[] Bones { get => bones; set => bones = value; }
        public TextureConfig[] TextureConfigs { get => textureConfigs; set => textureConfigs = value; }
        public SkinnedVertexDescriptor VertexController { get => skinnedVertexDescriptor; set => skinnedVertexDescriptor = value; }
        public Submesh[] Submeshes { get => submeshes; set => submeshes = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref magic);
                reader.ReadX(ref attributes);
                reader.ReadX(ref boundingSphere, true);
                reader.ReadX(ref textureCount);
                reader.ReadX(ref materialCount);
                reader.ReadX(ref translucidMaterialCount);
                reader.ReadX(ref transformMatrixCount);
                reader.ReadX(ref zero0x1F);
                reader.ReadX(ref gcmfTexturesSize);
                reader.ReadX(ref zeroes0x24, kZeroes0x24);
                reader.ReadX(ref transformMatrixIndexes8, true);
            }
            this.RecordEndAddress(reader);
            {
                // Align from after main deserialization
                reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);

                // Assert some of the data
                Assert.IsTrue(magic == kMagic);
                Assert.IsTrue(zero0x1F == 0);
                foreach (var zero in zeroes0x24)
                    Assert.IsTrue(zero == 0);
            }
            // Deserialize other structures
            {
                // Read in texture configs. Between each, align for GX FIFO
                textureConfigs = new TextureConfig[textureCount];
                for (int i = 0; i < textureConfigs.Length; i++)
                {
                    reader.ReadX(ref textureConfigs[i], true);
                    reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
                }

                // Read in bones. If 'transformMatrixCount' is 0, nothing happens
                reader.ReadX(ref bones, transformMatrixCount, true);
                reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);

                // Check GCMF attributes; some types has special data embedded
                if (IsSkinnedModel || IsPhysicsDrivenModel)
                {
                    reader.ReadX(ref skinnedVertexDescriptor, true);
                    reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);

                    //
                    SkinnedDataBasePtr = skinnedVertexDescriptor.AddressRange.startAddress;
                }

                // Read in submeshes. Each submesh contains a single material to be applied
                // to one or more triangle strips. If some special data, may not have any
                // vertices, though.
                submeshes = new Submesh[TotalSubmeshCount];
                for (int i = 0; i < submeshes.Length; i++)
                {
                    submeshes[i] = new Submesh();
                    submeshes[i].Attributes = attributes;
                    submeshes[i].Deserialize(reader);
                }

                // If GCMF is a skinned, we contain the following data.
                if (IsSkinnedModel)
                {
                    //
                    {
                        var address = SkinnedDataBasePtr + skinnedVertexDescriptor.SkinnedVerticesARelPtr;
                        reader.JumpToAddress(address);
                        reader.ReadX(ref skinnedVerticesA, skinnedVertexDescriptor.SkinnedVerticesACount, true);
                        // Redundant since the FIFO alignment is the same size as the structure (0x20)
                        //reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
                    }

                    // 
                    {
                        var address = SkinnedDataBasePtr + skinnedVertexDescriptor.UnkBoneMatrixIndicesRelPtr;
                        reader.JumpToAddress(address);
                        reader.ReadX(ref unkBoneMatrixIndices, TransformMatrixCount);
                        reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
                    }
                }

                // If GCMF is skinned or physics-driven, we contain the following data.
                if (IsSkinnedModel || IsPhysicsDrivenModel)
                {
                    // Skinned Vertices
                    {
                        var address = SkinnedDataBasePtr + skinnedVertexDescriptor.SkinnedVerticesBRelPtr;
                        reader.JumpToAddress(address);
                        reader.ReadX(ref skinnedVerticesB, skinnedVertexDescriptor.SkinnedVertexCount, true);
                        reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
                    }

                    // Skin Bone Binding
                    {
                        var address = SkinnedDataBasePtr + skinnedVertexDescriptor.SkinBoneBindingsRelPtr;
                        reader.JumpToAddress(address);
                        var bindings = new List<SkinBoneBinding>();

                        // Kinda hacky, but it works. Read so long as the first int of the type is 
                        // non-zero AND fits in a byte (matrix indexes are a single byte). This has been 
                        // checked to work (no non-zero data unread from any file) since that first int of
                        // the type (which is the count) must be none zero to declare the array's size.
                        //
                        // To do this with a single Peek call, we have to handle the case of 0 correctly.
                        // If we subtract by 1, 0 underflows and becomes an invalid index. To check if a max
                        // index of 255 is valid (which is now 254 due to the -1), we check for less than 255.
                        while (unchecked(reader.PeekUint()-1) < byte.MaxValue)
                        {
                            var skinBoneBinding = new SkinBoneBinding();
                            skinBoneBinding.Deserialize(reader);
                            bindings.Add(skinBoneBinding);
                        }
                        skinBoneBindings = bindings.ToArray();
                        reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
                    }
                }
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                //writer.WriteX();
            }
            this.RecordEndAddress(writer);
            writer.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
            throw new System.NotImplementedException();
        }

    }

}