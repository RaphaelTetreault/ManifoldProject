using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GameCube.GX
{

    [Serializable]
    public struct GxVtx : IBinarySerializable
    {
        [SerializeField] public GxVtxAttrFmt vertAttr;

        // (Raph:) Missing other data
        [SerializeField] public Vector3 position;
        [SerializeField] public Vector3 normal;
        [SerializeField] public Vector3 binormal;
        [SerializeField] public Vector3 tangent;
        [SerializeField] public Color32 color0;
        [SerializeField] public Color32 color1;
        [SerializeField] public Vector2 tex0;
        [SerializeField] public Vector2 tex1;
        [SerializeField] public Vector2 tex2;
        [SerializeField] public Vector2 tex3;
        [SerializeField] public Vector2 tex4;
        [SerializeField] public Vector2 tex5;
        [SerializeField] public Vector2 tex6;
        [SerializeField] public Vector2 tex7;

        public void Deserialize(BinaryReader reader)
        {
            //// POSITION
            //position = GxUtility.ReadGxVectorXYZ(reader, vertAttr.pos);

            //// NORMALS
            //if (vertAttr.nrm.enabled)
            //{
            //    normal = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nrm);
            //}
            //else if (vertAttr.nbt.enabled)
            //{
            //    // This code is untested...
            //    // And it lacks another case for NBT3
            //    throw new NotImplementedException();

            //    normal = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nbt);
            //    binormal = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nbt);
            //    tangent = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nbt);
            //}

            //// COLOR
            //if (vertAttr.clr0.enabled)
            //    color0 = GxUtility.ReadGXColor(reader, vertAttr.clr0);
            //if (vertAttr.clr1.enabled)
            //    color1 = GxUtility.ReadGXColor(reader, vertAttr.clr1);

            //// TEX
            //if (vertAttr.tex0.enabled)
            //    tex0 = GxUtility.ReadGxTextureST(reader, vertAttr.tex0.nElements, );
            //if (vertAttr.tex1.enabled)
            //    tex1 = GxUtility.ReadGxTextureST(reader, vertAttr.tex1);
            //if (vertAttr.tex2.enabled)
            //    tex2 = GxUtility.ReadGxTextureST(reader, vertAttr.tex2);
            //if (vertAttr.tex3.enabled)
            //    tex3 = GxUtility.ReadGxTextureST(reader, vertAttr.tex3);
            //if (vertAttr.tex4.enabled)
            //    tex4 = GxUtility.ReadGxTextureST(reader, vertAttr.tex4);
            //if (vertAttr.tex5.enabled)
            //    tex5 = GxUtility.ReadGxTextureST(reader, vertAttr.tex5);
            //if (vertAttr.tex6.enabled)
            //    tex6 = GxUtility.ReadGxTextureST(reader, vertAttr.tex6);
            //if (vertAttr.tex7.enabled)
            //    tex7 = GxUtility.ReadGxTextureST(reader, vertAttr.tex7);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}