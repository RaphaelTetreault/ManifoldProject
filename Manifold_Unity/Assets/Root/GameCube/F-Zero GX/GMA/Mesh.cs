//using GameCube.GX;
//using StarkTools.IO;
//using System;
//using System.IO;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Assertions;
//using UnityEngine.Serialization;

////namespace GameCube.FZeroGX.GMA
////{
////    [Serializable]
////    public class Mesh : IBinarySerializable
////    {
////        [Header("Mesh")]
////        [SerializeField, Hex(8)] private int materialCount;
////        [SerializeField, Hex(8)] private int translucidMaterialCount;

////        [SerializeField] private GxVertexAttributeTable vat;
////        [SerializeField] private GxDisplayList[] matMeshes;
////        [SerializeField] private GxDisplayList[] tlMatMeshes;

////        public Mesh() { }

////        public Mesh(GxVertexAttributeTable vat, GcmfProperties gcmf)
////        {
////            this.materialCount = gcmf.MaterialCount;
////            this.translucidMaterialCount = gcmf.TranslucidMaterialCount;
////            this.vat = vat;

////            throw new NotImplementedException();

////            //var matBuffer = gcmf.Materials.MatDisplayListSize;
////            //matMeshes = new GxDisplayList[this.materialCount];
////            //for (int i = 0; i < matMeshes.Length; i++)
////            //    matMeshes[i] = new GxDisplayList(vat, matBuffer);

////            //var tlMatBuffer = gcmf.Materials.TlMatDisplayListSize;
////            //tlMatMeshes = new GxDisplayList[this.translucidMaterialCount];
////            //for (int i = 0; i < tlMatMeshes.Length; i++)
////            //    tlMatMeshes[i] = new GxDisplayList(vat, tlMatBuffer);
////        }

////        public void Deserialize(BinaryReader reader)
////        {
////            // Requires initialization before use

////            for (int i = 0; i < matMeshes.Length; i++)
////                reader.ReadX(ref matMeshes[i], false);
////            reader.Align(GxUtility.GX_FIFO_ALIGN);

////            for (int i = 0; i < tlMatMeshes.Length; i++)
////                reader.ReadX(ref tlMatMeshes, tlMatMeshes.Length, false);
////            reader.Align(GxUtility.GX_FIFO_ALIGN);

////        }

////        public void Serialize(BinaryWriter writer)
////        {
////            throw new NotImplementedException();
////        }
////    }
////}
