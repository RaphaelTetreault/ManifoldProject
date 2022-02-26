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
        private UnkVertexType1[] unkVertexType1;
        private SkinnedVertex[] skinnedVertices;
        private SkinBoneBinding[] skinBoneBindings;
        private ushort[] unkMatrixIndices;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public bool IsSkinnedModel
        {
            get => attributes.HasFlag(GcmfAttributes.isSkinModel);
        }
        public bool IsPhysicsModel
        {
            get => attributes.HasFlag(GcmfAttributes.isEffectiveModel);
        }
        public bool IsSkinOrEffective
        {
            get
            {
                bool isSkin = attributes.HasFlag(GcmfAttributes.isSkinModel);
                bool isEffective = attributes.HasFlag(GcmfAttributes.isEffectiveModel);
                return isSkin || isEffective;
            }
        }

        public int TotalSubmeshCount
        {
            get => materialCount + translucidMaterialCount;
        }
        public int SkinnedVertexBasePtr;

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
                Assert.IsTrue(magic == kMagic);
                Assert.IsTrue(zero0x1F == 0);
                foreach (var zero in zeroes0x24)
                    Assert.IsTrue(zero == 0);
            }
            //
            {
                // Align from after main deserialization
                reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);

                //
                textureConfigs = new TextureConfig[textureCount];
                for (int i = 0; i < textureConfigs.Length; i++)
                {
                    textureConfigs[i] = new TextureConfig();
                    textureConfigs[i].Deserialize(reader);
                    reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
                }

                reader.ReadX(ref bones, transformMatrixCount, true);
                reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);

                // Hmm...
                //SkinnedVertexBasePtr = (int)reader.BaseStream.Position;

                if (IsSkinOrEffective)
                {
                    SkinnedVertexBasePtr = (int)reader.BaseStream.Position;
                    reader.ReadX(ref skinnedVertexDescriptor, true);
                    reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
                }


                submeshes = new Submesh[TotalSubmeshCount];
                for (int i = 0; i < submeshes.Length; i++)
                {
                    submeshes[i] = new Submesh();
                    submeshes[i].IsSkinOrEffective = IsSkinOrEffective;
                    submeshes[i].Deserialize(reader);
                }


                if (IsSkinnedModel)
                {
                    //
                    {
                        var address = SkinnedVertexBasePtr + skinnedVertexDescriptor.UnkType1RelPtr;
                        reader.JumpToAddress(address);
                        reader.ReadX(ref unkVertexType1, skinnedVertexDescriptor.UnkType1Count, true);
                        //reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
                    }

                    //
                    {
                        var address = SkinnedVertexBasePtr + skinnedVertexDescriptor.UnkType4RelPtr;
                        reader.JumpToAddress(address);
                        reader.ReadX(ref unkMatrixIndices, TransformMatrixCount);
                        reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
                    }
                }

                if (IsSkinnedModel || IsPhysicsModel)
                {

                    // Skinned Vertices
                    {
                        var address = SkinnedVertexBasePtr + skinnedVertexDescriptor.SkinnedVerticesRelPtr;
                        reader.JumpToAddress(address);
                        reader.ReadX(ref skinnedVertices, skinnedVertexDescriptor.SkinnedVertexCount, true);
                        reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
                    }

                    // Skin Bone Binding
                    {
                        var address = SkinnedVertexBasePtr + skinnedVertexDescriptor.SkinBoneBindingsRelPtr;
                        reader.JumpToAddress(address);

                        var bindings = new List<SkinBoneBinding>();
                        while (reader.PeekInt() > 0 && reader.PeekInt() <= byte.MaxValue)
                        {
                            var skinBoneBinding = new SkinBoneBinding();
                            skinBoneBinding.Deserialize(reader);
                            bindings.Add(skinBoneBinding);
                        }
                        skinBoneBindings = bindings.ToArray();
                    }
                }
            }
            reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                throw new NotFiniteNumberException();
                //writer.WriteX();
            }
            this.RecordEndAddress(writer);
            writer.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
        }

    }

}