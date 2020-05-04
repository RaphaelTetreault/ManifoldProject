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
    public sealed class GxDisplayList : IBinarySerializable
    {
        [Header("GX Display List Group")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;
        [SerializeField] GxVtxAttrTable vat;

        [Space]
        [SerializeField, Hex(8)] uint gxBufferSize;
        [SerializeField] GxDisplayObject[] gxDisplayList;


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

        public GxDisplayList() { }

        public GxDisplayList(GxVtxAttrTable vat, uint gxBufferSize)
        {
            this.vat = vat;
            this.gxBufferSize = gxBufferSize;
        }

        public void Deserialize(BinaryReader reader)
        {
            // RAPH: this could be better and cleaner if I made
            // utility scripts like Jasper/noclip.website to
            // calculate the components' sizes.

            // Temp list to store commands for this list
            var newList = new List<GxDisplayObject>();

            startAddress = reader.BaseStream.Position;

            // this code doesn't work because you're doing it wrong

            //var endPos = startAddress + gxBufferSize;
            //while (reader.BaseStream.Position < endPos)
            //{
            //    var absPtr = reader.BaseStream.Position;
            //    var gxCommand = new GxDisplayCommand();
            //    reader.ReadX(ref gxCommand, true);
            //    reader.BaseStream.Seek(absPtr, SeekOrigin.Begin);

            //    if (gxCommand.command != 0)
            //    {
            //        // Read command into list
            //        var displayList = new GxDisplayList();
            //        displayList.vat = vat;
            //        reader.ReadX(ref displayList, false);
            //        newList.Add(displayList);
            //    }
            //    // Break when we are reading GX_NOP (0)
            //    else break;
            //}
            endAddress = reader.BaseStream.Position;

            this.gxDisplayList = newList.ToArray();
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}