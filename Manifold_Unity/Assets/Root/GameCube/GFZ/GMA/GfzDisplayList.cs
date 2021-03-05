using GameCube.GX;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class GfzDisplayList : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [HideInInspector]
        [SerializeField]
        private AddressRange addressRange;

        [SerializeField]
        private GXAttrFlag_U32 attrFlags;

        [SerializeField]
        private int size;

        [SerializeField]
        private byte gxBegin;

        [SerializeField]
        private GxDisplayList[] gxDisplayLists;


        #endregion

        #region CONSTRUCTORS

        public GfzDisplayList() { }

        public GfzDisplayList(GXAttrFlag_U32 attrFlags, int size)
        {
            this.attrFlags = attrFlags;
            this.size = size;
        }

        #endregion

        #region PROPERTIES


        public GxDisplayList[] GxDisplayLists => gxDisplayLists;

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            if (size <= 0)
            {
                // Init as empty
                gxDisplayLists = new GxDisplayList[0];
                return;
            }

            this.RecordStartAddress(reader);
            {
                // Read real
                reader.ReadX(ref gxBegin);
                Assert.IsTrue(gxBegin == 0x00, $"{addressRange.startAddress:X8} {attrFlags}");

                var gxDisplayList = new List<GxDisplayList>();

                // I'm sure this is going to break.
                // Perhaps use size in this equation? 
                while (!reader.EndOfStream()
                    && reader.BaseStream.Position < (addressRange.startAddress + size)
                    && reader.PeekByte() != 0x00)
                {
                    GxDisplayList vtx = new GxDisplayList(attrFlags);
                    vtx.Deserialize(reader);
                    gxDisplayList.Add(vtx);
                }
                gxDisplayLists = gxDisplayList.ToArray();
            }
            this.RecordEndAddress(reader);

            // Apply padding if necessary
            var fifoPadding = (32 - reader.BaseStream.Position % 32) % 32;
            reader.BaseStream.Position += fifoPadding;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}