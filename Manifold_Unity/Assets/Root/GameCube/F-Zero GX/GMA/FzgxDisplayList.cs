using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

using GameCube.GX;

namespace GameCube.FZeroGX.GMA
{
    [Serializable]
    public class FzgxDisplayList : IBinarySerializable, IBinaryAddressable
    {
        [Header("Display List")]
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        [SerializeField] GXAttrFlag_U32 attrFlags;
        [SerializeField, Hex] int size;

        #region MEMBERS

        [SerializeField] byte gxBegin;
        [SerializeField] GxVtxPage[] gxVtxes;
        [SerializeField] byte gxEnd;

        #endregion

        public FzgxDisplayList() { }

        public FzgxDisplayList(GXAttrFlag_U32 attrFlags, int size)
        {
            this.attrFlags = attrFlags;
            this.size = size;
        }

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

        #endregion


        public void Deserialize(BinaryReader reader)
        {
            if (!(size > 0))
            {
                return;
            }

            try
            {
                startAddress = reader.BaseStream.Position;

                // Read real
                reader.ReadX(ref gxBegin);
                Assert.IsTrue(gxBegin == 0x00);

                var gxVtxList = new List<GxVtxPage>();
                while (reader.PeekByte() != 0x00)
                {
                    GxVtxPage vtx = new GxVtxPage(attrFlags);
                    vtx.Deserialize(reader);
                    gxVtxList.Add(vtx);
                }
                reader.ReadX(ref gxEnd);

                gxVtxes = gxVtxList.ToArray();
                // padding

                // Read fake
                //reader.ReadX(ref data, size);

                endAddress = reader.BaseStream.Position;

                var fifoPadding = 32 - reader.BaseStream.Position % 32;
                reader.BaseStream.Position += fifoPadding;

            } catch
            {
                Debug.LogError($"Error at: {reader.BaseStream.Position:X8} {attrFlags}");
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
            //writer.WriteX(data, false);
        }

    }
}