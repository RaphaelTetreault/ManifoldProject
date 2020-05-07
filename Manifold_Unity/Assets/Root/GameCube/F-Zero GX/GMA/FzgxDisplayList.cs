using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

using GameCube.GX;
using System.Net;

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
        [SerializeField] GxDisplayList[] gxDisplayLists;

        public GxDisplayList[] GxDisplayLists => gxDisplayLists;

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
            if (size <= 0)
            {
                // Init as empty
                gxDisplayLists = new GxDisplayList[0];
                return;
            }

            //try
            {
                startAddress = reader.BaseStream.Position;

                // Read real
                reader.ReadX(ref gxBegin);
                Assert.IsTrue(gxBegin == 0x00, $"{startAddress:X8} {attrFlags}");

                var gxDisplayList = new List<GxDisplayList>();

                // I'm sure this is going to break.
                // Perhaps use size in this equation? 
                while (!reader.EndOfStream()
                    && reader.BaseStream.Position < (startAddress + size)
                    && reader.PeekByte() != 0x00)
                {
                    GxDisplayList vtx = new GxDisplayList(attrFlags);
                    vtx.Deserialize(reader);
                    gxDisplayList.Add(vtx);
                }
                gxDisplayLists = gxDisplayList.ToArray();

                endAddress = reader.BaseStream.Position;

                var fifoPadding = (32 - reader.BaseStream.Position % 32) % 32;
                reader.BaseStream.Position += fifoPadding;
                //Debug.Log($"Begin:{StartAddress:X8} End:{EndAddress:X8}");

            }
            //catch (Exception e)
            //{
            //    Debug.LogError($"Error {e.GetType().Name} at: {reader.BaseStream.Position:X8} {attrFlags}");
            //}
        }



        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
            //writer.WriteX(data, false);
        }

    }
}