//using GameCube.GX;
//using StarkTools.IO;
//using System;
//using System.IO;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.Text;
//using UnityEngine;
//using UnityEngine.Assertions;
//using UnityEngine.Serialization;

//namespace GameCube.FZeroGX.GMA
//{
//    [Serializable]
//    public class SkinningData : IBinarySerializable, IBinaryAddressable
//    {
//        [Header("Stitch Data")]
//        [SerializeField, Hex] long startAddress;
//        [SerializeField, Hex] long endAddress;

//        #region MEMBERS

//        [SerializeField] SkinningHeader skinningHeader;
//        [SerializeField] VertexControl_T1 vertexControl_T1;
//        [SerializeField] VertexControl_T2 vertexControl_T2;
//        [SerializeField] VertexControl_T3 vertexControl_T3;
//        [SerializeField] VertexControl_T4 vertexControl_T4;

//        #endregion

//        #region PROPERTIES


//        #endregion

//        // Metadata

//        public long StartAddress
//        {
//            get => startAddress;
//            set => startAddress = value;
//        }
//        public long EndAddress
//        {
//            get => endAddress;
//            set => endAddress = value;
//        }

//        public void Deserialize(BinaryReader reader)
//        {
//            StartAddress = reader.BaseStream.Position;

//            reader.ReadX(ref skinningHeader, true);
//            reader.ReadX(ref vertexControl_T1, true);
//            reader.ReadX(ref vertexControl_T2, true);
//            reader.ReadX(ref vertexControl_T3, true);
//            reader.ReadX(ref vertexControl_T4, true);

//            EndAddress = reader.BaseStream.Position;
//        }

//        public void Serialize(BinaryWriter writer)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
//}