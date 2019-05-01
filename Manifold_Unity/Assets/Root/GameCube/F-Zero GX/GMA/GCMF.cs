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
        [SerializeField] string name;

        #region MEMBERS

        [SerializeField] GcmfProperties properties;
        [SerializeField] Texture[] textures;
        [SerializeField] GcmfTransformMatrices transformMatrices;
        [SerializeField] VertexControlHeader vertexControlHeader;
        [SerializeField] GcmfRenderData[] renderData;
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

        public GcmfRenderData[] RenderData
            => renderData;

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

            // Init matrix collection. If size is 0, this init doesn't move the stream forward.
            transformMatrices = new GcmfTransformMatrices(matrixCount);
            reader.ReadX(ref transformMatrices, false);

            // Read VertexControlData on appropriate GCMFs
            if (properties.IsSkinOrEffective)
            {
                reader.ReadX(ref vertexControlHeader, true);
            }

            // Properly paramatize render data based on GCMF properties
            renderData = new GcmfRenderData[materialCount];
            for (int i = 0; i < renderData.Length; i++)
            {
                // Not fond of passing this parameter in. Feels dissassociated from rest
                renderData[i] = new GcmfRenderData(properties.IsSkinOrEffective);
                reader.ReadX(ref renderData[i], false);
            }

            if (properties.IsSkinOrEffective)
            {
                var count = vertexControlHeader.VertexCount;
                var rootAddress = vertexControlHeader.StartAddress;

                reader.BaseStream.Seek(rootAddress + vertexControlHeader.Unk_type1_relPtr, SeekOrigin.Begin);
                reader.ReadX(ref vertexControl_T1, count, true);
                reader.BaseStream.Seek(rootAddress + vertexControlHeader.Unk_type2_relPtr, SeekOrigin.Begin);
                reader.ReadX(ref vertexControl_T2, count, true);
                reader.BaseStream.Seek(rootAddress + vertexControlHeader.Unk_type3_relPtr, SeekOrigin.Begin);
                reader.ReadX(ref vertexControl_T3, true);
                reader.BaseStream.Seek(rootAddress + vertexControlHeader.Unk_type4_relPtr, SeekOrigin.Begin);
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

            if (properties.IsSkinOrEffective)
            {
                writer.WriteX(vertexControlHeader);
            }

            foreach (var gcmf in renderData)
            {
                // Ensure gcmfProperties.IsSkinOrEffective?
                writer.WriteX(gcmf);
            }

            throw new NotImplementedException();
        }
    }
}