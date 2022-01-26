using GameCube.GX;
using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class GfzDisplayList : IBinarySerializable, IBinaryAddressable
    {

        #region FIELDS


        [HideInInspector]
        [SerializeField]
        private AddressRange addressRange;

        [SerializeField]
        private GXAttributes attrFlags;

        [SerializeField]
        private int size;

        [SerializeField]
        private byte gxBegin;

        [SerializeField]
        private DisplayList[] gxDisplayLists;


        #endregion

        #region CONSTRUCTORS

        public GfzDisplayList() { }

        public GfzDisplayList(GXAttributes attrFlags, int size)
        {
            this.attrFlags = attrFlags;
            this.size = size;
        }

        #endregion

        #region PROPERTIES


        public DisplayList[] GxDisplayLists => gxDisplayLists;

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
                gxDisplayLists = new DisplayList[0];
                return;
            }

            this.RecordStartAddress(reader);
            {
                // Read real
                reader.ReadX(ref gxBegin);
                Assert.IsTrue(gxBegin == 0x00, $"{addressRange.startAddress:X8} {attrFlags}");

                var gxDisplayList = new List<DisplayList>();

                // I'm sure this is going to break.
                // Perhaps use size in this equation? 
                while (!reader.IsAtEndOfStream()
                    && reader.BaseStream.Position < (addressRange.startAddress + size)
                    && reader.PeekByte() != 0x00)
                {
                    DisplayList vtx = new DisplayList(attrFlags);
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