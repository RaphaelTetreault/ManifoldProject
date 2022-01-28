using Manifold;
using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class Gcmf : IBinarySerializable, IBinaryAddressable
    {

        #region FIELDS


        /// <summary>
        /// Name of this GCMF model
        /// </summary>
        [Header("GCMF")]

        // metadata
        [SerializeField] ShiftJisCString name;
        [SerializeField] AddressRange addressRange;
        [SerializeField, Hex] int t1Count;

        [Space]
        [SerializeField] GcmfProperties properties;
        [SerializeField] TextureDescriptor[] textures;
        [SerializeField] GcmfTransformMatrices transformMatrices;
        [SerializeField] VertexControlHeader vertexControlHeader;
        [SerializeField] GcmfSubmesh[] submeshes;
        [SerializeField] VertexControl_T1[] vertexControl_T1;
        [SerializeField] VertexControl_T2[] vertexControl_T2;
        [SerializeField] VertexControl_T3 vertexControl_T3;
        [SerializeField] VertexControl_T4 vertexControl_T4;

        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public ShiftJisCString ModelName
        {
            get => name;
            set => name = value;
        }

        public GcmfProperties GcmfProperties
            => properties;

        public TextureDescriptor[] Textures
            => textures;

        public GcmfTransformMatrices TransformMatrices
            => transformMatrices;

        public VertexControlHeader VertexControlHeader
            => vertexControlHeader;

        public GcmfSubmesh[] Submeshes
            => submeshes;

        public VertexControl_T1[] VertexControl_T1
            => vertexControl_T1;

        public VertexControl_T2[] VertexControl_T2
            => vertexControl_T2;

        public VertexControl_T3 VertexControl_T3
            => vertexControl_T3;

        public VertexControl_T4 VertexControl_T4
            => vertexControl_T4;


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // Load properties
                reader.ReadX(ref properties, true);
                var textureCount = properties.TextureCount;
                var matrixCount = properties.TransformMatrixCount;
                var materialCount = properties.TotalMaterialCount;

                // Read textures array
                reader.ReadX(ref textures, textureCount, true);

                // Init matrix collection. If size is 0, this init/read doesn't move the stream forward.
                transformMatrices = new GcmfTransformMatrices(matrixCount);
                reader.ReadX(ref transformMatrices, false);

                // Read VertexControlData on appropriate GCMFs
                if (properties.IsSkinOrEffective)
                {
                    reader.ReadX(ref vertexControlHeader, true);
                }

                // Get submeshes and materials
                submeshes = new GcmfSubmesh[materialCount];
                for (int i = 0; i < submeshes.Length; i++)
                {
                    // Properly paramatize render data based on GCMF properties
                    submeshes[i] = new GcmfSubmesh()
                    {
                        IsSkinOrEffective = properties.IsSkinOrEffective
                    };
                    reader.ReadX(ref submeshes[i], false);
                }

                // Read vertex control data if appropriate
                if (properties.IsSkinOrEffective)
                {
                    var t2Count = vertexControlHeader.VertexCount;
                    var rootAddress = vertexControlHeader.AddressRange.startAddress;

                    t1Count = vertexControlHeader.VertexControlT4RelPtr - vertexControlHeader.VertexControlT1RelPtr;
                    t1Count /= 0x20; // format to structure size

                    reader.BaseStream.Seek(rootAddress + vertexControlHeader.VertexControlT1RelPtr, SeekOrigin.Begin);
                    reader.ReadX(ref vertexControl_T1, t1Count, true);
                    reader.BaseStream.Seek(rootAddress + vertexControlHeader.VertexControlT2RelPtr, SeekOrigin.Begin);
                    reader.ReadX(ref vertexControl_T2, t2Count, true);
                    reader.BaseStream.Seek(rootAddress + vertexControlHeader.VertexControlT3RelPtr, SeekOrigin.Begin);
                    reader.ReadX(ref vertexControl_T3, true);
                    reader.BaseStream.Seek(rootAddress + vertexControlHeader.VertexControlT4RelPtr, SeekOrigin.Begin);
                    vertexControl_T4 = new VertexControl_T4(matrixCount);
                    reader.ReadX(ref vertexControl_T4, false);

                }
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(properties);
            writer.WriteX(textures, false);
            // If size is 0, this will not write anything
            writer.WriteX(transformMatrices);

            var vchAddress = writer.BaseStream.Position;
            if (properties.IsSkinOrEffective)
            {
                writer.WriteX(vertexControlHeader);
            }

            foreach (var submesh in submeshes)
            {
                // Ensure gcmfProperties.IsSkinOrEffective?
                submesh.IsSkinOrEffective = properties.IsSkinOrEffective;
                writer.WriteX(submesh);
            }

            if (properties.IsSkinOrEffective)
            {
                var address2 = writer.BaseStream.Position;
                writer.WriteX(vertexControl_T2, false);
                var address1 = writer.BaseStream.Position;
                writer.WriteX(vertexControl_T1, false);
                var address4 = writer.BaseStream.Position;
                writer.WriteX(vertexControl_T4);
                var address3 = writer.BaseStream.Position;
                writer.WriteX(vertexControl_T3);

                //Temp for now - make sure relative pointers are the same
                var pointer1 = (int)(address1 - vchAddress);
                var pointer2 = (int)(address2 - vchAddress);
                var pointer3 = (int)(address3 - vchAddress);
                var pointer4 = (int)(address4 - vchAddress);
                Assert.IsTrue(vertexControlHeader.VertexControlT1RelPtr == pointer1);
                Assert.IsTrue(vertexControlHeader.VertexControlT2RelPtr == pointer2);
                Assert.IsTrue(vertexControlHeader.VertexControlT3RelPtr == pointer3);
                Assert.IsTrue(vertexControlHeader.VertexControlT4RelPtr == pointer4);

                // Write back pointers
                var header = new VertexControlHeader()
                {
                    VertexCount = vertexControl_T2.Length,
                    VertexControlT1RelPtr = pointer1,
                    VertexControlT2RelPtr = pointer2,
                    VertexControlT3RelPtr = pointer3,
                    VertexControlT4RelPtr = pointer4,
                };
                var position = writer.BaseStream.Position;
                writer.BaseStream.Seek(vchAddress, SeekOrigin.Begin);
                writer.WriteX(header);
                // Reset position, though this shouldn't be needed
                writer.BaseStream.Seek(position, SeekOrigin.Begin);
            }
        }


        #endregion

    }
}