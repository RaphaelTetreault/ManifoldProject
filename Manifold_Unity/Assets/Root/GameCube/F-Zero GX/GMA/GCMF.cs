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

        [SerializeField] GcmfProperties gcmfProperties;
        [SerializeField] Texture[] textures;
        [SerializeField] TransformMatrix3x4Collection transformMatrixCollection;
        [SerializeField] VertexControlHeader vertexControlHeader;
        [SerializeField] GcmfRenderData[] gcmfRenderData;
        [SerializeField] VertexControlCollection vertexControlCollection;

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
            => gcmfProperties;

        public Texture[] Textures
            => textures;

        public TransformMatrix3x4Collection TransformMatrixCollection
            => transformMatrixCollection;

        public VertexControlHeader VertexControlHeader
            => vertexControlHeader;

        public GcmfRenderData[] GcmfRenderData
            => gcmfRenderData;

        #endregion


        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            // Load properties
            reader.ReadX(ref gcmfProperties, true);
            var textureCount = gcmfProperties.TextureCount;
            var matrixCount = gcmfProperties.TransformMatrixCount;
            var materialCount = gcmfProperties.TotalMaterialCount;

            reader.ReadX(ref textures, textureCount, true);
            transformMatrixCollection = new TransformMatrix3x4Collection(matrixCount);
            reader.ReadX(ref transformMatrixCollection, false);

            // Read VertexControlData on appropriate GCMFs
            if (gcmfProperties.IsSkinOrEffective)
                reader.ReadX(ref vertexControlHeader, true);

            // Properly paramatize render data based on GCMF properties
            gcmfRenderData = new GcmfRenderData[materialCount];
            for (int i = 0; i < gcmfRenderData.Length; i++)
            {
                // Not fond of passing this parameter in. Feels dissassociated from rest
                gcmfRenderData[i] = new GcmfRenderData(gcmfProperties.IsSkinOrEffective);
                reader.ReadX(ref gcmfRenderData[i], false);
            }

            vertexControlCollection = new VertexControlCollection(vertexControlHeader);
            reader.ReadX(ref vertexControlCollection, false);

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(gcmfProperties);
            writer.WriteX(textures, false);

            if (!(transformMatrixCollection is null))
                writer.WriteX(transformMatrixCollection);

            if (gcmfProperties.IsSkinOrEffective)
                writer.WriteX(vertexControlHeader);

            foreach (var gcmf in gcmfRenderData)
            {
                writer.WriteX(gcmf);
            }
        }
    }
}