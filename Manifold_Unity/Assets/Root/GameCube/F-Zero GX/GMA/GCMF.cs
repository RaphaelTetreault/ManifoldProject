using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GameCube.FZeroGX.GMA
{
    [Serializable]
    public class GCMF : IBinarySerializable, IBinaryAddressable
    {
        [Header("GCMF")]
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;
        /// <summary>
        /// Name of this GCMF model
        /// </summary>
        [SerializeField] string name;
        [SerializeField, Hex] int t1Size;

        #region MEMBERS

        [SerializeField] GcmfProperties properties;
        [SerializeField] Texture[] textures;
        [SerializeField] GcmfTransformMatrices transformMatrices;
        [SerializeField] VertexControlHeader vertexControlHeader;
        [SerializeField] GcmfSubmesh[] submeshes;
        [SerializeField] VertexControl_T1[] vertexControl_T1;
        [SerializeField] VertexControl_T2[] vertexControl_T2;
        [SerializeField] VertexControl_T3 vertexControl_T3;
        [SerializeField] VertexControl_T4 vertexControl_T4;

        #endregion

        #region PROPERTIES

        // Metadata
        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        public string ModelName
        {
            get => name;
            set => name = value;
        }

        public GcmfProperties GcmfProperties
            => properties;

        public Texture[] Textures
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


        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

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
                // Not fond of passing this parameter in. When serializing, this data
                // will need to be double checked before seriaalization.
                submeshes[i] = new GcmfSubmesh(properties.IsSkinOrEffective);
                reader.ReadX(ref submeshes[i], false);
            }

            // Read vertex control data if appropriate
            if (properties.IsSkinOrEffective)
            {
                var count = vertexControlHeader.VertexCount;
                var rootAddress = vertexControlHeader.StartAddress;

                t1Size = vertexControlHeader.VertexControlT4RelPtr - vertexControlHeader.VertexControlT1RelPtr;
                t1Size /= 0x20; // format to structure size

                reader.BaseStream.Seek(rootAddress + vertexControlHeader.VertexControlT1RelPtr, SeekOrigin.Begin);
                reader.ReadX(ref vertexControl_T1, t1Size, true);
                reader.BaseStream.Seek(rootAddress + vertexControlHeader.VertexControlT2RelPtr, SeekOrigin.Begin);
                reader.ReadX(ref vertexControl_T2, count, true);
                reader.BaseStream.Seek(rootAddress + vertexControlHeader.VertexControlT3RelPtr, SeekOrigin.Begin);
                reader.ReadX(ref vertexControl_T3, true);
                reader.BaseStream.Seek(rootAddress + vertexControlHeader.VertexControlT4RelPtr, SeekOrigin.Begin);
                vertexControl_T4 = new VertexControl_T4(matrixCount);
                reader.ReadX(ref vertexControl_T4, false);

            }

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(properties);
            writer.WriteX(textures, false);
            // If size is 0, this should not write anything
            writer.WriteX(transformMatrices);

            var vchAddress = writer.BaseStream.Position;
            if (properties.IsSkinOrEffective)
            {
                writer.WriteX(vertexControlHeader);
            }

            foreach (var submesh in submeshes)
            {
                // Ensure gcmfProperties.IsSkinOrEffective?
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
                Assert.IsTrue(vertexControlHeader.VertexControlT1RelPtr == address1 - vchAddress);
                Assert.IsTrue(vertexControlHeader.VertexControlT2RelPtr == address2 - vchAddress);
                Assert.IsTrue(vertexControlHeader.VertexControlT3RelPtr == address3 - vchAddress);
                Assert.IsTrue(vertexControlHeader.VertexControlT4RelPtr == address4 - vchAddress);

                // We would normally have to go back and write the proper pointers
                //writer.BaseStream.Seek(vchAddress, SeekOrigin.Begin);
            }
        }

    }
}