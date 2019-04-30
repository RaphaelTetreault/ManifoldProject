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
        [SerializeField] VertexControlData vertexControlData;
        [SerializeField] GcmfRenderData[] gcmfRenderData;

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

        public VertexControlData VertexControlData
            => VertexControlData;

        public GcmfRenderData[] GcmfRenderData
            => GcmfRenderData;

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
            var isSkinModel = (gcmfProperties.Attributes & GcmfAttributes_U32.IS_SKIN_MODEL) != 0;
            var isEffectiveModel = (gcmfProperties.Attributes & GcmfAttributes_U32.IS_EFFECTIVE_MODEL) != 0;
            var isSkinOrEffective = isSkinModel || isEffectiveModel;
            if (isSkinOrEffective)
                reader.ReadX(ref vertexControlData, true);

            // Properly paramatize render data based on GCMF properties
            gcmfRenderData = new GcmfRenderData[materialCount];
            for (int i = 0; i < gcmfRenderData.Length; i++)
            {
                // Not fond of passing this parameter in. Feels dissassociated from rest
                //gcmfRenderData[i] = new GcmfRenderData(isSkinOrEffective);
                reader.ReadX(ref gcmfRenderData[i], false);
            }

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(gcmfProperties);
            writer.WriteX(textures, false);

            if (!(transformMatrixCollection is null))
                writer.WriteX(transformMatrixCollection);

            var isSkinModel = (gcmfProperties.Attributes & GcmfAttributes_U32.IS_SKIN_MODEL) != 0;
            var isEffectiveModel = (gcmfProperties.Attributes & GcmfAttributes_U32.IS_EFFECTIVE_MODEL) != 0;
            var isSkinOrEffective = isSkinModel || isEffectiveModel;
            if (isSkinOrEffective)
                writer.WriteX(vertexControlData);

            foreach (var gcmf in gcmfRenderData)
            {
                //gcmf.
                writer.WriteX(gcmf);
            }
        }
    }
}